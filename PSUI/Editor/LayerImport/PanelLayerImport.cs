using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace PSDUIImporter
{
    public class PanelLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public PanelLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }

        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_IMAGE, layer.name, parent);//GameObject.Instantiate(temp) as UnityEngine.UI.Image;
            UnityEngine.UI.Image panel = node.GetComponent<UnityEngine.UI.Image>();
            PSDImportUtility.SetRectTransform(layer, panel.GetComponent<RectTransform>(),parent.GetComponent<RectTransform>());

            ctrl.DrawLayers(layer.layers, node);//子节点

            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];

                if (image.name.ToLower().StartsWith("b_"))
                {
                    PSDImportUtility.SetPictureOrLoadColor(image, panel);
                    PSDImportUtility.SetRectTransform(image, panel.GetComponent<RectTransform>());
                    panel.name = layer.name;
                }
                else
                {
                    ctrl.DrawImage(image, node);
                }
            }
            return node;

        }

    }
}