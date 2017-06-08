using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity
{
    [CustomPropertyDrawer(typeof(GroupNode))]
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
            groupsProp = property.FindPropertyRelative("groups");
            imagesProp = property.FindPropertyRelative("images");
            argumentsProp = property.FindPropertyRelative("arguments");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            RefeshProp(property);
            float height = 3;
            Debug.Log(nameProp.stringValue);
            //switch ((ControlType)controltypeProp.enumValueIndex)
            //{
            //    case ControlType.Panel:
            //        break;
            //    case ControlType.ScrollView:
            //        break;
            //    case ControlType.Grid:
            //        break;
            //    case ControlType.Button:
            //        break;
            //    case ControlType.Label:
            //        break;
            //    case ControlType.Toggle:
            //        break;
            //    case ControlType.Slider:
            //        break;
            //    case ControlType.Group:
            //        break;
            //    case ControlType.InputField:
            //        break;
            //    case ControlType.ScrollBar:
            //        break;
            //    case ControlType.Dropdown:
            //        break;
            //    default:
            //        break;
            //}
            return EditorGUI.GetPropertyHeight(property,label,true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(position, property,true);
        }
    }
}