using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;



namespace PSDUnity
{
    public class SpriteImport : IImageImport
    {
        public UGUINode DrawImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = null;
            switch (image.type)
            {
                case ImgType.Image:
                    node = DrawNormalImage(image, parent);
                    break;
                case ImgType.Texture:
                    node = DrawRawImage(image, parent);
                    break;
                case ImgType.AtlasImage:
                    node = DrawSliceImage(image, parent);
                    break;
                default:
                    break;
            }
            return node;
        }

        private UGUINode DrawRawImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(PrefabName.PREFAB_RawIMAGE, image.sprite.name, parent);
            UnityEngine.UI.RawImage pic = node.InitComponent<UnityEngine.UI.RawImage>();
            PSDImportUtility.SetPictureOrLoadColor(image, pic);
            PSDImportUtility.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }

        private UGUINode DrawNormalImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(PrefabName.PREFAB_IMAGE, image.sprite.name, parent);
            UnityEngine.UI.Image pic = node.InitComponent<UnityEngine.UI.Image>();
            PSDImportUtility.SetPictureOrLoadColor(image, pic);
            pic.type = UnityEngine.UI.Image.Type.Simple;
            PSDImportUtility.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }

        private UGUINode DrawSliceImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = PSDImportUtility.InstantiateItem(PrefabName.PREFAB_IMAGE, image.sprite.name, parent);
            UnityEngine.UI.Image pic = node.InitComponent<UnityEngine.UI.Image>();
            PSDImportUtility.SetPictureOrLoadColor(image, pic);
            pic.type = UnityEngine.UI.Image.Type.Sliced;
            PSDImportUtility.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }

    }
}
