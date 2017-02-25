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
        public void DrawImage(Image image, GameObject parent)
        {
            UnityEngine.UI.Text myText = PSDImportUtility.InstantiateItem<Text>(PSDImporterConst.PREFAB_PATH_TEXT, image.name,parent);

            Color color;
            if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + image.arguments[0]), out color))
            {
                myText.color = color;
            }
            else
            {
                Debug.Log(image.arguments[0]);
            }

            float size;
            if (float.TryParse(image.arguments[2], out size))
            {
                myText.fontSize = (int)size;
            }

            myText.text = image.arguments[3];

            RectTransform rectTransform = myText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(image.size.width + 10, image.size.height + 10);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
        }
    }
}
