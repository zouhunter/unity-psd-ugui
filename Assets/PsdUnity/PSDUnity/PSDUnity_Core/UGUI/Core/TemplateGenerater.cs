//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;
//using System;

//namespace PSDUnity.UGUI
//{
//    public static class TemplateGenerater
//    {
//        public static Color defultColor;

//        private static GameObject CreateButton()
//        {
//            var btn = new GameObject("Button",  typeof(Button),typeof(RectTransform)).GetComponent<Button>();
//            return btn.gameObject;
//        }

//        private static GameObject CreateEmpty()
//        {
//            return new GameObject("Empty", typeof(RectTransform));
//        }

//        private static GameObject CreateToggle()
//        {
//            var toggle = new GameObject("Toggle", typeof(Toggle), typeof(Image));
//            toggle.GetComponent<Toggle>().targetGraphic = toggle.GetComponent<Image>();
//            var mask = new GameObject("Mask", typeof(RectTransform), typeof(Image));
//            toggle.GetComponent<Toggle>().graphic = mask.GetComponent<Image>();
//            mask.transform.SetParent(toggle.transform,false);
//            PSDUnity.UGUI.PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, toggle.GetComponent<RectTransform>(), mask.transform as RectTransform);
//            return toggle;
//        }

//        private static GameObject CreateCanvas()
//        {
//            var canvas = new GameObject("Canvas",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
//            canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//            return canvas;
//        }

//        private static GameObject CreateGrid()
//        {
//            var grid = new GameObject("Grid", typeof(GridLayoutGroup));
//            return grid;
//        }

//        private static GameObject CreateImage()
//        {
//            var img = new GameObject("Image",typeof(Image));
//            return img;
//        }

//        private static GameObject CreateRawImage()
//        {
//            var img = new GameObject("RawImage", typeof(RawImage));
//            return img;
//        }

//        private static GameObject CreateScrollView()
//        {
//            var scroll = new GameObject("ScrollView",typeof(ScrollRect));
//            return scroll;
//        }

//        private static GameObject CreateSlider()
//        {
//            var slider = new GameObject("Slider",typeof(Slider));
//            return slider;
//        }

//        private static GameObject CreateText(string title = null)
//        {
//            var text = new GameObject(title ?? "Text", typeof(Text)).GetComponent<Text>();
//            text.alignment = TextAnchor.MiddleCenter;
//            return text.gameObject;
//        }

//        private static GameObject CreateScollbar()
//        {
//            var scollbar = new GameObject("Scollbar",typeof(Scrollbar),typeof(Image)).GetComponent<Scrollbar>();
//            var sliding_Area = new GameObject("Sliding Area",typeof(RectTransform));
//            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));

//            scollbar.targetGraphic = scollbar.GetComponent<Image>();
//            sliding_Area.transform.SetParent(scollbar.transform, false);
//            PSDUnity.UGUI.PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, scollbar.GetComponent<RectTransform>(), sliding_Area.transform as RectTransform);

//            handle.transform.SetParent(sliding_Area.transform, false);
//            PSDUnity.UGUI.PSDImporterUtility.SetNormalAnchor(AnchoType.XCenter | AnchoType.XCenter, sliding_Area.GetComponent<RectTransform>(), handle.transform as RectTransform);

//            scollbar.GetComponent<Scrollbar>().handleRect = handle.GetComponent<RectTransform>();
//            return scollbar.gameObject;
//        }

//        private static GameObject CreateGroup()
//        {
//            var group = new GameObject("Group",typeof(ContentSizeFitter));
//            return group;
//        }

//        private static GameObject CreateInputField()
//        {
//            var inputfield = new GameObject("InputField",typeof(InputField),typeof(Image)).GetComponent<InputField>();
//            var holder = new GameObject("Placeholder", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
//            var text = new GameObject("Text", typeof(Text)).GetComponent<Text>() ;

//            inputfield.targetGraphic = inputfield.GetComponent<Image>();

//            holder.transform.SetParent(inputfield.transform, false);
//            text.transform.SetParent(inputfield.transform, false);

//            PSDUnity.UGUI.PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, inputfield.GetComponent<RectTransform>(), holder.transform as RectTransform);
//            PSDUnity.UGUI.PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, inputfield.GetComponent<RectTransform>(), text.transform as RectTransform);

//            inputfield.GetComponent<InputField>().placeholder = holder;
//            holder.alignment = TextAnchor.MiddleLeft;
//            holder.supportRichText = false;
//            Color color;
//            if(ColorUtility.TryParseHtmlString("#32323280",out color)) {
//                holder.color = color;
//            }


//            inputfield.GetComponent<InputField>().textComponent = text;
//            text.alignment = TextAnchor.MiddleLeft;
//            text.supportRichText = false;
//            if (ColorUtility.TryParseHtmlString("#32323280", out color))
//            {
//                text.color = color;
//            }
//            return inputfield.gameObject;
//        }

//        private static GameObject CreateDropDown()
//        {
//            var dropdown = new GameObject("DropDown",typeof(Dropdown),typeof(Image)).GetComponent<Dropdown>();
//            var label = new GameObject("Label",typeof(Text)).GetComponent<Text>();
//            var template = new GameObject("Template",typeof(ScrollRect),typeof(Image)).GetComponent<ScrollRect>();
//            var viewport = new GameObject("Viewport",typeof(Image),typeof(Mask)).GetComponent<Image>();
//            var content = new GameObject("Content",typeof(RectTransform)).GetComponent<RectTransform>();
//            var item = new GameObject("Item",typeof(Toggle)).GetComponent<Toggle>();
//            var item_bg = new GameObject("Item Background",typeof(Image)).GetComponent<Image>();
//            var item_cm = new GameObject("Item CheckMask",typeof(Image)).GetComponent<Image>();
//            var item_lb = new GameObject("Item Label",typeof(Text)).GetComponent<Text>();

//            label.transform.SetParent(dropdown.transform, false);
//            template.transform.SetParent(dropdown.transform, false);
//            viewport.transform.SetParent(template.transform, false);
//            content.transform.SetParent(viewport.transform, false);
//            item.transform.SetParent(content.transform,false);
//            item_bg.transform.SetParent(item.transform,false);
//            item_bg.transform.SetParent(item.transform,false);
//            item_cm.transform.SetParent(item.transform,false);
//            item_lb.transform.SetParent(item.transform,false);

//            dropdown.targetGraphic = dropdown.GetComponent<Image>();
//            dropdown.captionText = label;
//            dropdown.template = template.transform as RectTransform;
//            dropdown.itemText = item_lb;

//            template.viewport = viewport.transform as RectTransform;
//            template.content = content;

//            item.targetGraphic = item_bg;
//            item.graphic = item_cm;

//            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, dropdown.transform as RectTransform, label.transform as RectTransform);
//            PSDImporterUtility.SetNormalAnchor(AnchoType.XCenter | AnchoType.YCenter, dropdown.transform as RectTransform, template.transform as RectTransform);
//            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, template.transform as RectTransform, viewport.transform as RectTransform);
//            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.Up, viewport.transform as RectTransform, content.transform as RectTransform);
//            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, content.transform as RectTransform, item.transform as RectTransform);
//            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, item.transform as RectTransform, item_bg.transform as RectTransform);
//            PSDImporterUtility.SetNormalAnchor(AnchoType.Left | AnchoType.YCenter, item.transform as RectTransform, item_cm.transform as RectTransform);
//            PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, item.transform as RectTransform, item_lb.transform as RectTransform);

//            (template.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
//            content.pivot = new Vector2(0.5f, 1); 
//            content.sizeDelta = new Vector2(0,content.sizeDelta.y);
//            (item_bg.transform as RectTransform).sizeDelta = Vector2.zero;
//            (item_cm.transform as RectTransform).sizeDelta = new Vector2(20,20);
//            (item_lb.transform as RectTransform).sizeDelta = Vector2.zero;

//            item_lb.alignment = label.alignment = TextAnchor.MiddleCenter;
//            viewport.GetComponent<Image>().color = new Color(0,0,0,0.1f);
//            return dropdown.gameObject;
//        }
//    }
//}