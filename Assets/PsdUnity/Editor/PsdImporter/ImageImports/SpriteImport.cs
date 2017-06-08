using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;



namespace PSDUIImporter
{
    public class SpriteImport : IImageImport
    {
        public UINode DrawImage(Image image, UINode parent)
        {
            UINode node = null;
            switch (image.imageType)
            {
                case ImageType.Image:
                    node = DrawNormalImage(image, parent);
                    break;
                case ImageType.Texture:
                    node = DrawRawImage(image, parent);
                    break;
                case ImageType.SliceImage:
                    node = DrawSliceImage(image, parent);
                    break;
                default:
                    break;
            }
            return node;
        }

        private UINode DrawRawImage(Image image, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_RawIMAGE, image.name, parent);
            UnityEngine.UI.RawImage pic = node.InitComponent<UnityEngine.UI.RawImage>();
            PSDImportUtility.SetPictureOrLoadColor(image, pic);
            PSDImportUtility.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }

        private UINode DrawNormalImage(Image image, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_IMAGE, image.name, parent);
            UnityEngine.UI.Image pic = node.InitComponent<UnityEngine.UI.Image>();
            PSDImportUtility.SetPictureOrLoadColor(image, pic);
            pic.type = UnityEngine.UI.Image.Type.Simple;
            PSDImportUtility.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }

        private UINode DrawSliceImage(Image image, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_IMAGE, image.name, parent);
            UnityEngine.UI.Image pic = node.InitComponent<UnityEngine.UI.Image>();
            PSDImportUtility.SetPictureOrLoadColor(image, pic);
            pic.type = UnityEngine.UI.Image.Type.Sliced;
            PSDImportUtility.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }

    }
}
