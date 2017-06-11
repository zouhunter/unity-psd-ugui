using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using Ntreev.Library.Psd;
using System;
using PSDUnity;
public class PSDConfigWindow : EditorWindow
{
    public class Data
    {
        public bool isExpanded { get; private set; }
        public bool selected { get; private set; }
        public int indent = 0;
        public GUIContent content;
        public List<Data> childs = new List<Data>();
        public IPsdLayer layer;
        public LayerType layerType;

        private GUIContent _groupff;
        private GUIContent _groupOn;
        private GUIContent _spritenormal;

        public Data(IPsdLayer layer)
        {
            this.layer = layer;
            if(layer is PsdDocument)
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
                    _spritenormal = new GUIContent(layer.Name, EditorGUIUtility.IconContent("iconselector back").image);
                    break;
                case LayerType.SolidImage:
                    _spritenormal = new GUIContent(layer.Name, EditorGUIUtility.IconContent("createrect").image);
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
        window = GetWindow<PSDConfigWindow>();
        window.position = new Rect(100, 100, 800, 600);
        window.Repaint();
    }
    private const string Prefs_pdfPath = "Prefs_pdfPath";
    private const string Prefs_atlasID = "Prefs_atlasID";
    private AtlasObject atlasObj;
    private string psdPath { get { return atlasObj.psdFile; } set { atlasObj.psdFile = value; } }
    private Ntreev.Library.Psd.PsdDocument psd;
    private Data data;

    private static PSDConfigWindow window;
    private SerializedProperty scriptProp;

    private void OnEnable()
    {
        scriptProp = new SerializedObject(this).FindProperty("m_Script");
        var guid = EditorPrefs.GetString(Prefs_atlasID);
        if (!string.IsNullOrEmpty(guid))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path))
            {
                atlasObj = AssetDatabase.LoadAssetAtPath<AtlasObject>(path);

            }
        }
    }
    private void OnGUI()
    {
        EditorGUILayout.PropertyField(scriptProp);
        if (DrawAtlasObj())
        {
            if (DrawFileSelect())
            {
                DrawData(data);
            }
            else
            {
                DrawErrBox("请先选择正确的PDF文件路径");
            }
        }
        else
        {
            DrawErrBox("请先选择正确的PDF文件路径");
        }

        if (GUILayout.Button("生成Atlas"))
        {
            foreach (var item in psd.Childs)
            {
                PsdExportUtility.CreateAtlas(item as PsdLayer, 1, 4096, string.Format("Assets/{0}.png", item.Name));
            }
        }
    }
    private bool DrawAtlasObj()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("AtlasObj:");
            EditorGUI.BeginChangeCheck();
            atlasObj = EditorGUILayout.ObjectField(atlasObj, typeof(AtlasObject), false) as AtlasObject;
            if (GUILayout.Button("创建"))
            {
                atlasObj = ScriptableObject.CreateInstance<AtlasObject>();
                ProjectWindowUtil.CreateAsset(atlasObj, "atlasObj.asset");
            }
            var change = EditorGUI.EndChangeCheck();
            if (change)
            {
                var path = AssetDatabase.GetAssetPath(atlasObj);
                EditorPrefs.SetString(Prefs_atlasID, AssetDatabase.AssetPathToGUID(path));
            }
        }

        return atlasObj != null;
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
                EditorUtility.SetDirty(atlasObj);
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


    void DrawData(Data data)
    {
        if (data.content != null)
        {
            EditorGUI.indentLevel = data.indent;
            DrawGUIData(data);
        }
        if (data.isExpanded)
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
            data = new Data(psd);
            LoadDataLayers(data);
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

    void OnDisable()
    {
        if (psd != null) psd.Dispose();
    }
}
