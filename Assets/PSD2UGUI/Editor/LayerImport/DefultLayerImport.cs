using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class DefultLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public DefultLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public void DrawLayer(Layer layer, GameObject parent)
        {
            RectTransform obj = PSDImportUtility.InstantiateItem<RectTransform>(PSDImporterConst.PREFAB_PATH_EMPTY,layer.name);
            
            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];
                    ctrl.DrawImage(image, obj.gameObject);
                }
            }

            ctrl.DrawLayers(layer.layers, obj.gameObject);
            obj.transform.SetParent(parent.transform, false); //parent.transform;
        }
    }
}