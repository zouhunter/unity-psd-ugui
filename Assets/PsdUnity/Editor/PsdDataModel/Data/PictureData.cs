using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
namespace PSDUnity
{
    [System.Serializable]
    public class PictureData
    {
        public string picturename;
        public ImgType type;
        public Sprite sprite;
        public Texture texture;

        public PictureData(string picturename,ImgType type)
        {
            this.picturename = picturename;
            this.type = type;
        }
    }
}
