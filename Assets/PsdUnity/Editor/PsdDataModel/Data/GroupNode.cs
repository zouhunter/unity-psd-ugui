using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSDUnity
{
    public abstract class GroupNode
    {
        public string name;
        public ControlType controltype;
        public List<ImgNode> images = new List<ImgNode>();
        public List<string> arguments = new List<string>();
        public Rect rect;

        public abstract List<GroupNode> groups { get; set; }

    }
    [System.Serializable]
    public class GroupNode1 : GroupNode
    {
        public List<GroupNode2> _groups;
        public override List<GroupNode> groups
        {
            get
            {
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode2>(x => (GroupNode2)x);
            }
        }
    }
    [System.Serializable]
    public class GroupNode2 : GroupNode
    {
        public List<GroupNode3> _groups;
        public override List<GroupNode> groups
        {
            get
            {
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode3>(x => (GroupNode3)x);
            }
        }
    }
    [System.Serializable]
    public class GroupNode3 : GroupNode
    {
        public List<GroupNode4> _groups;
        public override List<GroupNode> groups
        {
            get
            {
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode4>(x => (GroupNode4)x);
            }
        }
    }
    [System.Serializable]
    public class GroupNode4 : GroupNode
    {
        public List<GroupNode5> _groups;
        public override List<GroupNode> groups
        {
            get
            {
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode5>(x => (GroupNode5)x);
            }
        }
    }
    [System.Serializable]
    public class GroupNode5 : GroupNode
    {
        public List<GroupNode6> _groups;
        public override List<GroupNode> groups
        {
            get
            {
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode6>(x => (GroupNode6)x);
            }
        }
    }
    [System.Serializable]
    public class GroupNode6 : GroupNode
    {
        public List<GroupNode7> _groups;
        public override List<GroupNode> groups
        {
            get
            {
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode7>(x => (GroupNode7)x);
            }
        }
    }
    [System.Serializable]
    public class GroupNode7 : GroupNode
    {
        public override List<GroupNode> groups { get; set; }
    }
}

