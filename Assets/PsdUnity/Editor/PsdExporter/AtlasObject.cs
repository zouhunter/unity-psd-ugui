using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace PSDUnity
{
    public class AtlasObject : ScriptableObject
    {
        public string psdFile;
        public string exportPath;
        public string globalPath = "Assets/Common/Images";
        public bool maskAsColor;
        public bool forceSprite;
        public string fileExt = ".png";
        public Vector2 uiSize;
        public PictureExportInfo atlasInfo;
        public RouleObject prefabObj;
        public List<GroupNode1> groups = new List<GroupNode1>();
        internal bool forceMove;
    }
}
