using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PSDUnity;

namespace PSDUnity.UGUI
{
    public class PSDImportCtrl
    {
        private Dictionary<ImgType, ImageImport> imgImporterDic;
        private Dictionary<GroupType, LayerImport> layerImporterDic;
        public Canvas canvas { get; private set; }
        public  RuleObject rule { get; private set; }
        public  UGUINode uinode { get; private set; }
        public Vector2 canvasSize { get; private set; }

        public PSDImportCtrl(Canvas canvas,RuleObject rule,Vector2 canvasSize)
        {
            this.canvas = canvas;
            this.rule = rule;
            this.canvas = canvas;
            this.canvasSize = canvasSize;
            uinode = new UGUINode(canvas.transform, null);
            InitDrawers();
        }

        private void InitDrawers()
        {
            imgImporterDic = new Dictionary<ImgType, ImageImport>();
            imgImporterDic.Add(ImgType.Texture, new TextureImport(this));
            imgImporterDic.Add(ImgType.Image, new SpriteImport(this));
            imgImporterDic.Add(ImgType.AtlasImage, new SpriteImport(this));
            imgImporterDic.Add(ImgType.Color, new SpriteImport(this));
            imgImporterDic.Add(ImgType.Label, new TextImport(this));

            layerImporterDic = new Dictionary<GroupType, LayerImport>();
            layerImporterDic.Add(GroupType.SLIDER, new SliderLayerImport(this));
            layerImporterDic.Add(GroupType.INPUTFIELD, new InputFieldLayerImport(this));
            layerImporterDic.Add(GroupType.BUTTON, new ButtonLayerImport(this));
            layerImporterDic.Add(GroupType.TOGGLE, new ToggleLayerImport(this));
            layerImporterDic.Add(GroupType.PANEL, new PanelLayerImport(this));
            layerImporterDic.Add(GroupType.EMPTY, new PanelLayerImport(this));
            layerImporterDic.Add(GroupType.SCROLLVIEW, new ScrollViewLayerImport(this));
            layerImporterDic.Add(GroupType.SCROLLBAR, new ScrollBarLayerImport(this));
            layerImporterDic.Add(GroupType.GRID, new GridLayerImport(this));
            layerImporterDic.Add(GroupType.GROUP, new GroupLayerImport(this));
            layerImporterDic.Add(GroupType.DROPDOWN, new DropDownLayerImport(this));
        }

        public void Import(GroupNode rootNode)
        {
            InitBaseSize(uinode, canvasSize);
            DrawLayer(rootNode, uinode);//直接绘制所有层级
            BeginSetUIParents(uinode);//设置层级之前的父子关系
            BeginSetAnchers(uinode);//设置层级的锚点
            BeginReprocess(uinode);//后处理
            if (rule.scaleWithCanvas){
                BeginScaleWithCanvas(uinode, canvasSize);//尺寸缩放
            }
        }

        private void InitBaseSize(UGUINode uinode,Vector2 uiSize)
        {
            var rect = uinode.InitComponent<RectTransform>();
            rect.sizeDelta = uiSize;
        }

        private void BeginScaleWithCanvas(UGUINode uinode,Vector2 uiSize)
        {
            foreach (var item in uinode.childs)
            {
                var child = item.InitComponent<RectTransform>();
                child.anchorMin = Vector2.zero;
                child.anchorMax = Vector2.one;
                child.anchoredPosition = Vector2.zero;
            }
        }

        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = layerImporterDic[layer.groupType].DrawLayer(layer, parent);
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
            UGUINode node = imgImporterDic[image.type].DrawImage(image,parent);
            if(node == null)
            {
                Debug.Log(image.type);
                Debug.Log(image);
                Debug.Log(parent);
            }
            return node;
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
                PSDImporterUtility.SetAnchorByNode(item);
            }
        }

        public void BeginReprocess(UGUINode node)
        {
            foreach (var item in node.childs)
            {
                BeginReprocess(item);
            }
            node.InversionReprocess();
        }
    }
}