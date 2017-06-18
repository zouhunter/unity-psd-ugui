using System;
using UnityEngine;
using UnityEditor;

namespace PSDUnity
{
    internal class InputFieldLayerImport : ILayerImport
    {
        public UGUINode DrawLayer(GroupNode layer, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(GroupType.INPUTFIELD, layer.Name, parent);
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
                            PSDImportUtility.SetPictureOrLoadColor(image, text);
                        }
                        else if (lowerName.StartsWith("p_"))
                        {
                            UnityEngine.UI.Text text = (UnityEngine.UI.Text)inputfield.placeholder;//.transform.Find("Placeholder").GetComponent<UnityEngine.UI.Text>();
                            PSDImportUtility.SetPictureOrLoadColor(image, text);
                        }
                    }
                    else
                    {
                        if (lowerName.StartsWith("b_"))
                        {
                            PSDImportUtility.SetPictureOrLoadColor(image, inputfield.image);
                            PSDImportUtility.SetRectTransform(image,inputfield.GetComponent<RectTransform>());
                        }
                    }
                }
            }
            return node;
        }
    }
}