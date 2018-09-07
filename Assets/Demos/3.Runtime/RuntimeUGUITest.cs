using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSDUnity;
using PSDUnity.UGUI;
using PSDUnity.Data;

public class RuntimeUGUITest : MonoBehaviour {
    public RuleObject rule;
    public Exporter exporter;
    public Canvas canvas;
    private void Awake()
    {
        var ctrl = PSDImporterUtility.CreatePsdImportCtrlSafty(rule, new Vector2(1600, 900), canvas);
        var tree = PSDImporterUtility.ListToTree(exporter.groups);
        ctrl.Import(tree);
    }
}
