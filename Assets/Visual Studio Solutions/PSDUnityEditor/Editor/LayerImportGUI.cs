using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace PSDUnity.UGUI
{
    
    public abstract class LayerImportGUI
    {
        public virtual Texture Icon { get { return null; } }
        public virtual void HeadGUI(Rect dirRect, Data.GroupNode item) { }
    }

}