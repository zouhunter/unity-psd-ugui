using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class ScrollViewLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public ScrollViewLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_SCROLLVIEW,layer.name,parent);
            UnityEngine.UI.ScrollRect scrollRect = node.GetComponent<UnityEngine.UI.ScrollRect>();

            UINode childNode = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_IMAGE, "Viewport", node);
            scrollRect.viewport = childNode.GetComponent<RectTransform>();
            Color color;
            if (ColorUtility.TryParseHtmlString("#FFFFFF01",out color))
            {
                childNode.GetComponent<UnityEngine.UI.Image>().color = color;
                Debug.Log(color);
            }
            childNode.AddComponent<Mask>();
            childNode.anchoType = UINode.AnchoType.XStretch | UINode.AnchoType.YStretch;

            UnityEngine.UI.Image graph = scrollRect.GetComponent<UnityEngine.UI.Image>();
            bool havebg = false;
            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];

                if (image.name.ToLower().StartsWith("b_"))
                {
                    havebg = true;

                    PSDImportUtility.SetPictureOrLoadColor(image, graph);

                    PSDImportUtility.SetRectTransform(image, graph.GetComponent<RectTransform>());
                }
                else
                {
                    ctrl.DrawImage(image, node);
                }
            }

            if (!havebg)
            {
                UnityEngine.Object.DestroyImmediate(graph);
                PSDImportUtility.SetRectTransform(layer, scrollRect.GetComponent<RectTransform>(),parent.GetComponent<RectTransform>());
            }

            PSDImportUtility.SetRectTransform(layer, childNode.GetComponent<RectTransform>(), scrollRect.GetComponent<RectTransform>());

            if (layer.arguments != null)
            {
                string type = layer.arguments[0].ToUpper();
                switch (type)
                {
                    case "V":
                        scrollRect.vertical = true;
                        scrollRect.horizontal = false;
                        break;
                    case "H":
                        scrollRect.vertical = false;
                        scrollRect.horizontal = true;
                        break;
                    case "VH":
                    case "HV":
                        scrollRect.vertical = true;
                        scrollRect.horizontal = true;
                        break;
                    default:
                        break;
                }
            }

            if (layer.layers != null)
            {
                for (int i = 0; i < layer.layers.Length; i++)
                {
                    Layer child = layer.layers[i];
                    string childLowerName = child.name;
                    UINode c_Node = ctrl.DrawLayer(child, childNode);

                    if (childLowerName.StartsWith("c_"))
                    {
                        scrollRect.content = c_Node.GetComponent<RectTransform>();
                    }
                    else if (childLowerName.StartsWith("vb_"))
                    {
                        scrollRect.verticalScrollbar = c_Node.GetComponent<Scrollbar>();
                        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                    }
                    else if (childLowerName.StartsWith("hb_"))
                    {
                        scrollRect.horizontalScrollbar = c_Node.GetComponent<Scrollbar>();
                        scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                    }
                }
            }
            return node;
        }
    }
}