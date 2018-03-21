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
        public ScrollViewLayerImport(PSDImportCtrl ctrl) : base(ctrl) { }

        public override GameObject CreateTemplate()
        {
            var scrollRect = new GameObject("ScrollView", typeof(ScrollRect)).GetComponent<ScrollRect>();
            scrollRect.viewport = new GameObject("ViewPort",typeof(Mask),typeof(Image)).GetComponent<RectTransform>();
            scrollRect.viewport.transform.SetParent(scrollRect.transform,false);
            Color color;
            if (ColorUtility.TryParseHtmlString("#FFFFFF01", out color)){
                scrollRect.viewport.GetComponent<UnityEngine.UI.Image>().color = color;
            }
            return scrollRect.gameObject;
        }

        public override UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            ScrollRect scrollRect = node.InitComponent<UnityEngine.UI.ScrollRect>();
            SetScrollViewDirection(layer, scrollRect, layer.direction);
            UGUINode viewNode = CreateNormalNode(scrollRect.viewport.gameObject, layer.rect, node);
            DrawImages(layer, node, scrollRect);
            DrawChildLayers(layer, viewNode, scrollRect);
            return node;
        }

        private void DrawImages(GroupNode layer, UGUINode node, ScrollRect scrollRect)
        {
            for (int i = 0; i < layer.images.Count; i++)
            {
                ImgNode image = layer.images[i];

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

        private void DrawChildLayers(GroupNode layer, UGUINode viewNode, ScrollRect scrollRect)
        {

            if (layer.children != null)
            {
                for (int i = 0; i < layer.children.Count; i++)
                {
                    GroupNode child = layer.children[i] as GroupNode;
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
            scrollRect.content = childNode.InitComponent<RectTransform>();
            childNode.inversionReprocess += (x) =>
            {
                childNode.InitComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                childNode.InitComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                if(scrollRect.vertical)
                {
                    scrollRect.content.pivot = new Vector2(0.5f, 1);
                    PSDImporterUtility.SetNormalAnchor(AnchoType.Up | AnchoType.XCenter, scrollRect.viewport, scrollRect.content);
                }
                else if(scrollRect.horizontal)
                {
                    scrollRect.content.pivot = new Vector2(0, 0.5f);
                    PSDImporterUtility.SetNormalAnchor(AnchoType.YCenter | AnchoType.Left, scrollRect.viewport, scrollRect.content);
                }
                else
                {
                    scrollRect.content.pivot = Vector2.zero;
                    PSDImporterUtility.SetNormalAnchor(AnchoType.Up | AnchoType.Left, scrollRect.viewport, scrollRect.content);
                }
                scrollRect.content.anchoredPosition = Vector2.zero;
            };
        }

        private void InitScrollViewBackground(UGUINode node,ImgNode image,ScrollRect scrollRect)
        {
            UnityEngine.UI.Image graph = node.InitComponent<UnityEngine.UI.Image>();
            PSDImporterUtility.SetPictureOrLoadColor(image, graph);
            SetRectTransform(image.rect, scrollRect.GetComponent<RectTransform>());
        }

        private void SetScrollViewDirection(GroupNode node,ScrollRect scrollRect,Direction direction)
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