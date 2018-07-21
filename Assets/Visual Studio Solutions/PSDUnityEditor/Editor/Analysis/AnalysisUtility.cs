using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using Ntreev.Library.Psd;
using PSDUnity;
using PSDUnity.Analysis;
using System.Reflection;
using System.Linq;
using PSDUnity.UGUI;

namespace PSDUnity
{
    public class AnalysisUtility
    {
        public static Data.RuleObject RuleObj { get; private set; }
        private static Dictionary<string, Texture> previewIcons = new Dictionary<string, Texture>();
        private static Dictionary<string, LayerImportGUI> drawerDic = new Dictionary<string, LayerImportGUI>();
        private static LayerImportGUI[] _layerImportEditorTypes;
        public static LayerImportGUI[] layerImportEditorTypes
        {
            get
            {
                if (_layerImportEditorTypes == null)
                {
                    var types = LoadAllLayerImporterEditors();
                    _layerImportEditorTypes = new LayerImportGUI[types.Length];
                    for (int i = 0; i < types.Length; i++)
                    {
                        _layerImportEditorTypes[i] = Activator.CreateInstance(types[i]) as LayerImportGUI;
                    }
                }
                return _layerImportEditorTypes;
            }
        }
        private static string[] _layerImportEditorOptions;
        public static string[] layerImportEditorOptions
        {
            get
            {
                if (_layerImportEditorOptions == null)
                {
                    if (RuleObj == null)
                        Debug.LogError("init enviroment first!");

                    _layerImportEditorOptions = RuleObj.layerImports.ConvertAll(x => x.Suffix).ToArray();
                }
                return _layerImportEditorOptions;
            }
        }

        private static Type[] _layerImportTypes;
        public static Type[] layerImportTypes
        {
            get
            {
                if (_layerImportTypes == null)
                {
                    _layerImportTypes = LoadAllLayerImpoters();
                }
                return _layerImportTypes;
            }
        }

        static AnalysisUtility()
        {
            InitPreviewIcons();
        }

        static void InitPreviewIcons()
        {
            previewIcons.Add(LayerType.Group.ToString(), EditorGUIUtility.FindTexture("GameObject Icon"));
            previewIcons.Add(LayerType.Normal.ToString(), EditorGUIUtility.IconContent("Image Icon").image);
            previewIcons.Add(LayerType.Color.ToString(), EditorGUIUtility.FindTexture("Material Icon"));
            previewIcons.Add(LayerType.Text.ToString(), EditorGUIUtility.IconContent("Text Icon").image);
            previewIcons.Add(LayerType.Complex.ToString(), EditorGUIUtility.IconContent("console.warnicon").image);
            previewIcons.Add(LayerType.Overflow.ToString(), EditorGUIUtility.IconContent("console.erroricon").image);
            previewIcons.Add("CANVAS", EditorGUIUtility.IconContent("Canvas Icon").image);

        }
        public static void InitEnviroment(Data.RuleObject ruleObj)
        {
            RuleObj = ruleObj;
        }

        public static Texture GetPreviewIcon(PreviewItem item, Data.RuleObject rule)
        {
            RuleObj = rule;
            if (rule == null || item.layerType != LayerType.Group)
            {
                return previewIcons[item.layerType.ToString()];
            }
            else
            {
                string groupType = PSDUnityConst.emptySuffix;
                string[] args;
                rule.AnalysisGroupName(item.name, out groupType, out args);
                if (!previewIcons.ContainsKey(groupType))
                {
                    previewIcons[groupType] = LoadTexture(groupType);
                }
                return previewIcons[groupType];
            }
        }

        private static Texture LoadTexture(string suffix)
        {
            var layerImportEditor = GetLayerImportEditor(suffix);
            if (layerImportEditor != null)
            {
                previewIcons[suffix] = layerImportEditor.Icon;
            }
            else
            {
                previewIcons[suffix] = EditorGUIUtility.FindTexture("GameObject Icon");
            }
            return previewIcons[suffix];
        }

        private static LayerImportGUI GetLayerImportEditor(string suffix)
        {
            if (RuleObj == null)
                Debug.LogError("init enviroment first!");

            var ruleObj = RuleObj;
            var importer = ruleObj.layerImports.Where(x => string.Compare(x.Suffix, suffix, true) == 0).FirstOrDefault();
            if (importer != null)
            {
                var supportTypes = from editorType in layerImportEditorTypes
                                   let atts = editorType.GetType().GetCustomAttributes(typeof(CustomLayerAttribute), true)
                                   where atts.Count() > 0
                                   select editorType;

                if (supportTypes.Count() > 0)
                {
                    LayerImportGUI layerImportEditor = null;
                    var type = importer.GetType();
                    while (type != typeof(LayerImport))
                    {
                        layerImportEditor = (from editorType in supportTypes
                                             let att = editorType.GetType().GetCustomAttributes(typeof(CustomLayerAttribute), true)[0] as CustomLayerAttribute
                                             where att.type == type
                                             select editorType as LayerImportGUI).FirstOrDefault();

                        if (layerImportEditor != null)
                        {
                            break;
                        }

                        type = type.BaseType;
                    }

                    if (layerImportEditor == null)
                        layerImportEditor = new EmplyLayerImportEditor();

                    return layerImportEditor;
                }
            }
            return new EmplyLayerImportEditor();
        }

        private static Type[] LoadAllLayerImpoters()
        {
            Type[] types = null;
            bool haveAssemble = false;
            Assembly innerAssemble = null;
            try
            {
                innerAssemble = Assembly.Load("Assembly-CSharp");
                haveAssemble = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                haveAssemble = false;
            }

            if (haveAssemble)
            {
                types = LoadAllInstenceTypes<UGUI.LayerImport>(typeof(UGUI.LayerImport).Assembly,innerAssemble).ToArray();
            }
            else
            {
                types = LoadAllInstenceTypes<UGUI.LayerImport>(typeof(UGUI.LayerImport).Assembly).ToArray();
            }

            return types;
        }

        private static Type[] LoadAllLayerImporterEditors()
        {

            Type[] types = null;
            bool haveAssemble = false;
            Assembly innerAssemble = null;
            try
            {
                innerAssemble = Assembly.Load("Assembly-CSharp-Editor");
                haveAssemble = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                haveAssemble = false;
            }

            if (haveAssemble)
            {
                types = LoadAllInstenceTypes<UGUI.LayerImportGUI>(typeof(UGUI.LayerImportGUI).Assembly, innerAssemble).ToArray();
            }
            else
            {
                types = LoadAllInstenceTypes<UGUI.LayerImportGUI>(typeof(UGUI.LayerImportGUI).Assembly).ToArray();
            }

            return types;
        }

        private static List<Type> LoadAllInstenceTypes<T>(params Assembly[] assemblys)
        {
            var baseType = typeof(T);
            var types = new List<Type>();
            for (int i = 0; i < assemblys.Length; i++)
            {
                var assembly = assemblys[i];
                var asbTypes = assembly.GetTypes();
                for (int j = 0; j < asbTypes.Length; j++)
                {
                    var userType = asbTypes[j];
                    if (userType.IsSubclassOf(baseType) && !userType.IsAbstract && !types.Contains(userType))
                    {
                        types.Add(userType);
                    }
                }
            }
            return types;
        }

        public static LayerImportGUI GetLayerEditor(string suffix)
        {
            LayerImportGUI drawer = null;
            if (drawerDic.ContainsKey(suffix))
            {
                drawer = drawerDic[suffix];
            }

            if (drawer == null || !drawerDic.ContainsKey(suffix))
            {
                drawer = drawerDic[suffix] = GetLayerImportEditor(suffix);
            }
            return drawer;
        }

    }

}
