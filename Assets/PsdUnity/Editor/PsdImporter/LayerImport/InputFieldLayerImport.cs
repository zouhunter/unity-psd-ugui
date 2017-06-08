using System;
using UnityEngine;
using UnityEditor;

namespace PSDUnity
{
    internal class InputFieldLayerImport : ILayerImport
    {
        public UINode DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_INPUTFIELD, layer.name, parent);
            UnityEngine.UI.InputField inputfield = node.InitComponent<UnityEngine.UI.InputField>();

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];
                    string lowerName = image.sprite.name.ToLower();

                    if (image.imageType == ImageType.Label)
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