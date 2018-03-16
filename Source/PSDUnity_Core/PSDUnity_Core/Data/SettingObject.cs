using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace PSDUnity
{
    public class SettingObject:ScriptableObject
    {
        public string globalPath = "Assets/Common/Images";
        public bool maskAsColor = false;
        public bool forceSprite = true;
        public string fileExt = ".png";
        public Vector2 defultUISize = new Vector2(1600, 900);
        public string picNameTemp = "{0}.png";
        public float pixelsToUnitSize = 100;
        public int maxSize = 4096;
    }
}
