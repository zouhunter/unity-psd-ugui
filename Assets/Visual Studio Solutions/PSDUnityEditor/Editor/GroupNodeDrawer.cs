//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections.Generic;
//using UnityEditor;

//namespace PSDUnity.Data
//{
//    [CustomPropertyDrawer(typeof(Data.GroupNode),true)]
//    public class GroupNodeDrawer : PropertyDrawer
//    {
//        SerializedProperty nameProp;
//        SerializedProperty groupTypeProp;
//        SerializedProperty rectProp;
//        SerializedProperty groupsProp;
//        SerializedProperty imagesProp;
//        SerializedProperty directionProp;
//        SerializedProperty constraintCountProp;
//        SerializedProperty spacingProp;
//        float SingleHeight { get { return EditorGUIUtility.singleLineHeight; } }
//        public void RefeshProp(SerializedProperty property)
//        {
//            nameProp = property.FindPropertyRelative("Name");//
//            groupTypeProp = property.FindPropertyRelative("suffix");//
//            rectProp = property.FindPropertyRelative("rect");//
//            groupsProp = property.FindPropertyRelative("_groups");
//            imagesProp = property.FindPropertyRelative("_images");
//            directionProp = property.FindPropertyRelative("direction");
//            constraintCountProp = property.FindPropertyRelative("constraintCount");
//            spacingProp = property.FindPropertyRelative("spacing");
//        }

//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            RefeshProp(property);
//            if (!property.isExpanded)
//            {
//                return SingleHeight;
//            }
//            else
//            {
//                float height = 5;
//                suffix
//                //switch ((GroupType)groupTypeProp.intValue)
//                //{
//                //    case GroupType.SCROLLVIEW:
//                //        height +=1;
//                //        break;
//                //    case GroupType.GRID:
//                //        height += 2;
//                //        break;
//                //    case GroupType.BUTTON:
//                //        break;
//                //    case GroupType.PANEL:
//                //        break;
//                //    case GroupType.TOGGLE:
//                //        break;
//                //    case GroupType.SLIDER:
//                //        height += 1;
//                //        break;
//                //    case GroupType.GROUP:
//                //        height += 2;
//                //        break;
//                //    case GroupType.INPUTFIELD:
//                //        break;
//                //    case GroupType.SCROLLBAR:
//                //        height += 1;
//                //        break;
//                //    case GroupType.DROPDOWN:
//                //        break;
//                //    default:
//                //        break;
//                //}
//                return height * SingleHeight + EditorGUI.GetPropertyHeight(imagesProp,GUIContent.none,true) + EditorGUI.GetPropertyHeight(groupsProp, GUIContent.none, true);
//            }
//        }
//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            var rect = new Rect(position.x, position.y, position.width, SingleHeight);
//            property.isExpanded = EditorGUI.ToggleLeft(rect, nameProp.stringValue, property.isExpanded);
//            if (property.isExpanded)
//            {
//                rect.y += SingleHeight;
//                EditorGUI.PropertyField(rect, nameProp);

//                rect.y += SingleHeight;
//                EditorGUI.PropertyField(rect, groupTypeProp);

//                rect.y += SingleHeight;
//                EditorGUI.PropertyField(rect, groupsProp, true);
//                rect.y += EditorGUI.GetPropertyHeight(groupsProp, new GUIContent("Group"), true);

//                EditorGUI.PropertyField(rect, imagesProp, true);
//                rect.y += EditorGUI.GetPropertyHeight(imagesProp, new GUIContent("Images"), true);

//                var lastDir = enumIndexToDirection(directionProp.enumValueIndex);
////                switch ((GroupType)groupTypeProp.enumValueIndex)
////                {
////                    case GroupType.EMPTY:
////                        break;
////                    case GroupType.BUTTON:
////                        break;
////                    case GroupType.TOGGLE:
////                        break;
////                    case GroupType.CANVAS:
////                        break;
////                    case GroupType.GRID:
////                        directionProp.enumValueIndex = enumDirectionToIndex((Direction)EditorGUI.EnumPopup(rect,new GUIContent("方向"), lastDir));
////                        rect.y += SingleHeight;
////                        EditorGUI.PropertyField(rect,constraintCountProp);
////                        rect.y += SingleHeight;
////                        break;
////                    case GroupType.PANEL:
////                        break;
////                    case GroupType.SCROLLVIEW:
////#if UNITY_5_6
////                        directionProp.enumValueIndex = enumDirectionToIndex((Direction)EditorGUI.EnumMaskField(rect, new GUIContent("方向"), lastDir));
////#elif UNITY_2017
////                        directionProp.enumValueIndex = enumDirectionToIndex((Direction)EditorGUI.EnumFlagsField(rect, new GUIContent("方向"), lastDir));
////#else
////                        directionProp.enumValueIndex = enumDirectionToIndex((Direction)EditorGUI.EnumMaskField(rect, new GUIContent("方向"), lastDir));
////#endif
////                        rect.y += SingleHeight;
////                        break;
////                    case GroupType.SLIDER:
////                    case GroupType.SCROLLBAR:
////                        directionProp.enumValueIndex = enumDirectionToIndex((Direction)EditorGUI.EnumPopup(rect, new GUIContent("方向"), lastDir));
////                        rect.y += SingleHeight;
////                        break;
////                    case GroupType.GROUP:
////                        directionProp.enumValueIndex = enumDirectionToIndex((Direction)EditorGUI.EnumPopup(rect, new GUIContent("方向"), lastDir));
////                        rect.y += SingleHeight;
////                        EditorGUI.PropertyField(rect, spacingProp);
////                        rect.y += SingleHeight;
////                        break;
////                    case GroupType.INPUTFIELD:
////                        break;
////                    case GroupType.DROPDOWN:
////                        break;
////                    default:
////                        break;
////                }
               
//                EditorGUI.PropertyField(rect, rectProp);
//            }
//        }

//        private static Direction enumIndexToDirection(int index)
//        {
//            switch ((DirectionID)index)
//            {
//                case DirectionID.None:
//                    return Direction.None;
//                case DirectionID.Horizontal:
//                    return Direction.Horizontal;
//                case DirectionID.Vertical:
//                    return Direction.Vertical;
//                case DirectionID.LeftToRight:
//                    return Direction.LeftToRight;
//                case DirectionID.BottomToTop:
//                    return Direction.BottomToTop;
//                case DirectionID.TopToBottom:
//                    return Direction.TopToBottom;
//                case DirectionID.RightToLeft:
//                    return Direction.RightToLeft;
//                default:
//                    return Direction.None;
//            }
//        }
//        private static int enumDirectionToIndex(Direction direction)
//        {
//            DirectionID id = DirectionID.None;
//            switch (direction)
//            {
//                case Direction.None:
//                    id = DirectionID.None;
//                    break;
//                case Direction.Horizontal:
//                    id = DirectionID.Horizontal;
//                    break;
//                case Direction.Vertical:
//                    id = DirectionID.Vertical;
//                    break;
//                case Direction.LeftToRight:
//                    id = DirectionID.LeftToRight;
//                    break;
//                case Direction.BottomToTop:
//                    id = DirectionID.BottomToTop;
//                    break;
//                case Direction.TopToBottom:
//                    id = DirectionID.TopToBottom;
//                    break;
//                case Direction.RightToLeft:
//                    id = DirectionID.RightToLeft;
//                    break;
//                default:
//                    return 0;
//            }
//            return (int)id;
//        }
        
//    }
//}