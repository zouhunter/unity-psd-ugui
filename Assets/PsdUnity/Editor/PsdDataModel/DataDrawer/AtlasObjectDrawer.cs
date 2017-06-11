using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using Rotorz.ReorderableList;
using System;

namespace PSDUnity
{
    [CustomEditor(typeof(AtlasObject))]
    public class AtlasObjectDrawer : Editor
    {
        SerializedProperty scriptProp;
        SerializedProperty psdFileProp;
        SerializedProperty psdSizeProp;
        SerializedProperty groupsProp;

        readonly GUIContent pageSizeContent = new GUIContent("界面尺寸", EditorGUIUtility.IconContent("breadcrump mid act").image, "界面尺寸");
        private void OnEnable()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            psdFileProp = serializedObject.FindProperty("psdFile");
            psdSizeProp = serializedObject.FindProperty("psdSize");
            groupsProp = serializedObject.FindProperty("groups");
        }
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(scriptProp);
            EditorGUI.EndDisabledGroup();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPageSize();
            DrawGroupNode();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPageSize()
        {
            psdFileProp.stringValue = EditorGUILayout.TextField("PSD路径：",psdFileProp.stringValue);
            psdSizeProp.vector2Value = EditorGUILayout.Vector2Field(pageSizeContent, psdSizeProp.vector2Value);
        }
        private void DrawGroupNode()
        {
            ReorderableListGUI.Title("资源列表");
            ReorderableListGUI.ListField(groupsProp);

        }
    }

}