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
            UnityEngine.UI.ScrollRect scrollRect = node.GetCompoment<UnityEngine.UI.ScrollRect>();

            UINode childNode = new UINode(node.transform.GetChild(0), node);

            UnityEngine.UI.Image graph = scrollRect.GetComponent<UnityEngine.UI.Image>();
            bool havebg = false;
            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];

                if (image.name.ToLower().StartsWith("b_"))
                {
                    havebg = true;
                    
                    if (image.arguments == null || image.arguments.Length == 0)
                    {
                        string assetPath = PSDImportUtility.GetPicturePath(image);
                        Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                        graph.sprite = sprite;
                    }
                    else
                    {
                        PSDImportUtility.TrySetImageColor(image, graph);
                    }

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
                PSDImportUtility.SetRectTransform(layer, scrollRect.GetComponent<RectTransform>());
            }

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
                    case "VH":
                    case "HV":
                        scrollRect.vertical = true;
                        scrollRect.horizontal = true;
                        break;
                    default:
                        break;
                }

                ctrl.DrawLayers(layer.layers, childNode);
            }
            return node;
        }
    }
}