using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using PSDUnity.Data;

namespace PSDUnity.UGUI
{
    [CustomLayer(typeof(GroupLayerImport))]
    public class GroupLayerImportGUI : UGUI.LayerImportGUI
    {
        public override Texture Icon
        {
            get
            {
                return EditorGUIUtility.IconContent("GridLayoutGroup Icon").image;
            }
        }
        public override void HeadGUI(Rect dirRect, GroupNode item)
        {
            base.HeadGUI(dirRect, item);
            var dir = (DirectionAxis)EditorGUI.EnumPopup(dirRect, item.directionAxis);
            if (dir == DirectionAxis.Horizontal || dir == DirectionAxis.Vertical)
            {
                item.directionAxis = dir;
            }
            if (item.directionAxis == 0) item.directionAxis = DirectionAxis.Horizontal;
            var spanRect = dirRect;
            spanRect.width *= 0.5f;
            spanRect.x -= 50;
            spanRect.height *= 0.8f;
            spanRect.y += 2f;
            item.spacing = EditorGUI.Slider(spanRect, item.spacing, 0, 50);
        }
    }
}
