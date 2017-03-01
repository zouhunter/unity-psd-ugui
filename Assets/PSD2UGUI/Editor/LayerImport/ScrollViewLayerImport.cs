using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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
        public void DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_SCROLLVIEW,layer.name,parent);
            UnityEngine.UI.ScrollRect scrollRect = node.GetCompoment<UnityEngine.UI.ScrollRect>();

            RectTransform rectTransform = scrollRect.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);

            UINode childNode = new UINode(node.transform.GetChild(0), node);
            UINode contentNode = new UINode(scrollRect.content, childNode);

            if (layer.layers != null)
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
                    default:
                        break;
                }

                ctrl.DrawLayers(layer.layers, contentNode);
            }
        }
    }
}