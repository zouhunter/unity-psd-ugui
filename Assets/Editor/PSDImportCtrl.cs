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
    public class PSDImportCtrl
    {
        public string baseFilename { get; private set; }
        public string baseDirectory { get; private set; }

        private PSDUI psdUI;
        private Canvas canvas;

        private IImageImport spriteImport;
        private IImageImport textImport;
        private IImageImport textureImport;
        private IImageImport slicedSpriteImport;

        private ILayerImport buttonImport;
        private ILayerImport toggleImport;
        private ILayerImport panelImport;
        private ILayerImport scrollViewImport;
        private ILayerImport scrollBarImport;
        private ILayerImport sliderImport;
        private ILayerImport gridImport;
        private ILayerImport emptyImport;
        private ILayerImport groupImport;
        private ILayerImport inputFiledImport;
        public PSDImportCtrl(string xmlFilePath)
        {
            InitDataAndPath(xmlFilePath);
            InitCanvas();
            LoadLayers();
            MoveLayers();
            InitDrawers();
        }

        private void InitDataAndPath(string xmlFilePath)
        {
            psdUI = (PSDUI)XMLSerializeUtility.DeserializeXml(xmlFilePath, typeof(PSDUI));
            Debug.Log(psdUI.psdSize.width + "=====psdSize======" + psdUI.psdSize.height);
            if (psdUI == null)
            {
                Debug.Log("The file " + xmlFilePath + " wasn't able to generate a PSDUI.");
                return;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false) { return; }

            baseFilename = Path.GetFileNameWithoutExtension(xmlFilePath);
            baseDirectory = "Assets/" + Path.GetDirectoryName(xmlFilePath.Remove(0, Application.dataPath.Length + 1)) + "/";
        }

        private void InitCanvas()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);//  EditorApplication.NewScene ();
            Canvas temp = Resources.Load(PSDImporterConst.PREFAB_PATH_CANVAS, typeof(Canvas)) as Canvas;
            canvas = GameObject.Instantiate(temp) as Canvas;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UnityEngine.UI.CanvasScaler scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            scaler.referenceResolution = new Vector2(psdUI.psdSize.width, psdUI.psdSize.height);
        }

        private void LoadLayers()
        {
            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                ImportLayer(psdUI.layers[layerIndex], baseDirectory);
            }
        }

        private void InitDrawers()
        {
            spriteImport = new SpriteImport(this);
            textImport = new TextImport(this);
            textureImport = new TextureImport(this);
            slicedSpriteImport = new SliceSpriteImport(this);

            buttonImport = new ButtonLayerImport(this);
            toggleImport = new ToggleLayerImport(this);
            panelImport = new PanelLayerImport(this);
            scrollViewImport = new ScrollViewLayerImport(this);
            scrollBarImport = new ScrollBarLayerImport(this);
            sliderImport = new SliderLayerImport(this);
            gridImport = new GridLayerImport(this);
            emptyImport = new DefultLayerImport(this);
            groupImport = new GroupLayerImport(this);
            inputFiledImport = new InputFieldLayerImport(this);
        }

        public void StartDrawUILayers()
        {
            GameObject obj = CreateEmptyParent(baseFilename);
            obj.transform.SetParent(canvas.transform, false);

            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                DrawLayer(psdUI.layers[layerIndex], obj);
            }
            AssetDatabase.Refresh();
        }

        public void DrawLayer(Layer layer, GameObject parent)
        {
            switch (layer.type)
            {
                case LayerType.Panel:
                    panelImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Normal:
                    emptyImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Button:
                    buttonImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Toggle:
                    toggleImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Grid:
                    gridImport.DrawLayer(layer, parent);
                    break;
                case LayerType.ScrollView:
                    scrollViewImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Slider:
                    sliderImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Group:
                    groupImport.DrawLayer(layer, parent);
                    break;
                case LayerType.InputField:
                    inputFiledImport.DrawLayer(layer, parent);
                    break;
                case LayerType.ScrollBar:
                    scrollBarImport.DrawLayer(layer, parent);
                    break;
                default:
                    break;
                    
            }
        }

        public void DrawLayers(Layer[] layers, GameObject parent)
        {
            if (layers != null)
            {
                for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
                {
                    DrawLayer(layers[layerIndex], parent);
                }
            }
        }

        public void DrawImage(Image image,GameObject parent)
        {
            switch (image.imageType)
            {
                case ImageType.Image:
                    spriteImport.DrawImage(image, parent);
                    break;
                case ImageType.Texture:
                    textureImport.DrawImage(image, parent);
                    break;
                case ImageType.Label:
                    textImport.DrawImage(image, parent);
                    break;
                case ImageType.SliceImage:
                    slicedSpriteImport.DrawImage(image, parent);
                    break;
                default:
                    break;
            }
        }


        public GameObject CreateEmptyParent(string parentName)
        {
            GameObject pfb = Resources.Load<GameObject>(PSDImporterConst.PREFAB_PATH_EMPTY);
            GameObject go = GameObject.Instantiate<GameObject>(pfb);
            go.name = parentName;
            return go;
        }

        private void MoveLayers()
        {
            if (baseFilename.Contains("Common"))
            {
                for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
                {
                    MoveAsset(psdUI.layers[layerIndex], baseDirectory);
                }

                AssetDatabase.Refresh();
            }
        }

        //--------------------------------------------------------------------------
        // private methods,按texture或image的要求导入图片到unity可加载的状态
        //-------------------------------------------------------------------------

        private void ImportLayer(Layer layer, string baseDirectory)
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
        private void MoveAsset(Layer layer, string baseDirectory)
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
    }
}