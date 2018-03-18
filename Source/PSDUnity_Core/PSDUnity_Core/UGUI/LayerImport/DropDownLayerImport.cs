using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
namespace PSDUnity.UGUI
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
            UGUINode node = PSDImporter.InstantiateItem(GroupType.DROPDOWN, layer.displayName, parent);
            Dropdown dropdown = node.InitComponent<Dropdown>();
            PSDImporter.SetRectTransform(layer, dropdown.GetComponent<RectTransform>());
            ScrollRect scrllRect = dropdown.template.GetComponent<ScrollRect>();
            RectTransform content = scrllRect.content;
            Toggle toggle = content.GetComponentInChildren<Toggle>();

            UGUINode childNode = new UGUINode(dropdown.template, node);
            childNode.transform.SetParent(PSDImporter.canvas.transform);
            childNode.anchoType = AnchoType.Down | AnchoType.XStretch;
            //由于设置相对坐标需要，所以修改了部分预制体的状态
            childNode.inversionReprocess += () => {
                RectTransform rt = childNode.InitComponent<RectTransform>();
                rt.pivot = new Vector2(0.5f, 1);
                rt.anchoredPosition = Vector3.zero;
            };
            for (int i = 0; i < layer.images.Count; i++)
            {
                ImgNode image = layer.images[i];
                string lowerName = image.Name.ToLower();
                if (lowerName.StartsWith("b1_"))
                {
                    PSDImporter.SetPictureOrLoadColor(image, dropdown.image);
                    PSDImporter.SetRectTransform(image, dropdown.GetComponent<RectTransform>());
                    dropdown.name = layer.displayName;
                }
                else if(lowerName.StartsWith("b2_"))
                {
                    PSDImporter.SetPictureOrLoadColor(image, dropdown.template.GetComponent<Graphic>());
                    PSDImporter.SetRectTransform(image, dropdown.template);
                }
                else if (lowerName.StartsWith("b3_"))
                {
                    UnityEngine.UI.Image itemimage = (UnityEngine.UI.Image)toggle.targetGraphic;
                    PSDImporter.SetPictureOrLoadColor(image, itemimage);
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, image.rect.height);
                }
                else if (lowerName.StartsWith("t1_"))
                {
                    PSDImporter.SetPictureOrLoadColor(image, dropdown.captionText);
                }
                else if (lowerName.StartsWith("t2_"))
                {
                    PSDImporter.SetPictureOrLoadColor(image, dropdown.itemText);
                }
                else if (lowerName.StartsWith("m_"))
                {
                    UnityEngine.UI.Image mask = (UnityEngine.UI.Image)toggle.graphic;
                    PSDImporter.SetPictureOrLoadColor(image, mask);
                }
                else
                {
                    ctrl.DrawImage(image,node);
                }
            }

            if(layer.children != null)
            {
                for (int i = 0; i < layer.children.Count; i++)
                {
                    GroupNode child = layer.children[i] as GroupNode;
                    string lowerName = child.displayName;
                    if (lowerName.StartsWith("vb_"))
                    {
                        UGUINode barNode = ctrl.DrawLayer(child, childNode);
                        scrllRect.verticalScrollbar = barNode.InitComponent<Scrollbar>();
                        scrllRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                        barNode.anchoType = AnchoType.Right | AnchoType.YCenter;
                    }
                    else
                    {
                        ctrl.DrawLayer(child, node);
                    }
                }
            }

            return node;
        }
    }
}
