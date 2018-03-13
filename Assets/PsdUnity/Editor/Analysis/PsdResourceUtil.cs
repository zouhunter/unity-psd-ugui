using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PSDUnity
{
    public static class PsdResourceUtil
    {
        public const string guiskin_guid = "c84e9308de93a0d4cb0501b862af8a7d";
        public const string defultRule_guid = "f7d3181f5b8957245adfabda058c8541";
        public const string defultSetting_guid = "5257652353e9b3d47a466e2d5ba56cf4";
        public static GUISkin GetGuiSkin()
        {
            var path = AssetDatabase.GUIDToAssetPath(guiskin_guid);
            if(string.IsNullOrEmpty(path))
            {
                return null;
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<GUISkin>(path);
            }
        }

        public static RuleObject GetRuleObj()
        {
            var path = AssetDatabase.GUIDToAssetPath(defultRule_guid);
            return AssetDatabase.LoadAssetAtPath<RuleObject>(path);
        }

        public static SettingObject GetSettingObj()
        {
            var path = AssetDatabase.GUIDToAssetPath(defultSetting_guid);
            return AssetDatabase.LoadAssetAtPath<SettingObject>(path);
        }

    }
}