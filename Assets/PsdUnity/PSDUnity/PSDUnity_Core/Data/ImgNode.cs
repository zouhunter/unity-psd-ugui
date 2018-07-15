using System;
using UnityEngine;

namespace PSDUnity.Data
{
    [Serializable]
    public class ImgNode : INameAnalyzing<Data.ImgNode>
    {
        public string Name;
        private int hashImage = 0;
        private int index = 0;
        private SuffixType customNameType;
        private string baseName;
        public string TextureName
        {
            get
            {
                if(forceAddress || source == ImgSource.Custom)
                {
                    if (customNameType == SuffixType.appendRectHash)
                    {
                        return Name + hashImage;
                    }
                    else if( customNameType == SuffixType.appendIndex)
                    {
                        return Name + index;
                    }
                    else
                    {
                        return Name + baseName;
                    }
                }
                else
                {
                    return Name;
                }
            }
        }
        public ImgType type;
        public ImgSource source;
        public bool forceAddress;
        public Rect rect;
        public Sprite sprite;
        public Texture2D texture;
        public string text = "";
        public Font font;
        public int fontSize = 0;
        public Color color = UnityEngine.Color.white;
        public ImgNode() { }

        public ImgNode(string baseName,Rect rect, Texture2D texture) : this(rect)
        {
            this.baseName = baseName;
            this.rect = rect;
            this.texture = texture;
        }
        public ImgNode(string name, Rect rect, Color color) : this(rect)
        {
            this.Name = name;
            this.type = ImgType.Color;
            this.color = color;
        }
        public ImgNode(string name, Rect rect, string font, int fontSize, string text, Color color) : this(rect)
        {
            this.type = ImgType.Label;
            this.Name = name;
            this.font = null; /*Debug.Log(font);*/
            this.fontSize = fontSize;
            this.text = text;
            this.color = color;
        }

        private ImgNode(Rect rect)
        {
            this.rect = rect;
        }

        public Data.ImgNode SetIndex(int index)
        {
            this.index = index;
            return this;
        }

        /// <summary>
        /// 将名字转换（去除标记性字符）
        /// </summary>
        /// <returns></returns>
        public Data.ImgNode Analyzing(RuleObject Rule, string name)
        {
            this.Name = Rule.AnalySisImgName(name, out source, out type);
            this.customNameType = Rule.nameType;
            this.forceAddress = Rule.forceAddress;
            //添加后缀
            if (texture != null)
            {
                this.hashImage = rect.GetHashCode();
                this.texture.name = TextureName;
                //Debug.Log(TextureName);
            }
            return this;
        }
    }


}