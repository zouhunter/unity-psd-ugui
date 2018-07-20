using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using PSDUnity.Data;

namespace PSDUnity.UGUI
{
    [CustomLayer(typeof(ScrollViewLayerImport))]
    public class ScrollViewLayerImportGUI : UGUI.LayerImportGUI
    {
        public override Texture Icon
        {
            get
            {
                return EditorGUIUtility.IconContent("ScrollRect Icon").image;
            }
        }
        public override void HeadGUI(Rect dirRect, GroupNode item)
        {
            base.HeadGUI(dirRect, item);
            //#向下不兼容的写法
#if UNITY_5_6
            var dir = (DirectionAxis)EditorGUI.EnumMaskField(dirRect, item.directionAxis);
            item.directionAxis = dir;

#elif UNITY_2017
            dir = (DirectionAxis)EditorGUI.EnumFlagsField(dirRect, item.directionAxis);#else
            item.directionAxis = dir;
            dir = (DirectionAxis)EditorGUI.EnumMaskField(dirRect, item.directionAxis);
#endif
        }
    }
}
