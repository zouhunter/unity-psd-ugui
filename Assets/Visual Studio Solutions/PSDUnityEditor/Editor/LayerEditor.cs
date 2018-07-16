using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace PSDUnity.UGUI
{

    [CustomEditor(typeof(LayerImport),true)]
    public class LayerImportEditor : Editor
    {
        public virtual Texture Icon { get { return null; } }
    }

}