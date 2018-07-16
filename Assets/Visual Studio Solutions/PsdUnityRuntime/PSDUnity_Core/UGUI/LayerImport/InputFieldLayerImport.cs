using System;
using UnityEngine;
using UnityEngine.UI;

namespace PSDUnity.UGUI
{
    public class InputFieldLayerImport : LayerImport
    {
        [Header("[前缀-----------------------------------")]
        [SerializeField,CustomField("背景图")] protected string backgroundAddress = "b_";
        [SerializeField, CustomField("文本")] protected string titleAddress = "t_";
        [SerializeField,CustomField("占位")] protected string placeAddress = "p_";

        public InputFieldLayerImport()
        {
            _suffix = "InputField";
        }
        public override GameObject CreateTemplate()
        {
            var inputfield = new GameObject("InputField", typeof(InputField), typeof(Image)).GetComponent<InputField>();
            var holder = new GameObject("Placeholder", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
            var text = new GameObject("Text", typeof(Text)).GetComponent<Text>();

            inputfield.targetGraphic = inputfield.GetComponent<Image>();

            holder.transform.SetParent(inputfield.transform, false);
            text.transform.SetParent(inputfield.transform, false);

            //设置默认锚点
            PSDImporterUtility.SetCustomAnchor(inputfield.GetComponent<RectTransform>(), holder.rectTransform);
            PSDImporterUtility.SetCustomAnchor(inputfield.GetComponent<RectTransform>(), text.rectTransform);

            inputfield.GetComponent<InputField>().placeholder = holder;
            holder.alignment = TextAnchor.MiddleLeft;
            holder.supportRichText = false;
            Color color;
            if (ColorUtility.TryParseHtmlString("#32323280", out color)){
                holder.color = color;
            }

            inputfield.GetComponent<InputField>().textComponent = text;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = false;
            if (ColorUtility.TryParseHtmlString("#32323280", out color)){
                text.color = color;
            }
            return inputfield.gameObject;
        }

        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            UnityEngine.UI.InputField inputfield = node.InitComponent<UnityEngine.UI.InputField>();
            DrawImages(inputfield, node, layer);
            return node;
        }

        private void DrawImages(InputField inputfield, UGUINode node, Data.GroupNode layer)
        {
            if (layer.images != null)
            {
                for (int i = 0; i < layer.images.Count; i++)
                {
                    Data.ImgNode image = layer.images[i];

                    if (image.type == ImgType.Label && MatchAddress(image.Name, titleAddress))
                    {
                        UnityEngine.UI.Text text = (UnityEngine.UI.Text)inputfield.textComponent;//inputfield.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
                        var childNode = CreateNormalNode(text.gameObject, image.rect, node);
                        childNode.anchoType = AnchoType.XStretch | AnchoType.YStretch;
                        PSDImporterUtility.SetPictureOrLoadColor(image, text);
                    }
                    else if (image.type == ImgType.Label && MatchAddress(image.Name, placeAddress))
                    {
                        UnityEngine.UI.Text text = (UnityEngine.UI.Text)inputfield.placeholder;//.transform.Find("Placeholder").GetComponent<UnityEngine.UI.Text>();
                        var childNode = CreateNormalNode(text.gameObject, image.rect, node);
                        childNode.anchoType = AnchoType.XStretch | AnchoType.YStretch;
                        PSDImporterUtility.SetPictureOrLoadColor(image, text);
                    }
                    else if (MatchAddress(image.Name, backgroundAddress))
                    {
                        PSDImporterUtility.SetPictureOrLoadColor(image, inputfield.image);
                        SetRectTransform(image.rect, inputfield.GetComponent<RectTransform>());
                    }
                    else
                    {
                        ctrl.DrawImage(image, node);
                    }
                }
            }
        }

        public override void AfterGenerate(UGUINode node)
        {
            base.AfterGenerate(node);
            var inputField = node.InitComponent<InputField>();
            var text = inputField.textComponent.GetComponent<RectTransform>();
            var holder = inputField.placeholder.GetComponent<RectTransform>();
            text.anchorMin = holder.anchorMin = Vector2.zero;
            text.anchorMax = holder.anchorMax = Vector2.one;
            text.anchoredPosition = holder.anchoredPosition = Vector2.zero;
            text.sizeDelta = holder.sizeDelta = new Vector2(-10, -10);
        }
    }
}