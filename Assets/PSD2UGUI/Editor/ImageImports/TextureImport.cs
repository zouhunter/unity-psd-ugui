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
        public void DrawImage(Image image, GameObject parent)
        {
            UnityEngine.UI.RawImage pic = PSDImportUtility.InstantiateItem<UnityEngine.UI.RawImage>(PSDImporterConst.PREFAB_PATH_RawIMAGE, image.name, parent);

            string assetPath = "";

            if (image.imageSource == ImageSource.Common || image.imageSource == ImageSource.Custom)
            {
                assetPath = PSDImportUtility.baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
            }
            else if (image.imageSource == ImageSource.Globle)
            {
                assetPath = PSDImporterConst.Globle_BASE_FOLDER + image.name.Replace(".", "/") + PSDImporterConst.PNG_SUFFIX;
                Debug.Log("==  CommonImagePath  ====" + assetPath);
            }

            Texture texture = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture)) as Texture;

            pic.texture = texture;
            pic.name = image.name;

            RectTransform rectTransform = pic.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
        }
    }
}
