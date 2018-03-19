using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.IMGUI.Controls;

namespace PSDUnity
{
    [System.Serializable]
    public class GroupNode :TreeViewItem
    {
        public int _id;
        public int _depth;
        public string _displayName;
        public GroupType groupType = GroupType.EMPTY;
        public Direction direction = Direction.Vertical;
        public int constraintCount;
        public float spacing;
        public Rect rect;   //利用名字解析controlType和arguments
        public List<ImgNode> images = new List<ImgNode>();

        public GroupNode(Rect rect, int id, int depth):base(id,depth)
        {
            this.rect = rect;
            children = new List<TreeViewItem>();
            this._id = id;
            this._depth = depth;
        }
        public override int id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        public override int depth
        {
            get
            {
                return _depth;
            }

            set
            {
               _depth = value;
            }
        }
        public override string displayName
        {
            get
            {
                return _displayName;
            }

            set
            {
                _displayName = value;
            }
        }
        public void GetImgNodes(List<ImgNode> imgNodes)
        {
            if (images != null)
            {
                imgNodes.AddRange(images);
            }
            if (children != null)
            {
                foreach (GroupNode  item in children)
                {
                    item.GetImgNodes(imgNodes);
                }
            }
        }
        public GroupNode Analyzing(RuleObject Rule, string name)
        {
            string[] areguments = null;
            this.displayName = Rule.AnalysisGroupName(name, out groupType, out areguments);
            switch (groupType)
            {
                case GroupType.GRID:
                    if (areguments != null && areguments.Length > 1)
                    {
                        var key = areguments[0];
                        direction = RuleObject.GetDirectionByKey(key);
                    }
                    if (areguments != null && areguments.Length > 2)
                    {
                        var key = areguments[1];
                        constraintCount = int.Parse(key);
                    }
                    break;
                case GroupType.SCROLLVIEW:
                case GroupType.SLIDER:
                case GroupType.SCROLLBAR:
                    if (areguments != null && areguments.Length > 0)
                    {
                        var key = areguments[0];
                        direction = RuleObject.GetDirectionByKey(key);
                    }
                    break;
                case GroupType.GROUP:
                    if (areguments != null && areguments.Length > 1)
                    {
                        var key = areguments[0];
                        direction = RuleObject.GetDirectionByKey(key);
                    }
                    if (areguments != null && areguments.Length > 2)
                    {
                        var key = areguments[1];
                        spacing = float.Parse(key);
                    }
                    break;
                default:
                    break;
            }
            return this;
        }
    }
}

