using System;
using UnityEngine;

namespace PSDUIImporter
{
    internal class GroupLayerImport : ILayerImport
    {
        private PSDImportCtrl pSDImportCtrl;

        public GroupLayerImport(PSDImportCtrl pSDImportCtrl)
        {
            this.pSDImportCtrl = pSDImportCtrl;
        }

        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_GROUP, layer.name, parent);
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

            UINode[] nodes = pSDImportCtrl.DrawImages(layer.images,node);
            foreach (var item in nodes)
            {
                item.anchoType = UINode.AnchoType.Left | UINode.AnchoType.Up;
            }
            nodes = pSDImportCtrl.DrawLayers(layer.layers, node);
            foreach (var item in nodes)
            {
                item.anchoType = UINode.AnchoType.Left | UINode.AnchoType.Up;
            }
            return node;
        }
    }
}