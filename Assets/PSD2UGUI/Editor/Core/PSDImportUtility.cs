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

        public static void TrySetImageColor(Image image,UnityEngine.UI.Graphic pic)
        {
            if (image.arguments.Length > 0)
            {
                Debug.Log(image.arguments[0]);
                Color color;
                if (ColorUtility.TryParseHtmlString(image.arguments[0], out color))
                {
                    pic.color = color;
                }
            }
        }

        /// <summary>
        /// 生成图片路径
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string GetPicturePath(Image image)
        {
            string assetPath = "";
            if (image.imageSource == ImageSource.Normal || image.imageSource == ImageSource.Custom)
            {
                assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
            }
            else if (image.imageSource == ImageSource.Globle)
            {
                assetPath = PSDImporterConst.Globle_BASE_FOLDER + image.name.Replace(".", "/") + PSDImporterConst.PNG_SUFFIX;
                Debug.Log("==  CommonImagePath  ====" + assetPath);
            }
            return assetPath;
        }
        public static void SetRectTransform(Image image,RectTransform rectTransform)
        {
            rectTransform.name = image.name;
            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
        }

        public static void SetRectTransform(Layer layer, RectTransform rectTransform)
        {
            rectTransform.name = layer.name;
            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);
        }
        public static void SetAnchorByNode(UINode node)
        {
            switch (node.anchoType)
            {
                case UINode.AnchoType.Custom:
                    RectTransform p_rt = node.parent.GetCompoment<RectTransform>();
                    RectTransform c_rt = node.GetCompoment<RectTransform>();
                    SetCustomAnchor(p_rt, c_rt);
                    break;
                case UINode.AnchoType.Expland:
                    break;
                case UINode.AnchoType.Center:
                    break;
                case UINode.AnchoType.LeftCenter:
                    break;
                case UINode.AnchoType.RightCenter:
                    break;
                case UINode.AnchoType.UpCenter:
                    break;
                case UINode.AnchoType.DownCenter:
                    break;
                case UINode.AnchoType.LeftExpland:
                    break;
                case UINode.AnchoType.RightExpland:
                    break;
                case UINode.AnchoType.UpExpland:
                    break;
                case UINode.AnchoType.DownExpland:
                    break;
                case UINode.AnchoType.LeftUp:
                    break;
                case UINode.AnchoType.RightUp:
                    break;
                case UINode.AnchoType.LeftDown:
                    break;
                case UINode.AnchoType.RightDown:
                    break;
                default:
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
            rectt.anchorMin = new Vector2(xmin / p_sizeDelta.x, ymin / p_sizeDelta.y);
            rectt.anchorMax = new Vector2(xmax / p_sizeDelta.x, ymax / p_sizeDelta.y);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta.x);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta.y);
            rectt.sizeDelta = Vector2.zero;
            rectt.anchoredPosition = Vector2.zero;
        }
    }
}