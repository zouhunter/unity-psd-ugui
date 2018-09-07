using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace PSDUnity.Data
{
    [System.Serializable]
    public class GroupNode
    {
        public int id;
        public int depth;
        public string displayName;
        public string suffix = PSDUnityConst.emptySuffix;
        public Direction direction = Direction.LeftToRight;
        public DirectionAxis directionAxis = DirectionAxis.Vertical;
        public int constraintCount;
        public float spacing;
        public Rect rect;   //利用名字解析controlType和arguments
        public List<Data.ImgNode> images = new List<Data.ImgNode>();
        public List<object> children { get; set; }
        public Data.GroupNode parent { get; set; }
    }
}

