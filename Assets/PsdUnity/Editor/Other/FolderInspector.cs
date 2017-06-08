using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using PSDUnity;

[CustomEditor(typeof(UnityEditor.DefaultAsset))]
public class FolderInspector : Editor
{

    Data data;
    Data selectData;
    void OnEnable()
    {
        if (Directory.Exists(AssetDatabase.GetAssetPath(target)))
        {
            data = new Data();
            LoadFiles(data, AssetDatabase.GetAssetPath(Selection.activeObject));
        }

    }
    public override void OnInspectorGUI()
    {

        if (Directory.Exists(AssetDatabase.GetAssetPath(target)))
        {
            GUI.enabled = true;
            EditorGUIUtility.SetIconSize(Vector2.one * 16);
            DrawData(data);
        }

    }

    void LoadFiles(Data data, string currentPath, int indent = 0)
    {
        GUIContent content = GetGUIContent(currentPath);

        if (content != null)
        {
            data.indent = indent;
            data.content = content;
            data.assetPath = currentPath;

        }

        foreach (var path in Directory.GetFiles(currentPath))
        {
            content = GetGUIContent(path);
            if (content != null)
            {
                Data child = new Data();
                child.indent = indent + 1;
                child.content = content;
                child.assetPath = path;
                data.childs.Add(child);
            }
        }


        foreach (var path in Directory.GetDirectories(currentPath))
        {
            Data childDir = new Data();
            data.childs.Add(childDir);
            LoadFiles(childDir, path, indent + 1);
        }
    }



    void DrawData(Data data)
    {
        if (data.content != null)
        {
            EditorGUI.indentLevel = data.indent;
            DrawGUIData(data);

        }
        for (int i = 0; i < data.childs.Count; i++)
        {
            Data child = data.childs[i];
            if (child.content != null)
            {
                EditorGUI.indentLevel = child.indent;
                if (child.childs.Count > 0)
                    DrawData(child);
                else
                    DrawGUIData(child);
            }
        }
    }




    void DrawGUIData(Data data)
    {
        GUIStyle style = "Label";
        Rect rt = GUILayoutUtility.GetRect(data.content, style);
        if (data.isSelected)
        {
            EditorGUI.DrawRect(rt, Color.gray);
        }

        rt.x += (16 * EditorGUI.indentLevel);
        if (GUI.Button(rt, data.content, style))
        {
            if (selectData != null)
            {
                selectData.isSelected = false;
            }
            data.isSelected = true;
            selectData = data;
            Debug.Log(data.assetPath);
        }
    }

    GUIContent GetGUIContent(string path)
    {
        Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        if (asset)
        {
            return new GUIContent(asset.name, AssetDatabase.GetCachedIcon(path));
        }
        return null;
    }

    class Data
    {
        public bool isSelected = false;
        public int indent = 0;
        public GUIContent content;
        public string assetPath;
        public List<Data> childs = new List<Data>();
    }
}