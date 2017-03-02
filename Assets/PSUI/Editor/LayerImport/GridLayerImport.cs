using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class GridLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public GridLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_GRID, layer.name, parent);
            GridLayoutGroup gridLayoutGroup = node.GetComponent<GridLayoutGroup>();
            PSDImportUtility.SetRectTransform(layer, gridLayoutGroup.GetComponent<RectTransform>());

            gridLayoutGroup.padding = new RectOffset(1,1,1,1);
            gridLayoutGroup.cellSize = new Vector2(layer.size.width, layer.size.height);

            if (layer.arguments != null && layer.arguments.Length > 1 )
            {
                string ancho = layer.arguments[0].ToLower();
                if (ancho.Contains("l")) node.anchoType |= UINode.AnchoType.Left;
                if (ancho.Contains("r")) node.anchoType |= UINode.AnchoType.Right;
                if (ancho.Contains("u")) node.anchoType |= UINode.AnchoType.Up;
                if (ancho.Contains("d")) node.anchoType |= UINode.AnchoType.Down;

                string rc = layer.arguments[1];
                gridLayoutGroup.constraint = rc.ToLower() == "c" ? GridLayoutGroup.Constraint.FixedColumnCount : (rc.ToLower() == "r"? GridLayoutGroup.Constraint.FixedRowCount: GridLayoutGroup.Constraint.Flexible);
                int count = int.Parse(layer.arguments[2]);
                gridLayoutGroup.constraintCount = count;
              
            }

            ctrl.DrawImages(layer.images, node);
            ctrl.DrawLayers(layer.layers, node);
            return node;
        }
    }
}
