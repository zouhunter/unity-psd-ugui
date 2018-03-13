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

namespace PSDUnity
{
    public class RuleObject : ScriptableObject
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
        [HideInInspector]
        public List<PrefabItem> prefabs = new List<PrefabItem>();

        [Header("图层标记"), Space(10)]
        public char sepraterCharimg = '#';
        public string asAtalsMark = "A";
        public string asSingleMark = "S";
        public string asTextureMark = "T";
        public ImgType defultImgType = ImgType.AtlasImage;
        public string asGoubleMark = "G";
        public string asNoRepetMark = "N";
        public string asCustomMark = "C";
        public NameType nameType = NameType.donothing;
        public ImgSource defultImgSource = ImgSource.Custom;
        [Header("组名分割"), Space(10)]
        public char sepraterChargroup = '@';
        public char argumentChar = ':';


        public string AnalysisGroupName(string name,out GroupType groupType,out string[] areguments)
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
            if (name.Contains(sepraterChargroup.ToString()))
            {
                var index = name.IndexOf(sepraterChargroup);
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

        public string AnalySisImgName(string name,out ImgSource source, out ImgType type)
        {
            string clampName = name;

            if (name.Contains(sepraterCharimg.ToString()))
            {
                var index = name.IndexOf(sepraterCharimg);
                clampName = name.Remove(index);

                name = name.ToUpper();
                if (name.Contains((sepraterCharimg + asGoubleMark).ToUpper()))
                {
                    source = ImgSource.Globle;
                }
                else if (name.Contains((sepraterCharimg + asNoRepetMark).ToUpper()))
                {
                    source = ImgSource.Normal;
                }
                else if(name.Contains((sepraterCharimg + asCustomMark).ToUpper()))
                {
                    source = ImgSource.Custom;
                }
                else
                {
                    source = defultImgSource;
                }

                if (name.Contains((sepraterCharimg + asSingleMark).ToUpper()))
                {
                    type = ImgType.Image;
                }
                else if (name.Contains((sepraterCharimg + asTextureMark).ToUpper()))
                {
                    type = ImgType.Texture;
                }
                else if(name.Contains((sepraterCharimg + asAtalsMark).ToUpper()))
                {
                    type = ImgType.AtlasImage;
                }
                else
                {
                    type = defultImgType;
                }
            }
            else
            {
                clampName = name;
                type = defultImgType;
                source = defultImgSource;
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
