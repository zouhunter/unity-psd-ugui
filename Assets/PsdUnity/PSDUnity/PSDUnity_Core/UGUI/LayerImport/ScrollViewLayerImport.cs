using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PSDUnity;
namespace PSDUnity.UGUI
{
    public class ScrollViewLayerImport : LayerImport
    {
        public ScrollViewLayerImport()
        {
            _suffix = "ScrollView";
        }
        public override void AnalysisAreguments(Data.GroupNode layer, string[] areguments)
        {
            base.AnalysisAreguments(layer, areguments);
            if (areguments != null && areguments.Length > 0)
            {
                var key = areguments[0];
                layer.direction = RuleObject.GetDirectionByKey(key);
            }
        }
        public override GameObject CreateTemplate()
        {
            var scrollRect = new GameObject("ScrollView", typeof(ScrollRect)).GetComponent<ScrollRect>();
            return scrollRect.gameObject;
        }

        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            ScrollRect scrollRect = node.InitComponent<UnityEngine.UI.ScrollRect>();
            SetScrollViewDirection(layer, scrollRect, layer.direction);
            var viewNode = DrawViewNode(layer, scrollRect, node);
            DrawImages(layer, node, scrollRect);
            DrawChildLayers(layer, viewNode, scrollRect);
            return node;
        }

        private UGUINode DrawViewNode(Data.GroupNode layer, ScrollRect scrollRect,UGUINode node)
        {
            var viewPort = new GameObject("ViewPort", typeof(Mask), typeof(Image)).GetComponent<RectTransform>();
            Color color;
            if (ColorUtility.TryParseHtmlString("#FFFFFF01", out color))
            {
                viewPort.GetComponent<UnityEngine.UI.Image>().color = color;
            }
            UGUINode viewNode = CreateNormalNode(viewPort.gameObject, layer.rect, node);
            if (scrollRect.vertical)
            {
                viewNode.anchoType = AnchoType.Up | AnchoType.XCenter;
            }
            else if (scrollRect.horizontal)
            {
                viewNode.anchoType = AnchoType.YCenter | AnchoType.Left;
            }
            else
            {
                viewNode.anchoType = AnchoType.Up | AnchoType.Left;
            }
            return viewNode;
        }

        private void DrawImages(Data.GroupNode layer, UGUINode node, ScrollRect scrollRect)
        {
            for (int i = 0; i < layer.images.Count; i++)
            {
                Data.ImgNode image = layer.images[i];

                if (MatchAddress(image.Name, rule.backgroundAddress))
                {
                    InitScrollViewBackground(node, image, scrollRect);
                }
                else
                {
                    ctrl.DrawImage(image, node);
                }
            }

        }

        private void DrawChildLayers(Data.GroupNode layer, UGUINode viewNode, ScrollRect scrollRect)
        {

            if (layer.children != null)
            {
                for (int i = 0; i < layer.children.Count; i++)
                {
                    Data.GroupNode child = layer.children[i] as Data.GroupNode;
                    UGUINode childNode = ctrl.DrawLayer(child, viewNode);

                    if (MatchAddress(child.displayName, rule.contentAddress))
                    {
                        DrawConent(childNode, scrollRect);
                    }
                    else if (MatchAddress(child.displayName, rule.vbarAddress))
                    {
                        scrollRect.verticalScrollbar = childNode.InitComponent<Scrollbar>();
                        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                        childNode.anchoType = AnchoType.Right | AnchoType.YCenter;
                    }
                    else if (MatchAddress(child.displayName, rule.hbarAddress))
                    {
                        scrollRect.horizontalScrollbar = childNode.InitComponent<Scrollbar>();
                        scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                        childNode.anchoType = AnchoType.Down | AnchoType.XCenter;
                    }
                }
            }
        }
        /// <summary>
        /// 空器
        /// </summary>
        /// <param name="childNode"></param>
        /// <param name="scrollRect"></param>
        private void DrawConent(UGUINode childNode, ScrollRect scrollRect)
        {
            childNode.inversionReprocess += (x) =>
            {
                var content = childNode.InitComponent<RectTransform>();
                scrollRect.content = content;
                scrollRect.content.anchoredPosition = Vector2.zero;
            };
        }

        private void InitScrollViewBackground(UGUINode node,Data.ImgNode image,ScrollRect scrollRect)
        {
            UnityEngine.UI.Image graph = node.InitComponent<UnityEngine.UI.Image>();
            PSDImporterUtility.SetPictureOrLoadColor(image, graph);
            SetRectTransform(image.rect, scrollRect.GetComponent<RectTransform>());
        }

        private void SetScrollViewDirection(Data.GroupNode node,ScrollRect scrollRect,Direction direction)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    scrollRect.vertical = false;
                    scrollRect.horizontal = true;
                    break;
                case Direction.Vertical:
                    scrollRect.vertical = true;
                    scrollRect.horizontal = false;
                    break;
                case Direction.Horizontal | Direction.Vertical:
                    scrollRect.vertical = true;
                    scrollRect.horizontal = true;
                    break;
                default:
                   if(node.children != null)
                    {
                        if(node.children.Find(x=>MatchAddress(x.displayName,rule.vbarAddress)) != null)
                        {
                            scrollRect.vertical = true;
                        }
                        else if(node.children.Find(x => MatchAddress(x.displayName, rule.hbarAddress)) != null)
                        {
                            scrollRect.horizontal = true;
                        }
                    }
                    break;
            }

        }
    }
}