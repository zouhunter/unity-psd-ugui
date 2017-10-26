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
        public Rect rect;
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

        public GroupNode(Rect rect)
        {
            //利用名字解析controlType和arguments
            this.rect = rect;
        }

       
    }
    [System.Serializable]
    public class GroupNode1 : GroupNode
    {
        public List<GroupNode2> _groups = new List<GroupNode2>();
        public List<ImgNode> _images = new List<ImgNode>();

        public GroupNode1(Rect rect) : base(rect)
        {
        }

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
                if (_groups == null) _groups = new List<GroupNode2>();
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode2>(x => (GroupNode2)x);
            }
        }

        public override GroupNode InsertChild(Rect rect)
        {
            GroupNode2 node = new GroupNode2(rect);
            _groups.Add(node);
            return node;
        }
    }
    [System.Serializable]
    public class GroupNode2 : GroupNode
    {
        public List<GroupNode3> _groups = new List<GroupNode3>();
        public List<ImgNode> _images = new List<ImgNode>();

        public GroupNode2(Rect rect) : base(rect)
        {
        }

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
                if (_groups == null) _groups = new List<GroupNode3>();
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode3>(x => (GroupNode3)x);
            }
        }
        public override GroupNode InsertChild(Rect rect)
        {
            GroupNode3 node = new GroupNode3(rect);
            _groups.Add(node);
            return node;
        }
    }
    [System.Serializable]
    public class GroupNode3 : GroupNode
    {
        public List<GroupNode4> _groups = new List<GroupNode4>();
        public List<ImgNode> _images = new List<ImgNode>();

        public GroupNode3(Rect rect) : base(rect)
        {
        }

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
                if (_groups == null) _groups = new List<GroupNode4>();
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode4>(x => (GroupNode4)x);
            }
        }
        public override GroupNode InsertChild(Rect rect)
        {
            GroupNode4 node = new GroupNode4(rect);
            _groups.Add(node);
            return node;
        }
    }
    [System.Serializable]
    public class GroupNode4 : GroupNode
    {
        public List<GroupNode5> _groups = new List<GroupNode5>();
        public List<ImgNode> _images = new List<ImgNode>();

        public GroupNode4(Rect rect) : base(rect)
        {
        }

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
                if (_groups == null) _groups = new List<GroupNode5>();
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode5>(x => (GroupNode5)x);
            }
        }
        public override GroupNode InsertChild(Rect rect)
        {
            GroupNode5 node = new GroupNode5(rect);
            _groups.Add(node);
            return node;
        }
    }
    [System.Serializable]
    public class GroupNode5 : GroupNode
    {
        public List<GroupNode6> _groups;
        public List<ImgNode> _images = new List<ImgNode>();

        public GroupNode5(Rect rect) : base(rect)
        {
        }

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
                if (_groups == null) _groups = new List<GroupNode6>();
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode6>(x => (GroupNode6)x);
            }
        }
        public override GroupNode InsertChild(Rect rect)
        {
            GroupNode6 node = new GroupNode6(rect);
            _groups.Add(node);
            return node;
        }
    }
    [System.Serializable]
    public class GroupNode6 : GroupNode
    {
        public List<GroupNode7> _groups;
        public List<ImgNode> _images = new List<ImgNode>();

        public GroupNode6(Rect rect) : base(rect)
        {
        }

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
                if (_groups == null) _groups = new List<GroupNode7>();
                return _groups.ConvertAll<GroupNode>(x => x);
            }

            set
            {
                _groups = value.ConvertAll<GroupNode7>(x => (GroupNode7)x);
            }
        }
        public override GroupNode InsertChild(Rect rect)
        {
            GroupNode7 node = new GroupNode7(rect);
            _groups.Add(node);
            return node;
        }
    }
    [System.Serializable]
    public class GroupNode7 : GroupNode
    {
        public GroupNode7(Rect rect) : base(rect)
        {
        }

        public override List<ImgNode> images { get { return null; } set { } }
        public override List<GroupNode> groups { get; set; }
        public override GroupNode InsertChild(Rect rect)
        {
            Debug.Log("cant Insert");
            return null;
        }
    }
}

