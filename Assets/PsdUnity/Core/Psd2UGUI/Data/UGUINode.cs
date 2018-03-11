using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using PSDUnity.Data;
namespace PSDUnity.PSD2UGUI
{
    public class UGUINode
    {
        public UGUINode parent;
        public AnchoType anchoType = AnchoType.Custom;
        public List<UGUINode> childs = new List<UGUINode>();
        public Transform transform;
        public UnityAction ReprocessEvent { get; set; }
        public UGUINode(Transform transform, UGUINode parent)
        {
            this.transform = transform;
            this.parent = parent;
            if (parent != null)
            {
                parent.childs.Add(this);
            }
        }
        public T InitComponent<T>() where T : Component
        {
            var t = transform.gameObject.GetComponent<T>();
            if (t == null)
            {
                t = transform.gameObject.AddComponent<T>();
            }
            return t;
        }
    }
}
