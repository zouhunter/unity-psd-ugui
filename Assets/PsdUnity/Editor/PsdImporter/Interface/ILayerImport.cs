using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace PSDUnity
{
    public interface ILayerImport
    {
        UGUINode DrawLayer(GroupNode layer, UGUINode parent);
    }
}
