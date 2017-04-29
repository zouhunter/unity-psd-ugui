using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PSDUIImporter
{
    public static class PSDImportUtility
    {
        public static string baseFilename;
        public static string baseDirectory;
        public static bool forceMove;
        public static Canvas canvas;
        public static UINode uinode;
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

        public static UINode InstantiateItem(string resourcePatn, string name,UINode parent)
        {
            GameObject temp = Resources.Load(resourcePatn, typeof(GameObject)) as GameObject;
            GameObject item = GameObject.Instantiate(temp) as GameObject;
            item.name = name;
            item.transform.SetParent(canvas.transform, false);
            UINode node = new UINode(item.transform,parent);
            return node;
        }

        /// <summary>
        /// 生成图片路径
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        static string GetPicturePath(Image image)
        {
            string assetPath = "";
            if (PSDImportUtility.forceMove|| image.imageSource == ImageSource.Globle)
            {
                assetPath = PSDImporterConst.Globle_BASE_FOLDER + image.name.Replace(".", "/") + PSDImporterConst.PNG_SUFFIX;
                Debug.Log("==  CommonImagePath  ====" + assetPath);
            }
            else if (image.imageSource == ImageSource.Normal || image.imageSource == ImageSource.Custom)
            {
                assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
            }
            return assetPath;
        }

        public static void SetPictureOrLoadColor(Image image,UnityEngine.UI.Graphic graph)
        {
            if (image.arguments != null && image.arguments.Length > 0)
            {
                if (image.arguments.Length > 0)
                {
                    Debug.Log(image.arguments[0]);
                    Color color;
                    if (ColorUtility.TryParseHtmlString(image.arguments[0], out color))
                    {
                        graph.color = color;
                    }
                }
            }
            else
            {
                string assetPath = PSDImportUtility.GetPicturePath(image);

                Object sprite = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite));
                if (sprite != null)
                {
                    if (graph is UnityEngine.UI.Image)
                    {
                       ((UnityEngine.UI.Image)graph).sprite = sprite as Sprite;
                    }
                    else if (graph is UnityEngine.UI.RawImage)
                    {
                        ((UnityEngine.UI.RawImage)graph).texture = sprite as Texture;
                    }

                }
                else
                {
                    Debug.Log("loading asset at path: " + assetPath + "\nname:" + image.name);
                }
            }
        }

        public static void SetRectTransform(Image image,RectTransform rectTransform)
        {
            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
        }

        public static void SetRectTransform(Layer layer, RectTransform rectTransform, RectTransform parentTrans)
        {
            if (layer.size != null)
            {
                rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
                rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);
            }
            else if(parentTrans != null)
            {
                rectTransform.sizeDelta = parentTrans.sizeDelta;
                rectTransform.anchoredPosition = parentTrans.anchoredPosition;
            }
            else
            {
                Debug.Log("尺寸设置失败");
            }
        }
        public static void SetAnchorByNode(UINode node)
        {
            RectTransform p_rt = node.parent.InitComponent<RectTransform>();
            RectTransform c_rt = node.InitComponent<RectTransform>();

            switch (node.anchoType)
            {
                case UINode.AnchoType.Custom:
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
            rectt.sizeDelta = new Vector2(xSize,ySize);
            rectt.anchoredPosition = new Vector2(xanchored,yanchored);
        }
        public static void SetNormalAnchor(UINode.AnchoType anchoType, RectTransform parentRectt, RectTransform rectt)
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

            if ((anchoType & UINode.AnchoType.Up) == UINode.AnchoType.Up)
            {
                ymin = ymax = 1;
                yanchored = anchoredPosition.y - p_sizeDelta.y * 0.5f;
                ySize = sizeDelta.y;
            }
            if ((anchoType & UINode.AnchoType.Down) == UINode.AnchoType.Down)
            {
                ymin = ymax = 0;
                yanchored = anchoredPosition.y + p_sizeDelta.y * 0.5f;
                ySize = sizeDelta.y;
            }
            if ((anchoType & UINode.AnchoType.Left) == UINode.AnchoType.Left)
            {
                xmin = xmax = 0;
                xanchored = anchoredPosition.x + p_sizeDelta.x * 0.5f;
                xSize = sizeDelta.x;
            }
            if ((anchoType & UINode.AnchoType.Right) == UINode.AnchoType.Right)
            {
                xmin = xmax = 1;
                xanchored = anchoredPosition.x - p_sizeDelta.x * 0.5f;
                xSize = sizeDelta.x;
            }
            if ((anchoType & UINode.AnchoType.XStretch) == UINode.AnchoType.XStretch)
            {
                xmin = 0; xmax = 1;
                xanchored = anchoredPosition.x;
                xSize = sizeDelta.x - p_sizeDelta.x;
            }
            if ((anchoType & UINode.AnchoType.YStretch) == UINode.AnchoType.YStretch)
            {
                ymin = 0; ymax = 1;
                yanchored = anchoredPosition.y;
                ySize = sizeDelta.y - p_sizeDelta.y;
            }
            if ((anchoType & UINode.AnchoType.XCenter) == UINode.AnchoType.XCenter)
            {
                xmin = xmax = 0.5f;
                xanchored = anchoredPosition.x;
                xSize = sizeDelta.x;
            }
            if ((anchoType & UINode.AnchoType.YCenter) == UINode.AnchoType.YCenter)
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