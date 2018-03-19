using UnityEngine;
using UnityEngine.UI;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class ScrollBarLayerImport : ILayerImport
    {
        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.SCROLLBAR, layer.displayName, parent);
            Scrollbar scrollBar = node.InitComponent<Scrollbar>();

            switch (layer.direction)
            {
                case Direction.LeftToRight:
                    scrollBar.direction = Scrollbar.Direction.LeftToRight;
                    break;
                case Direction.BottomToTop:
                    scrollBar.direction = Scrollbar.Direction.BottomToTop;
                    break;
                case Direction.TopToBottom:
                    scrollBar.direction = Scrollbar.Direction.TopToBottom;
                    break;
                case Direction.RightToLeft:
                    scrollBar.direction = Scrollbar.Direction.RightToLeft;
                    break;
                default:
                    if(layer.rect.width > layer.rect.height)
                    {
                        scrollBar.direction = Scrollbar.Direction.LeftToRight;
                    }
                    else
                    {
                        scrollBar.direction = Scrollbar.Direction.BottomToTop;
                    }
                    break;
            }

            for (int i = 0; i < layer.images.Count; i++)
            {
                ImgNode image = layer.images[i];
                string lowerName = image.Name.ToLower();
                UnityEngine.UI.Image graph = null;

                if (lowerName.StartsWith("b_"))
                {
                    graph = scrollBar.GetComponent<UnityEngine.UI.Image>();
                    PSDImporter.SetRectTransform(image, scrollBar.GetComponent<RectTransform>());
                    scrollBar.name = layer.displayName;
                }
                else if (lowerName.StartsWith("h_"))
                {
                    graph = scrollBar.handleRect.GetComponent<UnityEngine.UI.Image>();
                    node.inversionReprocess += () =>
                    {
                        PSDImporter.SetCustomAnchor(scrollBar.GetComponent<RectTransform>(),scrollBar.handleRect);
                    };
                }
                if (graph == null)
                {
                    //忽略Scorllbar其他的artLayer
                    continue;
                }

                PSDImporter.SetPictureOrLoadColor(image, graph);
            }
            return node;
        }
    }
}
