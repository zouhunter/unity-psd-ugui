using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINode {
    public enum AnchoType
    {
        Custom,
        Expland,
        Center,
        LeftCenter,RightCenter,UpCenter,DownCenter,
        LeftExpland,RightExpland,UpExpland,DownExpland,
        LeftUp,RightUp,LeftDown,RightDown
    }
    public UINode parent;
    public AnchoType anchoType = AnchoType.Custom;
    public List<UINode> childs = new List<UINode>();
    public Transform transform;
    
    public UINode(Transform transform,UINode parent)
    {
        this.transform = transform;
        this.parent = parent;
        if(parent != null) parent.childs.Add(this);
    }

    public T GetCompoment<T>() where T:Object{
        return transform.GetComponent<T>();
    }
}
