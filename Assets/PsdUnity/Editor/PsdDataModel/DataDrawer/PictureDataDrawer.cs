using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
namespace PSDUnity
{
    [CustomPropertyDrawer(typeof(PictureData))]
    public class PictureDataDrawer : PropertyDrawer
    {
        public SerializedProperty picturenameProp;
        public SerializedProperty typeProp;
        public SerializedProperty spriteProp;
        public SerializedProperty textureProp;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            picturenameProp = property.FindPropertyRelative("picturename");
            typeProp = property.FindPropertyRelative("type");
            spriteProp = property.FindPropertyRelative("sprite");
            textureProp = property.FindPropertyRelative("texture");
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            rect.width = 0.3f * position.width;
            EditorGUI.TextField(rect, picturenameProp.stringValue);
            rect.x += 0.4f * position.width;
            typeProp.enumValueIndex = (int)(ImgType)EditorGUI.EnumPopup(rect, (ImgType)typeProp.enumValueIndex);
            rect.x += 0.4f * position.width;
            switch ((ImgType)typeProp.enumValueIndex)
            {
                case ImgType.Label:
                    break;
                case ImgType.Image:
                case ImgType.AtlasImage:
                    spriteProp.objectReferenceValue = EditorGUI.ObjectField(rect, spriteProp.objectReferenceValue, typeof(Sprite), false);
                    break;
                case ImgType.Texture:
                    textureProp.objectReferenceValue = EditorGUI.ObjectField(rect, textureProp.objectReferenceValue, typeof(Texture), false);
                    break;
                default:
                    break;
            }
        }

    }
}
