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
            this.ctrl = ctrl; Debug.Log(ctrl);
        }
        public void DrawLayer(Layer layer, GameObject parent)
        {
            GameObject obj = PSDImportUtility.CreateEmptyParent(layer.name);
            obj.transform.SetParent(parent.transform, false); //parent.transform;

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];
                    ctrl.DrawImage(image, obj);
                }
            }

            ctrl.DrawLayers(layer.layers, obj);
        }
    }
}