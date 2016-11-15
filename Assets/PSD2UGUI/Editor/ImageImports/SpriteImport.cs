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
            if (image.imageSource == ImageSource.Common || image.imageSource == ImageSource.Custom)
            {
                UnityEngine.UI.Image pic =  PSDImportUtility.InstantiateItem<UnityEngine.UI.Image>(PSDImporterConst.PREFAB_PATH_IMAGE, image.name,parent);

                string assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;

                if (sprite == null)
                {
                    Debug.Log("loading asset at path: " + PSDImportUtility.baseDirectory + image.name);
                }

                pic.sprite = sprite;

                RectTransform rectTransform = pic.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
            }
            else if (image.imageSource == ImageSource.Globle)
            {
                UnityEngine.UI.Image pic = PSDImportUtility.InstantiateItem<UnityEngine.UI.Image>(PSDImporterConst.PREFAB_PATH_IMAGE, image.name,parent);

                string commonImagePath = PSDImporterConst.Globle_BASE_FOLDER + image.name.Replace(".", "/") + PSDImporterConst.PNG_SUFFIX;
                Debug.Log("==  CommonImagePath  ====" + commonImagePath);
                Sprite sprite = AssetDatabase.LoadAssetAtPath(commonImagePath, typeof(Sprite)) as Sprite;
                pic.sprite = sprite;

                pic.name = image.name;

                if (image.imageType == ImageType.SliceImage)
                {
                    pic.type = UnityEngine.UI.Image.Type.Sliced;
                }

                RectTransform rectTransform = pic.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
            }
        }
    }
}
