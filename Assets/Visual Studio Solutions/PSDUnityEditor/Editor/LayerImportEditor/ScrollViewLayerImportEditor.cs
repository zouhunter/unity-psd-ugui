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
    public class ScrollViewLayerImportEditor : UGUI.LayerImportEditor
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
            var dir = (Direction)EditorGUI.EnumMaskField(dirRect, item.direction);
#elif UNITY_2017
                                dir = (Direction)EditorGUI.EnumFlagsField(dirRect, item.direction);
#else
                                dir = (Direction)EditorGUI.EnumMaskField(dirRect, item.direction);
#endif
            if (dir == Direction.Horizontal || dir == Direction.Vertical || dir == (Direction.Horizontal | Direction.Vertical))
            {
                item.direction = dir;
            }
        }
    }
}
