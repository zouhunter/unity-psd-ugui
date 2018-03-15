using System;
using UnityEngine;
using PSDUnity;
namespace PSDUnity.UGUI
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
            UGUINode node = PSDImporter.InstantiateItem(GroupType.GROUP, layer.displayName, parent);
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

            PSDImporter.SetRectTransform(layer, group.GetComponent<RectTransform>());

            UGUINode[] nodes = pSDImportCtrl.DrawImages(layer.images.ToArray(),node);
            foreach (var item in nodes)
            {
                item.anchoType = AnchoType.Left | AnchoType.Up;
            }
            nodes = pSDImportCtrl.DrawLayers(layer.children.ConvertAll(x=>x as GroupNode).ToArray(), node);
            foreach (var item in nodes)
            {
                item.anchoType = AnchoType.Left | AnchoType.Up;
            }
            return node;
        }
    }
}