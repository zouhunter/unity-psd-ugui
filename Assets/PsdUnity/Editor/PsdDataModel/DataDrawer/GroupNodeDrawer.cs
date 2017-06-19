using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity.Data
{
    [CustomPropertyDrawer(typeof(GroupNode),true)]
    public class GroupNodeDrawer : PropertyDrawer
    {
        SerializedProperty nameProp;
        SerializedProperty groupTypeProp;
        SerializedProperty rectProp;
        SerializedProperty groupsProp;
        SerializedProperty imagesProp;
        SerializedProperty directionProp;
        SerializedProperty constraintCountProp;
        SerializedProperty spacingProp;
        float SingleHeight { get { return EditorGUIUtility.singleLineHeight; } }
        public void RefeshProp(SerializedProperty property)
        {
            nameProp = property.FindPropertyRelative("Name");//
            groupTypeProp = property.FindPropertyRelative("groupType");//
            rectProp = property.FindPropertyRelative("rect");//
            groupsProp = property.FindPropertyRelative("_groups");
            imagesProp = property.FindPropertyRelative("_images");
            directionProp = property.FindPropertyRelative("direction");
            constraintCountProp = property.FindPropertyRelative("constraintCount");
            spacingProp = property.FindPropertyRelative("spacing");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            RefeshProp(property);
            if (!property.isExpanded)
            {
                return SingleHeight;
            }
            else
            {
                float height = 5;
                switch ((GroupType)groupTypeProp.intValue)
                {
                    case GroupType.IMAGE:
                        break;
                    case GroupType.SCROLLVIEW:
                        height +=1;
                        break;
                    case GroupType.GRID:
                        height += 2;
                        break;
                    case GroupType.BUTTON:
                        break;
                    case GroupType.TEXT:
                        break;
                    case GroupType.TOGGLE:
                        break;
                    case GroupType.SLIDER:
                        height += 1;
                        break;
                    case GroupType.GROUP:
                        height += 2;
                        break;
                    case GroupType.INPUTFIELD:
                        break;
                    case GroupType.SCROLLBAR:
                        height += 1;
                        break;
                    case GroupType.DROPDOWN:
                        break;
                    default:
                        break;
                }
                return height * SingleHeight + EditorGUI.GetPropertyHeight(imagesProp,GUIContent.none,true) + EditorGUI.GetPropertyHeight(groupsProp, GUIContent.none, true);
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(position.x, position.y, position.width, SingleHeight);
            property.isExpanded = EditorGUI.ToggleLeft(rect, nameProp.stringValue, property.isExpanded);
            if (property.isExpanded)
            {
                rect.y += SingleHeight;
                EditorGUI.PropertyField(rect, nameProp);

                rect.y += SingleHeight;
                EditorGUI.PropertyField(rect, groupTypeProp);

                rect.y += SingleHeight;
                EditorGUI.PropertyField(rect, groupsProp, true);
                rect.y += EditorGUI.GetPropertyHeight(groupsProp, new GUIContent("Group"), true);

                EditorGUI.PropertyField(rect, imagesProp, true);
                rect.y += EditorGUI.GetPropertyHeight(imagesProp, new GUIContent("Images"), true);

                var lastDir = directionProp.enumValueIndex > 0 ? (Direction)directionProp.enumValueIndex : Direction.None;
                switch ((GroupType)groupTypeProp.enumValueIndex)
                {
                    case GroupType.EMPTY:
                        break;
                    case GroupType.BUTTON:
                        break;
                    case GroupType.TOGGLE:
                        break;
                    case GroupType.CANVAS:
                        break;
                    case GroupType.GRID:
                        directionProp.enumValueIndex = (int)(Direction)EditorGUI.EnumPopup(rect,new GUIContent("方向"), lastDir);
                        rect.y += SingleHeight;
                        EditorGUI.PropertyField(rect,constraintCountProp);
                        rect.y += SingleHeight;
                        break;
                    case GroupType.IMAGE:
                        break;
                    case GroupType.RawIMAGE:
                        break;
                    case GroupType.SCROLLVIEW:
                        directionProp.enumValueIndex = (int)(Direction)EditorGUI.EnumMaskPopup(rect, new GUIContent("方向"), lastDir);
                        rect.y += SingleHeight;
                        break;
                    case GroupType.SLIDER:
                    case GroupType.SCROLLBAR:
                        directionProp.enumValueIndex = (int)(Direction)EditorGUI.EnumPopup(rect, new GUIContent("方向"), lastDir);
                        rect.y += SingleHeight;
                        break;
                    case GroupType.TEXT:
                        break;
                    case GroupType.GROUP:
                        directionProp.enumValueIndex = (int)(Direction)EditorGUI.EnumPopup(rect, new GUIContent("方向"), lastDir);
                        rect.y += SingleHeight;
                        EditorGUI.PropertyField(rect, spacingProp);
                        rect.y += SingleHeight;
                        break;
                    case GroupType.INPUTFIELD:
                        break;
                    case GroupType.DROPDOWN:
                        break;
                    default:
                        break;
                }
               
                EditorGUI.PropertyField(rect, rectProp);
            }
        }
    }
}