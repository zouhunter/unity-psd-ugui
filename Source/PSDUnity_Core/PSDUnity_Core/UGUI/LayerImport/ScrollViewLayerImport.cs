using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PSDUnity;
namespace PSDUnity.UGUI
{
    public class ScrollViewLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public ScrollViewLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.SCROLLVIEW,layer.Name,parent);
            UnityEngine.UI.ScrollRect scrollRect = node.InitComponent<UnityEngine.UI.ScrollRect>();

            UGUINode childNode = PSDImporter.InstantiateItem(GroupType.IMAGE, "Viewport", node);
            scrollRect.viewport = childNode.InitComponent<RectTransform>();
            Color color;
            if (ColorUtility.TryParseHtmlString("#FFFFFF01",out color))
            {
                childNode.InitComponent<UnityEngine.UI.Image>().color = color;
            }
            childNode.InitComponent<Mask>();
            childNode.anchoType = AnchoType.XStretch | AnchoType.YStretch;

            bool havebg = false;
            for (int i = 0; i < layer.images.Count; i++)
            {
                ImgNode image = layer.images[i];

                if (image.Name.ToLower().StartsWith("b_"))
                {
                    havebg = true;
                    UnityEngine.UI.Image graph = node.InitComponent<UnityEngine.UI.Image>();

                    PSDImporter.SetPictureOrLoadColor(image, graph);
                    PSDImporter.SetRectTransform(image, scrollRect.GetComponent<RectTransform>());
                }
                else
                {
                    ctrl.DrawImage(image, node);
                }
            }

            if (!havebg)
            {
                PSDImporter.SetRectTransform(layer, scrollRect.GetComponent<RectTransform>());
            }

            PSDImporter.SetRectTransform(layer, childNode.InitComponent<RectTransform>());

            switch (layer.direction)
            {
                case Direction.Horizontal:
                    scrollRect.vertical = true;
                    scrollRect.horizontal = false;
                    break;
                case Direction.Vertical:
                    scrollRect.vertical = false;
                    scrollRect.horizontal = true;
                    break;
                case Direction.Horizontal | Direction.Vertical:
                    scrollRect.vertical = true;
                    scrollRect.horizontal = true;
                    break;
                default:
                    break;
            }


            if (layer.children != null)
            {
                for (int i = 0; i < layer.children.Count; i++)
                {
                    GroupNode child = layer.children[i] as GroupNode;
                    string childLowerName = child.Name;

                    UGUINode c_Node = ctrl.DrawLayer(child, childNode);

                    if (childLowerName.StartsWith("c_"))
                    {
                        scrollRect.content = c_Node.InitComponent<RectTransform>();
                    }
                    else if (childLowerName.StartsWith("vb_"))
                    {
                        scrollRect.verticalScrollbar = c_Node.InitComponent<Scrollbar>();
                        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                    }
                    else if (childLowerName.StartsWith("hb_"))
                    {
                        scrollRect.horizontalScrollbar = c_Node.InitComponent<Scrollbar>();
                        scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                    }
                }
            }
            return node;
        }
    }
}