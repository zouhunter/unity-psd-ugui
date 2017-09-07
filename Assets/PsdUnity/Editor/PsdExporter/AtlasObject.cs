using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using PSDUnity.Data;
using UnityEditor;

namespace PSDUnity.Exprot
{
    public class AtlasObject : ScriptableObject
    {
        public string atlasName { get { return name + settingObj.fileExt; } }
        public string _exportPath;
        [HideInInspector]
        public string psdFile;
        public string exportPath {
            get {
                _exportPath = AssetDatabase.GetAssetPath(this).Replace("/" + name + ".asset", "");
                return _exportPath;
            }
        }
        [HideInInspector]
        public SettingObject settingObj;
        [HideInInspector]
        public RouleObject prefabObj;
        public List<GroupNode1> groups = new List<GroupNode1>();
        public void Reset()
        {
            if (prefabObj == null)
            {
                var path = AssetDatabase.GUIDToAssetPath("f7d3181f5b8957245adfabda058c8541");
                prefabObj = AssetDatabase.LoadAssetAtPath<RouleObject>(path);

                path = AssetDatabase.GUIDToAssetPath("79102a4c6ecda994b9437a6c701177a2");
                settingObj = AssetDatabase.LoadAssetAtPath<SettingObject>(path);
            }
        }
    }
}
