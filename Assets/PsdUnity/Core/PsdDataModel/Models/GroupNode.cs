using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSDUnity.Data
{
    public abstract class GroupNode : INameAnalyzing<GroupNode>
    {
        public string Name;
        public GroupType groupType;
        public Direction direction;
        public int constraintCount;
        public float spacing;
        public Rect rect;   //利用名字解析controlType和arguments
        public abstract List<ImgNode> images { get; set; }
        public abstract List<GroupNode> groups { get; set; }
        public abstract GroupNode InsertChild(Rect rect);
        public void GetImgNodes(List<ImgNode> imgNodes)
        {
            if (images != null)
            {
                imgNodes.AddRange(images);
            }
            if (groups != null)
            {
                foreach (var item in groups)
                {
                    item.GetImgNodes(imgNodes);
                }
            }
        }
        public GroupNode Analyzing(RuleObject Rule,string name)
        {
            string[] areguments = null;
            this.Name = Rule.AnalysisGroupName(name, out groupType, out areguments);
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

    public abstract class GroupNode<T> :GroupNode where T:GroupNode,new()
    {
        public List<T> _groups = new List<T>();
        public List<ImgNode> _images = new List<ImgNode>();
        public override List<ImgNode> images
        {
            get
            {
                return _images;
            }

            set
            {
                _images = value;
            }
        }
        public override List<GroupNode> groups
        {
            get
            {
                if (_groups == null) _groups = new List<T>();
                return _groups.ConvertAll<GroupNode>(x => x);
            }
            set
            {
                _groups = value.ConvertAll<T>(x => (T)x);
            }
        }
        public override GroupNode InsertChild(Rect rect)
        {
            T node = new T();
            node.rect = rect;
            _groups.Add(node);
            return node;
        }
    }

    [System.Serializable]
    public class GroupNode1 : GroupNode<GroupNode2> { }
    [System.Serializable]
    public class GroupNode2 : GroupNode<GroupNode3> { }
    [System.Serializable]
    public class GroupNode3 : GroupNode<GroupNode4> { }
    [System.Serializable]
    public class GroupNode4 : GroupNode<GroupNode5> { }
    [System.Serializable]
    public class GroupNode5 : GroupNode<GroupNode6> { }
    [System.Serializable]
    public class GroupNode6 : GroupNode<GroupNode7> { }
    [System.Serializable]
    public class GroupNode7 : GroupNode
    {
        public override List<ImgNode> images { get { return null; } set { } }
        public override List<GroupNode> groups { get; set; }
        public override GroupNode InsertChild(Rect rect)
        {
            Debug.Log("cant Insert");
            return null;
        }
    }
}

