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


        public static object DeserializeXml(string filePath, System.Type type)
        {
            object instance = null;
            StreamReader xmlFile = File.OpenText(filePath);
            if (xmlFile != null)
            {
                string xml = xmlFile.ReadToEnd();
                if ((xml != null) && (xml.ToString() != ""))
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    byte[] byteArray = encoding.GetBytes(xml);
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

        public static GameObject CreateEmptyParent(string parentName)
        {
            GameObject pfb = Resources.Load<GameObject>(PSDImporterConst.PREFAB_PATH_EMPTY);
            GameObject go = GameObject.Instantiate<GameObject>(pfb);
            go.name = parentName;
            return go;
        }

        public static T InstantiateItem<T>(string resourcePatn, string name, Transform parent) where T : UnityEngine.MonoBehaviour
        {
            GameObject temp = Resources.Load(resourcePatn, typeof(GameObject)) as GameObject;
            GameObject item = GameObject.Instantiate(temp) as GameObject;
            item.transform.SetParent(parent.transform, false);//.parent = parent.transform;
            item.name = name;
            return item.GetComponent<T>();
        }

    }
}