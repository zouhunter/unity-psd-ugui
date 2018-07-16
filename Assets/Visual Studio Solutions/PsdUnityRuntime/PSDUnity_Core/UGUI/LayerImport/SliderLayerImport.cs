using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class SliderLayerImport : LayerImport
    {
        [Header("[前缀-----------------------------------")]
        [SerializeField, CustomField("背景图片")] protected string backgroundAddress = "b_";
        [SerializeField, CustomField("填充图片")] protected string fillAddress = "f_";
        [SerializeField, CustomField("手柄图标")] protected string handleAddress = "h_";

        [Header("[参数-----------------------------------")]
        [SerializeField,CustomField("从左到右")] protected string left_right = "l";
        [SerializeField,CustomField("从右到左")] protected string right_left = "r";
        [SerializeField,CustomField("从下到上")] protected string bottom_top = "b";
        [SerializeField,CustomField("从上到下")] protected string top_bottom = "l";


        public Direction GetDirectionByKey(string key)
        {
            Direction dir = 0;

            if (string.Compare(key, bottom_top, true) == 0)
            {
                dir = Direction.BottomToTop;
            }
            else if (string.Compare(key, top_bottom, true) == 0)
            {
                dir = Direction.TopToBottom;
            }
            else if (string.Compare(key, left_right, true) == 0)
            {
                dir = Direction.LeftToRight;
            }
            else if (string.Compare(key, right_left, true) == 0)
            {
                dir = Direction.RightToLeft;
            }

            return dir;
        }
        public SliderLayerImport()
        {
            _suffix = "Slider";
        }

        public override void AnalysisAreguments(Data.GroupNode layer, string[] areguments)
        {
            base.AnalysisAreguments(layer, areguments);
            if (areguments != null && areguments.Length > 0)
            {
                var key = areguments[0];
                layer.direction = GetDirectionByKey(key);
            }
        }
        public override GameObject CreateTemplate()
        {
            var slider = new GameObject("Slider", typeof(Slider)).GetComponent<Slider>();
            slider.value = 1;
            return slider.gameObject;
        }

        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName,layer.rect,parent); //GameObject.Instantiate(temp) as UnityEngine.UI.Slider;
            UnityEngine.UI.Slider slider = node.InitComponent<UnityEngine.UI.Slider>();
            SetSliderDirection(slider, layer);
            for (int i = 0; i < layer.images.Count; i++)
            {
                var imgNode = layer.images[i];
                if (MatchAddress(imgNode.Name, backgroundAddress))
                {
                    DrawBackground(imgNode,node);
                }
                else if (MatchAddress(imgNode.Name, fillAddress))
                {
                    DrawFill(imgNode,node);
                }
                else if (MatchAddress(imgNode.Name, handleAddress))
                {
                    DrawHandle(imgNode, node,layer);
                }
                else
                {
                    ctrl.DrawImage(imgNode,node);
                }
            }
            return node;
        }

        private void DrawHandle(Data.ImgNode handle, UGUINode node,Data.GroupNode layer)
        {
            var slider = node.InitComponent<Slider>();
            Data.ImgNode bg = layer.images.Find(x => MatchAddress(x.Name, backgroundAddress));
            Data.ImgNode fill = layer.images.Find(x => MatchAddress(x.Name, fillAddress));

            var tempRect = fill != null ? fill : bg;
            var rect = new Rect(tempRect.rect.x, tempRect.rect.y, tempRect.rect.width - handle.rect.width, tempRect.rect.height);//x,y 为中心点的坐标！
            var handAreaNode = CreateNormalNode(new GameObject("Handle Slide Area", typeof(RectTransform)), rect,  node);
            var handNode = ctrl.DrawImage(handle, handAreaNode);
            slider.handleRect = handNode.InitComponent<RectTransform>();

            ///设置handle最后的锚点类型
            switch (layer.direction)
            {
                case Direction.LeftToRight:
                    handNode.anchoType = AnchoType.Right | AnchoType.YStretch;
                    break;
                case Direction.BottomToTop:
                    handNode.anchoType = AnchoType.Up | AnchoType.XStretch;
                    break;
                case Direction.TopToBottom:
                    handNode.anchoType = AnchoType.Down | AnchoType.XStretch;
                    break;
                case Direction.RightToLeft:
                    handNode.anchoType = AnchoType.Left | AnchoType.YStretch;
                    break;
                default:
                    break;
            }
            
            handNode.inversionReprocess += (n) =>
            {
                //写slider进行关联后,尺寸信息丢失
                slider.handleRect.anchoredPosition = Vector3.zero;
                slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, handle.rect.width);
                slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, handle.rect.height);
            };
        }

        private void DrawBackground(Data.ImgNode bg,UGUINode node)
        {
            var bgnode = ctrl.DrawImage(bg, node);
            var graph = bgnode.InitComponent<UnityEngine.UI.Image>();
            node.InitComponent<Slider>().targetGraphic = graph;
            PSDImporterUtility.SetPictureOrLoadColor(bg, graph);
        }

        private void DrawFill(Data.ImgNode fill, UGUINode node)
        {
            var fillAreaNode = CreateNormalNode(new GameObject("Fill Area", typeof(RectTransform)), fill.rect,  node);
            var fileNode = ctrl.DrawImage(fill, fillAreaNode);
            fileNode.InitComponent<Image>().type = Image.Type.Tiled;

            node.InitComponent<Slider>().fillRect = fileNode.InitComponent<RectTransform>();
        }

        private void SetSliderDirection(Slider slider,Data.GroupNode layer)
        {
            var dir = layer.direction;
            switch (dir)
            {
                case Direction.LeftToRight:
                    slider.direction = Slider.Direction.LeftToRight;
                    break;
                case Direction.RightToLeft:
                    slider.direction = Slider.Direction.RightToLeft;
                    break;

                case Direction.BottomToTop:
                    slider.direction = Slider.Direction.BottomToTop;
                    break;
                case Direction.TopToBottom:
                    slider.direction = Slider.Direction.TopToBottom;
                    break;
                default:
                    if (layer.rect.width > layer.rect.height)
                    {
                        slider.direction = Slider.Direction.LeftToRight;
                    }
                    else
                    {
                        slider.direction = Slider.Direction.BottomToTop;
                    }
                    break;
            }
        }
    }
}