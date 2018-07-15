using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace PSDUnity.UGUI
{

    [CustomEditor(typeof(LayerImport),true)]
    public abstract class LayerImportEditor : Editor
    {
        public abstract Texture2D Icon { get; }
    }

}