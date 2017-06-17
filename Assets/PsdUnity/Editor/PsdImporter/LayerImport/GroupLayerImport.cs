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
            UGUINode node = PSDImportUtility.InstantiateItem(PrefabName.PREFAB_GROUP, layer.name, parent);
            UnityEngine.UI.LayoutGroup group = null;
            string type = layer.arguments[0].ToLower();
            float span = 0;
            float.TryParse(layer.arguments[1], out span);
            switch (type)
            {
                case "v":
                    group = node.InitComponent<UnityEngine.UI.VerticalLayoutGroup>();
                    group.childAlignment = TextAnchor.UpperLeft;
                    ((UnityEngine.UI.VerticalLayoutGroup)group).spacing = span;
                    break;
                case "h":
                    group = node.InitComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                    group.childAlignment = TextAnchor.UpperLeft;
                    ((UnityEngine.UI.HorizontalLayoutGroup)group).spacing = span;
                    break;
            }

            PSDImportUtility.SetRectTransform(layer, group.GetComponent<RectTransform>(),parent.InitComponent<RectTransform>());

            UGUINode[] nodes = pSDImportCtrl.DrawImages(layer.images.ToArray(),node);
            foreach (var item in nodes)
            {
                item.anchoType = UGUINode.AnchoType.Left | UGUINode.AnchoType.Up;
            }
            nodes = pSDImportCtrl.DrawLayers(layer.groups.ToArray(), node);
            foreach (var item in nodes)
            {
                item.anchoType = UGUINode.AnchoType.Left | UGUINode.AnchoType.Up;
            }
            return node;
        }
    }
}