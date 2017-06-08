using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace PSDUnity
{
    public interface IImageImport
    {
        UGUINode DrawImage(ImgNode image, UGUINode parent);
    }
}
