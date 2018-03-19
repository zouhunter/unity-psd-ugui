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
        private bool isGloble;
        private void OnEnable()
        {
            isGloble = RuleHelper.IsGlobleRule(target as RuleObject);
        }
        public override void OnInspectorGUI()
        {
            DrawerHeadTools();
            base.OnInspectorGUI();
        }

        private void DrawerHeadTools()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if(isGloble)
                {
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Defult Rule", EditorStyles.largeLabel);
                    GUI.color = Color.white;

                    if (GUILayout.Button("[Clean]", EditorStyles.miniLabel, GUILayout.Width(60)))
                    {
                        RuleHelper.SetDefultRuleObject(null);
                        isGloble = false;
                    }
                }
                else
                {
                    if (GUILayout.Button("Set As Defult", EditorStyles.largeLabel))
                    {
                        var ok = EditorUtility.DisplayDialog("设为默认", "如果设置为默认规则，新建项将使用该规则，确认请按继续", "继续");
                        if (ok)
                        {
                            RuleHelper.SetDefultRuleObject(target as RuleObject);
                            isGloble = true;
                        }
                    }
                }
              
              
            }
        }
    }

}