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
using System;
namespace PSDUnity
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RuleTypeAttribute : Attribute
    {
        public int id;
        public RuleTypeAttribute(int id)
        {
            this.id = id;
        }
    }

    public class RuleObject : ScriptableObject
    {
        //[Header("前缀标记"), Space(10)]
        [RuleType(0)]public string titleAddress = "t_";
        [RuleType(0)]public string normalAddress = "n_";
        [RuleType(0)]public string pressedAddress = "p_";
        [RuleType(0)]public string highlightedAddress = "h_";
        [RuleType(0)]public string disableAddress = "d_";
        [RuleType(0)]public string backgroundAddress = "b_";
        [RuleType(0)]public string maskAddress = "m_";
        [RuleType(0)]public string handleAddress = "h_";
        [RuleType(0)]public string fillAddress = "f_";
        [RuleType(0)]public string placeAddress = "p_";
        [RuleType(0)]public string vbarAddress = "vb_";
        [RuleType(0)]public string hbarAddress = "hb_";
        [RuleType(0)]public string contentAddress = "c_";
        [RuleType(0)]public string backgroundsFormat = "b{0}_";
        [RuleType(0)] public string titlesFormat = "t{0}_";


        //[Header("分割标记"), Space(10)]
        [RuleType(1)] public char sepraterCharimg = '#';
        [RuleType(1)] public char sepraterChargroup = '@';
        [RuleType(1)] public char argumentChar = ':';

        //[Header("参数标记"), Space(10)]
        [RuleType(2)]public string horizontal = "h";
        [RuleType(2)]public string vertical = "v";
        [RuleType(2)]public string veritcal_horizontal = "vh";
        [RuleType(2)]public string left_right = "l";
        [RuleType(2)]public string right_left = "r";
        [RuleType(2)]public string bottom_top = "b";
        [RuleType(2)] public string top_bottom = "l";


        //[Header("后缀标记"), Space(10)]
        [RuleType(3)]public string asAtalsMark = "A";
        [RuleType(3)]public string asSingleMark = "S";
        [RuleType(3)]public string asTextureMark = "T";
        [RuleType(3)]public string asGoubleMark = "G";
        [RuleType(3)]public string asNoRepetMark = "N";
        [RuleType(3)] public string asCustomMark = "C";

        //[Header("生成配制"), Space(10)]
        [RuleType(4)]public bool createAtlas = true;
        [RuleType(4)]public ImgSource defultImgSource = ImgSource.Custom;
        [RuleType(4)]public SuffixType nameType = SuffixType.appendIndex;
        [RuleType(4)]public bool forceAddress = false;
        [RuleType(4)]public string globalSprite = "Assets/Common/Sprite";
        [RuleType(4)]public string globalTexture = "Assets/Common/Texture";
        [RuleType(4)]public string subFolder = "Image";
        [RuleType(4)]public bool forceSprite = true;
        [RuleType(4)]public bool scale = false;
        [RuleType(4)]public Vector2 defultUISize = new Vector2(1600, 900);
        [RuleType(4)] public int maxSize = 4096;

        //[Header("导入规则")]
        [RuleType(5)]public float spritePixelsPerUnit = 100;
        [RuleType(5)]public TextureImporterCompression textureCompression = TextureImporterCompression.Uncompressed;
        [RuleType(5)]public bool mipmapEnabled = true;
        [RuleType(5)]public bool isReadable = false;
        [RuleType(5)]public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        [RuleType(5)] public FilterMode filterMode = FilterMode.Trilinear;

        private static string[] groupNames;

        static RuleObject()
        {
            groupNames = System.Enum.GetNames(typeof(GroupType));
        }

        public string AnalysisGroupName(string name, out GroupType groupType, out string[] areguments)
        {
            areguments = null;
            string clampName = name;
            string typeName = "";

            if (name.Contains(sepraterChargroup.ToString()))
            {
                var index = name.IndexOf(sepraterChargroup);
                typeName = name.Substring(index + 1, name.Length - 1 - index);
                clampName = name.Substring(0, index);
            }

            groupType = GroupType.EMPTY;


            var item = System.Array.Find(groupNames, x =>
            {
                return typeName.ToLower().Contains(x.ToLower());
            });
            if (item != null)
            {
                groupType = (GroupType)System.Enum.Parse(typeof(GroupType), item);
            }

            if (typeName.Contains(argumentChar.ToString()))
            {
                var oldarg = typeName.Split(argumentChar);
                if (oldarg.Length > 1)
                {
                    areguments = new string[oldarg.Length - 1];
                    for (int i = 0; i < areguments.Length; i++)
                    {
                        areguments[i] = oldarg[i + 1];
                    }
                }
            }
            return clampName;
        }

        public string AnalySisImgName(string name, out ImgSource source, out ImgType type)
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
                else if (name.Contains((sepraterCharimg + asCustomMark).ToUpper()))
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
                else if (name.Contains((sepraterCharimg + asAtalsMark).ToUpper()))
                {
                    type = ImgType.AtlasImage;
                }
                else
                {
                    type = createAtlas ? ImgType.AtlasImage : ImgType.Image;
                }
            }
            else
            {
                clampName = name;
                type = createAtlas ? ImgType.AtlasImage : ImgType.Image;
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
