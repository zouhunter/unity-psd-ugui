using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity.UGUI
{
    [CustomEditor(typeof(ScrollBarLayerImport))]
    public class ScrollbarLayerImportEditor : UGUI.LayerImportEditor
    {
        public override Texture Icon
        {
            get
            {
                return EditorGUIUtility.IconContent("Scrollbar Icon").image;
            }
        }
    }
}
