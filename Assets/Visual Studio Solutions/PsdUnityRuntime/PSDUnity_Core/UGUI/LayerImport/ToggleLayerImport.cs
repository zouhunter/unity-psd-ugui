using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace PSDUnity.UGUI
{
    public class ToggleLayerImport : LayerImport
    {
        [Header("[前缀-----------------------------------")]
        [SerializeField, CustomField("标题")]
        protected string titleAddress = "t_";
        [SerializeField, CustomField("背景")]
        protected string backgroundAddress = "b_";
        [SerializeField, CustomField("遮罩")]
        protected string maskAddress = "m_";
        [Header("[可选-----------------------------------")]
        //自动将按扭下的文字当作标题
        [SerializeField, CustomField("查找标题")]
        protected bool autoToggleTitle = true;

        public ToggleLayerImport()
        {
            _suffix = "Toggle";
        }

        public override GameObject CreateTemplate()
        {
            var toggle = new GameObject("Toggle", typeof(Toggle)).GetComponent<Toggle>();

            var background = new GameObject("Background", typeof(RectTransform));
            var mask = new GameObject("Mask", typeof(RectTransform));

            mask.transform.SetParent(background.transform, false);
            background.transform.SetParent(toggle.transform, false);
            Debug.Log(background.transform.parent);
            return toggle.gameObject;
        }

        private T SetTargetGraphic<T>(Toggle toggle) where T : Graphic
        {
            if (toggle.targetGraphic == null)
            {
                var background = toggle.transform.Find("Background").gameObject.AddComponent<T>();
                toggle.targetGraphic = background;
            }
            return toggle.targetGraphic as T;
        }

        private T SetGraphic<T>(Toggle toggle) where T : Graphic
        {
            if (toggle.graphic == null)
            {
                var mask = toggle.targetGraphic.transform.Find("Mask").gameObject.AddComponent<T>();
                toggle.graphic = mask;
            }
            return toggle.graphic as T;
        }

        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            UnityEngine.UI.Toggle toggle = node.InitComponent<UnityEngine.UI.Toggle>();
            if (layer.images != null)
            {
                var bgNode = DrawBackground(layer, toggle, node);
                DrawMask(layer, toggle, bgNode);
                DrawOtherLayers(layer, node);
            }
            return node;
        }

        private UGUINode DrawBackground(Data.GroupNode layer, Toggle toggle, UGUINode node)
        {
            var bgImage = layer.images.Find(x => MatchAddress(x.Name, backgroundAddress));

            UGUINode bgNode = null;
            if (bgImage != null)
            {
                var targetGraphic = toggle.transform.Find("Background").gameObject;

                if (bgImage.sprite != null)
                {
                    SetTargetGraphic<Image>(toggle);
                }
                else if (bgImage.texture != null)
                {
                    SetTargetGraphic<RawImage>(toggle);
                }
                else if (bgImage.text != null)
                {
                    var text = SetTargetGraphic<Text>(toggle);
                    text.horizontalOverflow = HorizontalWrapMode.Overflow;
                    text.verticalOverflow = VerticalWrapMode.Overflow;
                }

                bgNode = CreateNormalNode(targetGraphic, bgImage.rect, node);
                PSDImporterUtility.SetPictureOrLoadColor(bgImage, toggle.targetGraphic);
                SetRectTransform(bgImage.rect, toggle.GetComponent<RectTransform>());
            }
            else
            {
                SetTargetGraphic<Image>(toggle);
                bgNode = CreateNormalNode(toggle.targetGraphic.gameObject, layer.rect, node);
            }
            return bgNode;
        }

        private void DrawMask(Data.GroupNode layer, Toggle toggle, UGUINode bgNode)
        {
            var mask = layer.images.Find(x => MatchAddress(x.Name, maskAddress));

            if (mask != null)
            {
                if (mask.sprite != null)
                {
                    SetGraphic<Image>(toggle);
                }
                else if (mask.texture != null)
                {
                    SetGraphic<RawImage>(toggle);
                }
                else if (mask.text != null)
                {
                    var text = SetGraphic<Text>(toggle);
                    text.horizontalOverflow = HorizontalWrapMode.Overflow;
                    text.verticalOverflow = VerticalWrapMode.Overflow;
                }

                PSDImporterUtility.SetPictureOrLoadColor(mask, toggle.graphic);
                var graphic = toggle.targetGraphic.transform.Find("Mask").gameObject;
                CreateNormalNode(graphic, mask.rect, bgNode);
            }
            else
            {
                SetGraphic<Image>(toggle);
            }
        }

        private void DrawOtherLayers(Data.GroupNode layer, UGUINode node)
        {
            for (int imageIndex = 0; imageIndex < layer.images.Count; imageIndex++)
            {
                Data.ImgNode image = layer.images[imageIndex];
                if (!MatchAddress(image.Name, backgroundAddress, maskAddress))
                {
                    if (autoToggleTitle && image.type == ImgType.Label && !image.Name.StartsWith(titleAddress))
                    {
                        var array = layer.images.Where(x => x.type == ImgType.Label).ToList();
                        if (array.Count > 0 && array.IndexOf(image) == 0)
                        {
                            image.Name = titleAddress + image.Name;
                        }
                    }

                    ctrl.DrawImage(image, node);
                }
            }
        }
        public override void AfterGenerate(UGUINode node)
        {
            base.AfterGenerate(node);
            StretchTitle(node);
        }
        protected void StretchTitle(UGUINode node)
        {
            var texts = node.transform.GetComponentsInChildren<Text>();

            var title = Array.Find(texts, x => MatchAddress(x.name, titleAddress));

            if (title)
            {
                PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, node.transform as RectTransform, title.transform as RectTransform);
                title.rectTransform.anchoredPosition = Vector3.zero;
                title.alignment = TextAnchor.MiddleCenter;
                title.horizontalOverflow = HorizontalWrapMode.Overflow;
                title.verticalOverflow = VerticalWrapMode.Truncate;
            }
        }
    }
}