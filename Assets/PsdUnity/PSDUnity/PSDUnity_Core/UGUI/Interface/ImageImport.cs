using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PSDUnity;
namespace PSDUnity.UGUI
{
    public abstract class ImageImport: Import
    {
        public ImageImport(PSDImportCtrl ctrl) : base(ctrl) { }
        public abstract UGUINode DrawImage(ImgNode image, UGUINode parent);
    }
}
