using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class ScrollBarLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public ScrollBarLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
            Debug.Log(this.ctrl);


        }
        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_SCROLLBAR, layer.name, parent);
            Scrollbar scrollBar = node.GetCompoment<Scrollbar>();

            string type = layer.arguments[0].ToUpper();
            switch (type)
            {
                case "R":
                    scrollBar.direction = Scrollbar.Direction.RightToLeft;
                    break;
                case "L":
                    scrollBar.direction = Scrollbar.Direction.LeftToRight;
                    break;
                case "T":
                    scrollBar.direction = Scrollbar.Direction.TopToBottom;
                    break;
                case "B":
                    scrollBar.direction = Scrollbar.Direction.BottomToTop;
                    break;
                default:
                    break;
            }

            float pecent;
            if (float.TryParse(layer.arguments[1], out pecent))
            {
                scrollBar.size = pecent;
            }


            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];
                string lowerName = image.name.ToLower();
                UnityEngine.UI.Image graph = null;

                if (lowerName.StartsWith("b_"))
                {
                    graph = scrollBar.GetComponent<UnityEngine.UI.Image>();
                    PSDImportUtility.SetRectTransform(image, scrollBar.GetComponent<RectTransform>());
                }
                else if (lowerName.StartsWith("h_"))
                {
                    graph = scrollBar.handleRect.GetComponent<UnityEngine.UI.Image>();
                }

                if (graph == null)
                {
                    //忽略Scorllbar其他的artLayer
                    continue;
                }

                PSDImportUtility.SetPictureOrLoadColor(image, graph);
            }
            return node;
        }
    }
}
