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
using UnityEditor;

namespace PSDUnity
{
    public class PrefabObject : ScriptableObject
    {
        public GameObject PREFAB_BUTTON;
        public GameObject PREFAB_CANVAS;
        public GameObject PREFAB_DROPDOWN;
        public GameObject PREFAB_EMPTY;
        public GameObject PREFAB_GRID;
        public GameObject PREFAB_GROUP;
        public GameObject PREFAB_IMAGE;
        public GameObject PREFAB_INPUTFIELD;
        public GameObject PREFAB_RawIMAGE;
        public GameObject PREFAB_SCROLLBAR;
        public GameObject PREFAB_SCROLLVIEW;
        public GameObject PREFAB_SLIDER;
        public GameObject PREFAB_TEXT;
        public GameObject PREFAB_TOGGLE;

        public PrefabObject()
        {
            var path = AssetDatabase.GUIDToAssetPath("9a94c0cbaca3a48468b0b1e51fefcbfb");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_BUTTON = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("dec381fc4475a4a33bcfd41351b26fcc");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_CANVAS = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("d6eef6649ab97db4f900c0f6a896a73e");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_DROPDOWN = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("a47d3ef1adf404641b6d23a93e59ee53");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_EMPTY = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("25904f280417ef748977530a357b5fb0");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_GROUP = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("ed417986c539a4fc7b09f220ff4ceef6");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_GRID = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("9d52d80f2b2a64fbe96cd04604170d32");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_IMAGE = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("2e48a2d5b2f2ccb439d0cf1eeba3e6dd");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_INPUTFIELD = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("b22710b4e94fd4947ae6eebd62b8ac32");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_RawIMAGE = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("2b101ae8ff3e18e4eb5aca168965d85c");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_SCROLLBAR = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("ca7d801212b074407a4fc212216da328");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_SCROLLVIEW = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("95fdb08286306794583b6e1d19021aaa");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_SLIDER = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("0f5f099613ddb4155b4c775d8db5984d");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_TEXT = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            path = AssetDatabase.GUIDToAssetPath("f4c3d75106f0b944faa6f14731cec415");
            if (!string.IsNullOrEmpty(path))
            {
                PREFAB_TOGGLE = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
        }
    }
    /*
Button-->9a94c0cbaca3a48468b0b1e51fefcbfb
Canvas-->dec381fc4475a4a33bcfd41351b26fcc
Dropdown-->d6eef6649ab97db4f900c0f6a896a73e
Empty-->a47d3ef1adf404641b6d23a93e59ee53
Grid-->ed417986c539a4fc7b09f220ff4ceef6
Group-->25904f280417ef748977530a357b5fb0
Image-->9d52d80f2b2a64fbe96cd04604170d32
InputField-->2e48a2d5b2f2ccb439d0cf1eeba3e6dd
RawImage-->b22710b4e94fd4947ae6eebd62b8ac32
Scrollbar-->2b101ae8ff3e18e4eb5aca168965d85c
ScrollView-->ca7d801212b074407a4fc212216da328
Slider-->95fdb08286306794583b6e1d19021aaa
Text-->0f5f099613ddb4155b4c775d8db5984d
Toggle-->f4c3d75106f0b944faa6f14731cec415


    GameObject[] items = Resources.LoadAll<GameObject>("Prefabs");
        var str = "";
        foreach (var item in items)
        {
            var path = AssetDatabase.GetAssetPath(item);
            str += item.name + "-->" + AssetDatabase.AssetPathToGUID(path) + "\n";
        }
        Debug.Log(str);
*/
}
