using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace PSDUnity.Data
{
    public class Exporter : ScriptableObject
    {
        public string _exportPath;
        public string psdFile;
        public RuleObject ruleObj;
        public List<Data.GroupNode> groups = new List<Data.GroupNode>();
    }
}
