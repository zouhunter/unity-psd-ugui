using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using Ntreev.Library.Psd;
using System;
using PSDUnity;
using PSDUnity.Data;
namespace PSDUnity.Analysis
{
    public class PreviewItem : TreeViewItem
    {
        public string name { get { return layer.Name; } }
        public LayerType layerType { get; private set; }
        public IPsdLayer layer;

        public PreviewItem(int id,int depth,IPsdLayer layer):base(id,depth,layer.Name){
            this.layer = layer;
            InitTypes();
        }

        private void InitTypes()
        {
            if (layer is PsdDocument){
                layerType = LayerType.Group;
            }
            else{
                layerType = (layer as PsdLayer).LayerType;
            }
        }
    }
}