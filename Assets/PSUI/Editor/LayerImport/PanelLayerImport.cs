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
            //UnityEngine.UI.Image temp = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
                UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_IMAGE, layer.name, parent);//GameObject.Instantiate(temp) as UnityEngine.UI.Image;
            UnityEngine.UI.Image panel = node.GetCompoment<UnityEngine.UI.Image>();


            ctrl.DrawLayers(layer.layers, node);//子节点

            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];

                if (image.name.ToLower().StartsWith("b_"))
                {
                    if (image.arguments == null || image.arguments.Length == 0)
                    {
                        string assetPath = PSDImportUtility.GetPicturePath(image);
                        Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                        panel.sprite = sprite;
                    }
                    else
                    {
                        PSDImportUtility.TrySetImageColor(image, panel);
                    }

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