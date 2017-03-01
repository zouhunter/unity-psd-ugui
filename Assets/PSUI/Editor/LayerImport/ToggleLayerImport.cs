using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class ToggleLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public ToggleLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public void DrawLayer(Layer layer, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_TOGGLE,layer.name,parent);// GameObject.Instantiate(temp) as UnityEngine.UI.Toggle;
            UnityEngine.UI.Toggle toggle = node.GetCompoment<UnityEngine.UI.Toggle>();
            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    Image image = layer.images[imageIndex];
                    string lowerName = image.name.ToLower();
                    if (lowerName.StartsWith("b_"))
                    {
                        if (image.imageSource == ImageSource.Normal || image.imageSource == ImageSource.Custom)
                        {
                            if (image.arguments == null)
                            {
                                string assetPath = PSDImportUtility.GetPicturePath(image);
                                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                                toggle.image.sprite = sprite;
                            }
                            else
                            {
                                PSDImportUtility.TrySetImageColor(image, toggle.targetGraphic);
                            }

                            PSDImportUtility.SetRectTransform(image, toggle.GetComponent<RectTransform>());
                        }
                    }
                    else if (lowerName.StartsWith("m_"))
                    {
                        if (image.imageSource == ImageSource.Normal || image.imageSource == ImageSource.Custom)
                        {
                            if (image.arguments == null)
                            {
                                string assetPath = PSDImportUtility.GetPicturePath(image);
                                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                                toggle.graphic.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                            }
                            else
                            {
                                PSDImportUtility.TrySetImageColor(image, toggle.graphic);
                            }
                        }
                    }
                    else
                    {
                        ctrl.DrawImage(image, node);
                    }
                }
            }
        }
    }
}