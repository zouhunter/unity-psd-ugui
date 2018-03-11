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

        private void OnEnable()
        {
          
            psdPath = EditorPrefs.GetString(Prefs_pdfPath);
        }

        private void OnGUI()
        {
            DrawFileSelect();

            if (psd != null)
            {
                TryInitTreeView();

                var rect = GUILayoutUtility.GetRect(position.width, position.height - 2 * EditorGUIUtility.singleLineHeight);
                var searchRect = new Rect(rect.position.x, rect.position.y, 200, EditorGUIUtility.singleLineHeight);
                m_TreeView.searchString = m_SearchField.OnGUI(searchRect, m_TreeView.searchString);
                var treeViewRect = new Rect(rect.position.x, rect.position.y + EditorGUIUtility.singleLineHeight, rect.width * 0.3f, rect.height - EditorGUIUtility.singleLineHeight);

                GUI.Box(treeViewRect, "");
                m_TreeView.OnGUI(treeViewRect);
            }
            else
            {
                EditorGUILayout.HelpBox("请先选择正确的PDF文件路径", MessageType.Warning);
            }

        }
        private void TryInitTreeView()
        {
            if(m_TreeView == null)
            {
                m_TreeView = new PsdPreviewer(m_TreeViewState, psd);
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
                    psdPath = EditorUtility.OpenFilePanel("选择一个pdf文件", psdPath, "psd");
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