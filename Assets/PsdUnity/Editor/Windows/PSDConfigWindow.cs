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
        public Data(IPsdLayer layer)
        {
            this.layer = layer;
            isFolder = layer.Childs.Length != 0;
            if (!isFolder)
            {
                Debug.Log("-->");
                foreach (var item in layer.Resources)
                {
                    Debug.Log(item.Key);
                    Debug.Log(item.Value);
                }
            }
              
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
        if (data.isSelected && data.isFolder)
        {
            EditorGUI.DrawRect(rt, Color.gray);
        }

        rt.x += (16 * EditorGUI.indentLevel);

        data.isSelected = EditorGUI.Toggle(rt, data.isSelected, style);

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
        GUIContent content = GetGUIContent(data.layer);

        if (content != null)
        {
            data.indent = indent;
            data.content = content;
        }

        foreach (var layer in data.layer.Childs)
        {
            content = GetGUIContent(layer);
            if (content != null)
            {
                Data child = new Data(layer);
                child.indent = indent + 1;
                child.content = content;
                data.childs.Add(child);
                LoadDataLayers(child, child.indent);
            }
        }
    }
    GUIContent GetGUIContent(IPsdLayer layer)
    {
        Texture texture = null;
        if (layer.Childs != null && layer.Childs.Length > 0)
        {
            texture = EditorGUIUtility.IconContent("IN foldout focus").image;
        }
        else
        {
            texture = EditorGUIUtility.IconContent("eventpin").image;
        }
        return new GUIContent(layer.Name, texture);
    }

}
