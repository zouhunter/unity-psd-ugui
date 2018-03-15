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

namespace PSDUnity.Data
{
    [CustomEditor(typeof(RuleObject))]
    public class RuleObjectDrawer : Editor
    {
        SerializedProperty scriptProp;
        public SerializedProperty sepraterCharProp;
        public SerializedProperty argumentCharProp;
        void OnEnable()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            sepraterCharProp = serializedObject.FindProperty("sepraterChar");
            argumentCharProp= serializedObject.FindProperty("argumentChar");
        }
     
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(scriptProp);
            serializedObject.Update();
        }
    }

}