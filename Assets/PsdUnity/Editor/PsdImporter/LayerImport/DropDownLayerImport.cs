using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUnity
{
    public class DropDownLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public DropDownLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(PrefabName.PREFAB_DROPDOWN, layer.name, parent);
            Dropdown dropdown = node.InitComponent<Dropdown>();
            ScrollRect scrllRect = dropdown.template.GetComponent<ScrollRect>();
            RectTransform content = scrllRect.content;
            Toggle toggle = content.GetComponentInChildren<Toggle>();

            UGUINode childNode = new UGUINode(dropdown.template, node);
            childNode.transform.SetParent(PSDImportUtility.canvas.transform);
            childNode.anchoType = UGUINode.AnchoType.Down | UGUINode.AnchoType.XStretch;
            //由于设置相对坐标需要，所以修改了部分预制体的状态
            childNode.ReprocessEvent = () => {
                RectTransform rt = childNode.InitComponent<RectTransform>();
                rt.pivot = new Vector2(0.5f, 1);
                rt.anchoredPosition = Vector3.zero;
            };
            for (int i = 0; i < layer.images.Count; i++)
            {
                ImgNode image = layer.images[i];
                string lowerName = image.clampname.ToLower();
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
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, image.rect.height);
                }
                else if (lowerName.StartsWith("t1_"))
                {
                    PSDImportUtility.SetPictureOrLoadColor(image, dropdown.captionText);
                }
                else if (lowerName.StartsWith("t2_"))
                {
                    PSDImportUtility.SetPictureOrLoadColor(image, dropdown.itemText);
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

            for (int i = 0; i < layer.groups.Count; i++)
            {
                GroupNode child = layer.groups[i] as GroupNode;
                string lowerName = child.name;
                if (lowerName.StartsWith("vb_"))
                {
                    UGUINode barNode = ctrl.DrawLayer(child, childNode);
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
