using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using PSDUnity;

namespace PSDUnity
{
    public class Exporter : ScriptableObject
    {
        public string atlasName { get { return name + settingObj.fileExt; } }
        public string _exportPath;
        [HideInInspector]
        public string psdFile;

        [HideInInspector]
        public SettingObject settingObj;
        [HideInInspector]
        public RuleObject ruleObj;
        public List<GroupNode1> groups = new List<GroupNode1>();
    }
}
