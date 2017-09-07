using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using PSDUnity.Data;
namespace PSDUnity.Exprot
{
    public class SettingObject:ScriptableObject
    {
        public string globalPath = "Assets/Common/Images";
        public bool maskAsColor;
        public bool forceSprite;
        public string fileExt = ".png";
        public Vector2 uiSize = new Vector2(1600, 900);
        public string picNameTemp = "{0}.png";
        public float pixelsToUnitSize = 1;
        public ImgType defultImgType = ImgType.AtlasImage;
        public int atlassize = 1024;
        public void Reset()
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(this);
            Debug.Log(UnityEditor.AssetDatabase.AssetPathToGUID(path));
        }

    }
}
