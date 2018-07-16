using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity.UGUI
{
    [CustomLayer(typeof(GridLayerImport))]
    public class GridLayerImportGUI : UGUI.LayerImportGUI
    {
        public override Texture Icon
        {
            get
            {
                return EditorGUIUtility.IconContent("GridLayoutGroup Icon").image;
            }
        }
        public override void HeadGUI(Rect dirRect, Data.GroupNode item)
        {
            base.HeadGUI(dirRect, item);

            var dir = (DirectionAxis)EditorGUI.EnumPopup(dirRect, item.directionAxis);
            if (dir == DirectionAxis.Horizontal || dir == DirectionAxis.Vertical){
                item.directionAxis = dir;
            }
            if (item.directionAxis == 0) item.directionAxis = DirectionAxis.Horizontal;
            var constenctCountRect = dirRect;
            constenctCountRect.width *= 0.5f;
            constenctCountRect.x -= 50;
            item.constraintCount = EditorGUI.IntField(constenctCountRect, item.constraintCount, EditorStyles.label);
        }
    }
}
