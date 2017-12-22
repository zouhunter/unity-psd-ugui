using UnityEngine;
using UnityEngine.UI;
using PSDUnity.Data;
namespace PSDUnity.Import
{
    public class ScrollBarLayerImport : ILayerImport
    {
        public ScrollBarLayerImport()
        {
        }

        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.SCROLLBAR, layer.Name, parent);
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
                    scrollBar.name = layer.Name;
                }
                else if (lowerName.StartsWith("h_"))
                {
                    graph = scrollBar.handleRect.GetComponent<UnityEngine.UI.Image>();
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
