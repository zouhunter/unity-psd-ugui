using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace PSDUnity
{
    public class UGUINode
    {
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
        public UGUINode parent;
        public AnchoType anchoType = AnchoType.Custom;
        public List<UGUINode> childs = new List<UGUINode>();
        public Transform transform;
        public UnityAction ReprocessEvent;
        public UGUINode(Transform transform, UGUINode parent)
        {
            this.transform = transform;
            this.parent = parent;
            if (parent != null) parent.childs.Add(this);
        }
        public T InitComponent<T>() where T : Component
        {
            var t = transform.gameObject.GetComponent<T>();
            if (t == null)
                transform.gameObject.AddComponent<T>();
            return t;
        }
    }
}
