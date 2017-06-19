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
using System.Collections.Generic;
using UnityEditor;

namespace PSDUnity
{
    public class RouleObject : ScriptableObject
    {
        [System.Serializable]
        public class PrefabItem
        {
            public GroupType groupType;
            public string keyName;
            public GameObject prefab;
            public PrefabItem()
            {
            }

            public PrefabItem(GroupType groupType,GameObject prefab)
            {
                this.groupType = groupType;
                this.prefab = prefab;
                this.keyName = groupType.ToString().ToLower();
            }
        }
        public List<PrefabItem> prefabs = new List<PrefabItem>();
        public char sepraterChar = '@';
        public char argumentChar = ':';

        public void Reset()
        {
            LoadDefultDatas(prefabs);
        }

        public string AnalysisName(string name,out GroupType groupType,out string[] areguments)
        {
            areguments = null;
            string clampName = name;
            groupType = GroupType.EMPTY;
            var item = prefabs.Find(x =>
            {
                return name.ToLower().Contains(x.keyName.ToLower());
            });
            if (item != null)
            {
                groupType = item.groupType;
            }
            if (name.Contains(sepraterChar.ToString()))
            {
                var index = name.IndexOf(sepraterChar);
                clampName = name.Remove(index);
            }
            if (name.Contains(argumentChar.ToString()))
            {
                var oldarg = name.Split(argumentChar);
                if (oldarg.Length > 1)
                {
                    areguments = new string[oldarg.Length -1];
                    for (int i = 0; i < areguments.Length; i++)
                    {
                        areguments[i] = oldarg[i + 1];
                    }
                }
            }
            return clampName;
        }
        public static Direction GetDirectionByKey(string key)
        {
            var dir = Direction.None;
            switch (key.ToLower())
            {
                case "v":
                    dir = Direction.Vertical;
                    break;
                case "h":
                    dir = Direction.Horizontal;
                    break;
                case "vh":
                case "hv":
                    dir = Direction.Vertical | Direction.Horizontal;
                    break;
                case "b":
                    dir = Direction.BottomToTop;
                    break;
                case "t":
                    dir = Direction.TopToBottom;
                    break;
                case "l":
                    dir = Direction.LeftToRight;
                    break;
                case "r":
                    dir = Direction.RightToLeft;
                    break;
                default:
                    break;
            }
            return dir;
        }

        private static void LoadDefultDatas(List<PrefabItem> prefabs)
        {
            var path = AssetDatabase.GUIDToAssetPath("9a94c0cbaca3a48468b0b1e51fefcbfb");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.BUTTON, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("dec381fc4475a4a33bcfd41351b26fcc");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.CANVAS, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("d6eef6649ab97db4f900c0f6a896a73e");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.DROPDOWN, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("a47d3ef1adf404641b6d23a93e59ee53");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.EMPTY, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("25904f280417ef748977530a357b5fb0");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.GROUP, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("ed417986c539a4fc7b09f220ff4ceef6");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.GRID, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("9d52d80f2b2a64fbe96cd04604170d32");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.IMAGE, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("2e48a2d5b2f2ccb439d0cf1eeba3e6dd");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.INPUTFIELD, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("b22710b4e94fd4947ae6eebd62b8ac32");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.RawIMAGE, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("2b101ae8ff3e18e4eb5aca168965d85c");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.SCROLLBAR, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("ca7d801212b074407a4fc212216da328");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.SCROLLVIEW, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("95fdb08286306794583b6e1d19021aaa");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.SLIDER, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("0f5f099613ddb4155b4c775d8db5984d");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.TEXT, obj));
                }
            }
            path = AssetDatabase.GUIDToAssetPath("f4c3d75106f0b944faa6f14731cec415");
            if (!string.IsNullOrEmpty(path))
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    prefabs.Add(new PrefabItem(GroupType.TOGGLE, obj));
                }
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
