using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using PSDUnity;
using System;

namespace PSDUnity.UGUI
{
    public class UGUINode
    {
        public UGUINode parent;
        public AnchoType anchoType = AnchoType.Custom;//设置最终父级时锚点
        public List<UGUINode> childs = new List<UGUINode>();
        public Transform transform;
        public event UnityAction<UGUINode> inversionReprocess;//从树的末端开始处理
        public UGUINode(Transform transform, UGUINode parent)
        {
            this.transform = transform;
            this.parent = parent;
            if (parent != null){
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
        internal void InversionReprocess()
        {
            if(inversionReprocess != null)
            {
                inversionReprocess.Invoke(this);
            }
        }
    }
}
