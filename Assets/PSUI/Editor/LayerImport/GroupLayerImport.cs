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
            GameObject temp = null;
            string type = layer.arguments[0].ToUpper();
            switch (type.ToUpper())
            {
                case "V":
                    temp = Resources.Load(PSDImporterConst.PREFAB_PATH_GROUP_V, typeof(GameObject)) as GameObject;
                    break;
                case "H":
                    temp = Resources.Load(PSDImporterConst.PREFAB_PATH_GROUP_H, typeof(GameObject)) as GameObject;
                    break;
            }

            UnityEngine.UI.HorizontalOrVerticalLayoutGroup group = GameObject.Instantiate(temp).GetComponent<UnityEngine.UI.HorizontalOrVerticalLayoutGroup>();//as UnityEngine.UI.HorizontalOrVerticalLayoutGroup;
            group.transform.SetParent(parent.transform, false); //parent = parent.transform;

            RectTransform rectTransform = group.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);

            float span;
            if (float.TryParse(layer.arguments[1],out span))
            {
                group.spacing = span;
            }
            UINode _node = new UINode(group.transform, parent);
            pSDImportCtrl.DrawLayers(layer.layers, _node);
            return null;
        }
    }
}