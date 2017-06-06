using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class ToggleLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public ToggleLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_TOGGLE,layer.name,parent);// GameObject.Instantiate(temp) as UnityEngine.UI.Toggle;
            UnityEngine.UI.Toggle toggle = node.InitComponent<UnityEngine.UI.Toggle>();
            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];
                    string lowerName = image.name.ToLower();
                    if (lowerName.StartsWith("b_"))
                    {
                        PSDImportUtility.SetPictureOrLoadColor(image, toggle.targetGraphic);
                        PSDImportUtility.SetRectTransform(image, toggle.GetComponent<RectTransform>());
                    }
                    else if (lowerName.StartsWith("m_"))
                    {
                        PSDImportUtility.SetPictureOrLoadColor(image, toggle.graphic);
                    }
                    else
                    {
                        ctrl.DrawImage(image, node);
                    }
                }
            }
            return node;
        }
    }
}