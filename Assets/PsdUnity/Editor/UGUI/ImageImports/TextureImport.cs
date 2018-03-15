using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class TextureImport : IImageImport
    {
        public UGUINode DrawImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.RawIMAGE, image.sprite.name, parent);
            UnityEngine.UI.RawImage pic = node.InitComponent<UnityEngine.UI.RawImage>();
            PSDImporter.SetPictureOrLoadColor(image, pic);
            PSDImporter.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }
    }
}
