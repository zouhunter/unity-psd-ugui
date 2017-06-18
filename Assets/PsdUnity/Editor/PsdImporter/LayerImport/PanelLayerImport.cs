using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace PSDUnity
{
    public class PanelLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public PanelLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }

        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(GroupType.IMAGE, layer.Name, parent);//GameObject.Instantiate(temp) as UnityEngine.UI.Image;
            UnityEngine.UI.Image panel = node.InitComponent<UnityEngine.UI.Image>();

            ctrl.DrawLayers(layer.groups.ToArray(), node);//子节点
            bool havebg = false;
            for (int i = 0; i < layer.images.Count; i++)
            {
                ImgNode image = layer.images[i];

                if (image.Name.ToLower().StartsWith("b_"))
                {
                    havebg = true;
                    PSDImportUtility.SetPictureOrLoadColor(image, panel);
                    PSDImportUtility.SetRectTransform(image, panel.GetComponent<RectTransform>());
                    panel.name = layer.Name;
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
                panel.name = layer.Name;
            }
            return node;

        }

    }
}