using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSDUnity
{
    [System.Serializable]
    public class GroupNode
    {
        public string name;
        public ControlType controltype;
        public List<GroupNode> groups = new List<GroupNode>();
        public List<ImgNode> images = new List<ImgNode>();
        public List<string> arguments = new List<string>();
        public Rect rect;
    }
}

