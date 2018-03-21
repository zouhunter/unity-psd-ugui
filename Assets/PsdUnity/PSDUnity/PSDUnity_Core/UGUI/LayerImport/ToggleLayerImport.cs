using System;
using UnityEngine;
using UnityEngine.UI;

namespace PSDUnity.UGUI
{
    public class ToggleLayerImport : LayerImport
    {
        public ToggleLayerImport(PSDImportCtrl ctrl) : base(ctrl) { }

        public override GameObject CreateTemplate()
        {
            var toggle = new GameObject("Toggle", typeof(Toggle)).GetComponent<Toggle>();
            var background = new GameObject("Background", typeof(Image)).GetComponent<Image>();
            var mask = new GameObject("Mask", typeof(Image)).GetComponent<Image>();

            background.transform.SetParent(toggle.transform, false);
            mask.transform.SetParent(background.transform, false);

            toggle.targetGraphic = background;
            toggle.graphic = mask;

            return toggle.gameObject;
        }

        public override UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = CreateRootNode(layer.displayName, layer.rect, parent);
            UnityEngine.UI.Toggle toggle = node.InitComponent<UnityEngine.UI.Toggle>();
            if (layer.images != null)
            {
                var bgNode = DrawBackground(layer, toggle, node);
                DrawMask(layer, toggle, bgNode);
                DrawOtherLayers(layer, node);
            }
            return node;
        }

        private UGUINode DrawBackground(GroupNode layer, Toggle toggle, UGUINode node)
        {
            var bgImage = layer.images.Find(x => MatchAddress(x.Name, rule.backgroundAddress));

            UGUINode bgNode = null;
            if (bgImage != null)
            {
                bgNode = CreateNormalNode(toggle.targetGraphic.gameObject, bgImage.rect, node);
                PSDImporterUtility.SetPictureOrLoadColor(bgImage, toggle.targetGraphic);
                SetRectTransform(bgImage.rect, toggle.GetComponent<RectTransform>());
            }
            else
            {
                bgNode = CreateNormalNode(toggle.targetGraphic.gameObject, layer.rect, node);
            }
            return bgNode;
        }

        private void DrawMask(GroupNode layer, Toggle toggle, UGUINode bgNode)
        {
            var mask = layer.images.Find(x => MatchAddress(x.Name, rule.maskAddress));

            if (mask != null)
            {
                PSDImporterUtility.SetPictureOrLoadColor(mask, toggle.graphic);
                CreateNormalNode(toggle.graphic.gameObject, mask.rect, bgNode);
            }
        }

        private void DrawOtherLayers(GroupNode layer, UGUINode node)
        {
            for (int imageIndex = 0; imageIndex < layer.images.Count; imageIndex++)
            {
                ImgNode image = layer.images[imageIndex];
                if (!MatchAddress(image.Name, rule.backgroundAddress, rule.maskAddress))
                {
                    ctrl.DrawImage(image, node);
                }
            }
        }
        public override void AfterGenerate(UGUINode node)
        {
            base.AfterGenerate(node);
            StretchTitle(node);
        }
    }
}