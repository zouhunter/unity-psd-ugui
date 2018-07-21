using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PSDUnity.Data;
using System;
using System.Linq;
using UnityEngine.Events;

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
                LoadImageImports(current,()=> {
                   LoadLayerImports(current);
                });
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

        public static void LoadImageImports(RuleObject target,UnityAction onComplete)
        {
            var types = new List<Type>() {
                typeof(UGUI.TextImport),
                typeof(UGUI.ImageRawImageImport),
            };

            var assetPath = AssetDatabase.GetAssetPath(target);
            if(string.IsNullOrEmpty(assetPath))
            {
                Debug.Log("Dely LoadImageImports:" + target);
                DelyAcceptObject(target,(x)=> LoadImageImports(target,onComplete));
                return;
            }
            Debug.Log("LoadImageImports");
            foreach (var layerType in types)
            { 
                var ruleObj = target;

                var importer = ruleObj.imageImports.Find(x => x != null && x.GetType() == layerType);

                if (importer != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(importer)))
                {
                    ruleObj.imageImports.Remove(importer);
                }

                if (importer == null)
                {
                    if (string.IsNullOrEmpty(assetPath))
                    {
                        importer = ScriptableObject.CreateInstance(layerType) as UGUI.ImageImport;
                        Debug.Log("create instence:" + layerType.Name);
                        importer.name = layerType.Name;
                        ruleObj.imageImports.Add(importer);
                    }
                    else
                    {
                        var oldItems = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                        var oldItem = oldItems.Where(x => x is UGUI.ImageImport && x.name == layerType.Name).FirstOrDefault();
                        if (oldItem == null)
                        {
                            importer = ScriptableObject.CreateInstance(layerType) as UGUI.ImageImport;
                            importer.name = layerType.Name;
                            Debug.Log("add new:" + layerType.Name);
                            AssetDatabase.AddObjectToAsset(importer, assetPath);
                            ruleObj.imageImports.Add(importer);
                        }
                        else
                        {
                            ruleObj.imageImports.Add(oldItem as UGUI.ImageImport);
                            //Debug.Log(oldItem);
                        }
                    }
                }
                else
                {
                    //Debug.Log(importer);
                }
            }

            if(onComplete != null){
                onComplete.Invoke();
            }

            if (string.IsNullOrEmpty(assetPath)) return;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        public static void LoadLayerImports(RuleObject target)
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.Log("Dely LoadLayerImports:" + target);
                DelyAcceptObject(target, (x) => LoadLayerImports(x as RuleObject));
                return;
            }
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

        public static void DelyAcceptObject(UnityEngine.Object instence, UnityAction<UnityEngine.Object> onCreate)
        {
            if (onCreate == null) return;

            EditorApplication.CallbackFunction action = () =>
            {
                var path = AssetDatabase.GetAssetPath(instence);
                if (!string.IsNullOrEmpty(path))
                {
                    var item = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                    if (item)
                    {
                        onCreate.Invoke(item);
                    }

                    EditorApplication.update = null;

                }
            };
            EditorApplication.update = action;
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