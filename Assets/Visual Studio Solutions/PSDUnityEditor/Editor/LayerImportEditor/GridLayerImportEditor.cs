using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity.UGUI
{
    [CustomLayer(typeof(GridLayerImport))]
    public class GridLayerImportEditor : UGUI.LayerImportEditor
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

            var dir = (Direction)EditorGUI.EnumPopup(dirRect, item.direction);
            if (dir == Direction.Horizontal || dir == Direction.Vertical){
                item.direction = dir;
            }
            if (item.direction == 0) item.direction = Direction.Horizontal;
            var constenctCountRect = dirRect;
            constenctCountRect.width *= 0.5f;
            constenctCountRect.x -= 50;
            item.constraintCount = EditorGUI.IntField(constenctCountRect, item.constraintCount, EditorStyles.label);
        }
    }
}
