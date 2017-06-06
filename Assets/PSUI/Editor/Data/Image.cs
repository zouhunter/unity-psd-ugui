using System;
using UnityEngine;
namespace PSDUIImporter
{
    [Serializable]
    public class Image
    {
        public ImageType imageType;
        public ImageSource imageSource;
        public string name;
        public Position position;
        public Size size;

        public string[] arguments;
    }
}