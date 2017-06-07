using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UINode {
    public enum AnchoType
    {
        Custom = 1 << 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Up = 1 << 3,
        Down = 1 << 4,
        XStretch = 1 << 5,
        YStretch = 1 << 6,
        XCenter = 1 << 7,
        YCenter = 1 << 8,
    }
    public UINode parent;
    public AnchoType anchoType = AnchoType.Custom;
    public List<UINode> childs = new List<UINode>();
    public Transform transform;
    public UnityAction ReprocessEvent;
    public UINode(Transform transform,UINode parent)
    {
        this.transform = transform;
        this.parent = parent;
        if(parent != null) parent.childs.Add(this);
    }

    public T InitComponent<T>() where T : Component
    {
        var t = transform.gameObject.GetComponent<T>();
        if (t == null)
            transform.gameObject.AddComponent<T>();
        return t;
    }
}
