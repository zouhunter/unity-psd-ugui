using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity.Data
{
    [CustomPropertyDrawer(typeof(Data.ImgNode))]
    public class ImgNodeDrawer : PropertyDrawer
    {
        SerializedProperty typeProp;
        SerializedProperty sourceProp;
        SerializedProperty nameProp;
        SerializedProperty rectProp;
        SerializedProperty spriteProp;
        SerializedProperty textureProp;
        SerializedProperty textProp;
        SerializedProperty fontProp;
        SerializedProperty fontSizeProp;
        SerializedProperty colorProp;
        float SingleHeight { get { return EditorGUIUtility.singleLineHeight; } }
        public void RefeshProp(SerializedProperty property)
        {
            nameProp = property.FindPropertyRelative("Name");//
            typeProp = property.FindPropertyRelative("type");//
            rectProp = property.FindPropertyRelative("rect");//
            sourceProp = property.FindPropertyRelative("source");
            spriteProp = property.FindPropertyRelative("sprite");
            textureProp = property.FindPropertyRelative("texture");
            textProp = property.FindPropertyRelative("text");
            fontProp = property.FindPropertyRelative("font");
            fontSizeProp = property.FindPropertyRelative("fontSize");
            colorProp = property.FindPropertyRelative("color");
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
                switch ((ImgType)typeProp.enumValueIndex)
                {
                    case ImgType.Label:
                        height += 4;
                        break;
                    case ImgType.Image:
                    case ImgType.AtlasImage:
                        height += 2;
                        break;
                    case ImgType.Texture:
                        height += 2;
                        break;
                    case ImgType.Color:
                        height += 1;
                        break;
                    default:
                        break;
                }
                return height * SingleHeight;
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.ToggleLeft(rect, nameProp.stringValue,property.isExpanded);
            if(property.isExpanded)
            {
                rect.y += SingleHeight;
                EditorGUI.PropertyField(rect, nameProp);
                rect.y += SingleHeight;
                EditorGUI.PropertyField(rect, typeProp);
                rect.y += SingleHeight;
                switch ((ImgType)typeProp.enumValueIndex)
                {
                    case ImgType.Label:
                        EditorGUI.PropertyField(rect, textProp);
                        rect.y += SingleHeight;
                        EditorGUI.PropertyField(rect, fontProp);
                        rect.y += SingleHeight;
                        EditorGUI.PropertyField(rect, fontSizeProp);
                        rect.y += SingleHeight;
                        EditorGUI.PropertyField(rect, colorProp);
                        rect.y += SingleHeight;
                        break;
                    case ImgType.Image:
                    case ImgType.AtlasImage:
                        EditorGUI.PropertyField(rect, sourceProp);
                        rect.y += SingleHeight;
                        EditorGUI.PropertyField(rect, spriteProp);
                        rect.y += SingleHeight;
                        break;
                    case ImgType.Texture:
                        EditorGUI.PropertyField(rect, sourceProp);
                        rect.y += SingleHeight;
                        EditorGUI.PropertyField(rect, textureProp);
                        rect.y += SingleHeight;
                        break;
                    case ImgType.Color:
                        EditorGUI.PropertyField(rect,colorProp);
                        rect.y += SingleHeight;
                        break;
                    default:
                        break;
                }

                EditorGUI.PropertyField(rect, rectProp);
            }
        }
    }
}