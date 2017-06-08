using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUnity
{
    [Serializable]
    public class GroupNode
    {
        public string      name;
        public ControlType controltype;
        public GroupNode[] groups;
        public ImgNode[]   images;
        public Rect        rect;
        public string[]    arguments;
    }
}

