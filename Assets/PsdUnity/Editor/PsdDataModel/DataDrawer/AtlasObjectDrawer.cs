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
        //AtlasObject obj;
        readonly GUIContent pageSizeContent = new GUIContent("界面尺寸", EditorGUIUtility.IconContent("breadcrump mid act").image, "界面尺寸");
        private void OnEnable()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            psdFileProp = serializedObject.FindProperty("psdFile");
            psdSizeProp = serializedObject.FindProperty("psdSize");
            //obj = target as AtlasObject;
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
            DrawToolButtons();
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
        private void DrawToolButtons()
        {
            if (GUILayout.Button("导出到UI"))
            {
                PSDImportUtility.baseFilename = target.name;
                PSDImportUtility.baseDirectory = Application.dataPath;
                PSDImportCtrl import = new PSDImportCtrl((AtlasObject)target);
                import.BeginDrawUILayers();
                import.BeginSetUIParents(PSDImportUtility.uinode);
                import.BeginSetAnchers(PSDImportUtility.uinode.childs[0]);
                //最外层的要单独处理
                var rt = PSDImportUtility.uinode.childs[0].InitComponent<RectTransform>();
                PSDImportUtility.SetCustomAnchor(rt, rt);
                import.BeginReprocess(PSDImportUtility.uinode.childs[0]);//后处理
            }
        }
    }

}