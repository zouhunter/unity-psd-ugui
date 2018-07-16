using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class DropDownLayerImport : LayerImport
    {
        [Header("[前缀-----------------------------------")]
        [SerializeField,CustomField("选中遮罩")] protected string maskAddress = "m_";
        [SerializeField,CustomField("背景格式")] protected string backgroundsFormat = "b{0}_";
        [SerializeField,CustomField("标题格式")] protected string titlesFormat = "t{0}_";
        [SerializeField,CustomField("下拉条")] protected string vbarAddress = "vb_";

        public DropDownLayerImport()
        {
            _suffix = "DropDown";
        }

        public override GameObject CreateTemplate()
        {
            var dropdown = new GameObject("DropDown", typeof(Dropdown), typeof(Image)).GetComponent<Dropdown>();
            var label = new GameObject("Label", typeof(Text)).GetComponent<Text>();
            var template = new GameObject("Template", typeof(ScrollRect), typeof(Image)).GetComponent<ScrollRect>();
            var viewport = new GameObject("Viewport", typeof(Image), typeof(Mask)).GetComponent<Image>();
            var content = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
            var item = new GameObject("Item", typeof(Toggle)).GetComponent<Toggle>();
            var item_bg = new GameObject("Item Background", typeof(Image)).GetComponent<Image>();
            var item_cm = new GameObject("Item CheckMask", typeof(Image)).GetComponent<Image>();
            var item_lb = new GameObject("Item Label", typeof(Text)).GetComponent<Text>();

            label.transform.SetParent(dropdown.transform, false);
            template.transform.SetParent(dropdown.transform, false);
            viewport.transform.SetParent(template.transform, false);
            content.transform.SetParent(viewport.transform, false);
            item.transform.SetParent(content.transform, false);
            item_bg.transform.SetParent(item.transform, false);
            item_bg.transform.SetParent(item.transform, false);
            item_cm.transform.SetParent(item.transform, false);
            item_lb.transform.SetParent(item.transform, false);

            dropdown.targetGraphic = dropdown.GetComponent<Image>();
            dropdown.captionText = label;
            dropdown.template = template.transform as RectTransform;
            dropdown.itemText = item_lb;

            template.viewport = viewport.transform as RectTransform;
            template.content = content;

            item.targetGraphic = item_bg;
            item.graphic = item_cm;

            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, dropdown.transform as RectTransform, label.transform as RectTransform);
            PSDImporterUtility.SetNormalAnchor(AnchoType.XCenter | AnchoType.YCenter, dropdown.transform as RectTransform, template.transform as RectTransform);
            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, template.transform as RectTransform, viewport.transform as RectTransform);
            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.Up, viewport.transform as RectTransform, content.transform as RectTransform);
            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, content.transform as RectTransform, item.transform as RectTransform);
            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, item.transform as RectTransform, item_bg.transform as RectTransform);
            PSDImporterUtility.SetNormalAnchor(AnchoType.Left | AnchoType.YCenter, item.transform as RectTransform, item_cm.transform as RectTransform);
            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, item.transform as RectTransform, item_lb.transform as RectTransform);

            (template.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
            content.pivot = new Vector2(0.5f, 1);
            content.sizeDelta = new Vector2(0, content.sizeDelta.y);
            (item_bg.transform as RectTransform).sizeDelta = Vector2.zero;
            (item_cm.transform as RectTransform).sizeDelta = new Vector2(20, 20);
            (item_lb.transform as RectTransform).sizeDelta = Vector2.zero;

            item_lb.alignment = label.alignment = TextAnchor.MiddleCenter;
            viewport.GetComponent<Image>().color = new Color(0, 0, 0, 0.1f);
            item_cm.enabled = false;
            return dropdown.gameObject;
        }

        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);

            Dropdown dropdown = node.InitComponent<Dropdown>();
            ScrollRect scrllRect = dropdown.template.GetComponent<ScrollRect>();
            RectTransform content = scrllRect.content;
            Toggle toggle = content.GetComponentInChildren<Toggle>();

            UGUINode tempNode = CreateNormalNode(dropdown.template.gameObject, layer.rect, node);
            tempNode.anchoType = AnchoType.Down | AnchoType.XStretch;
            DrawImages(node, layer, dropdown, toggle, content);
            DrawSubLayers(layer, tempNode, scrllRect);
            return node;
        }

        public override void AfterGenerate(UGUINode node)
        {
            base.AfterGenerate(node);
            var dropDown = node.InitComponent<Dropdown>();
            RectTransform rt = dropDown.template;
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = Vector3.zero;
        }

        /// <summary>
        /// 绘制所有image
        /// </summary>
        /// <param name="node"></param>
        /// <param name="layer"></param>
        /// <param name="dropdown"></param>
        /// <param name="toggle"></param>
        /// <param name="content"></param>
        private void DrawImages(UGUINode node, Data.GroupNode layer, Dropdown dropdown, Toggle toggle, RectTransform content)
        {
            for (int i = 0; i < layer.images.Count; i++)
            {
                Data.ImgNode image = layer.images[i];
                if (MatchIDAddress(image.Name, 1, backgroundsFormat))
                {
                    PSDImporterUtility.SetPictureOrLoadColor(image, dropdown.image);
                    SetRectTransform(image.rect, dropdown.GetComponent<RectTransform>());
                    dropdown.name = layer.displayName;
                }
                else if (MatchIDAddress(image.Name, 2, backgroundsFormat))
                {
                    PSDImporterUtility.SetPictureOrLoadColor(image, dropdown.template.GetComponent<Graphic>());
                    SetRectTransform(image.rect, dropdown.template);
                }
                else if (MatchIDAddress(image.Name, 3, backgroundsFormat))
                {
                    UnityEngine.UI.Image itemimage = (UnityEngine.UI.Image)toggle.targetGraphic;
                    PSDImporterUtility.SetPictureOrLoadColor(image, itemimage);
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, image.rect.height);
                }
                else if (MatchIDAddress(image.Name, 1, titlesFormat))
                {
                    PSDImporterUtility.SetPictureOrLoadColor(image, dropdown.captionText);
                }
                else if (MatchIDAddress(image.Name, 2, titlesFormat))
                {
                    PSDImporterUtility.SetPictureOrLoadColor(image, dropdown.itemText);
                }
                else if (MatchAddress(image.Name, maskAddress))
                {
                    UnityEngine.UI.Image mask = (UnityEngine.UI.Image)toggle.graphic;
                    mask.enabled = true;
                    PSDImporterUtility.SetPictureOrLoadColor(image, mask);
                }
                else
                {
                    ctrl.DrawImage(image, node);
                }
            }
        }

        /// <summary>
        /// 滑动条绘制
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="tempNode"></param>
        /// <param name="scrollRect"></param>
        private void DrawSubLayers(Data.GroupNode layer, UGUINode tempNode, ScrollRect scrollRect)
        {
            if (layer.children != null)
            {
                for (int i = 0; i < layer.children.Count; i++)
                {
                    Data.GroupNode child = layer.children[i] as Data.GroupNode;
                    if (MatchAddress(child.displayName, vbarAddress))
                    {
                        UGUINode barNode = ctrl.DrawLayer(child, tempNode);
                        scrollRect.verticalScrollbar = barNode.InitComponent<Scrollbar>();
                        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                        barNode.anchoType = AnchoType.Right | AnchoType.YCenter;
                    }
                    else
                    {
                        ctrl.DrawLayer(child, tempNode);
                    }
                }
            }

        }
    }
}
