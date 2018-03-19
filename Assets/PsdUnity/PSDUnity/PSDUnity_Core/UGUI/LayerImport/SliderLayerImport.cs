using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class SliderLayerImport : ILayerImport
    {
        private PSDImportCtrl ctrl;
        public SliderLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.SLIDER, layer.displayName, parent); //GameObject.Instantiate(temp) as UnityEngine.UI.Slider;
            UnityEngine.UI.Slider slider = node.InitComponent<UnityEngine.UI.Slider>();
            PSDImporter.SetRectTransform(layer, slider.GetComponent<RectTransform>());
            slider.value = 1;

            ImgNode bg = layer.images.Find(x => x.Name.ToLower().StartsWith("b_"));
            ImgNode fill = layer.images.Find(x => x.Name.ToLower().StartsWith("f_"));
            ImgNode handle = layer.images.Find(x => x.Name.ToLower().StartsWith("h_"));

            if (bg != null)
            {
                var bgnode = ctrl.DrawImage(bg, node);
                var graph = bgnode.InitComponent<UnityEngine.UI.Image>();
                PSDImporter.SetPictureOrLoadColor(bg, graph);
            }

            if (fill != null)
            {
                var fillAreaNode = PSDImporter.InstantiateItem(GroupType.EMPTY, "Fill Area", node);
                var fileNode = ctrl.DrawImage(fill, fillAreaNode);
                slider.fillRect = fileNode.InitComponent<RectTransform>();
                fileNode.InitComponent<Image>().type = Image.Type.Tiled;
                PSDImporter.SetRectTransform(fill, fillAreaNode.InitComponent<RectTransform>());
            }

            if (handle != null && bg != null)
            {
                var tempRect = fill != null ? fill : bg;
                SetSliderDirection(slider, handle, layer);
                var handAreaNode = PSDImporter.InstantiateItem(GroupType.EMPTY, "Handle Slide Area", node);
                var rect = new Rect(tempRect.rect.x, tempRect.rect.y, tempRect.rect.width - handle.rect.width, tempRect.rect.height);//x,y 为中心点的坐标！
                PSDImporter.SetRectTransform(rect, handAreaNode.InitComponent<RectTransform>());

                var handNode = ctrl.DrawImage(handle, handAreaNode);

                ///设置handle最后的锚点类型
                switch (layer.direction)
                {
                    case Direction.LeftToRight:
                        handNode.anchoType = AnchoType.Right|AnchoType.YStretch;
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

                ///修复handleRect
                handNode.inversionReprocess += () =>
                {
                    slider.handleRect = handNode.InitComponent<RectTransform>();
                    slider.handleRect.anchoredPosition = Vector3.zero;
                    slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, handle.rect.width);
                    slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, handle.rect.height);
                };
            }
            else
            {
                SetSliderDirection(slider, layer);
            }
            return node;
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

        private void SetSliderDirection(Slider slider, ImgNode handlePos, GroupNode groupPos)
        {
            var hRect = handlePos.rect;
            var gRect = groupPos.rect;

            if (gRect.width > gRect.height)
            {
                if (hRect.x > gRect.x)
                {
                    slider.direction = Slider.Direction.LeftToRight;
                }
                else
                {
                    slider.direction = Slider.Direction.RightToLeft;
                }
            }
            else
            {
                if (hRect.y > gRect.y)
                {
                    slider.direction = Slider.Direction.BottomToTop;
                }
                else
                {
                    slider.direction = Slider.Direction.TopToBottom;
                }
            }

        }
    }
}