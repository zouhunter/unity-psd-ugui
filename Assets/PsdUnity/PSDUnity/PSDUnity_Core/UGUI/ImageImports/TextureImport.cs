using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class TextureImport : ImageImport
    {
        public override GameObject CreateTemplate()
        {
           return new GameObject("RawImage", typeof(RawImage));
        }

        public override UGUINode DrawImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = CreateRootNode(image.Name, image.rect, parent);
            UnityEngine.UI.RawImage pic = node.InitComponent<UnityEngine.UI.RawImage>();
            PSDImporterUtility.SetPictureOrLoadColor(image, pic);
            return node;
        }
    }
}
