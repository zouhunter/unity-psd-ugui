using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;

namespace PSDUnity.UGUI
{
    public class TextImport : ImageImport
    {
        public TextImport(PSDImportCtrl ctrl) : base(ctrl) { }

        public override GameObject CreateTemplate()
        {
            var text = new GameObject("Text", typeof(Text)).GetComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            return text.gameObject;
        }

        public override UGUINode DrawImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = CreateRootNode(image.Name, AdjustTextRect( image.rect,image.fontSize), parent);
            UnityEngine.UI.Text myText = node.InitComponent<Text>();
            PSDImporterUtility.SetPictureOrLoadColor(image, myText);
            return node;
        }
     
        /// <summary>
        /// 调节字边距
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private Rect AdjustTextRect(Rect oldRect,int fontSize)
        {
            var rect = oldRect;
            rect.width += fontSize;
            rect.height += fontSize;
            return rect;
        }
    }
}
