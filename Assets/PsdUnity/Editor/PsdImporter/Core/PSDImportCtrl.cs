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

namespace PSDUnity
{
    public class PSDImportCtrl
    {
        private AtlasObject psdUI;

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


        public PSDImportCtrl(AtlasObject psdUI)
        {
            this.psdUI = psdUI;
            //InitDataAndPath(xmlFilePath);
            InitCanvas();
            LoadLayers();
            MoveLayers();
            InitDrawers();
            PSDImportUtility.uinode = new UGUINode(PSDImportUtility.canvas.transform,null);
        }

        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = null;
            switch (layer.controltype)
            {
                case ControlType.Panel:
                    node= panelImport.DrawLayer(layer, parent);
                    break;
                case ControlType.Button:
                    node = buttonImport.DrawLayer(layer, parent);
                    break;
                case ControlType.Toggle:
                    node = toggleImport.DrawLayer(layer, parent);
                    break;
                case ControlType.Grid:
                    node = gridImport.DrawLayer(layer, parent);
                    break;
                case ControlType.ScrollView:
                    node = scrollViewImport.DrawLayer(layer, parent);
                    break;
                case ControlType.Slider:
                    node = sliderImport.DrawLayer(layer, parent);
                    break;
                case ControlType.Group:
                    node = groupImport.DrawLayer(layer, parent);
                    break;
                case ControlType.InputField:
                    node = inputFiledImport.DrawLayer(layer, parent);
                    break;
                case ControlType.ScrollBar:
                    node = scrollBarImport.DrawLayer(layer, parent);
                    break;
                case ControlType.Dropdown:
                    node = dropdownImport.DrawLayer(layer, parent);
                    break;
                default:
                    break;
            }
            return node;
        }

        public UGUINode[] DrawLayers(GroupNode[] layers, UGUINode parent)
        {
            UGUINode[] nodes = new UGUINode[layers.Length];
            if (layers != null)
            {
                for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
                {
                    nodes[layerIndex] = DrawLayer(layers[layerIndex], parent);
                }
            }
            return nodes;
        }
        public UGUINode[] DrawImages(ImgNode[] images,UGUINode parent)
        {
            UGUINode[] nodes = new UGUINode[images.Length];
            if (images != null)
            {
                for (int layerIndex = 0; layerIndex < images.Length; layerIndex++)
                {
                    nodes[layerIndex] = DrawImage(images[layerIndex], parent);
                }
            }
            return nodes;
        }

        public UGUINode DrawImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = null;
            switch (image.type)
            {
                case ImgType.Image:
                case ImgType.Texture:
                case ImgType.AtlasImage:
                    node = spriteImport.DrawImage(image, parent);
                    break;
                case ImgType.Label:
                    node = textImport.DrawImage(image, parent);
                    break;
                default:
                    break;
            }
            return node;
        }

        //private void InitDataAndPath(string xmlFilePath)
        //{
        //    psdUI = (PSDUI)PSDImportUtility.DeserializeXml(xmlFilePath, typeof(PSDUI));
        //    Debug.Log(psdUI.psdSize.x + "=====psdSize======" + psdUI.psdSize.y);
        //    if (psdUI == null)
        //    {
        //        Debug.Log("The file " + xmlFilePath + " wasn't able to generate a PSDUI.");
        //        return;
        //    }

        //    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false) { return; }

        //    PSDImportUtility.baseFilename = Path.GetFileNameWithoutExtension(xmlFilePath);
        //    PSDImportUtility.baseDirectory = Path.GetDirectoryName(xmlFilePath) + "/";
        //}

        private void InitCanvas()
        {
            //EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);//  EditorApplication.NewScene ();
            Canvas temp = Resources.Load(PSDImporterConst.PREFAB_PATH_CANVAS, typeof(Canvas)) as Canvas;
            PSDImportUtility.canvas = GameObject.Instantiate(temp) as Canvas;
            PSDImportUtility.canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UnityEngine.UI.CanvasScaler scaler = PSDImportUtility.canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            scaler.referenceResolution = new Vector2(psdUI.uiSize.x, psdUI.uiSize.y);
        }

        private void LoadLayers()
        {
            for (int layerIndex = 0; layerIndex < psdUI.groups.Count; layerIndex++)
            {
                ImportLayer(psdUI.groups[layerIndex] as GroupNode, PSDImportUtility.baseDirectory);
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
            UGUINode empty = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_EMPTY,PSDImportUtility.baseFilename, PSDImportUtility.uinode);
            RectTransform rt = empty.InitComponent<RectTransform>();
            rt.sizeDelta = new Vector2(psdUI.uiSize.x, psdUI.uiSize.y);
            for (int layerIndex = 0; layerIndex < psdUI.groups.Count; layerIndex++)
            {
                DrawLayer(psdUI.groups[layerIndex] as GroupNode, empty);
            }
            AssetDatabase.Refresh();
        }

        public void BeginSetUIParents(UGUINode node)
        {
            foreach (var item in node.childs)
            {
                item.transform.SetParent(node.transform);
                BeginSetUIParents(item);
            }
        }

        public void BeginSetAnchers(UGUINode node)
        {
            foreach (var item in node.childs)
            {
                BeginSetAnchers(item);
                PSDImportUtility.SetAnchorByNode(item);
            }
        }

        public void BeginReprocess(UGUINode node)
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
            for (int layerIndex = 0; layerIndex < psdUI.groups.Count; layerIndex++)
            {
                //如果文件名有Globle，将强制全部移动到指定文件夹
                PSDImportUtility.forceMove = PSDImportUtility.baseFilename.Contains("Globle");
                MoveAsset(psdUI.groups[layerIndex] as GroupNode, PSDImportUtility.baseDirectory);
            }

            AssetDatabase.Refresh();
        }

        //--------------------------------------------------------------------------
        // private methods,按texture或image的要求导入图片到unity可加载的状态
        //-------------------------------------------------------------------------

        private void ImportLayer(GroupNode layer, string baseDirectory)
        {
            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Count; imageIndex++)
                {
                    // we need to fixup all images that were exported from PS
                    ImgNode image = layer.images[imageIndex];
                    if (image.type != ImgType.Label)
                    {
                        string texturePathName = PSDImportUtility.baseDirectory + layer.images[imageIndex].sprite + PSDImporterConst.PNG_SUFFIX;
                        TextureImporter textureImporter = AssetImporter.GetAtPath(texturePathName) as TextureImporter;

                        if (textureImporter == null) {
                            continue;//图片不存在，可能是颜色区域
                        }

                        
                        if (image.type == ImgType.Texture)
                        {
                            textureImporter.textureType = TextureImporterType.Image;
                        }
                        else
                        {
                            // modify the importer settings
                            textureImporter.textureType = TextureImporterType.Sprite;
                            textureImporter.spriteImportMode = SpriteImportMode.Single;
                            textureImporter.spritePackingTag = PSDImportUtility.baseFilename;

                            if (image.type == ImgType.AtlasImage) {
                                textureImporter.spriteBorder = new Vector4(3, 3, 3, 3);   // Set Default Slice type  UnityEngine.UI.Image's border to Vector4 (3, 3, 3, 3)
                            }
                        }
                        textureImporter.maxTextureSize = 2048;
                        AssetDatabase.WriteImportSettingsIfDirty(texturePathName);
                        AssetDatabase.ImportAsset(texturePathName);
                    }
                }
            }

            if (layer.groups != null)
            {
                for (int layerIndex = 0; layerIndex < layer.groups.Count; layerIndex++)
                {
                    ImportLayer(layer.groups[layerIndex] as GroupNode, PSDImportUtility.baseDirectory);
                }
            }
        }

        //------------------------------------------------------------------
        //when it's a common psd, then move the asset to special folder
        //------------------------------------------------------------------
        private void MoveAsset(GroupNode layer, string baseDirectory)
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

                for (int imageIndex = 0; imageIndex < layer.images.Count; imageIndex++)
                {
                    // we need to fixup all images that were exported from PS
                    ImgNode image = layer.images[imageIndex];

                    if (image.source == ImgSource.Globle || PSDImportUtility.forceMove)
                    {
                        string texturePathName = PSDImportUtility.baseDirectory + image.sprite + PSDImporterConst.PNG_SUFFIX;
                        string targetPathName = newPath + image.sprite + PSDImporterConst.PNG_SUFFIX;

                        Debug.Log(texturePathName);
                        Debug.Log(targetPathName);

                        AssetDatabase.MoveAsset(texturePathName, targetPathName);
                    }
                }
            }

            if (layer.groups != null)
            {
                for (int layerIndex = 0; layerIndex < layer.groups.Count; layerIndex++)
                {
                    MoveAsset(layer.groups[layerIndex] as GroupNode, PSDImportUtility.baseDirectory);
                }
            }
        }
    }
}