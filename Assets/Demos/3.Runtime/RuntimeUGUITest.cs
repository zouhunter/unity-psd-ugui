using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSDUnity;
using PSDUnity.UGUI;
using PSDUnity.Data;
using Ntreev.Library.Psd;
using System;

public class RuntimeUGUITest : MonoBehaviour {
    public string psdPath;
    public RuleObject rule;
    public Canvas canvas;
    private PsdDocument psdDocument;

    private void OnGUI()
    {
        if(GUILayout.Button("LoadFromPSD"))
        {
            LoadPsdFile();
        }
    }

    private void LoadPsdFile()
    {
        var fullPath = Application.streamingAssetsPath + "/" + psdPath;
        using (psdDocument = PsdDocument.Create(fullPath))
        {
            var tree = PSDUnity.Runtime.RuntimeExportUtility.CreateTree(rule, psdDocument, new Vector2(1600, 900), true);
            var ctrl = PSDImporterUtility.CreatePsdImportCtrlSafty(rule, new Vector2(1600, 900), canvas);
            ctrl.Import(tree);
            UpdatePanel(ctrl.uinode);
        }
    }

    private void UpdatePanel(UGUINode uinode)
    {
        if(uinode != null && uinode.childs != null && uinode.childs.Count > 0)
        {
            var panel = uinode.childs[0].transform .gameObject;
            panel.gameObject.SetActive(false);
            StartCoroutine(DelyActive(panel));
        }

    }

    private IEnumerator DelyActive(GameObject panel)
    {
        yield return new WaitForFixedUpdate();
        panel.gameObject.SetActive(true);
    }
}
