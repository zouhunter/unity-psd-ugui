using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace PSDUnity.UGUI
{
    public class ButtonLayerImport : LayerImport
    {
        public override GameObject CreateTemplate()
        {
            return new GameObject("Button", typeof(Button), typeof(RectTransform));
        }
        public ButtonLayerImport()
        {
            _suffix = "Button";
        }
        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            var node = base.CreateRootNode(layer.displayName, layer.rect, parent);
            UnityEngine.UI.Button button = node.InitComponent<UnityEngine.UI.Button>();

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Count; imageIndex++)
                {
                    Data.ImgNode image = layer.images[imageIndex];

                    if (MatchAddress(image.Name, rule.normalAddress, rule.pressedAddress, rule.disableAddress, rule.highlightedAddress))
                    {
                        if (MatchAddress(image.Name, rule.normalAddress))
                        {
                            SetRectTransform(image.rect, button.transform as RectTransform);
                        }

                        InitButton(image, button);

                        if (image.type == ImgType.Color)
                        {
                            SetColorSwipe(image, button);
                        }
                        else
                        {
                            SetSpriteSwipe(image, button);
                        }
                    }
                    else
                    {
                        if (rule.autoButtonTitle && image.type == ImgType.Label && !image.Name.StartsWith(rule.titleAddress))
                        {
                            var array = layer.images.Where(x => x.type == ImgType.Label).ToList();
                            if (array.Count > 0 && array.IndexOf(image) == 0){
                                image.Name = rule.titleAddress + image.Name;
                            }
                        }
                       
                        ctrl.DrawImage(image, node);
                    }

                }
            }
            return node;
        }

        /// <summary>
        /// 设置图片切换规则
        /// </summary>
        /// <param name="image"></param>
        /// <param name="button"></param>
        private void SetSpriteSwipe(Data.ImgNode image, UnityEngine.UI.Button button)
        {
            if (MatchAddress(image.Name, rule.normalAddress))
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
            else if (MatchAddress(image.Name, rule.pressedAddress))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
                UnityEngine.UI.SpriteState state = button.spriteState;
                state.pressedSprite = image.sprite;
                button.spriteState = state;
            }
            else if (MatchAddress(image.Name, rule.disableAddress))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
                UnityEngine.UI.SpriteState state = button.spriteState;
                state.disabledSprite = image.sprite;
                button.spriteState = state;
            }
            else if (MatchAddress(image.Name, rule.highlightedAddress))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
                UnityEngine.UI.SpriteState state = button.spriteState;
                state.highlightedSprite = image.sprite;
                button.spriteState = state;
            }
        }

        /// <summary>
        /// 设置颜色切换规则
        /// </summary>
        /// <param name="image"></param>
        /// <param name="button"></param>
        private void SetColorSwipe(Data.ImgNode image, UnityEngine.UI.Button button)
        {
            Color color = image.color;

            if (MatchAddress(image.Name, rule.normalAddress))
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
            else if (MatchAddress(image.Name, rule.pressedAddress))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
                UnityEngine.UI.ColorBlock state = button.colors;
                state.pressedColor = color;
                button.colors = state;
            }
            else if (MatchAddress(image.Name, rule.disableAddress))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
                UnityEngine.UI.ColorBlock state = button.colors;
                state.disabledColor = color;
                button.colors = state;
            }
            else if (MatchAddress(image.Name, rule.highlightedAddress))
            {
                button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
                UnityEngine.UI.ColorBlock state = button.colors;
                state.highlightedColor = color;
                button.colors = state;
            }
        }

        /// <summary>
        /// 初始化按扭为text或image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="button"></param>
        private void InitButton(Data.ImgNode image, UnityEngine.UI.Button button)
        {
            if (image.type == ImgType.Label)
            {
                var text = button.GetComponent<UnityEngine.UI.Text>();
                if (text == null)
                {
                    text = button.gameObject.AddComponent<UnityEngine.UI.Text>();

                    button.targetGraphic = text;

                    SetRectTransform(image.rect, text.rectTransform);
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


        public override void AfterGenerate(UGUINode node)
        {
            base.AfterGenerate(node);
            StretchTitle(node);
        }

    }
}