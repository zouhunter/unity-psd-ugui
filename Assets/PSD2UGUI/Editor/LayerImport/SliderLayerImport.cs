using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace PSDUIImporter
{
    public class SliderLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public SliderLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public void DrawLayer(Layer layer, GameObject parent)
        {
            //UnityEngine.UI.Slider temp = Resources.Load(PSDImporterConst.PREFAB_PATH_SLIDER, typeof(UnityEngine.UI.Slider)) as UnityEngine.UI.Slider;
            UnityEngine.UI.Slider slider = PSDImportUtility.InstantiateItem<UnityEngine.UI.Slider>(PSDImporterConst.PREFAB_PATH_SLIDER,layer.name,parent); //GameObject.Instantiate(temp) as UnityEngine.UI.Slider;

            RectTransform rectTransform = slider.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);

            //slider.transform.SetParent(parent.transform, true); //parent = parent.transform;

            PosLoader posloader = slider.gameObject.AddComponent<PosLoader>();
            posloader.worldPos = rectTransform.position;

            string type = layer.arguments[0].ToUpper();
            switch (type)
            {
                case "R":
                    slider.direction = Slider.Direction.RightToLeft;
                    break;
                case "L":
                    slider.direction = Slider.Direction.LeftToRight;
                    break;
                case "T":
                    slider.direction = Slider.Direction.TopToBottom;
                    break;
                case "B":
                    slider.direction = Slider.Direction.BottomToTop;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];
                string assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX; Debug.Log("==  CommonImagePath  ====" + assetPath);
                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;

                if (image.name.ToLower().Contains("background"))
                {
                    slider.transform.Find("Background").GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                }
                else if (image.name.ToLower().Contains("fill"))
                {
                    slider.fillRect.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                }
                else if (image.name.ToLower().Contains("handle"))
                {
                    slider.handleRect.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                }
            }
        }
    }
}