using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace PSDUIImporter
{
    public class ButtonLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public ButtonLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl; Debug.Log(ctrl);
        }
      
        public void DrawLayer(Layer layer, GameObject parent)
        {
            UnityEngine.UI.Button button = PSDImportUtility.InstantiateItem<UnityEngine.UI.Button>(PSDImporterConst.PREFAB_PATH_BUTTON, layer.name, parent.transform);
            //UnityEngine.UI.Button temp = Resources.Load(PSDImporterConst.PREFAB_PATH_BUTTON, typeof(UnityEngine.UI.Button)) as UnityEngine.UI.Button;
            //UnityEngine.UI.Button button = GameObject.Instantiate(temp) as UnityEngine.UI.Button;
            //button.transform.SetParent(parent.transform, false);//.parent = parent.transform;
            //button.name = layer.name;
            //GameObject temp = Resources.Load(PSDImporterConst.PREFAB_PATH_BUTTON, typeof(GameObject)) as GameObject;
            //GameObject item = GameObject.Instantiate(temp) as GameObject;
            //item.transform.SetParent(parent.transform, false);//.parent = parent.transform;
            //UnityEngine.UI.Button button = item.GetComponent<UnityEngine.UI.Button>();
            //item.name = layer.name;

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];
                    if (image.imageType == ImageType.Label)
                    {
                        if (image.name.ToLower().Contains("title"))
                        {
                            //生成文字 
                            UnityEngine.UI.Text text = button.GetComponentInChildren<UnityEngine.UI.Text>();
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
                    else
                    {
                        if (image.name.ToLower().Contains("normal"))
                        {
                            if (image.imageSource == ImageSource.Custom)
                            {
                                string assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                                button.image.sprite = sprite;

                                RectTransform rectTransform = button.GetComponent<RectTransform>();
                                rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                                rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
        }
    }
}