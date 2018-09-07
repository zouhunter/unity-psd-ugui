using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using UnityEditorInternal;
using PSDUnity.Data;

namespace PSDUnity
{
    public class GroupNodeItem : TreeViewItem
    {
        public override int id
        {
            get
            {
                return data.id;
            }

            set
            {
                data.id = value;
            }
        }
        public override int depth
        {
            get
            {
                return data.depth;
            }

            set
            {
                data.depth = value;
            }
        }
        public override string displayName
        {
            get
            {
                return data.displayName;
            }

            set
            {
                data.displayName = value;
            }
        }
        public Rect rect { get { return data.rect; } set { data.rect = value; } }
        public List<Data.ImgNode> images
        {
            get
            {
                return data.images;
            }
            set
            {
                data.images = value;
            }
        }
        public Data.GroupNode data = new Data.GroupNode();
        private UGUI.LayerImport layerImporter;

        public GroupNodeItem(Rect rect, int id, int depth) : base(id, depth)
        {
            data.rect = rect;
            children = new List<TreeViewItem>();
            data.id = id;
            data.depth = depth;
        }
        public void AddChild(GroupNodeItem item)
        {
            base.AddChild(item);
            if (!data.children.Contains(item))
            {
                data.children.Add(item.data);
            }
        }
        public GroupNodeItem(GroupNode data)
        {
            this.data = data;
        }

        public override List<TreeViewItem> children
        {
            get
            {
                return base.children;
            }

            set
            {
                base.children = value;
                if (value != null)
                {
                    data.children = value.Where(x => x != null).Select(x => (object)((x as GroupNodeItem).data)).ToList();
                }
            }
        }
        public override TreeViewItem parent
        {
            get
            {
                return base.parent;
            }

            set
            {
                base.parent = value;
                if (value != null)
                {
                    data.parent = (value as GroupNodeItem).data;
                }
            }
        }

        public void GetImgNodes(List<Data.ImgNode> imgNodes)
        {
            if (data.images != null)
            {
                imgNodes.AddRange(data.images);
            }
            if (children != null)
            {
                foreach (GroupNodeItem item in children)
                {
                    item.GetImgNodes(imgNodes);
                }
            }
        }
        public GroupNodeItem Analyzing(RuleObject Rule, string name)
        {
            string[] areguments = null;
            this.displayName = Rule.AnalysisGroupName(name, out data.suffix, out areguments);
            if (layerImporter == null)
            {
                layerImporter = Rule.layerImports.Where(x => x.Suffix == data.suffix).FirstOrDefault();
                if (layerImporter == null)
                {
                    layerImporter = ScriptableObject.CreateInstance<UGUI.PanelLayerImport>();
                }
            }
            layerImporter.AnalysisAreguments(data, areguments);
            return this;
        }
    }

}