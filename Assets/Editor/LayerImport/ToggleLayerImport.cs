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
            this.ctrl = ctrl; Debug.Log(ctrl);
        }
        public void DrawLayer(Layer layer, GameObject parent)
        {
            UnityEngine.UI.Toggle temp = Resources.Load(PSDImporterConst.PREFAB_PATH_TOGGLE, typeof(UnityEngine.UI.Toggle)) as UnityEngine.UI.Toggle;
            UnityEngine.UI.Toggle toggle = GameObject.Instantiate(temp) as UnityEngine.UI.Toggle;
            toggle.transform.SetParent(parent.transform, false);//.parent = parent.transform;


            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];

                    if (image.name.Contains("background"))
                    {
                        if (image.imageSource == ImageSource.Custom)
                        {
                            string assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                            toggle.image.sprite = sprite;

                            RectTransform rectTransform = toggle.GetComponent<RectTransform>();
                            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                        }
                    }
                    else if (image.name.Contains("mask"))
                    {
                        if (image.imageSource == ImageSource.Custom)
                        {
                            string assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                            toggle.graphic.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                        }
                    }
                    else if (image.name.Contains("title"))
                    {
                        //生成文字 
                        UnityEngine.UI.Text text = toggle.GetComponentInChildren<UnityEngine.UI.Text>();
                        Color color;
                        if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + image.arguments[0]), out color))
                        {
                            text.color = color;
                        }

                        int size;
                        if (int.TryParse(image.arguments[2], out size))
                        {
                            text.fontSize = size;
                        }
                    }
                }
            }
        }
    }
}