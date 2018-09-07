using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace PSDUnity.UGUI
{
    public static class PSDImporterUtility
    {
        /// <summary>
        /// 快速创建控制器
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="uiSize"></param>
        /// <param name="defultCanvas"></param>
        /// <returns></returns>
        public static PSDImportCtrl CreatePsdImportCtrlSafty(Data.RuleObject rule, Vector2 uiSize, Canvas defultCanvas = null)
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
            PSDImportCtrl import = new PSDImportCtrl(canvas, rule, uiSize);
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
            if (graph == null) return;

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

        public static Data.GroupNode ListToTree(IList<Data.GroupNode> list)
        {
            // Validate input
            ValidateDepthValues(list);

            // Clear old states
            foreach (var element in list)
            {
                element.parent = null;
                element.children = null;
            }

            // Set child and parent references using depth info
            for (int parentIndex = 0; parentIndex < list.Count; parentIndex++)
            {
                var parent = list[parentIndex];
                bool alreadyHasValidChildren = parent.children != null;
                if (alreadyHasValidChildren)
                    continue;

                int parentDepth = parent.depth;
                int childCount = 0;

                // Count children based depth value, we are looking at children until it's the same depth as this object
                for (int i = parentIndex + 1; i < list.Count; i++)
                {
                    if (list[i].depth == parentDepth + 1)
                        childCount++;
                    if (list[i].depth <= parentDepth)
                        break;
                }

                // Fill child array
                List<Data.GroupNode> childList = null;
                if (childCount != 0)
                {
                    childList = new List<Data.GroupNode>(childCount); // Allocate once
                    childCount = 0;
                    for (int i = parentIndex + 1; i < list.Count; i++)
                    {
                        if (list[i].depth == parentDepth + 1)
                        {
                            list[i].parent = parent;
                            childList.Add(list[i]);
                            childCount++;
                        }

                        if (list[i].depth <= parentDepth)
                            break;
                    }
                }
                if(childList!=null)
                parent.children = childList.ConvertAll(x=>(object)x);
            }

            return list[0];
        }

        // Check state of input list
        public static void ValidateDepthValues(IList<Data.GroupNode> list)
        {
            if (list.Count == 0)
                throw new ArgumentException("list should have items, count is 0, check before calling ValidateDepthValues", "list");

            if (list[0].depth != -1)
                throw new ArgumentException("list item at index 0 should have a depth of -1 (since this should be the hidden root of the tree). Depth is: " + list[0].depth, "list");

            for (int i = 0; i < list.Count - 1; i++)
            {
                int depth = list[i].depth;
                int nextDepth = list[i + 1].depth;
                if (nextDepth > depth && nextDepth - depth > 1)
                    throw new ArgumentException(string.Format("Invalid depth info in input list. Depth cannot increase more than 1 per row. Index {0} has depth {1} while index {2} has depth {3}", i, depth, i + 1, nextDepth));
            }

            for (int i = 1; i < list.Count; ++i)
                if (list[i].depth < 0)
                    throw new ArgumentException("Invalid depth value for item at index " + i + ". Only the first item (the root) should have depth below 0.");

            if (list.Count > 1 && list[1].depth != 0)
                throw new ArgumentException("Input list item at index 1 is assumed to have a depth of 0", "list");
        }

    }
}