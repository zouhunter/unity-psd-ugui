//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections.Generic;
//using UnityEditor;
//using Ntreev.Library.Psd;
//using System;
//using PSDUnity;
//using PSDUnity.Data;
//namespace PSDUnity.Analysis
//{
//    public partial class PSDPreviewWindow
//    {
//        public class LayerNode
//        {
//            public bool isExpanded { get; private set; }
//            public bool selected { get; private set; }
//            public int indent = 0;
//            public GUIContent content;
//            public List<LayerNode> childs = new List<LayerNode>();
//            public IPsdLayer layer;
//            public LayerType layerType;

//            private GUIContent _groupff;
//            private GUIContent _groupOn;
//            private GUIContent _spritenormal;

//            public LayerNode(IPsdLayer layer)
//            {
//                this.layer = layer;
//                if (layer is PsdDocument)
//                {
//                    layerType = LayerType.Group;
//                }
//                else
//                {
//                    layerType = (layer as PsdLayer).LayerType;
//                }
//                switch (layerType)
//                {
//                    case LayerType.Normal:
//                        _spritenormal = new GUIContent(layer.Name, EditorGUIUtility.IconContent("createrect").image);
//                        break;
//                    case LayerType.SolidImage:
//                        _spritenormal = new GUIContent(layer.Name, EditorGUIUtility.IconContent("iconselector back").image);
//                        break;
//                    case LayerType.Text:
//                        _spritenormal = new GUIContent(layer.Name, EditorGUIUtility.IconContent("eventpin").image);
//                        break;
//                    case LayerType.Group:
//                        _groupff = new GUIContent(layer.Name, EditorGUIUtility.IconContent("IN foldout focus").image);
//                        _groupOn = new GUIContent(layer.Name, EditorGUIUtility.IconContent("IN foldout focus on").image);
//                        break;
//                    default:
//                        break;
//                }

//                content = layerType == LayerType.Group ? _groupOn : _spritenormal;
//                isExpanded = true;
//            }

//            public void Expland(bool on)
//            {
//                content = on ? _groupOn : _groupff;
//                content.text = layer.Name;
//                isExpanded = on;
//            }
//            public void Select(bool on)
//            {
//                selected = on;
//                if (childs != null)
//                    foreach (var child in childs)
//                    {
//                        child.Select(on);
//                    }
//            }
//        }

//    }

//    public partial class PSDPreviewWindow : EditorWindow
//    {
//        //[MenuItem("PsdUnity/PSDView")]
//        //static void OpenPSDConfigWindow()
//        //{
//        //    window = GetWindow<PSDPreviewWindow>();
//        //    window.position = new Rect(100, 100, 800, 600);
//        //    window.Repaint();
//        //}

//        //[MenuItem("PsdUnity/exporter")]
//        //static void CreateTemp()
//        //{
//        //    Createexporter("");
//        //}
//        private const string Prefs_pdfPath = "Prefs_pdfPath";
//        private string psdPath;
//        private PsdDocument psd;
//        private LayerNode data;
//        private static PSDPreviewWindow window;
//        private SerializedProperty scriptProp;
//        private List<Texture2D> both;
//        private List<Texture2D> generated;
//        private Vector2 nodesVec;
//        private Vector2 previewVec;
//        private void OnEnable()
//        {
//            both = new List<Texture2D>();
//            generated = new List<Texture2D>();
//            scriptProp = new SerializedObject(this).FindProperty("m_Script");
//            psdPath = EditorPrefs.GetString(Prefs_pdfPath);
//        }
//        private void OnGUI()
//        {
//            EditorGUILayout.PropertyField(scriptProp);
//            if (DrawFileSelect())
//            {
//                using (var vec = new EditorGUILayout.ScrollViewScope(nodesVec, GUILayout.Height(300)))
//                {
//                    nodesVec = vec.scrollPosition;
//                    DrawData(data);
//                    EditorGUI.indentLevel = 0;
//                }
//                DrawTools();
//                using (var vec = new EditorGUILayout.ScrollViewScope(previewVec))
//                {
//                    previewVec = vec.scrollPosition;
//                    //Rotorz.ReorderableList.ReorderableListGUI.ListField<Texture2D>(generated, DrawTextureItem, DrawEmpty, 100,ReorderableListFlags.HideAddButton);
//                    //if (both != null){
//                    //    Rotorz.ReorderableList.ReorderableListGUI.ListField<Texture2D>(both, DrawTextureItem, () => { }, 300,ReorderableListFlags.HideAddButton);
//                    //}
//                }
//            }
//            else
//            {
//                EditorGUILayout.HelpBox("请先选择正确的PDF文件路径", MessageType.Warning);
//            }
//        }



//        private bool DrawFileSelect()
//        {
//            using (var hor = new EditorGUILayout.HorizontalScope())
//            {
//                EditorGUILayout.LabelField("PSD文件路径：");
//                EditorGUI.BeginChangeCheck();

//                psdPath = EditorGUILayout.TextField(psdPath);
//                if (GUILayout.Button("选择"))
//                {
//                    psdPath = EditorUtility.OpenFilePanel("选择一个pdf文件", psdPath, "psd");
//                    if (!string.IsNullOrEmpty(psdPath))
//                    {
//                        OpenPsdDocument();
//                    }
//                }
//                var change = EditorGUI.EndChangeCheck();
//                if (change)
//                {
//                    EditorPrefs.SetString(Prefs_pdfPath, psdPath);
//                }
//            }
//            if (!string.IsNullOrEmpty(psdPath) && psd == null)
//            {
//                OpenPsdDocument();
//            }
//            return psd != null;
//        }
//        private void DrawData(LayerNode data)
//        {
//            if (data.content != null)
//            {
//                EditorGUI.indentLevel = data.indent;
//                DrawGUIData(data);
//            }
//            if (data.isExpanded)
//                for (int i = 0; i < data.childs.Count; i++)
//                {
//                    LayerNode child = data.childs[i];
//                    if (child.content != null)
//                    {
//                        EditorGUI.indentLevel = child.indent;
//                        if (child.childs.Count > 0)
//                        {
//                            DrawData(child);
//                        }
//                        else
//                        {
//                            DrawGUIData(child);
//                        }
//                    }
//                }
//        }
//        private void DrawGUIData(LayerNode data)
//        {
//            GUIStyle style = "Label";
//            Rect rt = GUILayoutUtility.GetRect(data.content, style);

//            var offset = (16 * EditorGUI.indentLevel);
//            var pointWidth = 10;

//            var expanded = EditorGUI.Toggle(new Rect(rt.x + offset, rt.y, pointWidth, rt.height), data.isExpanded, style);
//            if (data.isExpanded != expanded && data.layerType == LayerType.Group)
//            {
//                data.Expland(expanded);
//            }

//            var srect = new Rect(rt.x + offset, rt.y, rt.width - offset - pointWidth, rt.height);
//            var selected = EditorGUI.Toggle(srect, data.selected, style);
//            if (selected != data.selected)
//            {
//                data.Select(selected);
//            }
//            if (data.selected)
//            {
//                EditorGUI.DrawRect(srect, Color.gray);
//            }

//            EditorGUI.LabelField(rt, data.content);
//        }

//        private void DrawTools()
//        {
//            EditorGUI.indentLevel = 0;
//            using (var hor = new EditorGUILayout.HorizontalScope())
//            {
//                if (GUILayout.Button("预览图片"))
//                {
//                    generated.Clear();
//                    RetriveArtLayer(data, (item) =>
//                    {
//                        if (item.selected)
//                        {
//                            var texture = ExportUtility.CreateTexture((PsdLayer)item.layer);
//                            texture.name = item.layer.Name;
//                            generated.Add(texture);
//                        }

//                    });
//                }

//                if (GUILayout.Button("所有图片"))
//                {
//                    generated.Clear();
//                    RetriveArtLayer(data, (item) =>
//                    {
//                        var texture = ExportUtility.CreateTexture((PsdLayer)item.layer);
//                        texture.name = item.layer.Name;
//                        generated.Add(texture);
//                    });
//                }
//                if (GUILayout.Button("整体效果"))
//                {
//                    generated.Clear();
//                    both.Clear();
//                    RetriveRootLayer(data, (root) =>
//                    {
//                        Texture2D texture = new Texture2D(psd.Width, psd.Height);
//                        RetriveArtLayer(root, (item) =>
//                        {
//                            var titem = ExportUtility.CreateTexture((PsdLayer)item.layer);
//                            for (int x = 0; x < titem.width; x++)
//                            {
//                                for (int y = 0; y < titem.height; y++)
//                                {
//                                    var color = titem.GetPixel(x, y);
//                                    if (color != Color.clear)
//                                        texture.SetPixel(x + item.layer.Left, psd.Height - (titem.height - y + item.layer.Top), color);
//                                }
//                            }
//                        });
//                        texture.Apply();
//                        both.Add(texture);
//                    });
//                }
                
//                if (GUILayout.Button("创建模板"))
//                {
//                    Createexporter(psdPath);
//                }
//            }
//        }
//        private static void Createexporter(string path)
//        {
//            var exporter = ScriptableObject.CreateInstance<Exporter>();
//            exporter.psdFile = path;
//            ProjectWindowUtil.CreateAsset(exporter, "exporter.asset");
//        }

//        private void RetriveArtLayer(LayerNode data, UnityAction<LayerNode> onRetrive)
//        {
//            if (data.layerType != LayerType.Group && data.layerType != LayerType.Divider)
//            {
//                onRetrive(data);
//            }
//            else
//            {
//                if (data.childs != null)
//                    foreach (var item in data.childs)
//                    {
//                        RetriveArtLayer(item, onRetrive);
//                    }
//            }
//        }
//        private void RetriveRootLayer(LayerNode data, UnityAction<LayerNode> onRetrive)
//        {
//            if (data.selected && data.layerType == LayerType.Group)
//            {
//                onRetrive(data);
//            }
//            else
//            {
//                if (data.childs != null)
//                    foreach (var item in data.childs)
//                    {
//                        RetriveRootLayer(item, onRetrive);
//                    }
//            }
//        }

//        private void DrawEmpty()
//        {
//            //EditorGUILayout.HelpBox("请先选择层级", MessageType.Warning);
//        }

//        private Texture2D DrawTextureItem(Rect position, Texture2D item)
//        {
//            var width = (item.width / (item.height + 0f)) * position.height;
//            var posx = (position.width - width) * 0.5f;
//            var rect = new Rect(posx, position.y, width, position.height);
//            EditorGUI.DrawTextureTransparent(rect, item, ScaleMode.ScaleToFit);
//            EditorGUI.LabelField(position, new GUIContent(item.name));
//            return item;
//        }

//        private void OpenPsdDocument()
//        {
//            if (System.IO.File.Exists(psdPath))
//            {
//                psd = PsdDocument.Create(psdPath);
//                data = new LayerNode(psd);
//                LoadDataLayers(data);
//            }
//        }
//        private void LoadDataLayers(LayerNode data, int indent = 0)
//        {
//            if (data.content != null)
//            {
//                data.indent = indent;
//            }

//            foreach (var layer in data.layer.Childs)
//            {
//                if (data.content != null)
//                {
//                    LayerNode child = new LayerNode(layer);
//                    child.indent = indent + 1;
//                    data.childs.Add(child);
//                    LoadDataLayers(child, child.indent);
//                }
//            }
//        }
//        void OnDisable()
//        {
//            if (psd != null) psd.Dispose();
//        }
//    }
//}