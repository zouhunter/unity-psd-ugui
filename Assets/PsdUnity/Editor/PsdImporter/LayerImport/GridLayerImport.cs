using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUnity
{
    public class GridLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public GridLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_GRID, layer.name, parent);
            node.anchoType = UGUINode.AnchoType.Up | UGUINode.AnchoType.Left;
            GridLayoutGroup gridLayoutGroup = node.InitComponent<GridLayoutGroup>();
            PSDImportUtility.SetRectTransform(layer, gridLayoutGroup.GetComponent<RectTransform>(),node.InitComponent<RectTransform>());

            gridLayoutGroup.padding = new RectOffset(1,1,1,1);
            gridLayoutGroup.cellSize = new Vector2(layer.rect.width, layer.rect.height);

            if (layer.arguments != null && layer.arguments.Count > 1 )
            {
                string rc = layer.arguments[0];
                gridLayoutGroup.constraint = rc.ToLower() == "c" ? GridLayoutGroup.Constraint.FixedColumnCount : (rc.ToLower() == "r"? GridLayoutGroup.Constraint.FixedRowCount: GridLayoutGroup.Constraint.Flexible);
                int count = int.Parse(layer.arguments[1]);
                gridLayoutGroup.constraintCount = count;
              
            }

            ctrl.DrawImages(layer.images.ToArray(), node);
            ctrl.DrawLayers(layer.groups.ToArray() as GroupNode[], node);
            return node;
        }
    }
}
