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
        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_EMPTY,layer.name,parent);
            RectTransform obj = node.GetCompoment<RectTransform>();
            PSDImportUtility.SetRectTransform(layer, obj,parent.GetCompoment<RectTransform>());

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];
                    ctrl.DrawImage(image, node);
                }
            }

            ctrl.DrawLayers(layer.layers, node);
            return node;
        }
    }
}