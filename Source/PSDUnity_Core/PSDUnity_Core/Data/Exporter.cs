using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using PSDUnity;
using UnityEditor.IMGUI.Controls;

namespace PSDUnity
{
    public class Exporter : ScriptableObject
    {
        public string _exportPath;
        public string psdFile;
        public SettingObject settingObj;
        public RuleObject ruleObj;
        public List<GroupNode> groups = new List<GroupNode>();

        public string CalcAtlasName()
        {
            return name + settingObj.fileExt;
        }
    }
}
