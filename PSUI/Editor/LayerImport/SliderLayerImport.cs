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
        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_SLIDER,layer.name,parent); //GameObject.Instantiate(temp) as UnityEngine.UI.Slider;
            UnityEngine.UI.Slider slider = node.InitComponent<UnityEngine.UI.Slider>();
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
            bool haveHandle = false;
            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];
                string lowerName = image.name.ToLower();
                UnityEngine.UI.Image graph = null;
                
                if (lowerName.StartsWith("b_"))
                {
                    graph = slider.transform.Find("Background").GetComponent<UnityEngine.UI.Image>();
                    PSDImportUtility.SetRectTransform(image, slider.GetComponent<RectTransform>());
                    slider.name = layer.name;
                }
                else if (lowerName.StartsWith("f_"))
                {
                    graph = slider.fillRect.GetComponent<UnityEngine.UI.Image>();
                }
                else if (lowerName.StartsWith("h_"))
                {
                    graph = slider.handleRect.GetComponent<UnityEngine.UI.Image>();
                    RectTransform rect = graph.GetComponent<RectTransform>();
                    rect.name = image.name;
                    rect.sizeDelta = new Vector2(image.size.width,image.size.height);
                    rect.anchoredPosition = Vector2.zero;
                    haveHandle = true;
                }

                if (graph == null)
                {
                    continue;
                }

                PSDImportUtility.SetPictureOrLoadColor(image, graph);
            }

            if (!haveHandle)
            {
                UnityEngine.Object.DestroyImmediate(slider.handleRect.parent.gameObject);
            }
            return node;
        }
    }
}