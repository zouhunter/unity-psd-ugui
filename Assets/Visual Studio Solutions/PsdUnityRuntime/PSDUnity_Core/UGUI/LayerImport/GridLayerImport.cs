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
        [Header("[参数-----------------------------------")]
        [SerializeField, CustomField("重置滚动条")]
        protected string horizontal = "h";
        [SerializeField, CustomField("重置滚动条")] protected string vertical = "v";

        public DirectionAxis GetDirectionByKey(string[] keys)
        {
            DirectionAxis dir = 0;
            foreach (var key in keys)
            {
                if (string.Compare(key, vertical, true) == 0)
                {
                    dir |= DirectionAxis.Vertical;
                }

                if (string.Compare(key, horizontal, true) == 0)
                {
                    dir |= DirectionAxis.Horizontal;
                }
            }

            return dir;
        }
        public GridLayerImport()
        {
            _suffix = "Grid";
        }

        public override GameObject CreateTemplate()
        {
            return new GameObject("Grid", typeof(GridLayoutGroup));
            
        }
        public override void AnalysisAreguments(Data.GroupNode layer, string[] areguments)
        {
            base.AnalysisAreguments(layer, areguments);
            if (areguments != null && areguments.Length > 1)
            {
                layer.directionAxis = GetDirectionByKey(areguments);
            }

            if (areguments != null)
            {
                if (areguments.Length > 2)
                {
                    var key = areguments[1];
                    int.TryParse(key,out layer.constraintCount);
                }
            }
        }


        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            node.anchoType = AnchoType.Up | AnchoType.Left;

            if (layer.children != null)
            {
                ctrl.DrawLayers(layer.children.ConvertAll(x => x as Data.GroupNode).ToArray(), node);
            }
            if (layer.images != null)
            {
                ctrl.DrawImages(layer.images.ToArray(), node);
            }

            InitGrid(node, layer);

            return node;
        }

        private void InitGrid(UGUINode node,Data.GroupNode layer)
        {
            GridLayoutGroup gridLayoutGroup = node.InitComponent<GridLayoutGroup>();

            gridLayoutGroup.padding = new RectOffset(1, 1, 1, 1);
            gridLayoutGroup.cellSize = new Vector2(layer.rect.width, layer.rect.height);

            switch (layer.directionAxis)
            {
                case DirectionAxis.Horizontal:
                    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    break;
                case DirectionAxis.Vertical:
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
