using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using Ntreev.Library.Psd;
using System.Linq;
using System.IO;
using System;
using NUnit.Framework;

public class NewEditorTest {

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
    public void DifferenceSprite()
    {
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath("Assets/atlas.png").OfType<Sprite>().ToArray();
        foreach (var item in sprites)
        {
            Debug.Log(item.name);
            Debug.Log(item.GetHashCode());
        }
    }
}
