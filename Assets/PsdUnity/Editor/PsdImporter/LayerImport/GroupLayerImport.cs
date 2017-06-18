using System;
using UnityEngine;

namespace PSDUnity
{
    internal class GroupLayerImport : ILayerImport
    {
        private PSDImportCtrl pSDImportCtrl;

        public GroupLayerImport(PSDImportCtrl pSDImportCtrl)
        {
            this.pSDImportCtrl = pSDImportCtrl;
        }

        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(GroupType.GROUP, layer.Name, parent);
            UnityEngine.UI.LayoutGroup group = null;
            switch (layer.direction)
            {
                case Direction.Horizontal:
                    group = node.InitComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                    group.childAlignment = TextAnchor.UpperLeft;
                    break;
                case Direction.Vertical:
                    group = node.InitComponent<UnityEngine.UI.VerticalLayoutGroup>();
                    group.childAlignment = TextAnchor.UpperLeft;
                    break;
                default:
                    break;
            }
            ((UnityEngine.UI.VerticalLayoutGroup)group).spacing = layer.spacing;

            PSDImportUtility.SetRectTransform(layer, group.GetComponent<RectTransform>(),parent.InitComponent<RectTransform>());

            UGUINode[] nodes = pSDImportCtrl.DrawImages(layer.images.ToArray(),node);
            foreach (var item in nodes)
            {
                item.anchoType = AnchoType.Left | AnchoType.Up;
            }
            nodes = pSDImportCtrl.DrawLayers(layer.groups.ToArray(), node);
            foreach (var item in nodes)
            {
                item.anchoType = AnchoType.Left | AnchoType.Up;
            }
            return node;
        }
    }
}