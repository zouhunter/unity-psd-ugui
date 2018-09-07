using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Reflection;

namespace PSDUnity.Data
{

    [CustomEditor(typeof(RuleObject),true)]
    public class RuleObjectDrawer : Editor
    {
        private bool isGloble;
        private int selected;
        private string[] options = { "图片生成", "资源导入", "字符匹配" };
        private Dictionary<int, Dictionary<string, List<SerializedProperty>>> propDic;
        private List<ReorderableList> reorderLists = new List<ReorderableList>();
        private SerializedObject tempObj;
        private SerializedProperty scriptProp;
        private Dictionary<string, List<SerializedProperty>> currentPropertys { get { return propDic[selected]; } }
        private void OnEnable()
        {
            isGloble = RuleHelper.IsGlobleRule(target as RuleObject);
            if (target == null)
                DestroyImmediate(this);
            else
            {
                RuleHelper.LoadImageImports(target as RuleObject, () => {
                    RuleHelper.LoadLayerImports(target as RuleObject);
                });
                InitPropertys();
                ChargeCurrent();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawScript();
            serializedObject.Update();
            DrawerHeadTools();
            DrawToolbar();
            DrawPropertys();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(scriptProp);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawerHeadTools()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (isGloble)
                {
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Defult Rule", EditorStyles.largeLabel);
                    GUI.color = Color.white;

                    if (GUILayout.Button("[Clean]", EditorStyles.miniLabel, GUILayout.Width(60)))
                    {
                        RuleHelper.SetDefultRuleObject(null);
                        isGloble = false;
                    }
                }
                else
                {
                    if (GUILayout.Button("Set As Defult", EditorStyles.largeLabel))
                    {
                        var ok = EditorUtility.DisplayDialog("设为默认", "如果设置为默认规则，新建项将使用该规则，确认请按继续", "继续");
                        if (ok)
                        {
                            RuleHelper.SetDefultRuleObject(target as RuleObject);
                            isGloble = true;
                        }
                    }
                }


            }
        }

        private void DrawToolbar()
        {
            EditorGUI.BeginChangeCheck();
            selected = GUILayout.Toolbar(selected, options);
            if (EditorGUI.EndChangeCheck())
            {
                ChargeCurrent();
            }
        }
        private void ChargeCurrent()
        {
            if (reorderLists == null)
                reorderLists = new List<ReorderableList>();
            else reorderLists.Clear();

            InitReorderLists();
        }

        private void InitReorderLists()
        {
            foreach (var propDic in currentPropertys)
            {
                var key = propDic.Key;
                var list = propDic.Value;
                var reorderList = new ReorderableList(list, typeof(SerializedProperty), true, true, false, false);
                reorderList.drawHeaderCallback = (rect) =>
                {
                    var labelRect = new Rect(rect.x, rect.y, rect.width - 60, rect.height);
                    EditorGUI.LabelField(labelRect, key);
                    var resetRect = new Rect(rect.width - 60, rect.y, 60, rect.height);
                    if (GUI.Button(resetRect, "重置", EditorStyles.miniButtonRight))
                    {
                        if (tempObj == null)
                        {
                            tempObj = new SerializedObject(RuleHelper.GetRuleObj());
                        }

                        foreach (var item in currentPropertys[key])
                        {
                            var path = item.propertyPath;
                            var tempProp = tempObj.FindProperty(path);
                            CopyPropertyValue(item, tempProp);
                        }
                    }
                };
                reorderList.elementHeightCallback = (index) =>
                {
                    var property = currentPropertys[key][index];
                    return EditorGUI.GetPropertyHeight(property, null, true);
                };
                reorderList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.PropertyField(rect, currentPropertys[key][index], true);
                };
                reorderLists.Add(reorderList);
            }

        }

        public static void CopyPropertyValue(SerializedProperty destProperty, SerializedProperty sourceProperty)
        {
            if (destProperty == null)
                throw new ArgumentNullException("destProperty");
            if (sourceProperty == null)
                throw new ArgumentNullException("sourceProperty");

            sourceProperty = sourceProperty.Copy();
            destProperty = destProperty.Copy();

            CopyPropertyValueSingular(destProperty, sourceProperty);

            if (sourceProperty.hasChildren)
            {
                int elementPropertyDepth = sourceProperty.depth;
                while (sourceProperty.Next(true) && destProperty.Next(true) && sourceProperty.depth > elementPropertyDepth)
                    CopyPropertyValueSingular(destProperty, sourceProperty);
            }
        }

        private static void CopyPropertyValueSingular(SerializedProperty destProperty, SerializedProperty sourceProperty)
        {
            switch (destProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.Boolean:
                    destProperty.boolValue = sourceProperty.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    destProperty.floatValue = sourceProperty.floatValue;
                    break;
                case SerializedPropertyType.String:
                    destProperty.stringValue = sourceProperty.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    destProperty.colorValue = sourceProperty.colorValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    destProperty.objectReferenceValue = sourceProperty.objectReferenceValue;
                    break;
                case SerializedPropertyType.LayerMask:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.Enum:
                    destProperty.enumValueIndex = sourceProperty.enumValueIndex;
                    break;
                case SerializedPropertyType.Vector2:
                    destProperty.vector2Value = sourceProperty.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    destProperty.vector3Value = sourceProperty.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    destProperty.vector4Value = sourceProperty.vector4Value;
                    break;
                case SerializedPropertyType.Rect:
                    destProperty.rectValue = sourceProperty.rectValue;
                    break;
                case SerializedPropertyType.ArraySize:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.Character:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    destProperty.animationCurveValue = sourceProperty.animationCurveValue;
                    break;
                case SerializedPropertyType.Bounds:
                    destProperty.boundsValue = sourceProperty.boundsValue;
                    break;
                case SerializedPropertyType.Gradient:
                    //!TODO: Amend when Unity add a public API for setting the gradient.
                    break;
            }
        }
        private void DrawPropertys()
        {
            foreach (var reorderList in reorderLists)
            {
                if (reorderList != null)
                {
                    reorderList.DoLayoutList();
                }
                else
                {
                    Debug.LogError(reorderList);
                }
            }

        }

        private void InitPropertys()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            propDic = new Dictionary<int, Dictionary<string, List<SerializedProperty>>>();
            var list = GetRuleItems();
            foreach (var item in list)
            {
                var prop = serializedObject.FindProperty(item.fieldName);
                if (prop != null)
                {
                    if (propDic.ContainsKey(item.id))
                    {
                        if (propDic[item.id].ContainsKey(item.key))
                        {
                            propDic[item.id][item.key].Add(prop);
                        }
                        else
                        {
                            propDic[item.id][item.key] = new List<SerializedProperty>() { prop };
                        }
                    }
                    else
                    {
                        propDic[item.id] = new Dictionary<string, List<SerializedProperty>>();
                        propDic[item.id][item.key] = new List<SerializedProperty>() { prop };
                    }
                }
            }
        }
        private List<RuleItem> GetRuleItems()
        {
            var fields = target.GetType().GetFields();
            var list = new List<RuleItem>();
            foreach (var item in fields)
            {
                var atts = item.GetCustomAttributes(false);

                if (atts != null)
                {
                    var ruleAtt = Array.Find(atts, x => x is RuleTypeAttribute);
                    if (ruleAtt != null)
                    {
                        var att = (ruleAtt as RuleTypeAttribute);
                        list.Add(new RuleItem(att.id, att.key, item.Name));
                    }
                }
            }
            return list;
        }
    }

}