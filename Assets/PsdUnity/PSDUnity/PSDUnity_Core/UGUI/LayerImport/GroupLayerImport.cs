using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PSDUnity.UGUI
{
    internal class GroupLayerImport : LayerImport
    {
        public override GameObject CreateTemplate()
        {
            return new GameObject("Group", typeof(ContentSizeFitter));
        }
        public GroupLayerImport(PSDImportCtrl ctrl) : base(ctrl)
        {
        }

        public override UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            var nodeList = new List<UGUINode>();

            if (layer.children != null)
            {
                var nodes = ctrl.DrawLayers(layer.children.ConvertAll(x => x as GroupNode).ToArray(), node);
                nodeList.AddRange(nodes);
            }
            if (layer.images != null)
            {
                var nodes = ctrl.DrawImages(layer.images.ToArray(), node);
                nodeList.AddRange(nodes);
            }


            InitLayoutNodes(nodeList);
            InitLayoutGroup(layer, node);
            return node;
        }

        /// <summary>
        /// 初始化内容节点 
        /// </summary>
        /// <param name="nodes"></param>
        private void InitLayoutNodes(IList<UGUINode> nodes)
        {
            foreach (var item in nodes)
            {
                item.anchoType = AnchoType.Left | AnchoType.Up;
                var layout = item.transform.gameObject.AddComponent<LayoutElement>();
                layout.preferredWidth = item.InitComponent<RectTransform>().sizeDelta.x;
                layout.preferredHeight = item.InitComponent<RectTransform>().sizeDelta.x;
            }
        }
        /// <summary>
        /// 初始化组
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="node"></param>
        private void InitLayoutGroup(GroupNode layer, UGUINode node)
        {
            LayoutGroup group = null;

            switch (layer.direction)
            {
                case Direction.Horizontal:
                    group = node.InitComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                    group.childAlignment = TextAnchor.UpperLeft;
                    (group as UnityEngine.UI.HorizontalLayoutGroup).spacing = layer.spacing;
                    break;
                case Direction.Vertical:
                default:
                    group = node.InitComponent<UnityEngine.UI.VerticalLayoutGroup>();
                    (group as UnityEngine.UI.VerticalLayoutGroup).spacing = layer.spacing;
                    group.childAlignment = TextAnchor.UpperLeft;
                    break;
            }
        }
    }
}