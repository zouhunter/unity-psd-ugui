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
        public void DrawImage(Image image, GameObject parent)
        {
            UnityEngine.UI.Image pic = PSDImportUtility.InstantiateItem<UnityEngine.UI.Image>(PSDImporterConst.PREFAB_PATH_IMAGE, image.name, parent);

            string assetPath = "";

            if (image.imageSource == ImageSource.Normal || image.imageSource == ImageSource.Custom)
            {
                assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
              
            }
            else if (image.imageSource == ImageSource.Globle)
            {
                assetPath = PSDImporterConst.Globle_BASE_FOLDER + image.name.Replace(".", "/") + PSDImporterConst.PNG_SUFFIX;
                Debug.Log("==  CommonImagePath  ====" + assetPath);
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;

            if (sprite == null)
            {
                Debug.Log("loading asset at path: " + assetPath +"\nname:" + image.name);
            }

            pic.sprite = sprite;
            pic.type = UnityEngine.UI.Image.Type.Simple;

            RectTransform rectTransform = pic.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
        }
    }
}
