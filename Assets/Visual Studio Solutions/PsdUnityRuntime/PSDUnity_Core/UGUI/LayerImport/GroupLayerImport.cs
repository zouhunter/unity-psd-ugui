using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PSDUnity.UGUI
{
    public class GroupLayerImport : LayerImport
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
        public GroupLayerImport()
        {
            _suffix = "Group";
        }

        public override void AnalysisAreguments(Data.GroupNode layer, string[] areguments)
        {
            base.AnalysisAreguments(layer, areguments);
      
            if (areguments != null)
            {
                if(areguments.Length > 0)
                {
                    layer.directionAxis = GetDirectionByKey(areguments);
                }

                if (areguments.Length == 2)
                {
                    float.TryParse(areguments[1], out layer.spacing);
                }

                else if(areguments.Length >= 3)
                {
                    float.TryParse(areguments[2], out layer.spacing);
                }
            }
        }
        public override GameObject CreateTemplate()
        {
            return new GameObject("Group", typeof(RectTransform));
        }

        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);

            if (layer.children != null)
            {
                foreach (Data.GroupNode item in layer.children)
                {
                    var childNode = ctrl.DrawLayer(item, node);
                    SetLayoutItem(childNode, item.rect);
                }
            }
            if (layer.images != null)
            {
                foreach (Data.ImgNode item in layer.images)
                {
                    var childNode = ctrl.DrawImage(item, node);
                    SetLayoutItem(childNode, item.rect);
                }
            }

            InitLayoutGroup(layer, node);
            return node;
        }
        private void SetLayoutItem(UGUINode childNode, Rect rect)
        {
            var layout = childNode.transform.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = rect.width;
            layout.preferredHeight = rect.height;
            childNode.anchoType = AnchoType.Left | AnchoType.Up;
        }

        /// <summary>
        /// 初始化组
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="node"></param>
        private void InitLayoutGroup(Data.GroupNode layer, UGUINode node)
        {
            HorizontalOrVerticalLayoutGroup group = null;

            switch (layer.directionAxis)
            {
                case DirectionAxis.Horizontal:
                    group = node.InitComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                    break;
                case DirectionAxis.Vertical:
                default:
                    group = node.InitComponent<UnityEngine.UI.VerticalLayoutGroup>();
                    break;
            }
            if (group)
            {
                (group as UnityEngine.UI.VerticalLayoutGroup).spacing = layer.spacing;
                group.childAlignment = TextAnchor.UpperLeft;
            }
        }
        public override void AfterGenerate(UGUINode node)
        {
            base.AfterGenerate(node);
            ContentSizeFitter content = node.InitComponent<ContentSizeFitter>();
            content.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            content.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}