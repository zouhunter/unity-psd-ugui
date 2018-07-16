using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PSDUnity.UGUI
{
    public class GroupLayerImport : LayerImport
    {
        public GroupLayerImport()
        {
            _suffix = "Group";
        }

        public override void AnalysisAreguments(Data.GroupNode layer, string[] areguments)
        {
            base.AnalysisAreguments(layer, areguments);
            if (areguments != null && areguments.Length > 1)
            {
                var key = areguments[0];
                layer. direction = RuleObject.GetDirectionByKey(key);
            }
            if (areguments != null && areguments.Length > 2)
            {
                var key = areguments[1];
                layer.spacing = float.Parse(key);
            }
        }
        public override GameObject CreateTemplate()
        {
            return new GameObject("Group", typeof(RectTransform));
        }
    
        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);

            if (layer.children != null)
            {
                foreach (Data.GroupNode item in layer.children)
                {
                   var childNode = ctrl.DrawLayer(item, node);
                    SetLayoutItem(childNode, item.rect);
                }
            }
            if (layer.images != null)
            {
                foreach (Data.ImgNode item in layer.images)
                {
                    var childNode = ctrl.DrawImage(item, node);
                    SetLayoutItem(childNode, item.rect);
                }
            }

            InitLayoutGroup(layer, node);
            return node;
        }
        private void SetLayoutItem(UGUINode childNode,Rect rect)
        {
            var layout = childNode.transform.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = rect.width;
            layout.preferredHeight = rect.height;
            childNode.anchoType = AnchoType.Left | AnchoType.Up;
        }
        
        /// <summary>
        /// 初始化组
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="node"></param>
        private void InitLayoutGroup(Data.GroupNode layer, UGUINode node)
        {
            HorizontalOrVerticalLayoutGroup group = null;

            switch (layer.direction)
            {
                case Direction.Horizontal:
                    group = node.InitComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                    break;
                case Direction.Vertical:
                default:
                    group = node.InitComponent<UnityEngine.UI.VerticalLayoutGroup>();
                    break;
            }
            if(group)
            {
                (group as UnityEngine.UI.VerticalLayoutGroup).spacing = layer.spacing;
                group.childAlignment = TextAnchor.UpperLeft;
            }
        }
        public override void AfterGenerate(UGUINode node)
        {
            base.AfterGenerate(node);
            ContentSizeFitter content = node.InitComponent<ContentSizeFitter>();
            content.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            content.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}