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

        public void DrawLayer(Layer layer, GameObject parent)
        {
            UnityEngine.UI.Image temp = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
            UnityEngine.UI.Image panel = GameObject.Instantiate(temp) as UnityEngine.UI.Image;
            panel.transform.SetParent(parent.transform, false); //parent = parent.transform;

            panel.name = layer.name;

            ctrl.DrawLayers(layer.layers, panel.gameObject);//子节点

            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];

                if (image.name.ToLower().Contains("background"))
                {
                    string assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                    Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                    panel.sprite = sprite;

                    RectTransform rectTransform = panel.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                    rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                }
                else
                {
                    ctrl.DrawImage(image, panel.gameObject);
                }
            }

        }
    }
}