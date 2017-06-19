using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using Ntreev.Library.Psd;
using System;
using PSDUnity;
public class PSDPreviewWindow : EditorWindow
{
    public class LayerNode
    {
        public bool isExpanded { get; private set; }
        public bool selected { get; private set; }
        public int indent = 0;
        public GUIContent content;
        public List<LayerNode> childs = new List<LayerNode>();
        public IPsdLayer layer;
        public LayerType layerType;

        private GUIContent _groupff;
        private GUIContent _groupOn;
        private GUIContent _spritenormal;

        public LayerNode(IPsdLayer layer)
        {
            this.layer = layer;
            if (layer is PsdDocument)
            {
                layerType = LayerType.Group;
            }
            else
            {
                layerType = (layer as PsdLayer).LayerType;
            }
            switch (layerType)
            {
                case LayerType.Normal:
                    _spritenormal = new GUIContent(layer.Name, EditorGUIUtility.IconContent("createrect").image);
                    break;
                case LayerType.SolidImage:
                    _spritenormal = new GUIContent(layer.Name, EditorGUIUtility.IconContent("iconselector back").image);
                    break;
                case LayerType.Text:
                    _spritenormal = new GUIContent(layer.Name, EditorGUIUtility.IconContent("eventpin").image);
                    break;
                case LayerType.Group:
                    _groupff = new GUIContent(layer.Name, EditorGUIUtility.IconContent("IN foldout focus").image);
                    _groupOn = new GUIContent(layer.Name, EditorGUIUtility.IconContent("IN foldout focus on").image);
                    break;
                default:
                    break;
            }

            content = layerType == LayerType.Group ? _groupff : _spritenormal;
        }

        public void Expland(bool on)
        {
            content = on ? _groupOn : _groupff;
            content.text = layer.Name;
            isExpanded = on;
        }
        public void Select(bool on)
        {
            selected = on;
            if (childs != null)
                foreach (var child in childs)
                {
                    child.Select(on);
                }
        }
    }

    [MenuItem("PsdUnity/ConfigWindow")]
    static void OpenPSDConfigWindow()
    {
        window = GetWindow<PSDPreviewWindow>();
        window.position = new Rect(100, 100, 800, 600);
        window.Repaint();
    }

    private const string Prefs_pdfPath = "Prefs_pdfPath";
    private string psdPath;
    private Ntreev.Library.Psd.PsdDocument psd;
    private LayerNode data;

    private static PSDPreviewWindow window;
    private SerializedProperty scriptProp;

    private void OnEnable()
    {
        scriptProp = new SerializedObject(this).FindProperty("m_Script");
        psdPath = EditorPrefs.GetString(Prefs_pdfPath);
    }
    private void OnGUI()
    {
        EditorGUILayout.PropertyField(scriptProp);
        if (DrawFileSelect())
        {
            DrawData(data);
            DrawTools();
        }
        else
        {
            DrawErrBox("请先选择正确的PDF文件路径");
        }
    }
    private bool DrawFileSelect()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("PSD文件路径：");
            EditorGUI.BeginChangeCheck();

            psdPath = EditorGUILayout.TextField(psdPath);
            if (GUILayout.Button("选择"))
            {
                psdPath = EditorUtility.OpenFilePanel("选择一个pdf文件", psdPath, "psd");
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
        return psd != null;
    }
    private void DrawErrBox(string str)
    {
        EditorGUILayout.HelpBox(str, MessageType.Error);
    }


    void DrawData(LayerNode data)
    {
        if (data.content != null)
        {
            EditorGUI.indentLevel = data.indent;
            DrawGUIData(data);
        }
        if (data.isExpanded)
            for (int i = 0; i < data.childs.Count; i++)
            {
                LayerNode child = data.childs[i];
                if (child.content != null)
                {
                    EditorGUI.indentLevel = child.indent;
                    if (child.childs.Count > 0)
                    {
                        DrawData(child);
                    }
                    else
                    {
                        DrawGUIData(child);
                    }
                }
            }
    }

    private void DrawTools()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("创建模板"))
            {
                var atlasObj = ScriptableObject.CreateInstance<AtlasObject>();
                atlasObj.psdFile = psdPath;
                ProjectWindowUtil.CreateAsset(atlasObj, "atlasObj.asset");
            }
        }
    }

    void DrawGUIData(LayerNode data)
    {
        GUIStyle style = "Label";
        Rect rt = GUILayoutUtility.GetRect(data.content, style);

        var offset = (16 * EditorGUI.indentLevel);
        var pointWidth = 10;

        var expanded = EditorGUI.Toggle(new Rect(rt.x + offset, rt.y, pointWidth, rt.height), data.isExpanded, style);
        if (data.isExpanded != expanded && data.layerType == LayerType.Group)
        {
            data.Expland(expanded);
        }

        var srect = new Rect(rt.x + offset, rt.y, rt.width - offset - pointWidth, rt.height);
        var selected = EditorGUI.Toggle(srect, data.selected, style);
        if (selected != data.selected)
        {
            data.Select(selected);
        }
        if (data.selected)
        {
            EditorGUI.DrawRect(srect, Color.gray);
        }

        EditorGUI.LabelField(rt, data.content);
    }

    private void OpenPsdDocument()
    {
        if (System.IO.File.Exists(psdPath))
        {
            psd = PsdDocument.Create(psdPath);
            data = new LayerNode(psd);
            LoadDataLayers(data);
        }
    }

    void LoadDataLayers(LayerNode data, int indent = 0)
    {
        if (data.content != null)
        {
            data.indent = indent;
        }

        foreach (var layer in data.layer.Childs)
        {
            if (data.content != null)
            {
                LayerNode child = new LayerNode(layer);
                child.indent = indent + 1;
                data.childs.Add(child);
                LoadDataLayers(child, child.indent);
            }
        }
    }

    void OnDisable()
    {
        if (psd != null) psd.Dispose();
    }
}
