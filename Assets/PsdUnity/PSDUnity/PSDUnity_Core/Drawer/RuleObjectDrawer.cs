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
using UnityEditor;
using System;
using UnityEditorInternal;

namespace PSDUnity.Data
{
    public class RuleItem
    {
        public int id;
        public string fieldName;

        public RuleItem(int id, string fieldName)
        {
            this.id = id;
            this.fieldName = fieldName;
        }
    }
    [CustomEditor(typeof(RuleObject))]
    public class RuleObjectDrawer : Editor
    {
        private bool isGloble;
        private int selected;
        private string[] options = { "前缀", "分割", "参数", "后缀", "界面", "导入" };
        private Dictionary<int, List<SerializedProperty>> propDic;
        private ReorderableList reorderList;
        private List<SerializedProperty> currentPropertys = new List<SerializedProperty>();
        private SerializedObject tempObj;
        private void OnEnable()
        {
            isGloble = RuleHelper.IsGlobleRule(target as RuleObject);
            InitPropertys();
            ChargeCurrent();
            InitReorderList();
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawerHeadTools();
            DrawToolbar();
            DrawPropertys();
            serializedObject.ApplyModifiedProperties();
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
            currentPropertys.Clear();
            if (propDic.ContainsKey(selected))
            {
                currentPropertys.AddRange(propDic[selected]);
            }
        }
        private void InitReorderList()
        {
            reorderList = new ReorderableList(currentPropertys, typeof(SerializedProperty), true, true, false, false);
            reorderList.drawHeaderCallback = DrawHead;
            reorderList.elementHeightCallback = GetElementHight;
            reorderList.drawElementCallback = DrawPropertyItem;
        }
        private void DrawHead(Rect rect)
        {
            var resetRect = new Rect(rect.width - 60, rect.y, 60, rect.height);
            if (GUI.Button(resetRect, "重置", EditorStyles.miniButtonRight))
            {
                if (tempObj == null)
                {
                    tempObj = new SerializedObject(ScriptableObject.CreateInstance<RuleObject>());
                    foreach (var item in currentPropertys)
                    {
                        var path = item.propertyPath;
                        var tempProp = tempObj.FindProperty(path);
                        CopyPropertyValue(item, tempProp);
                    }
                }
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
        private float GetElementHight(int index)
        {
            var property = currentPropertys[index];
            return EditorGUI.GetPropertyHeight(property);
        }

        private void DrawPropertyItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.PropertyField(rect, currentPropertys[index]);
        }

        private void DrawPropertys()
        {
            if (reorderList != null)
            {
                reorderList.DoLayoutList();
            }
        }

        private void InitPropertys()
        {
            propDic = new Dictionary<int, List<SerializedProperty>>();
            var list = GetRuleItems();
            foreach (var item in list)
            {
                var prop = serializedObject.FindProperty(item.fieldName);
                if (prop != null)
                {
                    if (propDic.ContainsKey(item.id))
                    {
                        propDic[item.id].Add(prop);
                    }
                    else
                    {
                        propDic.Add(item.id, new List<SerializedProperty>() { prop });
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
                        list.Add(new RuleItem((ruleAtt as RuleTypeAttribute).id, item.Name));
                    }
                }
            }
            return list;
        }
    }

}