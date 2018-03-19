using System;
using UnityEngine;
using PSDUnity;
namespace PSDUnity.UGUI
{
    internal class InputFieldLayerImport : ILayerImport
    {
        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.INPUTFIELD, layer.displayName, parent);
            UnityEngine.UI.InputField inputfield = node.InitComponent<UnityEngine.UI.InputField>();

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Count; imageIndex++)
                {
                    ImgNode image = layer.images[imageIndex];
                    string lowerName = image.Name.ToLower();

                    if (image.type == ImgType.Label)
                    {
                        if (lowerName.StartsWith("t_"))
                        {
                            UnityEngine.UI.Text text = (UnityEngine.UI.Text)inputfield.textComponent;//inputfield.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
                            text.alignment = TextAnchor.MiddleLeft;
                            PSDImporter.SetPictureOrLoadColor(image, text);
                        }
                        else if (lowerName.StartsWith("p_"))
                        {
                            UnityEngine.UI.Text text = (UnityEngine.UI.Text)inputfield.placeholder;//.transform.Find("Placeholder").GetComponent<UnityEngine.UI.Text>();
                            text.alignment = TextAnchor.MiddleLeft;
                            PSDImporter.SetPictureOrLoadColor(image, text);
                        }
                    }
                    else
                    {
                        if (lowerName.StartsWith("b_"))
                        {
                            PSDImporter.SetPictureOrLoadColor(image, inputfield.image);
                            PSDImporter.SetRectTransform(image,inputfield.GetComponent<RectTransform>());
                        }
                    }
                }
            }
            return node;
        }
    }
}