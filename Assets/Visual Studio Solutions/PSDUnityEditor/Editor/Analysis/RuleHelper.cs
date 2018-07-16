using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PSDUnity.Data;
using System;
using System.Linq;

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
                LoadLayerImports(current);
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

        public static void LoadLayerImports(RuleObject target)
        {
            var assetPath = AssetDatabase.GetAssetPath(target);

            Debug.Log("LoadLayerImports");
            var types = AnalysisUtility.layerImportTypes;
            foreach (var layerType in types)
            {
                var ruleObj = target;

                var importer = ruleObj.layerImports.Find(x => x != null && x.GetType() == layerType);

                if(importer != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(importer))){
                    ruleObj.layerImports.Remove(importer);
                }

                if (importer == null)
                {
                    if (string.IsNullOrEmpty(assetPath))
                    {
                        importer = ScriptableObject.CreateInstance(layerType) as UGUI.LayerImport;
                        Debug.Log("create instence:" + layerType.Name);
                        importer.name = layerType.Name;
                        ruleObj.layerImports.Add(importer);
                    }
                    else
                    {
                        var oldItems = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                        var oldItem = oldItems.Where(x => x is UGUI.LayerImport && x.name == layerType.Name).FirstOrDefault();
                        if (oldItem == null)
                        {
                            importer = ScriptableObject.CreateInstance(layerType) as UGUI.LayerImport;
                            importer.name = layerType.Name;
                            Debug.Log("add new:" + layerType.Name);
                            AssetDatabase.AddObjectToAsset(importer, assetPath);
                            ruleObj.layerImports.Add(importer);
                        }
                        else
                        {
                            ruleObj.layerImports.Add(oldItem as UGUI.LayerImport);
                            //Debug.Log(oldItem);
                        }
                    }
                }
                else
                {
                    //Debug.Log(importer);
                }
            }
            //EditorUtility.SetDirty(ruleObj);
            //AssetDatabase.Refresh();

            if (string.IsNullOrEmpty(assetPath)) return;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
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
            if (rule == null)
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