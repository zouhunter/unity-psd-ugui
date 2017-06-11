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
namespace PSDUnity
{
    public enum PsdLayerType
    {
        Normal,
        SolidColor,
        Text,
    }
    public class PsdLayerData
    {
        public PsdLayerType type;
        public Texture2D texture;
        public Color color;
        public int fontSize;
        public string fontName;
        public string text;
        public PsdLayerData(Texture2D texture)
        {
            type = PsdLayerType.Normal;
            this.texture = texture;
        }
        public PsdLayerData(Color color)
        {
            this.type = PsdLayerType.SolidColor;
        }
        public PsdLayerData(int fontSize,string fontName,string text,Color color)
        {
            this.type = PsdLayerType.Text;
            this.fontName = fontName;
            this.text = text;
            this.color = color;
        }
    }
}