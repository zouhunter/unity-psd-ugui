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
        public static readonly Dictionary<Transform, Transform> ParentDic = new Dictionary<Transform, Transform>();

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

        public static T InstantiateItem<T>(string resourcePatn, string name,GameObject parent) where T : UnityEngine.Object
        {
            GameObject temp = Resources.Load(resourcePatn, typeof(GameObject)) as GameObject;
            GameObject item = GameObject.Instantiate(temp) as GameObject;
            item.name = name;
            item.transform.SetParent(canvas.transform, false);
            ParentDic[item.transform] =  parent.transform;
            return item.GetComponent<T>();
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

        public static void SetRectTransform(Image image,RectTransform rectTransform)
        {
            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
        }

        public static void SetRectTransform(Layer layer, RectTransform rectTransform)
        {
            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);
        }
    }
}