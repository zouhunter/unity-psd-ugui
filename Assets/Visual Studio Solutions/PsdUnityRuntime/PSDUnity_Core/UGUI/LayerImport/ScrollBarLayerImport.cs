using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace PSDUnity.UGUI
{
    public class ScrollBarLayerImport : LayerImport
    {
        [Header("[前缀-----------------------------------")]
        [SerializeField, CustomField("背景图片")] protected string backgroundAddress = "b_";
        [SerializeField, CustomField("手柄图标")] protected string handleAddress = "h_";

        [Header("[参数-----------------------------------")]
        [SerializeField, CustomField("从左到右")] protected string left_right = "l";
        [SerializeField, CustomField("从右到左")] protected string right_left = "r";
        [SerializeField, CustomField("从下到上")] protected string bottom_top = "b";
        [SerializeField, CustomField("从上到下")] protected string top_bottom = "l";

        public Direction GetDirectionByKey(string key)
        {
            Direction dir = 0;

            if (string.Compare(key, bottom_top, true) == 0)
            {
                dir = Direction.BottomToTop;
            }
            else if (string.Compare(key, top_bottom, true) == 0)
            {
                dir = Direction.TopToBottom;
            }
            else if (string.Compare(key, left_right, true) == 0)
            {
                dir = Direction.LeftToRight;
            }
            else if (string.Compare(key, right_left, true) == 0)
            {
                dir = Direction.RightToLeft;
            }

            return dir;
        }
        public ScrollBarLayerImport()
        {
            _suffix = "ScrollBar";
        }

        public override void AnalysisAreguments(Data.GroupNode layer, string[] areguments)
        {
            base.AnalysisAreguments(layer, areguments);
            if (areguments != null && areguments.Length > 0)
            {
                var key = areguments[0];
                layer.direction = GetDirectionByKey(key);
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

        public override UGUINode DrawLayer(Data.GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            Scrollbar scrollbar = node.InitComponent<Scrollbar>();
            SetScrollBarDirection(scrollbar, layer);
            DrawImages(layer.images, scrollbar, node);
            return node;
        }

        private void DrawImages(List<Data.ImgNode> images, Scrollbar scrollbar, UGUINode node)
        {
            for (int i = 0; i < images.Count; i++)
            {
                Data.ImgNode image = images[i];
                UnityEngine.UI.Image graph = null;

                if (MatchAddress(image.Name, backgroundAddress))
                {
                    graph = scrollbar.GetComponent<UnityEngine.UI.Image>();
                    SetRectTransform(image.rect, scrollbar.GetComponent<RectTransform>());
                }
                else if (MatchAddress(image.Name, handleAddress))
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
        private void SetScrollBarDirection(Scrollbar scrollbar, Data.GroupNode layer)
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
