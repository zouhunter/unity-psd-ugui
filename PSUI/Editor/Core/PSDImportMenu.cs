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
                //最外层的要单独处理
                var rt = PSDImportUtility.uinode.childs[0].InitComponent<RectTransform>();
                PSDImportUtility.SetCustomAnchor(rt, rt);
                import.BeginReprocess(PSDImportUtility.uinode.childs[0]);//后处理
            }
            
            GC.Collect();
        }
    }
}
