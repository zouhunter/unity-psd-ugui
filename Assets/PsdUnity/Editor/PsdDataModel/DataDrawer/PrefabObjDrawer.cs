using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using UnityEditor;
using Rotorz.ReorderableList;
using System;

namespace PSDUnity
{

    [CustomEditor(typeof(PrefabObject))]
    public class PrefabObjDrawer : Editor
    {
        PrefabObject obj;
        void OnEnable()
        {
            obj = target as PrefabObject;
        }
        public override void OnInspectorGUI()
        {
            ReorderableListGUI.Title("预制体列表");
            ReorderableListGUI.ListField<PrefabObject.PrefabItem>(obj.prefabs,OnDrawItem,OnDrawEmpte,EditorGUIUtility.singleLineHeight);
        }

        private void OnDrawEmpte()
        {
        }

        private PrefabObject.PrefabItem OnDrawItem(Rect position, PrefabObject.PrefabItem item)
        {
            var rect = new Rect(position.x, position.y, position.width * 0.3f, position.height);
            item.prefabName = (PrefabName)EditorGUI.EnumPopup(rect, item.prefabName);
            rect = new Rect(position.x + position.width * 0.4f, rect.y, rect.width * 2, rect.height);
            item.prefab = EditorGUI.ObjectField(rect, item.prefab, typeof(GameObject), false) as GameObject;
            return item;
        }
    }

}