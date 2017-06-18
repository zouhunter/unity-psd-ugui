using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity
{
    [CustomPropertyDrawer(typeof(ImgNode))]
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

        public void RefeshProp(SerializedProperty property)
        {
            nameProp = property.FindPropertyRelative("Name");//
            typeProp = property.FindPropertyRelative("imageType");//
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
            float height = 3;
            //switch ((ImgType)typeProp.enumValueIndex)
            //{
            //    case ImgType.Label:
            //        height += 3;
            //        break;
            //    case ImgType.Image:
            //        height += 1;
            //        break;
            //    case ImgType.SliceImage:
            //        height += 2;
            //        break;
            //    case ImgType.Texture:
            //        height += 2;
            //        break;
            //    default:
            //        break;
            //}
            //return height * EditorGUIUtility.singleLineHeight;
            return EditorGUI.GetPropertyHeight(property, label, true);

        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            //EditorGUI.PropertyField(rect, nameProp);
            EditorGUI.PropertyField(position, property,true);
        }
    }
}