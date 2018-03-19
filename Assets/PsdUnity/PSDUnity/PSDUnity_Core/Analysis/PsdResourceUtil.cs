using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PSDUnity
{
    public static class PsdResourceUtil
    {
        private static string Pref_defultRuleGuid = "defultRuleObjectGuid";
        public static RuleObject DefultRuleObj()
        {
            var current = TryLoadLast();
            if(current == null)
            {
                current = ScriptableObject.CreateInstance<RuleObject>();
            }
            return current;
        }
        private static RuleObject TryLoadLast()
        {
            if (PlayerPrefs.HasKey(Pref_defultRuleGuid))
            {
                var guid = PlayerPrefs.GetString(Pref_defultRuleGuid);
                if (!string.IsNullOrEmpty(guid))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!string.IsNullOrEmpty(path))
                    {
                        return AssetDatabase.LoadAssetAtPath<RuleObject>(path);
                    }
                }
            }
            return null;
        }

        public static RuleObject CreateRuleObject()
        {
            var ruleObj = ScriptableObject.CreateInstance<RuleObject>();
            ProjectWindowUtil.CreateAsset(ruleObj, "new rule.asset");
            var path = AssetDatabase.GetAssetPath(ruleObj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            PlayerPrefs.SetString(Pref_defultRuleGuid, guid);
            return ruleObj;
        }

    }
}