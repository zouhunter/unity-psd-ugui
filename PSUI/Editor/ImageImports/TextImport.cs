using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class TextImport : IImageImport
    {
        public UINode DrawImage(Image image, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_TEXT, image.name,parent);
            UnityEngine.UI.Text myText = node.InitComponent<Text>();
            PSDImportUtility.SetPictureOrLoadColor(image, myText);

            float size;
            if (float.TryParse(image.arguments[2], out size))
            {
                myText.fontSize = Mathf.CeilToInt(size);
            }

            myText.text = image.arguments[3];

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
            image.size.width += fontSize;
            image.size.height += fontSize * 0.5f;
        }
    }
}
