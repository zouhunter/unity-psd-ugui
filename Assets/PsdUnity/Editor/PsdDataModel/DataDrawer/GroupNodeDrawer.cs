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
        SerializedProperty groupTypeProp;
        SerializedProperty rectProp;
        SerializedProperty groupsProp;
        SerializedProperty imagesProp;
        SerializedProperty argumentsProp;

        public void RefeshProp(SerializedProperty property)
        {
            nameProp = property.FindPropertyRelative("Name");//
            groupTypeProp = property.FindPropertyRelative("groupType");//
            rectProp = property.FindPropertyRelative("rect");//
            groupsProp = property.FindPropertyRelative("_groups");
            imagesProp = property.FindPropertyRelative("_images");
            argumentsProp = property.FindPropertyRelative("arguments");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            RefeshProp(property);
            //float height = 3;
            //Debug.Log(nameProp.stringValue);
            switch ((GroupType)groupTypeProp.intValue)
            {
                case GroupType.IMAGE:
                    break;
                case GroupType.SCROLLVIEW:
                    break;
                case GroupType.GRID:
                    break;
                case GroupType.BUTTON:
                    break;
                case GroupType.TEXT:
                    break;
                case GroupType.TOGGLE:
                    break;
                case GroupType.SLIDER:
                    break;
                case GroupType.GROUP:
                    break;
                case GroupType.INPUTFIELD:
                    break;
                case GroupType.SCROLLBAR:
                    break;
                case GroupType.DROPDOWN:
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