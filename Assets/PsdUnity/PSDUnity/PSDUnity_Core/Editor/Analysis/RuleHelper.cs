using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PSDUnity.Data;
using System;

namespace PSDUnity
{
    public static class RuleHelper
    {
        private const string Pref_defultRuleGuid = "defultRuleObjectGuid";
        public static RuleObject GetRuleObj()
        {
            var current = TryLoadGlobal();

            if (current == null)
            {
                current = ScriptableObject.CreateInstance<RuleObject>();
            }
            return current;
        }
        private static RuleObject TryLoadGlobal()
        {
            if (PlayerPrefs.HasKey(Pref_defultRuleGuid))
            {
                var guid = PlayerPrefs.GetString(Pref_defultRuleGuid);
                if (!string.IsNullOrEmpty(guid))
                {
                    return TryLoadFromGUID(guid);
                }
            }
            return null;
        }

        public static bool IsGlobleRule(RuleObject rule)
        {
            if (!PlayerPrefs.HasKey(Pref_defultRuleGuid)) return false;
            if (rule == null) return false;
            var path = AssetDatabase.GetAssetPath(rule);
            if (string.IsNullOrEmpty(path)) return false;
            var guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid)) return false;
            return PlayerPrefs.GetString(Pref_defultRuleGuid) == guid;
        }

        private static RuleObject TryLoadFromGUID(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<RuleObject>(path);
            }
            return null;
        }
        public static RuleObject CreateRuleObject()
        {
            var ruleObj = ScriptableObject.CreateInstance<RuleObject>();
            ProjectWindowUtil.CreateAsset(ruleObj, "new rule.asset");
            return ruleObj;
        }
        public static void SetDefultRuleObject(RuleObject rule)
        {
            if(rule == null)
            {
                PlayerPrefs.SetString(Pref_defultRuleGuid, "");
            }
            else
            {
                var path = AssetDatabase.GetAssetPath(rule);
                var guid = AssetDatabase.AssetPathToGUID(path);
                PlayerPrefs.SetString(Pref_defultRuleGuid, guid);
            }
           
        }
    }
}