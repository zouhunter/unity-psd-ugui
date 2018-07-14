using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.IMGUI.Controls;
using System.Linq;

namespace PSDUnity
{
    [System.Serializable]
    public class GroupNode :TreeViewItem
    {
        public int _id;
        public int _depth;
        public string _displayName;
        public string suffix = emptySuffix;
        public Direction direction = Direction.Vertical;
        public int constraintCount;
        public float spacing;
        public Rect rect;   //利用名字解析controlType和arguments
        public List<ImgNode> images = new List<ImgNode>();
        public UGUI.LayerImport layerImporter;

        public const string emptySuffix = "[-inner empty suffix]";
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
            this.displayName = Rule.AnalysisGroupName(name, out suffix, out areguments);
            layerImporter = Rule.layerImports.Where(x => x.Suffix == suffix).FirstOrDefault();
            if(layerImporter != null){
                layerImporter.AnalysisAreguments(this, areguments);
            }
            else
            {
                layerImporter =ScriptableObject.CreateInstance < UGUI.PanelLayerImport>();
            }
            return this;
        }
    }
}

