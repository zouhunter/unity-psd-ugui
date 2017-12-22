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
using PSDUnity.Exprot;
using PSDUnity.Import;

public class demo5 : MonoBehaviour {
    public AtlasObject atlasObj;

    private void Start()
    {
        PSDImportUtility.InitEnviroment(atlasObj.prefabObj, atlasObj.settingObj.uiSize);
        PSDImportCtrl import = new PSDImportCtrl();
        import.Import(atlasObj.groups.ToArray(), atlasObj.settingObj.uiSize);
    }
}
