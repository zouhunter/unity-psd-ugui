using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity
{
    [CustomPropertyDrawer(typeof(GroupNode),true)]
    public class GroupNodeDrawer : PropertyDrawer
    {
        SerializedProperty nameProp;
        SerializedProperty controltypeProp;
        SerializedProperty rectProp;
        SerializedProperty groupsProp;
        SerializedProperty imagesProp;
        SerializedProperty argumentsProp;

        public void RefeshProp(SerializedProperty property)
        {
            nameProp = property.FindPropertyRelative("name");//
            controltypeProp = property.FindPropertyRelative("controltype");//
            rectProp = property.FindPropertyRelative("rect");//
            groupsProp = property.FindPropertyRelative("_groups");
            imagesProp = property.FindPropertyRelative("_images");
            argumentsProp = property.FindPropertyRelative("arguments");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            RefeshProp(property);
            float height = 3;
            //Debug.Log(nameProp.stringValue);
            switch ((ControlType)controltypeProp.intValue)
            {
                case ControlType.Panel:
                    break;
                case ControlType.ScrollView:
                    break;
                case ControlType.Grid:
                    break;
                case ControlType.Button:
                    break;
                case ControlType.Label:
                    break;
                case ControlType.Toggle:
                    break;
                case ControlType.Slider:
                    break;
                case ControlType.Group:
                    break;
                case ControlType.InputField:
                    break;
                case ControlType.ScrollBar:
                    break;
                case ControlType.Dropdown:
                    break;
                default:
                    break;
            }
            return EditorGUI.GetPropertyHeight(property,label,true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            //property.isExpanded = EditorGUI.ToggleLeft(rect, nameProp.stringValue, property.isExpanded);
            //if (!property.isExpanded)
            //{
            //    return;
            //}
            //rect.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.PropertyField(rect, nameProp, true);
            //rect.y += EditorGUIUtility.singleLineHeight;
            //controltypeProp.intValue =(int) ((ControlType)EditorGUI.EnumPopup(rect, (ControlType)controltypeProp.intValue));
            //rect.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.PropertyField(rect, groupsProp, true);
            //rect.y += EditorGUI.GetPropertyHeight(groupsProp, new GUIContent("Group"), true);
            //EditorGUI.PropertyField(rect, imagesProp, true);
            //rect.y += EditorGUI.GetPropertyHeight(imagesProp, new GUIContent("Images"), true);
            //EditorGUI.PropertyField(rect, argumentsProp, true);
            //rect.y += EditorGUI.GetPropertyHeight(argumentsProp, new GUIContent("Aregument"), true);
            //EditorGUI.PropertyField(rect, rectProp);
            EditorGUI.PropertyField(position, property,new GUIContent( nameProp.stringValue), true);
        }
    }
}