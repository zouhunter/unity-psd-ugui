using System;
using UnityEngine;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class PanelLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public bool createMask;
        public PanelLayerImport(PSDImportCtrl ctrl,bool createMask)
        {
            this.ctrl = ctrl;
            this.createMask = createMask;
        }

        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.EMPTY, layer.displayName, parent);//GameObject.Instantiate(temp) as UnityEngine.UI.Image;
            UnityEngine.UI.Graphic graph = null;

            if (layer.children!=null)
                ctrl.DrawLayers(layer.children.ConvertAll(x=>x as GroupNode).ToArray(), node);//子节点

            for (int i = 0; i < layer.images.Count; i++)
            {
                ImgNode image = layer.images[i];

                if (image.Name.ToLower().StartsWith("b_"))
                {
                    if(image.type == ImgType.Texture)
                    {
                        graph = node.InitComponent<UnityEngine.UI.RawImage>();
                    }
                    else
                    {
                        graph = node.InitComponent<UnityEngine.UI.Image>();
                    }
                    PSDImporter.SetPictureOrLoadColor(image, graph);
                    PSDImporter.SetRectTransform(image, graph.GetComponent<RectTransform>());
                    graph.name = layer.displayName;
                }
                else
                {
                    ctrl.DrawImage(image, node);
                }
            }
            if (graph == null && createMask)
            {
                graph = node.InitComponent<UnityEngine.UI.Image>();
                PSDImporter.SetRectTransform(layer, graph.GetComponent<RectTransform>());
                Color color;
                if (ColorUtility.TryParseHtmlString("#FFFFFF01", out color))
                {
                    graph.color = color;
                }
                graph.name = layer.displayName;
            }
            return node;

        }

    }
}