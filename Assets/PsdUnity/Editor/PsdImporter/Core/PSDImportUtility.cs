using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PSDUnity
{
    public static class PSDImportUtility
    {
        public static string baseFilename;
        public static string baseDirectory;
        public static bool forceMove;
        public static Canvas canvas;
        public static UGUINode uinode;
        //public static readonly Dictionary<Transform, Transform> ParentDic = new Dictionary<Transform, Transform>();

        public static object DeserializeXml(string filePath, System.Type type)
        {
            object instance = null;
            StreamReader xmlFile = File.OpenText(filePath);
            if (xmlFile != null)
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                Encoding encoding = Encoding.GetEncoding("gb2312");
                string xml = encoding.GetString(bytes);
                if ((xml != null) && (xml.ToString() != ""))
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(xml);
                    XmlSerializer xs = new XmlSerializer(type);
                    MemoryStream memoryStream = new MemoryStream(byteArray);
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, System.Text.Encoding.UTF8);
                    if (xmlTextWriter != null)
                    {
                        instance = xs.Deserialize(memoryStream);
                    }
                }
            }
            xmlFile.Close();
            return instance;
        }

        public static UGUINode InstantiateItem(string resourcePatn, string name, UGUINode parent)
        {
            GameObject temp = Resources.Load(resourcePatn, typeof(GameObject)) as GameObject;
            GameObject item = GameObject.Instantiate(temp) as GameObject;
            item.name = name;
            item.transform.SetParent(canvas.transform, false);
            UGUINode node = new UGUINode(item.transform, parent);
            return node;
        }

        ///// <summary>
        ///// 生成图片路径
        ///// </summary>
        ///// <param name="image"></param>
        ///// <returns></returns>
        //static string GetPicturePath(Image image)
        //{
        //    string assetPath = "";
        //    if (PSDImportUtility.forceMove || image.imageSource == ImageSource.Globle)
        //    {
        //        assetPath = PSDImporterConst.Globle_BASE_FOLDER + image.texture.Replace(".", "/") + PSDImporterConst.PNG_SUFFIX;
        //        Debug.Log("==  CommonImagePath  ====" + assetPath);
        //    }
        //    else if (image.imageSource == ImageSource.Normal || image.imageSource == ImageSource.Custom)
        //    {
        //        assetPath = PSDImportUtility.baseDirectory + image.texture + PSDImporterConst.PNG_SUFFIX;
        //    }
        //    return assetPath;
        //}

        public static void SetPictureOrLoadColor(ImgNode image, UnityEngine.UI.Graphic graph)
        {
            graph.color = image.color;
            switch (image.type)
            {
                case ImgType.Image:
                    ((UnityEngine.UI.Image)graph).sprite = image.sprite;
                    break;
                case ImgType.Texture:
                    ((UnityEngine.UI.RawImage)graph).texture = image.texture;
                    break;
                case ImgType.Label:
                    var myText = (UnityEngine.UI.Text)graph;
                    myText.text = image.text;
                    myText.fontSize = image.fontSize;
                    break;
                case ImgType.AtlasImage:
                    ((UnityEngine.UI.Image)graph).sprite = image.sprite;
                    break;
                default:
                    break;
            }
        }

        public static void SetRectTransform(ImgNode image, RectTransform rectTransform)
        {
            rectTransform.sizeDelta = new Vector2(image.rect.width, image.rect.height);
            rectTransform.anchoredPosition = new Vector2(image.rect.x, image.rect.y);
        }

        public static void SetRectTransform(GroupNode layer, RectTransform rectTransform, RectTransform parentTrans)
        {
            if (layer.rect != null)
            {
                rectTransform.sizeDelta = new Vector2(layer.rect.width, layer.rect.height);
                rectTransform.anchoredPosition = new Vector2(layer.rect.x, layer.rect.y);
            }
            else if (parentTrans != null)
            {
                rectTransform.sizeDelta = parentTrans.sizeDelta;
                rectTransform.anchoredPosition = parentTrans.anchoredPosition;
            }
            else
            {
                Debug.Log("尺寸设置失败");
            }
        }
        public static void SetAnchorByNode(UGUINode node)
        {
            RectTransform p_rt = node.parent.InitComponent<RectTransform>();
            RectTransform c_rt = node.InitComponent<RectTransform>();

            switch (node.anchoType)
            {
                case UGUINode.AnchoType.Custom:
                    SetCustomAnchor(p_rt, c_rt);
                    break;
                default:
                    SetNormalAnchor(node.anchoType, p_rt, c_rt);
                    break;
            }
        }
        public static void SetCustomAnchor(RectTransform parentRectt, RectTransform rectt)
        {
            Vector2 sizeDelta = rectt.sizeDelta;
            Vector2 p_sizeDelta = parentRectt.sizeDelta;
            Vector2 anchoredPosition = rectt.anchoredPosition;
            float xmin = p_sizeDelta.x * 0.5f + anchoredPosition.x - sizeDelta.x * 0.5f;
            float xmax = p_sizeDelta.x * 0.5f + anchoredPosition.x + sizeDelta.x * 0.5f;
            float ymin = p_sizeDelta.y * 0.5f + anchoredPosition.y - sizeDelta.y * 0.5f;
            float ymax = p_sizeDelta.y * 0.5f + anchoredPosition.y + sizeDelta.y * 0.5f;
            float xSize = 0;
            float ySize = 0;
            float xanchored = 0;
            float yanchored = 0;
            rectt.anchorMin = new Vector2(xmin / p_sizeDelta.x, ymin / p_sizeDelta.y);
            rectt.anchorMax = new Vector2(xmax / p_sizeDelta.x, ymax / p_sizeDelta.y);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta.x);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta.y);
            rectt.sizeDelta = new Vector2(xSize, ySize);
            rectt.anchoredPosition = new Vector2(xanchored, yanchored);
        }
        public static void SetNormalAnchor(UGUINode.AnchoType anchoType, RectTransform parentRectt, RectTransform rectt)
        {
            Vector2 sizeDelta = rectt.sizeDelta;
            Vector2 p_sizeDelta = parentRectt.sizeDelta;
            Vector2 anchoredPosition = rectt.anchoredPosition;

            float xmin = 0;
            float xmax = 0;
            float ymin = 0;
            float ymax = 0;
            float xSize = 0;
            float ySize = 0;
            float xanchored = 0;
            float yanchored = 0;

            if ((anchoType & UGUINode.AnchoType.Up) == UGUINode.AnchoType.Up)
            {
                ymin = ymax = 1;
                yanchored = anchoredPosition.y - p_sizeDelta.y * 0.5f;
                ySize = sizeDelta.y;
            }
            if ((anchoType & UGUINode.AnchoType.Down) == UGUINode.AnchoType.Down)
            {
                ymin = ymax = 0;
                yanchored = anchoredPosition.y + p_sizeDelta.y * 0.5f;
                ySize = sizeDelta.y;
            }
            if ((anchoType & UGUINode.AnchoType.Left) == UGUINode.AnchoType.Left)
            {
                xmin = xmax = 0;
                xanchored = anchoredPosition.x + p_sizeDelta.x * 0.5f;
                xSize = sizeDelta.x;
            }
            if ((anchoType & UGUINode.AnchoType.Right) == UGUINode.AnchoType.Right)
            {
                xmin = xmax = 1;
                xanchored = anchoredPosition.x - p_sizeDelta.x * 0.5f;
                xSize = sizeDelta.x;
            }
            if ((anchoType & UGUINode.AnchoType.XStretch) == UGUINode.AnchoType.XStretch)
            {
                xmin = 0; xmax = 1;
                xanchored = anchoredPosition.x;
                xSize = sizeDelta.x - p_sizeDelta.x;
            }
            if ((anchoType & UGUINode.AnchoType.YStretch) == UGUINode.AnchoType.YStretch)
            {
                ymin = 0; ymax = 1;
                yanchored = anchoredPosition.y;
                ySize = sizeDelta.y - p_sizeDelta.y;
            }
            if ((anchoType & UGUINode.AnchoType.XCenter) == UGUINode.AnchoType.XCenter)
            {
                xmin = xmax = 0.5f;
                xanchored = anchoredPosition.x;
                xSize = sizeDelta.x;
            }
            if ((anchoType & UGUINode.AnchoType.YCenter) == UGUINode.AnchoType.YCenter)
            {
                ymin = ymax = 0.5f;
                yanchored = anchoredPosition.y;
                ySize = sizeDelta.y;
            }

            rectt.anchorMin = new Vector2(xmin, ymin);
            rectt.anchorMax = new Vector2(xmax, ymax);

            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta.x);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta.y);

            rectt.sizeDelta = new Vector2(xSize, ySize);
            rectt.anchoredPosition = new Vector2(xanchored, yanchored);
        }
    }
}