using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using Ntreev.Library.Psd;
using UnityEditor.IMGUI.Controls;

namespace PSDUnity.Analysis
{
    public class PsdPreviewWindow : EditorWindow
    {
        const string Prefs_pdfPath = "Prefs_pdfPath";
        [SerializeField] TreeViewState m_TreeViewState = new TreeViewState();
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
        SearchField m_SearchField;
        PsdPreviewer m_TreeView;
        string psdPath;
        PsdDocument psd;
        float menuRetio = 0.3f;
        RuleObject ruleObj;
        private void OnEnable()
        {
            psdPath = EditorPrefs.GetString(Prefs_pdfPath);
            ruleObj = PsdResourceUtil.GetRuleObj();
        }

        private void OnGUI()
        {
            DrawFileSelect();

            if (psd != null)
            {
                TryInitTreeView();
                var width = position.width * menuRetio;
                var searchRect = new Rect(0, EditorGUIUtility.singleLineHeight, width, EditorGUIUtility.singleLineHeight);
                m_TreeView.searchString = m_SearchField.OnGUI(searchRect, m_TreeView.searchString);
                var treeViewRect = new Rect(0,EditorGUIUtility.singleLineHeight, width, position.height - 2 * EditorGUIUtility.singleLineHeight);

                GUI.Box(treeViewRect, "");
                m_TreeView.OnGUI(treeViewRect);

                var tooRect = new Rect(0, position.height - EditorGUIUtility.singleLineHeight, width, EditorGUIUtility.singleLineHeight);
                BottomToolBar(tooRect);

                var rightWidth = position.width * (1 - menuRetio);
                var viewRect = new Rect(width, EditorGUIUtility.singleLineHeight, rightWidth, position.height - 2 * EditorGUIUtility.singleLineHeight);
                DrawTexturePreview(viewRect);

                var previewRect = new Rect(width, position.height - EditorGUIUtility.singleLineHeight, rightWidth, EditorGUIUtility.singleLineHeight);
                PreviewToolBar(previewRect);
            }
            else
            {
                EditorGUILayout.HelpBox("请先选择正确的PDF文件路径", MessageType.Warning);
            }

        }

        private void DrawTexturePreview(Rect viewRect)
        {
            if (m_TreeView.currentTexture != null)
            {
                GUI.Label(viewRect, new GUIContent(m_TreeView.currentTexture), EditorStyles.centeredGreyMiniLabel);
            }
        }

        private void BottomToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);

            using (new EditorGUILayout.HorizontalScope())
            {
                var style = "miniButton";
                if (GUILayout.Button("Expand All", style))
                {
                    m_TreeView.ExpandAll();
                }

                if (GUILayout.Button("Collapse All", style))
                {
                    m_TreeView.CollapseAll();
                }
            }

            GUILayout.EndArea();
        }

        private void PreviewToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Draw"))
                {
                    m_TreeView.GenerateTexture(true);
                }

            }
            GUILayout.EndArea();
        }
        private void TryInitTreeView()
        {
            if(m_TreeView == null)
            {
                m_TreeView = new PsdPreviewer(m_TreeViewState, psd);
                m_TreeView.rule = ruleObj;
                m_TreeView.Reload();
       
                m_SearchField = new SearchField();
                m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
            }
          
        }

        private void DrawFileSelect()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("PSD文件路径：",GUILayout.Width(30));
                EditorGUI.BeginChangeCheck();
                psdPath = EditorGUILayout.TextField(psdPath);
                if (GUILayout.Button("选择",EditorStyles.miniButtonRight,GUILayout.Width(40)))
                {
                    string dir = Application.dataPath;
                    if (!string.IsNullOrEmpty(psdPath))
                    {
                        dir = System.IO.Path.GetDirectoryName(psdPath);
                    }
                    psdPath = EditorUtility.OpenFilePanel("选择一个pdf文件", dir, "psd");
                    if (!string.IsNullOrEmpty(psdPath)){
                        OpenPsdDocument();
                    }
                }
                var change = EditorGUI.EndChangeCheck();
                if (change)
                {
                    EditorPrefs.SetString(Prefs_pdfPath, psdPath);
                }
            }
            if (!string.IsNullOrEmpty(psdPath) && psd == null)
            {
                OpenPsdDocument();
            }
        }

        private void OpenPsdDocument()
        {
            if (System.IO.File.Exists(psdPath))
            {
                psd = PsdDocument.Create(psdPath);
                m_TreeView = null;
            }
        }
    }
}