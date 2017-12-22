using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity.Data;
using UnityEngineInternal;
namespace PSDUnity.Import
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
            UGUINode node = PSDImporter.InstantiateItem(GroupType.BUTTON, layer.Name, parent);
            UnityEngine.UI.Button button = node.InitComponent<UnityEngine.UI.Button>();
            PSDImporter.SetRectTransform(layer, button.GetComponent<RectTransform>());

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Count; imageIndex++)
                {
                    ImgNode image = layer.images[imageIndex];
                    string lowerName = image.Name.ToLower();
                    if (lowerName.StartsWith("n_") || lowerName.StartsWith("p_") || lowerName.StartsWith("d_") || lowerName.StartsWith("h_"))
                    {
                        if (lowerName.StartsWith("n_"))
                        {
                            PSDImporter.SetRectTransform(image, button.GetComponent<RectTransform>());
                        }
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
        private void SetSpriteSwipe(ImgNode image, UnityEngine.UI.Button button)
        {
            string lowerName = image.Name.ToLower();
            InitButton(image,button);

            if (lowerName.StartsWith("n_"))
            {
                if (image.type == ImgType.Label)
                {
                    (button.targetGraphic as UnityEngine.UI.Text).text = image.text;
                }
                else
                {
                    button.image.sprite = image.sprite;
                }
            }
            else if (lowerName.StartsWith("p_"))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
                UnityEngine.UI.SpriteState state = button.spriteState;
                state.pressedSprite = image.sprite;
                button.spriteState = state;
            }
            else if (lowerName.StartsWith("d_"))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
                UnityEngine.UI.SpriteState state = button.spriteState;
                state.disabledSprite = image.sprite;
                button.spriteState = state;
            }
            else if (lowerName.StartsWith("h_"))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
                UnityEngine.UI.SpriteState state = button.spriteState;
                state.highlightedSprite = image.sprite;
                button.spriteState = state;
            }
        }

        private void InitButton(ImgNode image, UnityEngine.UI.Button button)
        {
            if (image.type == ImgType.Label)
            {
                var text = button.GetComponent<UnityEngine.UI.Text>();
                if (text == null)
                {
                    text = button.gameObject.AddComponent<UnityEngine.UI.Text>();
                    button.targetGraphic = text;
#if UNITY_EDITOR
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(text);
#endif
                }
            }
            else
            {
                var img = button.GetComponent<UnityEngine.UI.Image>();
                if (img == null)
                {
                    img = button.gameObject.AddComponent<UnityEngine.UI.Image>();
                    button.targetGraphic = img;
#if UNITY_EDITOR
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(img);
#endif
                }
            }
        }

        private void SetColorSwipe(ImgNode image, UnityEngine.UI.Button button)
        {
            string lowerName = image.Name.ToLower();
            Color color = image.color;

            InitButton(image, button);

            if (lowerName.StartsWith("n_"))
            {
                if (image.type == ImgType.Label)
                {
                    (button.targetGraphic as UnityEngine.UI.Text).text = image.text;
                }

                RectTransform rectTransform = button.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(image.rect.width, image.rect.height);
                rectTransform.anchoredPosition = new Vector2(image.rect.x, image.rect.y);

                button.targetGraphic.color = color;
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