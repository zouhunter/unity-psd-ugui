using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace PSDUnity
{
    public class SliderLayerImport : ILayerImport
    {
        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(PrefabName.PREFAB_SLIDER,layer.name,parent); //GameObject.Instantiate(temp) as UnityEngine.UI.Slider;
            UnityEngine.UI.Slider slider = node.InitComponent<UnityEngine.UI.Slider>();
            switch (layer.direction)
            {
                case Direction.LeftToRight:
                    slider.direction = Slider.Direction.LeftToRight;
                    break;
                case Direction.BottomToTop:
                    slider.direction = Slider.Direction.BottomToTop;
                    break;
                case Direction.TopToBottom:
                    slider.direction = Slider.Direction.TopToBottom;
                    break;
                case Direction.RightToLeft:
                    slider.direction = Slider.Direction.RightToLeft;
                    break;
                default:
                    break;
            }
            
            bool haveHandle = false;
            for (int i = 0; i < layer.images.Count; i++)
            {
                ImgNode image = layer.images[i];
                string lowerName = image.sprite.name.ToLower();
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
                    rect.name = image.sprite.name;
                    rect.sizeDelta = new Vector2(image.rect.width,image.rect.height);
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