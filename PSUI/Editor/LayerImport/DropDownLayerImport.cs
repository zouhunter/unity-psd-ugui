using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class DropDownLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public DropDownLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_DROPDOWN, layer.name, parent);
            Dropdown dropdown = node.InitComponent<Dropdown>();
            ScrollRect scrllRect = dropdown.template.GetComponent<ScrollRect>();
            RectTransform content = scrllRect.content;
            Toggle toggle = content.GetComponentInChildren<Toggle>();

            UINode childNode = new UINode(dropdown.template, node);
            childNode.transform.SetParent(PSDImportUtility.canvas.transform);
            childNode.anchoType = UINode.AnchoType.Down | UINode.AnchoType.XStretch;
            //由于设置相对坐标需要，所以修改了部分预制体的状态
            childNode.ReprocessEvent = () => {
                RectTransform rt = childNode.InitComponent<RectTransform>();
                rt.pivot = new Vector2(0.5f, 1);
                rt.anchoredPosition = Vector3.zero;
            };
            for (int i = 0; i < layer.images.Length; i++)
            {
                Image image = layer.images[i];
                string lowerName = image.name.ToLower();
                if (lowerName.StartsWith("b1_"))
                {
                    PSDImportUtility.SetPictureOrLoadColor(image, dropdown.image);
                    PSDImportUtility.SetRectTransform(image, dropdown.GetComponent<RectTransform>());
                    dropdown.name = layer.name;
                }
                else if(lowerName.StartsWith("b2_"))
                {
                    PSDImportUtility.SetPictureOrLoadColor(image, dropdown.template.GetComponent<Graphic>());
                    PSDImportUtility.SetRectTransform(image, dropdown.template);
                }
                else if (lowerName.StartsWith("b3_"))
                {
                    UnityEngine.UI.Image itemimage = (UnityEngine.UI.Image)toggle.targetGraphic;
                    PSDImportUtility.SetPictureOrLoadColor(image, itemimage);
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, image.size.height);
                }
                else if (lowerName.StartsWith("l1_"))
                {
                    PSDImportUtility.SetPictureOrLoadColor(image, dropdown.captionText);
                    float size;
                    if (float.TryParse(image.arguments[2], out size))
                    {
                        dropdown.captionText.fontSize = (int)size;
                    }
                    dropdown.captionText.text = image.arguments[3];
                }
                else if (lowerName.StartsWith("l2_"))
                {
                    PSDImportUtility.SetPictureOrLoadColor(image, dropdown.itemText);
                    float size;
                    if (float.TryParse(image.arguments[2], out size))
                    {
                        dropdown.itemText.fontSize = (int)size;
                    }
                    dropdown.itemText.text = image.arguments[3];
                }
                else if (lowerName.StartsWith("m_"))
                {
                    UnityEngine.UI.Image mask = (UnityEngine.UI.Image)toggle.graphic;
                    PSDImportUtility.SetPictureOrLoadColor(image, mask);
                }
                else
                {
                    ctrl.DrawImage(image,node);
                }
            }

            for (int i = 0; i < layer.layers.Length; i++)
            {
                Layer child = layer.layers[i];
                string lowerName = child.name;
                if (lowerName.StartsWith("vb_"))
                {
                    UINode barNode = ctrl.DrawLayer(child, childNode);
                    scrllRect.verticalScrollbar = barNode.InitComponent<Scrollbar>();
                    scrllRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                }
                else
                {
                    ctrl.DrawLayer(child,node);
                }
            }

            return node;
        }
    }
}
