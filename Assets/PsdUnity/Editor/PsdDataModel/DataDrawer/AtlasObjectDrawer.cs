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
        SerializedProperty uiSizeProp;
        SerializedProperty groupsProp;
        SerializedProperty forceSpriteProp;
        SerializedProperty atlasInfoProp;
        SerializedProperty prefabObjProp;
        AtlasObject obj;
        readonly GUIContent pageSizeContent = new GUIContent("界面尺寸", EditorGUIUtility.IconContent("breadcrump mid act").image, "界面尺寸");
        private void OnEnable()
        {
            obj = target as AtlasObject;
            scriptProp = serializedObject.FindProperty("m_Script");
            psdFileProp = serializedObject.FindProperty("psdFile");
            uiSizeProp = serializedObject.FindProperty("uiSize");
            groupsProp = serializedObject.FindProperty("groups");
            atlasInfoProp = serializedObject.FindProperty("atlasInfo");
            forceSpriteProp = serializedObject.FindProperty("forceSprite");
            prefabObjProp = serializedObject.FindProperty("prefabObj");
        }
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(obj, typeof(AtlasObject), false);
            EditorGUILayout.PropertyField(scriptProp);
            EditorGUI.EndDisabledGroup();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPageSize();
            DrawPictureData();
            DrawPictureOption();
            DrawGroupNode();
            DrawUICreateOption();
            DrawToolButtons();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUICreateOption()
        {
            EditorGUILayout.PropertyField(prefabObjProp, true);
            if (GUILayout.Button("创建规则"))
            {
                var obj = PrefabObject.CreateInstance<PrefabObject>();
                ProjectWindowUtil.CreateAsset(obj, "prefabObj.asset");
            }
        }

        private void DrawPictureData()
        {
            EditorGUILayout.PropertyField(atlasInfoProp,true);
        }

        private void DrawPageSize()
        {
            psdFileProp.stringValue = EditorGUILayout.TextField("PSD路径：",psdFileProp.stringValue);
            uiSizeProp.vector2Value = EditorGUILayout.Vector2Field(pageSizeContent, uiSizeProp.vector2Value);
        }
        private void DrawPictureOption()
        {
            EditorGUILayout.PropertyField(forceSpriteProp);
        }
        private void DrawGroupNode()
        {
            ReorderableListGUI.Title("信息列表");
            ReorderableListGUI.ListField(groupsProp);
        }
        private void DrawToolButtons()
        {
            if (GUILayout.Button("导出到UI"))
            {
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