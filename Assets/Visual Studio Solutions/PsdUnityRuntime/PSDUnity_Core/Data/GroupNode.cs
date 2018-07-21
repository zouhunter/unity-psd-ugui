using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PSDUnity.UGUI;

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
        public LayerImport layerImporter;

        public List<object> children { get; set; }
        public Data.GroupNode parent { get; set; }

        public GroupNode() { }
        public GroupNode(Rect rect, int id, int depth)
        {
            this.rect = rect;
            this.id = id;
            this.depth = depth;
        }

        public GroupNode Analyzing(RuleObject Rule, string name)
        {
            string[] areguments = null;
            this.displayName = Rule.AnalysisGroupName(name, out suffix, out areguments);
            if (layerImporter == null)
            {
                layerImporter = Rule.layerImports.Where(x => x.Suffix == suffix).FirstOrDefault();
                if (layerImporter == null)
                {
                    layerImporter = ScriptableObject.CreateInstance<UGUI.PanelLayerImport>();
                }
            }
            layerImporter.AnalysisAreguments(this, areguments);
            return this;
        }

        public void GetImgNodes(List<ImgNode> imgNodes)
        {
            if (images != null)
            {
                imgNodes.AddRange(images);
            }
            if (children != null)
            {
                foreach (var item in children)
                {
                    if (item is GroupNode)
                    {
                        (item as GroupNode).GetImgNodes(imgNodes);
                    }
                }
            }
        }

        internal void AddChild(GroupNode childNode)
        {
            if (children == null)
                children = new List<object>();
            children.Add(childNode);   
        }
    }
}

