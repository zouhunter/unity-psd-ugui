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
        private SearchField m_SearchField;
        private PsdPreviewer m_TreeView;
        private string psdPath;
        private PsdDocument psd;
        private float menuRetio = 0.3f;
        private RuleObject ruleObj;
        private SettingObject settingObj;
        private Exporter exporter;
        private void OnEnable()
        {
            psdPath = EditorPrefs.GetString(Prefs_pdfPath);
            ruleObj = PsdResourceUtil.GetRuleObj();
            settingObj = PsdResourceUtil.GetSettingObj();
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
                var treeViewRect = new Rect(0, EditorGUIUtility.singleLineHeight, width, position.height - 2 * EditorGUIUtility.singleLineHeight);

                GUI.Box(treeViewRect, "");
                m_TreeView.OnGUI(treeViewRect);

                var toolRect = new Rect(0, position.height - EditorGUIUtility.singleLineHeight, width, EditorGUIUtility.singleLineHeight);
                BottomToolBar(toolRect);

                var rightWidth = position.width * (1 - menuRetio);
                var configRect = new Rect(width, EditorGUIUtility.singleLineHeight, rightWidth, EditorGUIUtility.singleLineHeight);
                DrawConfigs(configRect);

                var viewRect = new Rect(width, 2 * EditorGUIUtility.singleLineHeight, rightWidth, position.height - 3 * EditorGUIUtility.singleLineHeight);
                DrawTexturePreview(viewRect);

                var previewToolRect = new Rect(width, position.height - EditorGUIUtility.singleLineHeight, rightWidth, EditorGUIUtility.singleLineHeight);
                PreviewToolBars(previewToolRect);
            }
            else
            {
                EditorGUILayout.HelpBox("请先选择正确的PDF文件路径", MessageType.Warning);
            }

        }

        private void DrawConfigs(Rect configRect)
        {
            GUI.Box(configRect, "自定义区");

            var groupWidth = configRect.width * 0.3f;

            var titleRect = new Rect(configRect.x, configRect.y, groupWidth * 0.3f, configRect.height);
            if (GUI.Button(titleRect, "[规则]", EditorStyles.label))
            {
                if (EditorUtility.DisplayDialog("创建新规则", "确认后将生成新的规则文件！", "确认", "取消"))
                {
                    ruleObj = ScriptableObject.CreateInstance<RuleObject>();
                    ProjectWindowUtil.CreateAsset(ruleObj, "new rule");
                }
            }
            var contentRect = new Rect(configRect.x + configRect.width * 0.1f, configRect.y, groupWidth * 0.7f, configRect.height);
            ruleObj = EditorGUI.ObjectField(contentRect, ruleObj, typeof(RuleObject), false) as RuleObject;



            titleRect.x += configRect.width - groupWidth;
            contentRect.x += configRect.width - groupWidth;
            if (GUI.Button(titleRect, "[设置]", EditorStyles.label))
            {
                if (EditorUtility.DisplayDialog("创建新设置", "确认后将生成新的设置文件！", "确认", "取消"))
                {
                    settingObj = ScriptableObject.CreateInstance<SettingObject>();
                    ProjectWindowUtil.CreateAsset(settingObj, "new setting");
                }
            }
            settingObj = EditorGUI.ObjectField(contentRect, settingObj, typeof(SettingObject), false) as SettingObject;
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

        private void PreviewToolBars(Rect rect)
        {
            GUILayout.BeginArea(rect);

            using (new EditorGUILayout.HorizontalScope())
            {
                var style = EditorStyles.miniButtonRight;
                var layout = GUILayout.Width(100);
                m_TreeView.autoGroupTexture = GUILayout.Toggle(m_TreeView.autoGroupTexture, "Auto Draw Group", EditorStyles.toggle);
                if (!m_TreeView.autoGroupTexture)
                {
                    if (GUILayout.Button("Draw Groups", style, layout))
                    {
                        m_TreeView.GenerateTexture(true);
                    }
                }
                if (GUILayout.Button("Gen Assets", style, layout))
                {
                    RecordSelectedInformation();
                }
            }
            GUILayout.EndArea();
        }

        private void RecordSelectedInformation()
        {
            if (m_TreeView == null || m_TreeView.selected.Count == 0) return;
            var psdLayers = m_TreeView.selected.ConvertAll(x => x.layer as IPsdLayer).ToArray();

            if (exporter == null)
            {
                exporter = ScriptableObject.CreateInstance<Exporter>();
            }
            exporter.settingObj = settingObj;
            exporter.ruleObj = ruleObj;
            exporter.name = "exporter" + System.DateTime.Now.Second.ToString();
            ProjectWindowUtil.CreateAsset(exporter, exporter.name + ".asset");
            EditorUtility.SetDirty(exporter);

            ExportUtility.InitPsdExportEnvrioment(exporter, new Vector2(psd.Width, psd.Height));
            var rootNode = new GroupNode(new Rect(Vector2.zero, exporter.settingObj.defultUISize), 0, -1);
            rootNode.Name = exporter.name;

            var groupDatas = ExportUtility.CreatePictures(psdLayers, new Vector2(psd.Width, psd.Height), exporter.settingObj.defultUISize, exporter.settingObj.forceSprite);

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
        }


        private void TryInitTreeView()
        {
            if (m_TreeView == null)
            {
                m_TreeView = new PsdPreviewer(m_TreeViewState, psd);
                m_TreeView.SetRule(ruleObj);
                m_TreeView.Reload();

                m_SearchField = new SearchField();
                m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
            }

        }

        private void DrawFileSelect()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("PSD文件路径：", GUILayout.Width(30));
                EditorGUI.BeginChangeCheck();
                psdPath = EditorGUILayout.TextField(psdPath);
                if (GUILayout.Button("选择", EditorStyles.miniButtonRight, GUILayout.Width(40)))
                {
                    string dir = Application.dataPath;
                    if (!string.IsNullOrEmpty(psdPath))
                    {
                        dir = System.IO.Path.GetDirectoryName(psdPath);
                    }
                    psdPath = EditorUtility.OpenFilePanel("选择一个pdf文件", dir, "psd");
                    if (!string.IsNullOrEmpty(psdPath))
                    {
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