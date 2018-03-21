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
        public SliderLayerImport(PSDImportCtrl ctrl) : base(ctrl) { }

        public override GameObject CreateTemplate()
        {
            var slider = new GameObject("Slider", typeof(Slider)).GetComponent<Slider>();
            slider.value = 1;
            return slider.gameObject;
        }

        public override UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName,layer.rect,parent); //GameObject.Instantiate(temp) as UnityEngine.UI.Slider;
            UnityEngine.UI.Slider slider = node.InitComponent<UnityEngine.UI.Slider>();
            SetSliderDirection(slider, layer);
            for (int i = 0; i < layer.images.Count; i++)
            {
                var imgNode = layer.images[i];
                if (MatchAddress(imgNode.Name, rule.backgroundAddress))
                {
                    DrawBackground(imgNode,node);
                }
                else if (MatchAddress(imgNode.Name, rule.fillAddress))
                {
                    DrawFill(imgNode,node);
                }
                else if (MatchAddress(imgNode.Name, rule.handleAddress))
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

        private void DrawHandle(ImgNode handle, UGUINode node,GroupNode layer)
        {
            var slider = node.InitComponent<Slider>();
            ImgNode bg = layer.images.Find(x => MatchAddress(x.Name, rule.backgroundAddress));
            ImgNode fill = layer.images.Find(x => MatchAddress(x.Name, rule.fillAddress));

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

        private void DrawBackground(ImgNode bg,UGUINode node)
        {
            var bgnode = ctrl.DrawImage(bg, node);
            var graph = bgnode.InitComponent<UnityEngine.UI.Image>();
            node.InitComponent<Slider>().targetGraphic = graph;
            PSDImporterUtility.SetPictureOrLoadColor(bg, graph);
        }

        private void DrawFill(ImgNode fill, UGUINode node)
        {
            var fillAreaNode = CreateNormalNode(new GameObject("Fill Area", typeof(RectTransform)), fill.rect,  node);
            var fileNode = ctrl.DrawImage(fill, fillAreaNode);
            fileNode.InitComponent<Image>().type = Image.Type.Tiled;

            node.InitComponent<Slider>().fillRect = fileNode.InitComponent<RectTransform>();
        }

        private void SetSliderDirection(Slider slider,GroupNode layer)
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