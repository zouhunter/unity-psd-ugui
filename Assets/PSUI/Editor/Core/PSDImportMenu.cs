using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor.SceneManagement;

namespace PSDUIImporter
{
    //------------------------------------------------------------------------------
    // class definition
    //------------------------------------------------------------------------------
    public class PSDImportMenu : Editor
    {

        [MenuItem("Assets/Create/PSDImport ...")]
        static public void ImportHogSceneMenuItem()
        {
            string startPath = null;
            if (Selection.activeObject != null)
            {
                startPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            }
            if (startPath != null && startPath.EndsWith("xml"))
            {
                PSDImportCtrl import = new PSDUIImporter.PSDImportCtrl(startPath);
                import.BeginDrawUILayers();
                import.BeginSetUIParents(PSDImportUtility.uinode);
                import.BeginSetAnchers(PSDImportUtility.uinode.childs[0]);
                var rt = PSDImportUtility.uinode.childs[0].GetComponent<RectTransform>();
                PSDImportUtility.SetCustomAnchor(rt, rt);
            }
            
            GC.Collect();
        }
    }
}
