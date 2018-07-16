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

namespace PSDUnity
{
    public class AnalysisUtility
    {
        private static Dictionary<string, Texture> previewIcons = new Dictionary<string, Texture>();
        private static Type[] _layerImportEditorTypes;
        public static Type[] layerImportEditorTypes
        {
            get
            {
                if (_layerImportEditorTypes == null)
                {
                    _layerImportEditorTypes = LoadAllLayerImporterEditors();
                }
                return _layerImportEditorTypes;
            }
        }

        private static Type[] _layerImportTypes;
        public static Type[] layerImportTypes
        {
            get
            {
                if(_layerImportTypes == null)
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
            //previewIcons.Add(GroupType.PANEL.ToString(), EditorGUIUtility.IconContent("RawImage Icon").image);

        }

        internal static Texture GetPreviewIcon(PreviewItem item, RuleObject rule)
        {
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
            var ruleObj = RuleHelper.GetRuleObj();
            var importer = ruleObj.layerImports.Where(x => string.Compare(x.Suffix, suffix, true) == 0).FirstOrDefault();
            if (importer != null)
            {
                var drawer = Editor.CreateEditor(importer) as UGUI.LayerImportEditor;
                if(drawer != null)
                {
                    return drawer.Icon;
                }
            }
            return EditorGUIUtility.FindTexture("GameObject Icon");
        }

        private static Type[] LoadAllLayerImpoters()
        {
            var types = LoadAllInstenceTypes<UGUI.LayerImport>(typeof(UGUI.LayerImport).Assembly, Assembly.Load("Assembly-CSharp"));
            return types.ToArray();
        }

        private static Type[] LoadAllLayerImporterEditors()
        {
            var types = LoadAllInstenceTypes<UGUI.LayerImportEditor>(typeof(UGUI.LayerImportEditor).Assembly, Assembly.Load("Assembly-CSharp-Editor"));
            return types.ToArray();
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
                    if(userType.IsSubclassOf(baseType) &&!userType.IsAbstract && !types.Contains(userType))
                    {
                        types.Add(userType);
                    }
                }
            }
            return types;
        }
    }

}
