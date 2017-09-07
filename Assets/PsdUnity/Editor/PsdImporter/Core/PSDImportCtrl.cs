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

using PSDUnity.Data;
namespace PSDUnity.Import
{
    public class PSDImportCtrl
    {
        private IImageImport spriteImport;
        private IImageImport textImport;

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


        public PSDImportCtrl()
        {
            InitDrawers();
        }

        public void Import(GroupNode1[] gourps, Vector2 uiSize)
        {
            BeginDrawUILayers(gourps, uiSize);
            BeginSetUIParents(PSDImportUtility.uinode);
            BeginSetAnchers(PSDImportUtility.uinode.childs[0]);

            //最外层的要单独处理
            var rp = PSDImportUtility.uinode.InitComponent<RectTransform>();
            var rt = PSDImportUtility.uinode.childs[0].InitComponent<RectTransform>();
            PSDImportUtility.SetNormalAnchor(AnchoType.XCenter|AnchoType.YCenter,rt, rp);

            BeginReprocess(PSDImportUtility.uinode.childs[0]);//后处理
        }

        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = null;
            switch (layer.groupType)
            {
                case GroupType.EMPTY:
                case GroupType.IMAGE:
                    node = panelImport.DrawLayer(layer, parent);
                    break;
                case GroupType.BUTTON:
                    node = buttonImport.DrawLayer(layer, parent);
                    break;
                case GroupType.TOGGLE:
                    node = toggleImport.DrawLayer(layer, parent);
                    break;
                case GroupType.GRID:
                    node = gridImport.DrawLayer(layer, parent);
                    break;
                case GroupType.SCROLLVIEW:
                    node = scrollViewImport.DrawLayer(layer, parent);
                    break;
                case GroupType.SLIDER:
                    node = sliderImport.DrawLayer(layer, parent);
                    break;
                case GroupType.GROUP:
                    node = groupImport.DrawLayer(layer, parent);
                    break;
                case GroupType.INPUTFIELD:
                    node = inputFiledImport.DrawLayer(layer, parent);
                    break;
                case GroupType.SCROLLBAR:
                    node = scrollBarImport.DrawLayer(layer, parent);
                    break;
                case GroupType.DROPDOWN:
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

        public UGUINode[] DrawImages(ImgNode[] images, UGUINode parent)
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
                case ImgType.Color:
                    node = spriteImport.DrawImage(image, parent);
                    break;
                case ImgType.Label:
                    node = textImport.DrawImage(image, parent);
                    break;
                default:
                    break;
            }
            if(node == null)
            {
                Debug.Log(image.type);
                Debug.Log(image);
                Debug.Log(parent);
            }
            return node;
        }

        private void InitDrawers()
        {
            spriteImport = new SpriteImport();
            textImport = new TextImport();
            sliderImport = new SliderLayerImport(this);
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

        public void BeginDrawUILayers(GroupNode1[] groups, Vector2 uiSize)
        {
            UGUINode empty = PSDImportUtility.InstantiateItem(GroupType.EMPTY, "PSDUnity", PSDImportUtility.uinode);
            RectTransform rt = empty.InitComponent<RectTransform>();
            rt.sizeDelta = new Vector2(uiSize.x, uiSize.y);
            for (int layerIndex = 0; layerIndex < groups.Length; layerIndex++)
            {
                DrawLayer(groups[layerIndex] as GroupNode, empty);
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
            }
            if (node.ReprocessEvent != null)
            {
                node.ReprocessEvent();
            }
        }
    }
}