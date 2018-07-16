using UnityEngine;
using UnityEngine.UI;

namespace PSDUnity.UGUI
{
    public static class PSDImporterUtility
    {
        /// <summary>
        /// 快速创建控制器
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="uiSize"></param>
        /// <param name="defultCanvas"></param>
        /// <returns></returns>
        public static PSDImportCtrl CreatePsdImportCtrlSafty(RuleObject arg, Vector2 uiSize, Canvas defultCanvas = null)
        {
            Canvas canvas = null;
            if (defultCanvas != null)
            {
                canvas = defultCanvas;
            }
            else
            {
                canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
                canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            PSDImportCtrl import = new PSDImportCtrl(canvas, arg, uiSize);
            UnityEngine.UI.CanvasScaler scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            scaler.referenceResolution = new Vector2(uiSize.x, uiSize.y);

            return import;
        }
        
        /// <summary>
        /// 快速设置图片的内容
        /// </summary>
        /// <param name="image"></param>
        /// <param name="graph"></param>
        public static void SetPictureOrLoadColor(Data.ImgNode image, UnityEngine.UI.Graphic graph)
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
        /// <summary>
        /// 设置目标Anchor
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rectTransform"></param>
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
        /// <summary>
        /// 设置目标Anchor(Custom 类型)
        /// </summary>
        /// <param name="parentRectt"></param>
        /// <param name="rectt"></param>
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
        /// <summary>
        /// 使用的前提是rectt的类型为双局中
        /// </summary>
        /// <param name="anchoType"></param>
        /// <param name="parentRectt"></param>
        /// <param name="rectt"></param>
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