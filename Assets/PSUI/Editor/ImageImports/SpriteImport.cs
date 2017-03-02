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
        public UINode DrawImage(Image image, UINode parent)
        {
            UINode node = null;
            switch (image.imageType)
            {
                case ImageType.Image:
                    node = DrawNormalImage(image, parent);
                    break;
                case ImageType.Texture:
                    node = DrawRawImage(image, parent);
                    break;
                case ImageType.SliceImage:
                    node = DrawSliceImage(image, parent);
                    break;
                default:
                    break;
            }
            return node;
        }

        private UINode DrawRawImage(Image image, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_RawIMAGE, image.name, parent);
            UnityEngine.UI.RawImage pic = node.GetComponent<UnityEngine.UI.RawImage>();
            string assetPath = PSDImportUtility.GetPicturePath(image);
            Texture texture = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture)) as Texture;
            pic.texture = texture;

            if (texture == null)
            {
                Debug.Log("loading asset at path: " + assetPath + "\nname:" + image.name);
                if (image.arguments != null && image.arguments.Length > 0)
                {
                    Debug.Log(image.arguments[0]);
                    Color color;
                    if (ColorUtility.TryParseHtmlString(image.arguments[0], out color))
                    {
                        pic.color = color;
                    }
                }
            }

            PSDImportUtility.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }

        private UINode DrawNormalImage(Image image, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_IMAGE, image.name, parent);
            UnityEngine.UI.Image pic = node.GetComponent<UnityEngine.UI.Image>();

            string assetPath = PSDImportUtility.GetPicturePath(image);

            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;

            if (sprite == null)
            {
                Debug.Log("loading asset at path: " + assetPath + "\nname:" + image.name);
                if (image.arguments != null && image.arguments.Length > 0)
                {
                    Debug.Log(image.arguments[0]);
                    Color color;
                    if (ColorUtility.TryParseHtmlString(image.arguments[0], out color))
                    {
                        pic.color = color;
                    }
                }
            }

            pic.sprite = sprite;

            pic.type = UnityEngine.UI.Image.Type.Simple;

            PSDImportUtility.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;
        }

        private UINode DrawSliceImage(Image image, UINode parent)
        {
            UINode node = PSDImportUtility.InstantiateItem(PSDImporterConst.PREFAB_PATH_IMAGE, image.name, parent);
            UnityEngine.UI.Image pic = node.GetComponent<UnityEngine.UI.Image>();
            string assetPath = PSDImportUtility.GetPicturePath(image);

            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;

            pic.sprite = sprite;

            pic.type = UnityEngine.UI.Image.Type.Sliced;

            PSDImportUtility.SetRectTransform(image, pic.GetComponent<RectTransform>());
            return node;

        }

    }
}
