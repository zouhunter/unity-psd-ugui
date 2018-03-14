using System;
using UnityEngine;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class PanelLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public PanelLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }

        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.EMPTY, layer.Name, parent);//GameObject.Instantiate(temp) as UnityEngine.UI.Image;
            UnityEngine.UI.Graphic panel = null;

            if (layer.children!=null)
                ctrl.DrawLayers(layer.children.ConvertAll(x=>x as GroupNode).ToArray(), node);//子节点

            for (int i = 0; i < layer.images.Count; i++)
            {
                ImgNode image = layer.images[i];

                if (image.Name.ToLower().StartsWith("b_"))
                {
                    if(image.type == ImgType.Texture)
                    {
                        panel = node.InitComponent<UnityEngine.UI.RawImage>();
                    }
                    else
                    {
                        panel = node.InitComponent<UnityEngine.UI.Image>();
                    }

                    PSDImporter.SetPictureOrLoadColor(image, panel);
                    PSDImporter.SetRectTransform(image, panel.GetComponent<RectTransform>());
                    panel.name = layer.Name;
                }
                else
                {
                    ctrl.DrawImage(image, node);
                }
            }
            if (panel == null)
            {
                panel = node.InitComponent<UnityEngine.UI.Image>();
                PSDImporter.SetRectTransform(layer, panel.GetComponent<RectTransform>());
                Color color;
                if (ColorUtility.TryParseHtmlString("#FFFFFF01", out color))
                {
                    panel.color = color;
                }
                panel.name = layer.Name;
            }
            return node;

        }

    }
}