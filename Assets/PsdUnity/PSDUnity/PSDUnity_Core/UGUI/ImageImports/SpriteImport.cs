using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
namespace PSDUnity.UGUI
{
    public class SpriteImport : ImageImport
    {
        public SpriteImport(PSDImportCtrl ctrl) : base(ctrl) { }

        public override GameObject CreateTemplate()
        {
           return new GameObject("Image", typeof(Image));
        }

        public override UGUINode DrawImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = CreateRootNode(image.Name,image.rect, parent);
            UnityEngine.UI.Image pic = node.InitComponent<UnityEngine.UI.Image>();
            pic.type = UnityEngine.UI.Image.Type.Simple;
            PSDImporterUtility.SetPictureOrLoadColor(image, pic);
            return node;
        }
    }
    
}
