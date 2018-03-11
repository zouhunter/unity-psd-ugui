using UnityEngine;
using System.Collections.Generic;
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
using System;
using PrefabItem = PSDUnity.Data.RuleObject.PrefabItem;

namespace PSDUnity.Data
{
    [CustomEditor(typeof(RuleObject))]
    public class RuleObjectDrawer : Editor
    {
        RuleObject obj;
        SerializedProperty scriptProp;
        public SerializedProperty sepraterCharProp;
        public SerializedProperty argumentCharProp;
        void OnEnable()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            sepraterCharProp = serializedObject.FindProperty("sepraterChar");
            argumentCharProp= serializedObject.FindProperty("argumentChar");
            obj = target as RuleObject;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(scriptProp);
            //ReorderableListGUI.Title("预制体列表");
            //ReorderableListGUI.ListField<RuleObject.PrefabItem>(obj.prefabs,OnDrawItem,EditorGUIUtility.singleLineHeight);
            if(GUILayout.Button("重置",EditorStyles.toolbarButton))
            {
                ResetObject();
            }
            serializedObject.Update();
        }
        private RuleObject.PrefabItem OnDrawItem(Rect position, RuleObject.PrefabItem item)
        {
            var rect = new Rect(position.x, position.y, position.width * 0.3f, position.height);
            item.groupType = (GroupType)EditorGUI.EnumPopup(rect, item.groupType);
            rect.x += rect.width + 5;
            item.keyName = EditorGUI.TextField(rect, item.keyName);
            rect.x += rect.width + 5;
            item.prefab = EditorGUI.ObjectField(rect, item.prefab, typeof(GameObject), false) as GameObject;
            return item;
        }
        public void ResetObject()
        {
            obj.prefabs.Clear();
            LoadDefultDatas(obj.prefabs);
            var path = UnityEditor.AssetDatabase.GetAssetPath(this);
            Debug.Log(UnityEditor.AssetDatabase.AssetPathToGUID(path));
        }

        private static void LoadDefultDatas(List<PrefabItem> prefabs)
        {
            var path = AssetDatabase.GUIDToAssetPath("9a94c0cbaca3a48468b0b1e51fefcbfb");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.BUTTON, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("dec381fc4475a4a33bcfd41351b26fcc");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.CANVAS, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("d6eef6649ab97db4f900c0f6a896a73e");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.DROPDOWN, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("a47d3ef1adf404641b6d23a93e59ee53");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.EMPTY, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("25904f280417ef748977530a357b5fb0");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.GROUP, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("ed417986c539a4fc7b09f220ff4ceef6");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.GRID, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("9d52d80f2b2a64fbe96cd04604170d32");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.IMAGE, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("2e48a2d5b2f2ccb439d0cf1eeba3e6dd");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.INPUTFIELD, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("b22710b4e94fd4947ae6eebd62b8ac32");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.RawIMAGE, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("2b101ae8ff3e18e4eb5aca168965d85c");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.SCROLLBAR, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("ca7d801212b074407a4fc212216da328");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.SCROLLVIEW, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("95fdb08286306794583b6e1d19021aaa");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.SLIDER, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("0f5f099613ddb4155b4c775d8db5984d");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.TEXT, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("f4c3d75106f0b944faa6f14731cec415");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.TOGGLE, obj));
                }
            }
        }
    }

}