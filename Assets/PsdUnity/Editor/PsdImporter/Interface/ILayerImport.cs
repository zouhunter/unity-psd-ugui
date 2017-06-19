using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PSDUnity.Data;
namespace PSDUnity.Import
{
    public interface ILayerImport
    {
        UGUINode DrawLayer(GroupNode layer, UGUINode parent);
    }
}
