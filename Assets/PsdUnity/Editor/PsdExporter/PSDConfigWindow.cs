using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using Ntreev.Library.Psd;
using System;

public class PSDConfigWindow : EditorWindow
{
    class Data
    {
        public bool isSelected = false;
        public int indent = 0;
        public GUIContent content;
        public List<Data> childs = new List<Data>();
        public IPsdLayer layer;
        public bool isFolder;

        private GUIContent _folderOff;
        private GUIContent _folderOn;
        private GUIContent _filenormal;

        public Data(IPsdLayer layer)
        {
            this.layer = layer;
            isFolder = layer.Childs.Length != 0;

            _folderOff =new GUIContent(layer.Name,EditorGUIUtility.IconContent("IN foldout focus").image);
            _folderOn = new GUIContent(layer.Name,EditorGUIUtility.IconContent("IN foldout focus on").image);
            _filenormal = new GUIContent(layer.Name, EditorGUIUtility.IconContent("eventpin").image);
            content = isFolder ? _folderOff : _filenormal;
        }

        public void Switch(bool on)
        {
            content =on ?_folderOn : _folderOff;
            content.text = layer.Name;
        }
    }

    [MenuItem("PsdUnity/ConfigWindow")]
    static void OpenPSDConfigWindow()
    {
        window = GetWindow<PSDConfigWindow>();
        window.position = new Rect(100, 100, 800, 600);
        window.Repaint();
    }
    private const string Prefs_pdfPath = "Prefs_pdfPath";
    private string psdPath;
    private Ntreev.Library.Psd.PsdDocument psd;
    private Data data;

    private static PSDConfigWindow window;
    private SerializedProperty scriptProp;

 

    private void OnEnable()
    {
        psdPath = EditorPrefs.GetString(Prefs_pdfPath);
        scriptProp = new SerializedObject(this).FindProperty("m_Script");
    }
    private void OnGUI()
    {
        EditorGUILayout.PropertyField(scriptProp);
        if (DrawFileSelect())
        {
            DrawData(data);
        }
        else
        {
            DrawHelpBox();
        }
        if (GUILayout.Button("生成"))
        {
            var obj = AtlasObject.CreateInstance<AtlasObject>();
            ProjectWindowUtil.CreateAsset(obj, "atlsObj.asset");
        }
    }

    private bool DrawFileSelect()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("PSD文件路径：");
            psdPath = EditorGUILayout.TextField(psdPath);
            if (GUILayout.Button("选择"))
            {
                psdPath = EditorUtility.OpenFilePanel("选择一个pdf文件", psdPath, "psd");
                if(!string.IsNullOrEmpty(psdPath))
                {
                    EditorPrefs.SetString(Prefs_pdfPath, psdPath);
                    OpenPsdDocument();
                }
            }
        }
        if (!string.IsNullOrEmpty(psdPath) && psd == null)
        {
            OpenPsdDocument();
        }
        return psd != null;
    }
    private void DrawHelpBox()
    {
        EditorGUILayout.HelpBox("请先选择正确的PDF文件路径", MessageType.Error);
    }


    void DrawData(Data data)
    {
        if (data.content != null)
        {
            EditorGUI.indentLevel = data.indent;
            DrawGUIData(data);
        }
        if(data.isSelected)
        for (int i = 0; i < data.childs.Count; i++)
        {
            Data child = data.childs[i];
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

    void DrawGUIData(Data data)
    {
        GUIStyle style = "Label";
        Rect rt = GUILayoutUtility.GetRect(data.content, style);
       
        rt.x += (16 * EditorGUI.indentLevel);
        
        var select = EditorGUI.Toggle(rt, data.isSelected, style);
        if(data.isSelected != select && data.isFolder)
        {
            data.isSelected = select;
            data.Switch(select);
        }
        EditorGUI.LabelField(rt, data.content);
    }

    private void OpenPsdDocument()
    {
        using (PsdDocument document = PsdDocument.Create(psdPath))
        {
            data = new Data(document);
            LoadDataLayers(data);
            psd = document;
        }
    }

    void LoadDataLayers(Data data, int indent = 0)
    {
        if (data.content != null)
        {
            data.indent = indent;
        }

        foreach (var layer in data.layer.Childs)
        {
            if (data.content != null)
            {
                Data child = new Data(layer);
                child.indent = indent + 1;
                data.childs.Add(child);
                LoadDataLayers(child, child.indent);
            }
        }
    }
}
