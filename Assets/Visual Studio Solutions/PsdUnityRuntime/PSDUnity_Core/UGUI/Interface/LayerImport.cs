using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;

namespace PSDUnity.UGUI
{
    public abstract class LayerImport : Import
    {
        [SerializeField,CustomField("后缀")]
        protected string _suffix = PSDUnity.PSDUnityConst.emptySuffix;

        public virtual string Suffix { get { return _suffix; } }
        public virtual UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            return node;
        }
        public virtual void AnalysisAreguments(Data.GroupNode layer, string[] areguments) { }
       
    }
}
