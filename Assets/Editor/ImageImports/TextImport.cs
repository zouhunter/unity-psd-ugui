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
        PSDImportCtrl ctrl;
        public TextImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }

        public void DrawImage(Image image, GameObject parent)
        {
            UnityEngine.UI.Text text = Resources.Load(PSDImporterConst.PREFAB_PATH_TEXT, typeof(UnityEngine.UI.Text)) as UnityEngine.UI.Text;

            UnityEngine.UI.Text myText = GameObject.Instantiate(text) as UnityEngine.UI.Text;

            //                        myText.color = image.arguments[0];
            //                        myText.font = image.arguments[1];
            Debug.Log("Label Color : " + image.arguments[0]);
            Debug.Log("fontSize : " + image.arguments[2]);

            Color color;
            if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + image.arguments[0]), out color))
            {
                myText.color = color;
            }

            int size;
            if (int.TryParse(image.arguments[2], out size))
            {
                myText.fontSize = size;
            }

            myText.text = image.arguments[3];
            myText.transform.SetParent(parent.transform, false);//.parent = obj.transform;

            RectTransform rectTransform = myText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
        }
    }
}
