using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PSDUnity;
namespace PSDUnity.UGUI
{
    public abstract class LayerImport: Import
    {
        public LayerImport(PSDImportCtrl ctrl) : base(ctrl) { }
        public abstract UGUINode DrawLayer(GroupNode layer, UGUINode parent);
    }
}
