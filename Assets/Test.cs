using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //public enum AnchoType
    //{
    //    Custom = 1 << 0,
    //    Left = 1 << 1,
    //    Right = 1 << 2,
    //    Up = 1 << 3,
    //    Down = 1 << 4,
    //    XStretch = 1 << 5,
    //    YStretch = 1 << 6,
    //    XCenter = 1 << 7,
    //    YCenter = 1 << 8,
    //}

    //private AnchoType anchoType;
    //public AnchoType anchoY;
    //public AnchoType anchoX;

    //// Update is called once per frame
    //private void Start()
    //{
    //    anchoType = anchoY | anchoX;
    //    SetNormalAnchor(anchoType, transform.parent.GetComponent<RectTransform>(), GetComponent<RectTransform>());
    //}
    //public static void SetNormalAnchor(UINode.AnchoType anchoType, RectTransform parentRectt, RectTransform rectt)
    //{
    //    Vector2 sizeDelta = rectt.sizeDelta;
    //    Vector2 p_sizeDelta = parentRectt.sizeDelta;
    //    Vector2 anchoredPosition = rectt.anchoredPosition;

    //    float xmin = 0;
    //    float xmax = 0;
    //    float ymin = 0;
    //    float ymax = 0;
    //    float xSize = 0;
    //    float ySize = 0;
    //    float xanchored = 0;
    //    float yanchored = 0;

    //    if ((anchoType & UINode.AnchoType.Up) == UINode.AnchoType.Up)
    //    {
    //        ymin = ymax = 1;
    //        yanchored = anchoredPosition.y - p_sizeDelta.y * 0.5f;
    //        ySize = sizeDelta.y;
    //    }
    //    if ((anchoType & UINode.AnchoType.Down) == UINode.AnchoType.Down)
    //    {
    //        ymin = ymax = 0;
    //        yanchored = anchoredPosition.y + p_sizeDelta.y * 0.5f;
    //        ySize = sizeDelta.y;
    //    }
    //    if ((anchoType & UINode.AnchoType.Left) == UINode.AnchoType.Left)
    //    {
    //        xmin = xmax = 0;
    //        xanchored = anchoredPosition.x + p_sizeDelta.x * 0.5f;
    //        xSize = sizeDelta.x;
    //    }
    //    if ((anchoType & UINode.AnchoType.Right) == UINode.AnchoType.Right)
    //    {
    //        xmin = xmax = 1;
    //        xanchored = anchoredPosition.x - p_sizeDelta.x * 0.5f;
    //        xSize = sizeDelta.x;
    //    }
    //    if ((anchoType & UINode.AnchoType.XStretch) == UINode.AnchoType.XStretch)
    //    {
    //        xmin = 0;xmax = 1;
    //        xanchored = anchoredPosition.x;
    //        xSize = sizeDelta.x - p_sizeDelta.x;
    //    }
    //    if ((anchoType & UINode.AnchoType.YStretch) == UINode.AnchoType.YStretch)
    //    {
    //        ymin = 0; ymax = 1;
    //        yanchored = anchoredPosition.y;
    //        ySize = sizeDelta.y - p_sizeDelta.y;
    //    }
    //    if ((anchoType & UINode.AnchoType.XCenter) == UINode.AnchoType.XCenter)
    //    {
    //        xmin = xmax = 0.5f;
    //        xanchored = anchoredPosition.x;
    //        xSize = sizeDelta.x;
    //    }
    //    if ((anchoType & UINode.AnchoType.YCenter) == UINode.AnchoType.YCenter)
    //    {
    //        ymin = ymax = 0.5f;
    //        yanchored = anchoredPosition.y;
    //        ySize = sizeDelta.y;
    //    }

    //    rectt.anchorMin = new Vector2(xmin, ymin);
    //    rectt.anchorMax = new Vector2(xmax, ymax);

    //    rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta.x);
    //    rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta.y);

    //    rectt.sizeDelta = new Vector2(xSize, ySize);
    //    rectt.anchoredPosition = new Vector2(xanchored, yanchored);
    //}
}
