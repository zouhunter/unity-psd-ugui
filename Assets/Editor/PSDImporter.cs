using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor.SceneManagement;

namespace PSDUIImporter
{
    //------------------------------------------------------------------------------
    // class definition
    //------------------------------------------------------------------------------
    public class CommonPSDImporter : Editor
    {
        //--------------------------------------------------------------------------
        // static private properties
        //--------------------------------------------------------------------------
        private static string baseFilename;
        private static string baseDirectory;

        [MenuItem("Assets/Create/Import PSD ...")]
        static public void ImportHogSceneMenuItem()
        {
            string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Application.dataPath, "xml");
            if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
            {
                ImportPSDUI(inputFile);
            }
        }

        static private void ImportPSDUI(string assetPath)
        {
            // before we do anything else, try to deserialize the input file and be sure it's actually the right kind of file
            PSDUI psdUI = (PSDUI)DeserializeXml(assetPath, typeof(PSDUI));

            Debug.Log(psdUI.psdSize.width + "=====psdSize======" + psdUI.psdSize.height);

            if (psdUI == null)
            {
                Debug.Log("The file " + assetPath + " wasn't able to generate a PSDUI.");
                return;
            }

            // next, we're going to be creating scenes, allow the user to save if they want
            // see if user wants to save current scene, bail if they don't
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()/*EditorApplication.SaveCurrentSceneIfUserWantsTo () */== false)
            {
                return;
            }

            // cache some useful variables
            baseFilename = Path.GetFileNameWithoutExtension(assetPath);
            baseDirectory = "Assets/" + Path.GetDirectoryName(assetPath.Remove(0, Application.dataPath.Length + 1)) + "/";

            Debug.Log("baseFilename " + baseFilename);
            Debug.Log("baseDirectory " + baseDirectory);

            // if the scene already exists, delete it
            //string scenePath = baseDirectory + baseFilename + " Scene.unity";
            //if (File.Exists(scenePath) == true)
            //{
            //    File.Delete(scenePath);
            //    AssetDatabase.Refresh();
            //}
            // now create a new scene
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);//  EditorApplication.NewScene ();

            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                ImportLayer(psdUI.layers[layerIndex], baseDirectory);
            }

            Canvas temp = Resources.Load(PSDImporterConst.PREFAB_PATH_CANVAS, typeof(Canvas)) as Canvas;
            Canvas canvas = GameObject.Instantiate(temp) as Canvas;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UnityEngine.UI.CanvasScaler scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            scaler.referenceResolution = new Vector2(psdUI.psdSize.width, psdUI.psdSize.height);

            GameObject obj = CreateEmptyParent(baseFilename);
            obj.transform.SetParent(canvas.transform, false);

            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                DrawLayer(psdUI.layers[layerIndex], obj);
            }

            AssetDatabase.Refresh();

            if (baseFilename.Contains("Common"))
            {
                for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
                {
                    MoveAsset(psdUI.layers[layerIndex], baseDirectory);
                }

                AssetDatabase.Refresh();
            }
        }

        static private GameObject CreateEmptyParent(string parentName)
        {
            GameObject pfb = Resources.Load<GameObject>(PSDImporterConst.PREFAB_PATH_EMPTY);
            GameObject go = Instantiate<GameObject>(pfb);
            go.name = parentName;
            return go;
        }

        //--------------------------------------------------------------------------
        // private methods,按texture或image的要求导入图片到unity可加载的状态
        //-------------------------------------------------------------------------

        static private void ImportLayer(Layer layer, string baseDirectory)
        {
            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    // we need to fixup all images that were exported from PS
                    Image image = layer.images[imageIndex];

                    if (image.imageSource == ImageSource.Custom)
                    {
                        string texturePathName = baseDirectory + layer.images[imageIndex].name + PSDImporterConst.PNG_SUFFIX;

                        Debug.Log(texturePathName);
                        // modify the importer settings
                        TextureImporter textureImporter = AssetImporter.GetAtPath(texturePathName) as TextureImporter;
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.spritePackingTag = baseFilename;
                        textureImporter.maxTextureSize = 2048;

                        if (baseFilename.Contains("Common") && layer.name == PSDImporterConst.NINE_SLICE)  //If Psd's name contains "Common", then it's Common type;
                        {
                            textureImporter.spriteBorder = new Vector4(3, 3, 3, 3);   // Set Default Slice type  UnityEngine.UI.Image's border to Vector4 (3, 3, 3, 3)
                        }

                        AssetDatabase.WriteImportSettingsIfDirty(texturePathName);
                        AssetDatabase.ImportAsset(texturePathName);
                    }
                }
            }

            if (layer.layers != null)
            {
                for (int layerIndex = 0; layerIndex < layer.layers.Length; layerIndex++)
                {
                    ImportLayer(layer.layers[layerIndex], baseDirectory);
                }
            }
        }

        //------------------------------------------------------------------
        //when it's a common psd, then move the asset to special folder
        //------------------------------------------------------------------
        static private void MoveAsset(Layer layer, string baseDirectory)
        {
            if (layer.images != null)
            {
                string newPath = baseDirectory;
                if (layer.name == PSDImporterConst.IMAGE)
                {
                    newPath = baseDirectory + PSDImporterConst.IMAGE + "/";
                    System.IO.Directory.CreateDirectory(newPath);
                }
                else if (layer.name == PSDImporterConst.NINE_SLICE)
                {
                    newPath = baseDirectory + PSDImporterConst.NINE_SLICE + "/";
                    System.IO.Directory.CreateDirectory(newPath);
                }

                Debug.Log("creating new folder : " + newPath);

                AssetDatabase.Refresh();

                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    // we need to fixup all images that were exported from PS
                    Image image = layer.images[imageIndex];

                    if (image.imageSource == ImageSource.Common)
                    {
                        string texturePathName = baseDirectory + layer.images[imageIndex].name + PSDImporterConst.PNG_SUFFIX;
                        string targetPathName = newPath + layer.images[imageIndex].name + PSDImporterConst.PNG_SUFFIX;

                        Debug.Log(texturePathName);
                        Debug.Log(targetPathName);

                        AssetDatabase.MoveAsset(texturePathName, targetPathName);
                    }
                }
            }

            if (layer.layers != null)
            {
                for (int layerIndex = 0; layerIndex < layer.layers.Length; layerIndex++)
                {
                    MoveAsset(layer.layers[layerIndex], baseDirectory);
                }
            }
        }

        static private void DrawChildLayer(Layer layer, GameObject parent)
        {
            if (layer.layers != null)
            {
                for (int layerIndex = 0; layerIndex < layer.layers.Length; layerIndex++)
                {
                    DrawLayer(layer.layers[layerIndex], parent);
                }
            }
        }
        static private void DrawNormalLayer(Layer layer, GameObject parent)
        {
            GameObject obj = CreateEmptyParent(layer.name);
            obj.transform.SetParent(parent.transform, false); //parent.transform;

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];
                    DrawImage(image, parent);
                }
            }

            DrawChildLayer(layer, obj);
        }

        static private void DrawImage(Image image,GameObject parent)
        {
            if (image.imageSource == ImageSource.Custom)
            {
                UnityEngine.UI.Image pic = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;

                string assetPath = baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;

                if (sprite == null)
                {
                    Debug.Log("loading asset at path: " + baseDirectory + image.name);
                }

                pic.sprite = sprite;

                UnityEngine.UI.Image myImage = GameObject.Instantiate(pic) as UnityEngine.UI.Image;
                myImage.name = image.name;
                myImage.transform.SetParent(parent.transform, false);//.parent = obj.transform;

                RectTransform rectTransform = myImage.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
            }
            else if (image.imageSource == ImageSource.Common)
            {
                if (image.imageType == ImageType.Label)
                {
                    UnityEngine.UI.Text text = Resources.Load(PSDImporterConst.PREFAB_PATH_TEXT, typeof(UnityEngine.UI.Text)) as UnityEngine.UI.Text;

                    UnityEngine.UI.Text myText = GameObject.Instantiate(text) as UnityEngine.UI.Text;

                    //                        myText.color = image.arguments[0];
                    //                        myText.font = image.arguments[1];
                    Debug.Log("Label Color : " + image.arguments[0]);
                    Debug.Log("fontSize : " + image.arguments[2]);

                    Color color;
                    if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + image.arguments[0]), out color))
                    {
                        myText.color = color;
                    }

                    int size;
                    if (int.TryParse(image.arguments[2], out size))
                    {
                        myText.fontSize = size;
                    }

                    myText.text = image.arguments[3];
                    myText.transform.SetParent(parent.transform, false);//.parent = obj.transform;

                    RectTransform rectTransform = myText.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                    rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                }
                else if (image.imageType == ImageType.Texture)
                {
                    UnityEngine.UI.Image pic = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
                    pic.sprite = null;
                    UnityEngine.UI.Image myImage = GameObject.Instantiate(pic) as UnityEngine.UI.Image;
                    myImage.name = image.name;
                    myImage.transform.SetParent(parent.transform, false);//.parent = obj.transform;

                    RectTransform rectTransform = myImage.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                    rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                }
                else
                {
                    UnityEngine.UI.Image pic = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;

                    string commonImagePath = PSDImporterConst.COMMON_BASE_FOLDER + image.name.Replace(".", "/") + PSDImporterConst.PNG_SUFFIX;
                    Debug.Log("==  CommonImagePath  ====" + commonImagePath);
                    Sprite sprite = AssetDatabase.LoadAssetAtPath(commonImagePath, typeof(Sprite)) as Sprite;
                    pic.sprite = sprite;

                    UnityEngine.UI.Image myImage = GameObject.Instantiate(pic) as UnityEngine.UI.Image;
                    myImage.name = image.name;
                    myImage.transform.SetParent(parent.transform, false);//.parent = obj.transform;

                    if (image.imageType == ImageType.SliceImage)
                    {
                        myImage.type = UnityEngine.UI.Image.Type.Sliced;
                    }

                    RectTransform rectTransform = myImage.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                    rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                }
            }
        }


        static private void DrawButton(Layer layer, GameObject parent)
        {
            UnityEngine.UI.Button temp = Resources.Load(PSDImporterConst.PREFAB_PATH_BUTTON, typeof(UnityEngine.UI.Button)) as UnityEngine.UI.Button;
            UnityEngine.UI.Button button = GameObject.Instantiate(temp) as UnityEngine.UI.Button;
            button.transform.SetParent(parent.transform, false);//.parent = parent.transform;

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];
                    if (image.imageType == ImageType.Label)
                    {
                        if (image.name.ToLower().Contains("title"))
                        {
                            //生成文字 
                            UnityEngine.UI.Text text = button.GetComponentInChildren<UnityEngine.UI.Text>();
                            Color color;
                            if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + image.arguments[0]), out color))
                            {
                                text.color = color;
                            }

                            int size;
                            if (int.TryParse(image.arguments[2], out size))
                            {
                                text.fontSize = size;
                            }
                        }
                    }
                    else
                    {
                        if (image.name.ToLower().Contains("normal"))
                        {
                            if (image.imageSource == ImageSource.Custom)
                            {
                                string assetPath = baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                                button.image.sprite = sprite;

                                RectTransform rectTransform = button.GetComponent<RectTransform>();
                                rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                                rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        static private UnityEngine.UI.Toggle DrawToggle(Layer layer, GameObject parent)
        {
            UnityEngine.UI.Toggle temp = Resources.Load(PSDImporterConst.PREFAB_PATH_TOGGLE, typeof(UnityEngine.UI.Toggle)) as UnityEngine.UI.Toggle;
            UnityEngine.UI.Toggle toggle = GameObject.Instantiate(temp) as UnityEngine.UI.Toggle;
            toggle.transform.SetParent(parent.transform, false);//.parent = parent.transform;


            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];

                    if (image.name.Contains("background"))
                    {
                        if (image.imageSource == ImageSource.Custom)
                        {
                            string assetPath = baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                            toggle.image.sprite = sprite;

                            RectTransform rectTransform = toggle.GetComponent<RectTransform>();
                            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                        }
                    }
                    else if (image.name.Contains("mask"))
                    {
                        if (image.imageSource == ImageSource.Custom)
                        {
                            string assetPath = baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                            toggle.graphic.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                        }
                    }
                    else if (image.name.Contains("title"))
                    {
                        //生成文字 
                        UnityEngine.UI.Text text = toggle.GetComponentInChildren<UnityEngine.UI.Text>();
                        Color color;
                        if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + image.arguments[0]), out color))
                        {
                            text.color = color;
                        }

                        int size;
                        if (int.TryParse(image.arguments[2], out size))
                        {
                            text.fontSize = size;
                        }
                    }
                }
            }
            return toggle;
        }

        static private UnityEngine.UI.GridLayoutGroup DrawGrid(Layer layer, GameObject parent)
        {
            UnityEngine.UI.GridLayoutGroup temp = Resources.Load(PSDImporterConst.PREFAB_PATH_GRID, typeof(UnityEngine.UI.GridLayoutGroup)) as UnityEngine.UI.GridLayoutGroup;
            UnityEngine.UI.GridLayoutGroup gridLayoutGroup = GameObject.Instantiate(temp) as UnityEngine.UI.GridLayoutGroup;
            gridLayoutGroup.transform.SetParent(parent.transform, false);//.parent = parent.transform;

            gridLayoutGroup.padding = new RectOffset();
            gridLayoutGroup.cellSize = new Vector2(System.Convert.ToInt32(layer.arguments[2]), System.Convert.ToInt32(layer.arguments[3]));

            RectTransform rectTransform = gridLayoutGroup.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);

            int cellCount = System.Convert.ToInt32(layer.arguments[0]) * System.Convert.ToInt32(layer.arguments[1]);
            for (int cell = 0; cell < cellCount; cell++)
            {
                UnityEngine.UI.Image pic = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
                pic.sprite = null;
                //            Sprite sprite = Resources.Load (relativeResoucesDirectory + "normal_13", typeof(Sprite)) as Sprite;
                //            pic.sprite = sprite;

                UnityEngine.UI.Image myImage = GameObject.Instantiate(pic) as UnityEngine.UI.Image;
                myImage.transform.SetParent(rectTransform, false); //parent = rectTransform;
            }
            return gridLayoutGroup;
        }

        static private void DrawScrollView(Layer layer, GameObject parent)
        {
            UnityEngine.UI.ScrollRect temp = Resources.Load(PSDImporterConst.PREFAB_PATH_SCROLLVIEW, typeof(UnityEngine.UI.ScrollRect)) as UnityEngine.UI.ScrollRect;
            UnityEngine.UI.ScrollRect scrollRect = GameObject.Instantiate(temp) as UnityEngine.UI.ScrollRect;
            scrollRect.transform.SetParent(parent.transform, false); //parent = parent.transform;

            RectTransform rectTransform = scrollRect.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);

            if (layer.layers != null)
            {
                string type = layer.arguments[0].ToUpper();
                switch (type)
                {
                    case "V":
                        scrollRect.vertical = true;
                        scrollRect.horizontal = false;
                        break;
                    case "H":
                        scrollRect.vertical = false;
                        scrollRect.horizontal = true;
                        break;
                    default:
                        break;
                }

                DrawChildLayer(layer, scrollRect.content.gameObject);
            }
        }

        static private void DrawPanel(Layer layer, GameObject parent)
        {
            UnityEngine.UI.Image temp = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
            UnityEngine.UI.Image panel = GameObject.Instantiate(temp) as UnityEngine.UI.Image;
            panel.transform.SetParent(parent.transform, false); //parent = parent.transform;

            panel.name = layer.name;

            DrawChildLayer(layer, panel.gameObject);

            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];

                if (image.name.Contains("background"))
                {
                    string assetPath = baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                    Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                    panel.sprite = sprite;

                    RectTransform rectTransform = panel.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                    rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                }
                else
                {
                    DrawImage(image, panel.gameObject);
                }
            }
            
        }

        static private void DrawLayer(Layer layer, GameObject parent)
        {
            switch (layer.type)
            {
                case LayerType.Panel:
                    DrawPanel(layer, parent);
                    break;
                case LayerType.Normal:
                    DrawNormalLayer(layer, parent);
                    break;
                case LayerType.Button:
                    DrawButton(layer, parent);
                    break;
                case LayerType.Toggle:
                    DrawToggle(layer, parent);
                    break;
                case LayerType.Grid:
                    DrawGrid(layer, parent);
                    break;
                case LayerType.ScrollView:
                    DrawScrollView(layer, parent);
                    break;
            }
        }

        static private object DeserializeXml(string filePath, System.Type type)
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