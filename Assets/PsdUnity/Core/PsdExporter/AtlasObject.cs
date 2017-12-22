using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using PSDUnity.Data;

namespace PSDUnity.Exprot
{
    public class AtlasObject : ScriptableObject
    {
        public string atlasName { get { return name + settingObj.fileExt; } }
        public string _exportPath;
        [HideInInspector]
        public string psdFile;

        [HideInInspector]
        public SettingObject settingObj;
        [HideInInspector]
        public RuleObject prefabObj;
        public List<GroupNode1> groups = new List<GroupNode1>();
    }
}
