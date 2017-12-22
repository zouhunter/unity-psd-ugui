using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity.Data;
namespace PSDUnity.Import
{
    public class TextImport : IImageImport
    {
        public UGUINode DrawImage(ImgNode image, UGUINode parent)
        {
            UGUINode node = PSDImporter.InstantiateItem(GroupType.TEXT, image.Name, parent);
            UnityEngine.UI.Text myText = node.InitComponent<Text>();
            PSDImporter.SetPictureOrLoadColor(image, myText);
            RectTransform rectTransform = myText.GetComponent<RectTransform>();
            AdjustImage(image, myText.fontSize);
            rectTransform.sizeDelta = new Vector2(image.rect.width, image.rect.height);
            rectTransform.anchoredPosition = new Vector2(image.rect.x, image.rect.y);
            return node;
        }
        /// <summary>
        /// 调节字边距
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private void AdjustImage(ImgNode image,int fontSize)
        {
            image.rect.width += fontSize;
            image.rect.height += fontSize;
        }
    }
}
