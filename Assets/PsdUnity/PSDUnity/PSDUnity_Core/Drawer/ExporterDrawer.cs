using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using System;
using Ntreev.Library.Psd;
using PSDUnity.Data;
using PSDUnity.UGUI;
using UnityEditor.IMGUI.Controls;

namespace PSDUnity.Analysis
{
    [CustomEditor(typeof(Exporter))]
    public  class ExporterDrawer : Editor
    {
        private SerializedProperty scriptProp;
        private Exporter exporter;
        private const string Prefs_LastPsdsDir = "lastPsdFileDir";
        private ExporterTreeView m_TreeView;
        private GroupNode rootNode;
        [SerializeField]
        TreeViewState m_TreeViewState = new TreeViewState();
        private void OnEnable()
        {
            exporter = target as Exporter;
            scriptProp = serializedObject.FindProperty("m_Script");
            AutoChargeRule();
            InitTreeView();
        }

        private void AutoChargeRule()
        {
            if (exporter.ruleObj == null)
            {
                exporter.ruleObj = RuleHelper.GetRuleObj();
            }
        }

        private void InitTreeView()
        {
            if (exporter.groups != null && exporter.groups.Count > 0)
            {
                if (exporter.groups.Count > 0)
                {
                    rootNode = TreeViewUtility.ListToTree<GroupNode>(exporter.groups);
                    m_TreeView = new ExporterTreeView(m_TreeViewState);
                    m_TreeView.root = rootNode;
                }
            }
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(exporter, typeof(Exporter), false);
            EditorGUILayout.PropertyField(scriptProp);
            EditorGUI.EndDisabledGroup();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPathOption();
            DrawConfigs();
            if (m_TreeView != null && rootNode != null)
            {
                DrawUICreateOption();
                DrawGroupNode();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUICreateOption()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                var style = "miniButton";
                var layout = GUILayout.Width(60);
                if (GUILayout.Button("Build-All", style, layout))
                {
                    var canvasObj = Array.Find(Selection.objects, x => x is GameObject && (x as GameObject).GetComponent<Canvas>() != null);
                    PSDImporter.InitEnviroment(exporter.ruleObj, exporter.ruleObj.defultUISize, canvasObj == null ? FindObjectOfType<Canvas>() : (canvasObj as GameObject).GetComponent<Canvas>());
                    PSDImporter.StartBuild(rootNode);
                    AssetDatabase.Refresh();
                }

                if (GUILayout.Button("Build-Sel", style, layout))
                {
                    var canvasObj = Array.Find(Selection.objects, x => x is GameObject && (x as GameObject).GetComponent<Canvas>() != null);
                    PSDImporter.InitEnviroment(exporter.ruleObj, exporter.ruleObj.defultUISize, canvasObj == null ? FindObjectOfType<Canvas>() : (canvasObj as GameObject).GetComponent<Canvas>());
                    var root = new GroupNode(new Rect(Vector2.zero, rootNode.rect.size), 0, -1);
                    root.displayName = "partial build";
                    foreach (var node in m_TreeView.selected){
                        root.AddChild(node);
                    }
                    PSDImporter.StartBuild(root);
                    AssetDatabase.Refresh();
                }

                if (GUILayout.Button("Expland", style, layout))
                {
                    m_TreeView.ExpandAll();
                }

            }

        }

        private void DrawGroupNode()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, m_TreeView.totalHeight);
            m_TreeView.OnGUI(rect);
        }

        private void RecordAllPsdInformation()
        {
            if (!string.IsNullOrEmpty(exporter.psdFile))
            {
                var psd = PsdDocument.Create(exporter.psdFile);
                if (psd != null)
                {
                    var rootSize = new Vector2(psd.Width, psd.Height);
                    ExportUtility.InitPsdExportEnvrioment(exporter, rootSize);
                    rootNode = new GroupNode(new Rect(Vector2.zero, rootSize), 0, -1);
                    rootNode.displayName = exporter.name;
                    var groupDatas = ExportUtility.CreatePictures(psd.Childs, rootSize, exporter.ruleObj.defultUISize, exporter.ruleObj.forceSprite);
                    if (groupDatas != null)
                    {
                        foreach (var groupData in groupDatas)
                        {
                            rootNode.AddChild(groupData);
                            ExportUtility.ChargeTextures(exporter, groupData);
                        }
                    }
                    TreeViewUtility.TreeToList<GroupNode>(rootNode, exporter.groups, true);
                    EditorUtility.SetDirty(exporter);
                    psd.Dispose();
                }
            }
        }

        private void DrawPathOption()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("文档路径:", GUILayout.Width(60));
                exporter.psdFile = EditorGUILayout.TextField(exporter.psdFile);

                if (GUILayout.Button("选择", GUILayout.Width(60)))
                {
                    var dir = PlayerPrefs.GetString(Prefs_LastPsdsDir);

                    if (string.IsNullOrEmpty(dir) && !string.IsNullOrEmpty(exporter.psdFile))
                    {
                        dir = System.IO.Path.GetDirectoryName(exporter.psdFile);
                    }

                    if (string.IsNullOrEmpty(dir))//|| !System.IO.Directory.Exists(dir)
                    {
                        dir = Application.dataPath;
                    }

                    var path = EditorUtility.OpenFilePanel("选择一个pdf文件", dir, "psd");

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (path.Contains(Application.dataPath))
                        {
                            path = path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                        }
                        exporter.psdFile = path;
                        PlayerPrefs.SetString(Prefs_LastPsdsDir, System.IO.Path.GetDirectoryName(path));
                    }

                }
            }
        }

        private void DrawConfigs()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("使用规则: " + "  [psd文件导出 + ui界面生成]");
                exporter.ruleObj = EditorGUILayout.ObjectField(exporter.ruleObj, typeof(RuleObject), false) as RuleObject;
                if (GUILayout.Button("创建", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("创建新规则", "确认后将生成新的规则文件！", "确认", "取消"))
                    {
                        exporter.ruleObj = RuleHelper.CreateRuleObject();
                    }
                }
            }

            if (GUILayout.Button("转换层级为图片，并记录索引", EditorStyles.toolbarButton))
            {
                if (EditorUtility.DisplayDialog("温馨提示", "重新加载目前将重写以下配制，继续请按确认！", "确认"))
                {
                    RecordAllPsdInformation();
                    m_TreeView = new ExporterTreeView(m_TreeViewState);
                    m_TreeView.root = rootNode;
                }

            }
        }

    }

}