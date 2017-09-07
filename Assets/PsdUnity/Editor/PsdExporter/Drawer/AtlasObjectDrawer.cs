using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using Rotorz.ReorderableList;
using System;
using Ntreev.Library.Psd;
using PSDUnity.Data;
using PSDUnity.Import;

namespace PSDUnity.Exprot
{
    [CustomEditor(typeof(AtlasObject))]
    public class AtlasObjectDrawer : Editor
    {
        SerializedProperty scriptProp;
        SerializedProperty psdFileProp;
        SerializedProperty groupsProp;
        SerializedProperty prefabObjProp;
        SerializedProperty settingObjProp;
        AtlasObject atlasObj;
        private void OnEnable()
        {
            atlasObj = target as AtlasObject;
            scriptProp = serializedObject.FindProperty("m_Script");
            psdFileProp = serializedObject.FindProperty("psdFile");
            groupsProp = serializedObject.FindProperty("groups");
            prefabObjProp = serializedObject.FindProperty("prefabObj");
            settingObjProp = serializedObject.FindProperty("settingObj");
        }
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(atlasObj, typeof(AtlasObject), false);
            EditorGUILayout.PropertyField(scriptProp);
            EditorGUI.EndDisabledGroup();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPathOption();
            DrawPictureOption();
            DrawGroupNode();
            DrawUICreateOption();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUICreateOption()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(prefabObjProp, true);
                if (GUILayout.Button("创建规则"))
                {
                    var obj = RouleObject.CreateInstance<RouleObject>();
                    ProjectWindowUtil.CreateAsset(obj, "Roule.asset");
                    prefabObjProp.objectReferenceValue = obj;
                }
                if (GUILayout.Button("导出到UI"))
                {
                    var atlasObj = (AtlasObject)target;
                    PSDImportUtility.InitEnviroment(atlasObj.prefabObj, atlasObj.settingObj.uiSize);
                    PSDImportCtrl import = new PSDImportCtrl();
                    import.Import(atlasObj.groups.ToArray(), atlasObj.settingObj.uiSize);
                }
            }

        }
        private void DrawGroupNode()
        {
            ReorderableListGUI.Title("信息列表");
            ReorderableListGUI.ListField(groupsProp);
        }

        private void SwitchLayerToTexture()
        {
            if (!string.IsNullOrEmpty(atlasObj.psdFile))
            {
                var psd = PsdDocument.Create(atlasObj.psdFile);
                if (psd != null)
                {
                    PsdExportUtility.InitPsdExportEnvrioment(atlasObj, new Vector2(psd.Width, psd.Height));
                    atlasObj.groups.Clear();
                    var groupDatas = PsdExportUtility.CreatePictures(psd.Childs, new Vector2(psd.Width, psd.Height), atlasObj.settingObj.uiSize, atlasObj.settingObj.forceSprite);
                    if (groupDatas != null)
                    {
                        foreach (var groupData in groupDatas)
                        {
                            PsdExportUtility.ChargeTextures(atlasObj, groupData);
                            atlasObj.groups.Add(groupData);
                        }
                    }

                    EditorUtility.SetDirty(atlasObj);
                }
            }
        }


        private void DrawPathOption()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.SelectableLabel("文档路径:",GUILayout.Width(60));
                if (GUILayout.Button(new GUIContent(psdFileProp.stringValue, "点击此处选择文件夹！"), EditorStyles.textField))
                {
                    var dir = string.IsNullOrEmpty(psdFileProp.stringValue) ? Application.dataPath : System.IO.Path.GetDirectoryName(psdFileProp.stringValue);
                    var path = EditorUtility.OpenFilePanel("选择一个pdf文件", dir, "psd");
                    if (!string.IsNullOrEmpty(path))
                    {
                        psdFileProp.stringValue = path;
                    }
                }
            }

            //using (var hor = new EditorGUILayout.HorizontalScope())
            //{
            //    EditorGUILayout.SelectableLabel("导出路径:", GUILayout.Width(60));
            //    if (GUILayout.Button(new GUIContent(exportProp.stringValue,"选中文件夹后点击此处！"), EditorStyles.textField))
            //    {
            //        if (Selection.activeObject != null && ProjectWindowUtil.IsFolder(Selection.activeInstanceID))
            //        {
            //            exportProp.stringValue = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            //        }
            //    }
            //}
        } 
        private void DrawPictureOption()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(settingObjProp, true);
                if (GUILayout.Button("创建规则"))
                {
                    var obj = RouleObject.CreateInstance<SettingObject>();
                    ProjectWindowUtil.CreateAsset(obj, "Setting.asset");
                    settingObjProp.objectReferenceValue = obj;
                }
                if (GUILayout.Button("读取层级"))
                {
                    SwitchLayerToTexture();
                }
            }

          
        }
      
    }

}