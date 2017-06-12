using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace PSDUnity
{
    public class ButtonLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public ButtonLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }

        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_BUTTON, layer.name, parent);
            UnityEngine.UI.Button button = node.InitComponent<UnityEngine.UI.Button>();

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Count; imageIndex++)
                {
                    ImgNode image = layer.images[imageIndex];
                    string lowerName = image.name.ToLower();
                    if (image.type == ImgType.Image && lowerName.StartsWith("n_") || lowerName.StartsWith("p_") || lowerName.StartsWith("d_") || lowerName.StartsWith("h_"))
                    {
                        if (image.color == UnityEngine.Color.white)
                        {
                            SetSpriteSwipe(image, button);
                        }
                        else
                        {
                            SetColorSwipe(image, button);
                        }
                    }
                    else
                    {
                        ctrl.DrawImage(image, node);
                    }
                }
            }
            return node;
        }
        private void SetSpriteSwipe(ImgNode image,UnityEngine.UI.Button button)
        {
            string lowerName = image.sprite.name.ToLower();
            string assetPath = PSDImportUtility.baseDirectory + image.sprite + PSDImporterConst.PNG_SUFFIX;
            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;

            if (lowerName.StartsWith("n_"))
            {
                button.image.sprite = sprite;

                RectTransform rectTransform = button.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(image.rect.width, image.rect.height);
                rectTransform.anchoredPosition = new Vector2(image.rect.x, image.rect.y);
            }
            else if (lowerName.StartsWith("p_"))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
                UnityEngine.UI.SpriteState state = button.spriteState;
                state.pressedSprite = sprite;
                button.spriteState = state;
            }
            else if (lowerName.StartsWith("d_"))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
                UnityEngine.UI.SpriteState state = button.spriteState;
                state.disabledSprite = sprite;
                button.spriteState = state;
            }
            else if (lowerName.StartsWith("h_"))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
                UnityEngine.UI.SpriteState state = button.spriteState;
                state.highlightedSprite = sprite;
                button.spriteState = state;
            }
        }

        private void SetColorSwipe(ImgNode image, UnityEngine.UI.Button button)
        {
            string lowerName = image.sprite.name.ToLower();
            Color color = image.color;
           
                if (lowerName.StartsWith("n_"))
                {
                    RectTransform rectTransform = button.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(image.rect.width, image.rect.height);
                    rectTransform.anchoredPosition = new Vector2(image.rect.x, image.rect.y);

                    button.image.color = color;
                    button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
                    UnityEngine.UI.ColorBlock state = button.colors;
                    state.normalColor = color;
                    button.colors = state;
                }
                else if (lowerName.StartsWith("p_"))
                {
                    button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
                    UnityEngine.UI.ColorBlock state = button.colors;
                    state.pressedColor = color;
                    button.colors = state;
                }
                else if (lowerName.StartsWith("d_"))
                {
                    button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
                    UnityEngine.UI.ColorBlock state = button.colors;
                    state.disabledColor = color;
                    button.colors = state;
                }
                else if (lowerName.StartsWith("h_"))
                {
                    button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
                    UnityEngine.UI.ColorBlock state = button.colors;
                    state.highlightedColor = color;
                    button.colors = state;
                }
        }
    }
}