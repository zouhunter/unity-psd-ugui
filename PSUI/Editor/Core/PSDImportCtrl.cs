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
        private PSDUI psdUI;

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
        private ILayerImport groupImport;
        private ILayerImport inputFiledImport;
        private ILayerImport dropdownImport;


        public PSDImportCtrl(string xmlFilePath)
        {
            InitDataAndPath(xmlFilePath);
            InitCanvas();
            LoadLayers();
            MoveLayers();
            InitDrawers();
            PSDImportUtility.uinode = new UINode(PSDImportUtility.canvas.transform,null);
        }

        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = null;
            switch (layer.type)
            {
                case LayerType.Panel:
                    node= panelImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Button:
                    node = buttonImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Toggle:
                    node = toggleImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Grid:
                    node = gridImport.DrawLayer(layer, parent);
                    break;
                case LayerType.ScrollView:
                    node = scrollViewImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Slider:
                    node = sliderImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Group:
                    node = groupImport.DrawLayer(layer, parent);
                    break;
                case LayerType.InputField:
                    node = inputFiledImport.DrawLayer(layer, parent);
                    break;
                case LayerType.ScrollBar:
                    node = scrollBarImport.DrawLayer(layer, parent);
                    break;
                case LayerType.Dropdown:
                    node = dropdownImport.DrawLayer(layer, parent);
                    break;
                default:
                    break;
            }
            return node;
        }

        public UINode[] DrawLayers(Layer[] layers, UINode parent)
        {
            UINode[] nodes = new UINode[layers.Length];
            if (layers != null)
            {
                for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
                {
                    nodes[layerIndex] = DrawLayer(layers[layerIndex], parent);
                }
            }
            return nodes;
        }
        public UINode[] DrawImages(Image[] images,UINode parent)
        {
            UINode[] nodes = new UINode[images.Length];
            if (images != null)
            {
                for (int layerIndex = 0; layerIndex < images.Length; layerIndex++)
                {
                    nodes[layerIndex] = DrawImage(images[layerIndex], parent);
                }
            }
            return nodes;
        }

        public UINode DrawImage(Image image, UINode parent)
        {
            UINode node = null;
            switch (image.imageType)
            {
                case ImageType.Image:
                case ImageType.Texture:
                case ImageType.SliceImage:
                    node = spriteImport.DrawImage(image, parent);
                    break;
                case ImageType.Label:
                    node = textImport.DrawImage(image, parent);
                    break;
                default:
                    break;
            }
            return node;
        }

        private void InitDataAndPath(string xmlFilePath)
        {
            psdUI = (PSDUI)PSDImportUtility.DeserializeXml(xmlFilePath, typeof(PSDUI));
            Debug.Log(psdUI.psdSize.width + "=====psdSize======" + psdUI.psdSize.height);
            if (psdUI == null)
            {
                Debug.Log("The file " + xmlFilePath + " wasn't able to generate a PSDUI.");
                return;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false) { return; }

            PSDImportUtility.baseFilename = Path.GetFileNameWithoutExtension(xmlFilePath);
            PSDImportUtility.baseDirectory = Path.GetDirectoryName(xmlFilePath) + "/";
        }

        private void InitCanvas()
        {
            //EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);//  EditorApplication.NewScene ();
            Canvas temp = Resources.Load(PSDImporterConst.PREFAB_PATH_CANVAS, typeof(Canvas)) as Canvas;
            PSDImportUtility.canvas = GameObject.Instantiate(temp) as Canvas;
            PSDImportUtility.canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UnityEngine.UI.CanvasScaler scaler = PSDImportUtility.canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            scaler.referenceResolution = new Vector2(psdUI.psdSize.width, psdUI.psdSize.height);
        }

        private void LoadLayers()
        {
            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                ImportLayer(psdUI.layers[layerIndex], PSDImportUtility.baseDirectory);
            }
        }

        private void InitDrawers()
        {
            spriteImport = new SpriteImport();
            textImport = new TextImport();

            sliderImport = new SliderLayerImport();
            inputFiledImport = new InputFieldLayerImport();
            buttonImport = new ButtonLayerImport(this);
            toggleImport = new ToggleLayerImport(this);
            panelImport = new PanelLayerImport(this);
            scrollViewImport = new ScrollViewLayerImport(this);
            scrollBarImport = new ScrollBarLayerImport();
            gridImport = new GridLayerImport(this);
            groupImport = new GroupLayerImport(this);
            dropdownImport = new DropDownLayerImport(this);

        }

        public void BeginDrawUILayers()
        {
            UINode empty = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_EMPTY,PSDImportUtility.baseFilename, PSDImportUtility.uinode);
            RectTransform rt = empty.InitComponent<RectTransform>();
            rt.sizeDelta = new Vector2(psdUI.psdSize.width, psdUI.psdSize.height);
            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                DrawLayer(psdUI.layers[layerIndex], empty);
            }
            AssetDatabase.Refresh();
        }

        public void BeginSetUIParents(UINode node)
        {
            foreach (var item in node.childs)
            {
                item.transform.SetParent(node.transform);
                BeginSetUIParents(item);
            }
        }

        public void BeginSetAnchers(UINode node)
        {
            foreach (var item in node.childs)
            {
                BeginSetAnchers(item);
                PSDImportUtility.SetAnchorByNode(item);
            }
        }

        public void BeginReprocess(UINode node)
        {
            foreach (var item in node.childs)
            {
                BeginReprocess(item);
                if (node.ReprocessEvent != null)
                {
                    node.ReprocessEvent.Invoke();
                }
            }
        }

        private void MoveLayers()
        {
            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                //如果文件名有Globle，将强制全部移动到指定文件夹
                PSDImportUtility.forceMove = PSDImportUtility.baseFilename.Contains("Globle");
                MoveAsset(psdUI.layers[layerIndex], PSDImportUtility.baseDirectory);
            }

            AssetDatabase.Refresh();
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
                    if (image.imageType != ImageType.Label)
                    {
                        string texturePathName = PSDImportUtility.baseDirectory + layer.images[imageIndex].name + PSDImporterConst.PNG_SUFFIX;
                        TextureImporter textureImporter = AssetImporter.GetAtPath(texturePathName) as TextureImporter;

                        if (textureImporter == null) {
                            continue;//图片不存在，可能是颜色区域
                        }

                        
                        if (image.imageType == ImageType.Texture)
                        {
                            textureImporter.textureType = TextureImporterType.Image;
                        }
                        else
                        {
                            // modify the importer settings
                            textureImporter.textureType = TextureImporterType.Sprite;
                            textureImporter.spriteImportMode = SpriteImportMode.Single;
                            textureImporter.spritePackingTag = PSDImportUtility.baseFilename;

                            if (image.imageType == ImageType.SliceImage) {
                                textureImporter.spriteBorder = new Vector4(3, 3, 3, 3);   // Set Default Slice type  UnityEngine.UI.Image's border to Vector4 (3, 3, 3, 3)
                            }
                        }
                        textureImporter.maxTextureSize = 2048;
                        AssetDatabase.WriteImportSettingsIfDirty(texturePathName);
                        AssetDatabase.ImportAsset(texturePathName);
                    }
                }
            }

            if (layer.layers != null)
            {
                for (int layerIndex = 0; layerIndex < layer.layers.Length; layerIndex++)
                {
                    ImportLayer(layer.layers[layerIndex], PSDImportUtility.baseDirectory);
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
                string newPath = PSDImporterConst.Globle_BASE_FOLDER;

                if (!Directory.Exists(newPath))
                {
                    Debug.Log("creating new folder : " + newPath);
                    Directory.CreateDirectory(newPath);
                }

                AssetDatabase.Refresh();

                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    // we need to fixup all images that were exported from PS
                    Image image = layer.images[imageIndex];

                    if (image.imageSource == ImageSource.Globle || PSDImportUtility.forceMove)
                    {
                        string texturePathName = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                        string targetPathName = newPath + image.name + PSDImporterConst.PNG_SUFFIX;

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
                    MoveAsset(layer.layers[layerIndex], PSDImportUtility.baseDirectory);
                }
            }
        }
    }
}