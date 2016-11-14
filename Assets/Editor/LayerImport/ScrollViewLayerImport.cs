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
            this.ctrl = ctrl; Debug.Log(ctrl);
        }
        public void DrawLayer(Layer layer, GameObject parent)
        {
            UnityEngine.UI.ScrollRect temp = Resources.Load(PSDImporterConst.PREFAB_PATH_SCROLLVIEW, typeof(UnityEngine.UI.ScrollRect)) as UnityEngine.UI.ScrollRect;
            UnityEngine.UI.ScrollRect scrollRect = GameObject.Instantiate(temp) as UnityEngine.UI.ScrollRect;
            scrollRect.transform.SetParent(parent.transform, false); //parent = parent.transform;

            RectTransform rectTransform = scrollRect.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);

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

                ctrl.DrawLayers(layer.layers, scrollRect.content.gameObject);
            }
        }
    }
}