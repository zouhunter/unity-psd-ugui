using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using Ntreev.Library.Psd;
using System.Linq;
using System.IO;
using System;
using NUnit.Framework;
using SyntaxTree.VisualStudio.Unity.Bridge;
using SyntaxTree.VisualStudio.Unity;

public class PsdUnityTest
{

    [Test]
    public void PrintDefultPrefabsGUID()
    {
        GameObject[] items = Resources.LoadAll<GameObject>("Prefabs");
        var str = "";
        foreach (var item in items)
        {
            var path = AssetDatabase.GetAssetPath(item);
            str += item.name + "-->" + AssetDatabase.AssetPathToGUID(path) + "\n";
        }
        Debug.Log(str);
    }
    [Test]
    public void DifferenceIDOfString()
    {
        string str1 = "DifferenceIDOfString";
        string str2 = "DifferenceIDOfString";

        Debug.Log(str1.GetHashCode());
        Debug.Log(str2.GetHashCode());
    }
    [Test]
    public void SyntaxTreeTest()
    {
        Project.BuildProject();
    }
}
