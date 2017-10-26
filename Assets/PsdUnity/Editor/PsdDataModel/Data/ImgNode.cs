using System;
using UnityEngine;

namespace PSDUnity.Data
{
    [Serializable]
    public class ImgNode: INameAnalyzing<ImgNode>
    {
        public string Name;
        private int hashImage = 0;
        public string TextureName
        {
            get
            {
                return Name + hashImage;
            }
        }
        public ImgType type;
        public ImgSource source;
        public Rect rect;
        public Sprite sprite;
        public Texture2D texture;
        public string text = "";
        public Font font;
        public int fontSize = 0;
        public Color color = UnityEngine.Color.white;

        public ImgNode() { }

        public ImgNode(Rect rect, Texture2D texture) : this(rect)
        {
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

        /// <summary>
        /// 将名字转换（去除标记性字符）
        /// </summary>
        /// <returns></returns>
        public ImgNode Analyzing(RuleObject Rule,string name)
        {
            this.Name = Rule.AnalySisImgName(name, out source, out type);
            //添加后缀
            if (texture != null)
            {
                this.hashImage = rect.GetHashCode();
                this.texture.name = TextureName;
            }
            return this;
        }
    }

   
}