using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class TextureImport : IImageImport
    {
        public void DrawImage(PsImage image, GameObject parent)
        {
            UnityEngine.UI.Image pic = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
            pic.sprite = null;
            UnityEngine.UI.Image myImage = GameObject.Instantiate(pic) as UnityEngine.UI.Image;
            myImage.name = image.name;
            myImage.transform.SetParent(parent.transform, false);//.parent = obj.transform;

            RectTransform rectTransform = myImage.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
        }
    }
}
