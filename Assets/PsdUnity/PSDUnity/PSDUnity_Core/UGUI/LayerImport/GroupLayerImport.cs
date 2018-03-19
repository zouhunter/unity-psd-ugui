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

            var nodes = pSDImportCtrl.DrawLayers(layer.children.ConvertAll(x => x as GroupNode).ToArray(), node);
            foreach (var item in nodes)
            {
                item.anchoType = AnchoType.Left | AnchoType.Up;
            }

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

            PSDImporter.SetRectTransform(layer, group.GetComponent<RectTransform>());

            nodes = pSDImportCtrl.DrawImages(layer.images.ToArray(), node);
            foreach (var item in nodes)
            {
                item.anchoType = AnchoType.Left | AnchoType.Up;
            }

            return node;
        }
    }
}