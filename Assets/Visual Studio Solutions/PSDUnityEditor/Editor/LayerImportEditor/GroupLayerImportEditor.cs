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
    public class GroupLayerImportEditor : UGUI.LayerImportEditor
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
            var dir = (Direction)EditorGUI.EnumPopup(dirRect, item.direction);
            if (dir == Direction.Horizontal || dir == Direction.Vertical)
            {
                item.direction = dir;
            }
            if (item.direction == 0) item.direction = Direction.Horizontal;
            var spanRect = dirRect;
            spanRect.width *= 0.5f;
            spanRect.x -= 50;
            spanRect.height *= 0.8f;
            spanRect.y += 2f;
            item.spacing = EditorGUI.Slider(spanRect, item.spacing, 0, 50);
        }
    }
}
