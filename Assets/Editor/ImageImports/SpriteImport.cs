using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;



namespace PSDUIImporter
{
    public class SpriteImport : IImageImport
    {
        PSDImportCtrl ctrl;
        public SpriteImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }

        public void DrawImage(Image image, GameObject parent)
        {
            if (image.imageSource == ImageSource.Custom)
            {
                UnityEngine.UI.Image pic = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;

                string assetPath = ctrl.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;

                if (sprite == null)
                {
                    Debug.Log("loading asset at path: " + ctrl.baseDirectory + image.name);
                }

                pic.sprite = sprite;

                UnityEngine.UI.Image myImage = GameObject.Instantiate(pic) as UnityEngine.UI.Image;
                myImage.name = image.name;
                myImage.transform.SetParent(parent.transform, false);//.parent = obj.transform;

                RectTransform rectTransform = myImage.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
            }
            else
            {

            }
        }
    }
}
