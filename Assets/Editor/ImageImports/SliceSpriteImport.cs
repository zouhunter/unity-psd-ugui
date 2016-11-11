using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class SliceSpriteImport : IImageImport
    {
        PSDImportCtrl ctrl;
        public SliceSpriteImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }

        public void DrawImage(Image image, GameObject parent)
        {
            UnityEngine.UI.Image pic = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;

            string commonImagePath = PSDImporterConst.COMMON_BASE_FOLDER + image.name.Replace(".", "/") + PSDImporterConst.PNG_SUFFIX;
            Debug.Log("==  CommonImagePath  ====" + commonImagePath);
            Sprite sprite = AssetDatabase.LoadAssetAtPath(commonImagePath, typeof(Sprite)) as Sprite;
            pic.sprite = sprite;

            UnityEngine.UI.Image myImage = GameObject.Instantiate(pic) as UnityEngine.UI.Image;
            myImage.name = image.name;
            myImage.transform.SetParent(parent.transform, false);//.parent = obj.transform;

            if (image.imageType == ImageType.SliceImage)
            {
                myImage.type = UnityEngine.UI.Image.Type.Sliced;
            }

            RectTransform rectTransform = myImage.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
            rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
        }
    }
}
