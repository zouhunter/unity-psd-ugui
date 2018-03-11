using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public static class PSDImporter
    {
        public static RuleObject RuleObject { get; private set; }
        public static Canvas canvas { get; private set; }
        public static UGUINode uinode { get; private set; }
        public static Vector2 canvasSize { get; private set; }

        public static Canvas InitEnviroment(RuleObject arg, Vector2 uiSize, Canvas defultCanvas = null)
        {
            RuleObject = arg;

            if (defultCanvas != null)
            {
                canvas = defultCanvas;
            }
            else
            {
                var canvasPfb = RuleObject.prefabs.Find(x => x.groupType == GroupType.CANVAS).prefab;
                canvas = GameObject.Instantiate(canvasPfb).GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                UnityEngine.UI.CanvasScaler scaler = PSDImporter.canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                scaler.referenceResolution = new Vector2(uiSize.x, uiSize.y);
            }
            canvasSize = canvas.GetComponent<UnityEngine.UI.CanvasScaler>().referenceResolution;
            uinode = new UGUINode(canvas.transform, null);
            return canvas;
        }

        public static void StartBuild(GroupNode1[] groupNodes)
        {
            PSDImportCtrl import = new PSDImportCtrl();
            import.Import(groupNodes, canvasSize);
        }

        public static UGUINode InstantiateItem(GroupType groupType, string name, UGUINode parent)
        {
            GameObject prefab = RuleObject.prefabs.Find(x => x.groupType == groupType).prefab;
            GameObject item = GameObject.Instantiate(prefab) as GameObject;
            item.name = name;
            item.transform.SetParent(canvas.transform, false);
            UGUINode node = new UGUINode(item.transform, parent);
            return node;
        }

        public static void SetPictureOrLoadColor(ImgNode image, UnityEngine.UI.Graphic graph)
        {
            graph.color = image.color;
            switch (image.type)
            {
                case ImgType.Image:
                    ((UnityEngine.UI.Image)graph).sprite = image.sprite;
                    break;
                case ImgType.Texture:
                    ((UnityEngine.UI.RawImage)graph).texture = image.texture;
                    break;
                case ImgType.Label:
                    var myText = (UnityEngine.UI.Text)graph;
                    myText.text = image.text;
                    myText.fontSize = image.fontSize;
                    break;
                case ImgType.AtlasImage:
                    ((UnityEngine.UI.Image)graph).sprite = image.sprite;
                    break;
                default:
                    break;
            }
        }

        public static void SetRectTransform(ImgNode image, RectTransform rectTransform)
        {
            rectTransform.sizeDelta = new Vector2(image.rect.width, image.rect.height);
            rectTransform.anchoredPosition = new Vector2(image.rect.x, image.rect.y);
        }

        public static void SetRectTransform(GroupNode layer, RectTransform rectTransform)
        {
            rectTransform.sizeDelta = new Vector2(layer.rect.width, layer.rect.height);
            rectTransform.anchoredPosition = new Vector2(layer.rect.x, layer.rect.y);
        }
        public static void SetAnchorByNode(UGUINode node)
        {
            RectTransform p_rt = node.parent.InitComponent<RectTransform>();
            RectTransform c_rt = node.InitComponent<RectTransform>();

            switch (node.anchoType)
            {
                case AnchoType.Custom:
                    SetCustomAnchor(p_rt, c_rt);
                    break;
                default:
                    SetNormalAnchor(node.anchoType, p_rt, c_rt);
                    break;
            }
        }

        public static void SetCustomAnchor(RectTransform parentRectt, RectTransform rectt)
        {
            Vector2 sizeDelta = rectt.sizeDelta;
            Vector2 p_sizeDelta = parentRectt.sizeDelta;
            Vector2 anchoredPosition = rectt.anchoredPosition;
            float xmin = p_sizeDelta.x * 0.5f + anchoredPosition.x - sizeDelta.x * 0.5f;
            float xmax = p_sizeDelta.x * 0.5f + anchoredPosition.x + sizeDelta.x * 0.5f;
            float ymin = p_sizeDelta.y * 0.5f + anchoredPosition.y - sizeDelta.y * 0.5f;
            float ymax = p_sizeDelta.y * 0.5f + anchoredPosition.y + sizeDelta.y * 0.5f;
            float xSize = 0;
            float ySize = 0;
            float xanchored = 0;
            float yanchored = 0;
            rectt.anchorMin = new Vector2(xmin / p_sizeDelta.x, ymin / p_sizeDelta.y);
            rectt.anchorMax = new Vector2(xmax / p_sizeDelta.x, ymax / p_sizeDelta.y);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta.x);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta.y);
            rectt.sizeDelta = new Vector2(xSize, ySize);
            rectt.anchoredPosition = new Vector2(xanchored, yanchored);
        }

        public static void SetNormalAnchor(AnchoType anchoType, RectTransform parentRectt, RectTransform rectt)
        {
            Vector2 sizeDelta = rectt.sizeDelta;
            Vector2 p_sizeDelta = parentRectt.sizeDelta;
            Vector2 anchoredPosition = rectt.anchoredPosition;

            float xmin = 0;
            float xmax = 0;
            float ymin = 0;
            float ymax = 0;
            float xSize = 0;
            float ySize = 0;
            float xanchored = 0;
            float yanchored = 0;

            if ((anchoType & AnchoType.Up) == AnchoType.Up)
            {
                ymin = ymax = 1;
                yanchored = anchoredPosition.y - p_sizeDelta.y * 0.5f;
                ySize = sizeDelta.y;
            }
            if ((anchoType & AnchoType.Down) == AnchoType.Down)
            {
                ymin = ymax = 0;
                yanchored = anchoredPosition.y + p_sizeDelta.y * 0.5f;
                ySize = sizeDelta.y;
            }
            if ((anchoType & AnchoType.Left) == AnchoType.Left)
            {
                xmin = xmax = 0;
                xanchored = anchoredPosition.x + p_sizeDelta.x * 0.5f;
                xSize = sizeDelta.x;
            }
            if ((anchoType & AnchoType.Right) == AnchoType.Right)
            {
                xmin = xmax = 1;
                xanchored = anchoredPosition.x - p_sizeDelta.x * 0.5f;
                xSize = sizeDelta.x;
            }
            if ((anchoType & AnchoType.XStretch) == AnchoType.XStretch)
            {
                xmin = 0; xmax = 1;
                xanchored = anchoredPosition.x;
                xSize = sizeDelta.x - p_sizeDelta.x;
            }
            if ((anchoType & AnchoType.YStretch) == AnchoType.YStretch)
            {
                ymin = 0; ymax = 1;
                yanchored = anchoredPosition.y;
                ySize = sizeDelta.y - p_sizeDelta.y;
            }
            if ((anchoType & AnchoType.XCenter) == AnchoType.XCenter)
            {
                xmin = xmax = 0.5f;
                xanchored = anchoredPosition.x;
                xSize = sizeDelta.x;
            }
            if ((anchoType & AnchoType.YCenter) == AnchoType.YCenter)
            {
                ymin = ymax = 0.5f;
                yanchored = anchoredPosition.y;
                ySize = sizeDelta.y;
            }

            rectt.anchorMin = new Vector2(xmin, ymin);
            rectt.anchorMax = new Vector2(xmax, ymax);

            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta.x);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta.y);

            rectt.sizeDelta = new Vector2(xSize, ySize);
            rectt.anchoredPosition = new Vector2(xanchored, yanchored);
        }
    }
}