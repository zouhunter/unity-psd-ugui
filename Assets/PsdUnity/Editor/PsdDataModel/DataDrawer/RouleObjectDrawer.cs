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

namespace PSDUnity.Data
{

    [CustomEditor(typeof(RouleObject))]
    public class RouleObjectDrawer : Editor
    {
        RouleObject obj;
        SerializedProperty scriptProp;
        public SerializedProperty sepraterCharProp;
        public SerializedProperty argumentCharProp;
        void OnEnable()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            sepraterCharProp = serializedObject.FindProperty("sepraterChar");
            argumentCharProp= serializedObject.FindProperty("argumentChar");
            obj = target as RouleObject;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(scriptProp);
            //ReorderableListGUI.Title("组合命名规则");
            //EditorGUILayout.PropertyField(sepraterCharProp);
            //EditorGUILayout.PropertyField(argumentCharProp);
            //ReorderableListGUI.Title("图层命名规则");

            ReorderableListGUI.Title("预制体列表");
            ReorderableListGUI.ListField<RouleObject.PrefabItem>(obj.prefabs,OnDrawItem,OnDrawEmpte,EditorGUIUtility.singleLineHeight);
            serializedObject.Update();
        }

        private void OnDrawEmpte()
        {

        }

        private RouleObject.PrefabItem OnDrawItem(Rect position, RouleObject.PrefabItem item)
        {
            var rect = new Rect(position.x, position.y, position.width * 0.3f, position.height);
            item.groupType = (GroupType)EditorGUI.EnumPopup(rect, item.groupType);
            rect.x += rect.width + 5;
            item.keyName = EditorGUI.TextField(rect, item.keyName);
            rect.x += rect.width + 5;
            item.prefab = EditorGUI.ObjectField(rect, item.prefab, typeof(GameObject), false) as GameObject;
            return item;
        }
    }

}