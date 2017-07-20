using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using PSDUnity.Data;
using UnityEditor;

namespace PSDUnity.Exprot
{
    public class AtlasObject : ScriptableObject
    {
        public string psdFile;
        public string exportPath;
        public string globalPath = "Assets/Common/Images";
        public bool maskAsColor;
        public bool forceSprite;
        public string fileExt = ".png";
        public Vector2 uiSize = new Vector2(1600,900);
        public PictureExportInfo atlasInfo;
        public RouleObject prefabObj;
        public List<GroupNode1> groups = new List<GroupNode1>();
        //public bool forceMove;

        public void Reset()
        {
            if (prefabObj == null)
            {
                var path = AssetDatabase.GUIDToAssetPath("993779c0603e6564db4973f34e385cc1");
                prefabObj = AssetDatabase.LoadAssetAtPath<RouleObject>(path);
            }
        }
    }
}
