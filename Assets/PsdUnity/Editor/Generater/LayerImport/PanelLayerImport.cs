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
            UnityEngine.UI.Image panel = node.InitComponent<UnityEngine.UI.Image>();

            ctrl.DrawLayers(layer.layers, node);//子节点
            bool havebg = false;
            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];

                if (image.name.ToLower().StartsWith("b_"))
                {
                    havebg = true;
                    PSDImportUtility.SetPictureOrLoadColor(image, panel);
                    PSDImportUtility.SetRectTransform(image, panel.GetComponent<RectTransform>());
                    panel.name = layer.name;
                }
                else
                {
                    ctrl.DrawImage(image, node);
                }
            }
            if (!havebg)
            {
                PSDImportUtility.SetRectTransform(layer, panel.GetComponent<RectTransform>(), parent.InitComponent<RectTransform>());
                Color color;
                if (ColorUtility.TryParseHtmlString("#FFFFFF01", out color))
                {
                    panel.GetComponent<UnityEngine.UI.Image>().color = color;
                }
                panel.name = layer.name;
            }
            return node;

        }

    }
}