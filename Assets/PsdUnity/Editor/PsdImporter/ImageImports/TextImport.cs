using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUnity
{
    public class TextImport : IImageImport
    {
        public UINode DrawImage(Image image, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_TEXT, image.sprite.name, parent);
            UnityEngine.UI.Text myText = node.InitComponent<Text>();
            PSDImportUtility.SetPictureOrLoadColor(image, myText);
            RectTransform rectTransform = myText.GetComponent<RectTransform>();
            AdjustImage(image, myText.fontSize);
            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
            return node;
        }
        /// <summary>
        /// 调节字边距
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private void AdjustImage(Image image,int fontSize)
        {
            image.size.width += 40;
            image.size.height += 20;
        }
    }
}
