using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity.UGUI
{
    [CustomLayer(typeof(EmptyLayerImport))]
    public class EmplyLayerImportEditor : LayerImportGUI
    {
        public override Texture Icon
        {
            get
            {
                return EditorGUIUtility.IconContent("GameObject Icon").image;
            }
        }
    }
}