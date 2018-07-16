using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity.UGUI
{
    [CustomLayer(typeof(InputFieldLayerImport))]
    public class InputFieldLayerImportGUI : UGUI.LayerImportGUI
    {
        public override Texture Icon
        {
            get
            {
                return EditorGUIUtility.IconContent("InputField Icon").image;
            }
        }
    }
}
