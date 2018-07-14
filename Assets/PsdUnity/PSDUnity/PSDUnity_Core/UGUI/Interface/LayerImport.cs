using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PSDUnity;
namespace PSDUnity.UGUI
{
    public class LayerImport: Import
    {
        [SerializeField]
        protected string _suffix;
        public virtual string Suffix { get { return _suffix; } }
        public virtual UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            return node;
        }
        public virtual void AnalysisAreguments(GroupNode layer, string[] areguments) { }
        public override GameObject CreateTemplate()
        {
            return new GameObject("empty");
        }
    }
}
