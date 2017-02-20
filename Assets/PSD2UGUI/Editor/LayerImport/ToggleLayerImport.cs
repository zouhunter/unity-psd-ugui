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
        public void DrawLayer(Layer layer, GameObject parent)
        {
            //UnityEngine.UI.Toggle temp = Resources.Load(PSDImporterConst.PREFAB_PATH_TOGGLE, typeof(UnityEngine.UI.Toggle)) as UnityEngine.UI.Toggle;
            UnityEngine.UI.Toggle toggle = PSDImportUtility.InstantiateItem<UnityEngine.UI.Toggle>(PSDImporterConst.PREFAB_PATH_TOGGLE,layer.name,parent);// GameObject.Instantiate(temp) as UnityEngine.UI.Toggle;

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];

                    if (image.name.ToLower().Contains("background"))
                    {
                        if (image.imageSource == ImageSource.Normal || image.imageSource == ImageSource.Custom)
                        {
                            string assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                            toggle.image.sprite = sprite;

                            RectTransform rectTransform = toggle.GetComponent<RectTransform>();
                            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);

                            //rectTransform.SetParent(parent.transform, true);

                            //PosLoader posloader = toggle.gameObject.AddComponent<PosLoader>();
                            //posloader.worldPos = rectTransform.position;
                        }
                    }
                    else if (image.name.ToLower().Contains("mask"))
                    {
                        if (image.imageSource == ImageSource.Normal || image.imageSource == ImageSource.Custom)
                        {
                            string assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                            toggle.graphic.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                        }
                    }
                    else
                    {
                        ctrl.DrawImage(image, toggle.graphic.gameObject);
                    }
                }
            }
        }
    }
}