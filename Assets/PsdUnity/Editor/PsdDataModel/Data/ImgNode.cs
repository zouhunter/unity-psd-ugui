using System;
using UnityEngine;

namespace PSDUnity
{
    [System.Serializable]
    public class ImgNode
    {
        public string name = "";
        public ImgType type;
        public ImgSource source;
        public Rect rect;
        public Sprite sprite;
        public Texture texture;
        public string text = "";
        public Font font;
        public int fontSize = 0;
        public Color color = UnityEngine.Color.white;
    }
}