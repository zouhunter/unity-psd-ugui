using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class GridLayerImport : LayerImport
    {
        public GridLayerImport(PSDImportCtrl ctrl) : base(ctrl) { }
        public override GameObject CreateTemplate()
        {
            return new GameObject("Grid", typeof(GridLayoutGroup));
            
        }
        public override UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            node.anchoType = AnchoType.Up | AnchoType.Left;

            if (layer.children != null)
            {
                ctrl.DrawLayers(layer.children.ConvertAll(x => x as GroupNode).ToArray(), node);
            }
            if (layer.images != null)
            {
                ctrl.DrawImages(layer.images.ToArray(), node);
            }

            InitGrid(node, layer);

            return node;
        }

        private void InitGrid(UGUINode node,GroupNode layer)
        {
            GridLayoutGroup gridLayoutGroup = node.InitComponent<GridLayoutGroup>();

            gridLayoutGroup.padding = new RectOffset(1, 1, 1, 1);
            gridLayoutGroup.cellSize = new Vector2(layer.rect.width, layer.rect.height);

            switch (layer.direction)
            {
                case Direction.Horizontal:
                    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    break;
                case Direction.Vertical:
                    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                    break;
                default:
                    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;
                    break;
            }
            gridLayoutGroup.constraintCount = layer.constraintCount;
        }
    }
}
