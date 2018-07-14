using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace PSDUnity.UGUI
{
    public class ScrollBarLayerImport : LayerImport
    {
        public ScrollBarLayerImport()
        {
            _suffix = "ScrollBar";
        }

        public override void AnalysisAreguments(GroupNode layer, string[] areguments)
        {
            base.AnalysisAreguments(layer, areguments);
            if (areguments != null && areguments.Length > 0)
            {
                var key = areguments[0];
                layer.direction = RuleObject.GetDirectionByKey(key);
            }
        }
        public override GameObject CreateTemplate()
        {
            var scollbar = new GameObject("Scollbar", typeof(Scrollbar), typeof(Image)).GetComponent<Scrollbar>();
            var sliding_Area = new GameObject("Sliding Area", typeof(RectTransform));
            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));

            scollbar.targetGraphic = scollbar.GetComponent<Image>();
            sliding_Area.transform.SetParent(scollbar.transform, false);
            PSDUnity.UGUI.PSDImporterUtility.SetNormalAnchor(AnchoType.XStretch | AnchoType.YStretch, scollbar.GetComponent<RectTransform>(), sliding_Area.transform as RectTransform);

            handle.transform.SetParent(sliding_Area.transform, false);
            PSDUnity.UGUI.PSDImporterUtility.SetNormalAnchor(AnchoType.XCenter | AnchoType.XCenter, sliding_Area.GetComponent<RectTransform>(), handle.transform as RectTransform);

            scollbar.GetComponent<Scrollbar>().handleRect = handle.GetComponent<RectTransform>();
            return scollbar.gameObject;
        }

        public override UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName,layer.rect, parent);
            Scrollbar scrollbar = node.InitComponent<Scrollbar>();
            SetScrollBarDirection(scrollbar, layer);
            DrawImages(layer.images, scrollbar,node);
            return node;
        }

        private void DrawImages(List<ImgNode> images, Scrollbar scrollbar,UGUINode node)
        {
            for (int i = 0; i < images.Count; i++)
            {
                ImgNode image = images[i];
                UnityEngine.UI.Image graph = null;

                if (MatchAddress(image.Name,rule.backgroundAddress))
                {
                    graph = scrollbar.GetComponent<UnityEngine.UI.Image>();
                    SetRectTransform(image.rect, scrollbar.GetComponent<RectTransform>());
                }
                else if (MatchAddress(image.Name, rule.handleAddress))
                {
                    graph = scrollbar.handleRect.GetComponent<UnityEngine.UI.Image>();
                }
                else
                {
                    ctrl.DrawImage(image, node);
                }

                if (graph != null)
                {
                    PSDImporterUtility.SetPictureOrLoadColor(image, graph);
                }

            }
        }

        /// <summary>
        /// 设置Scrollbar的方向
        /// </summary>
        /// <param name="scrollbar"></param>
        /// <param name="layer"></param>
        private void SetScrollBarDirection(Scrollbar scrollbar,GroupNode layer)
        {
            switch (layer.direction)
            {
                case Direction.LeftToRight:
                    scrollbar.direction = Scrollbar.Direction.LeftToRight;
                    break;
                case Direction.BottomToTop:
                    scrollbar.direction = Scrollbar.Direction.BottomToTop;
                    break;
                case Direction.TopToBottom:
                    scrollbar.direction = Scrollbar.Direction.TopToBottom;
                    break;
                case Direction.RightToLeft:
                    scrollbar.direction = Scrollbar.Direction.RightToLeft;
                    break;
                default:
                    if (layer.rect.width > layer.rect.height)
                    {
                        scrollbar.direction = Scrollbar.Direction.LeftToRight;
                    }
                    else
                    {
                        scrollbar.direction = Scrollbar.Direction.BottomToTop;
                    }
                    break;
            }
        }

        public override void AfterGenerate(UGUINode node)
        {
            base.AfterGenerate(node);
            var scrollbar = node.InitComponent<Scrollbar>();
            PSDImporterUtility.SetCustomAnchor(scrollbar.GetComponent<RectTransform>(), scrollbar.handleRect);
        }
    }
}
