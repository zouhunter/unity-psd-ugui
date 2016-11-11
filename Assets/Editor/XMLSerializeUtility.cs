using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor.SceneManagement;

namespace PSDUIImporter
{
    public static class XMLSerializeUtility
    {

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
    }
}