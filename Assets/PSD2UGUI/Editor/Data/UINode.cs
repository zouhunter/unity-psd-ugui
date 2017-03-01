using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINode {
    public UINode parent;
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
