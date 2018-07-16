using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity
{
    [CustomPropertyDrawer(typeof(CustomFieldAttribute))]
    public class CustomFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            var att = attribute as CustomFieldAttribute;
            label.text = att.title;
            EditorGUI.PropertyField(position, property, label);
        }
    }
}