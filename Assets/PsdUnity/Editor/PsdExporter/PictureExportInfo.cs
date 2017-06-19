using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class PictureExportInfo
{
    public string atlasName = "atlas.png";
    public string picNameTemp = "{0}.png";
    public string exportPath = "Assets";
    public float pixelsToUnitSize = 1;
    public int atlassize = 1024;
}
