using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using PSDUnity;
using PSDUnity.PSD2UGUI;

public class demo5 : MonoBehaviour {
    public AtlasObject atlasObj;
    public Canvas canvas;
    private void Start()
    {
        var rule = atlasObj.ruleObj;
        var defultUiSize = atlasObj.settingObj.defultUISize;
        var groupNodes = atlasObj.groups.ToArray();

        PSDImporter.InitEnviroment(rule, defultUiSize,canvas);
        PSDImporter.StartBuild(groupNodes);
    }
}
