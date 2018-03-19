using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class SpriteImport : IImageImport
    {
        public UGUINode DrawImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.IMAGE, image.Name, parent);
            UnityEngine.UI.Image pic = node.InitComponent<UnityEngine.UI.Image>();
            PSDImporter.SetPictureOrLoadColor(image, pic);
            pic.type = UnityEngine.UI.Image.Type.Simple;
            PSDImporter.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }
    }
    
}
