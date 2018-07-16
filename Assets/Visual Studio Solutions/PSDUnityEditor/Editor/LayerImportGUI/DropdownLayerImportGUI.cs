using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity.UGUI
{
    [CustomLayer(typeof(DropDownLayerImport))]
    public class DropdownLayerImportGUI : UGUI.LayerImportGUI
    {
        public override Texture Icon
        {
            get
            {
                return EditorGUIUtility.IconContent("Dropdown Icon").image;
            }
        }
    }
}
