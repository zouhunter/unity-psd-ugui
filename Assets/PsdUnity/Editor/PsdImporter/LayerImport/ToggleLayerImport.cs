using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUnity
{
    public class ToggleLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public ToggleLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_TOGGLE,layer.name,parent);// GameObject.Instantiate(temp) as UnityEngine.UI.Toggle;
            UnityEngine.UI.Toggle toggle = node.InitComponent<UnityEngine.UI.Toggle>();
            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Count; imageIndex++)
                {
                    ImgNode image = layer.images[imageIndex];
                    string lowerName = image.clampname.ToLower();
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