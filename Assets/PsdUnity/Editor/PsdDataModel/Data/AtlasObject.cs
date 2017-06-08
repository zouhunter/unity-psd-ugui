using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace PSDUnity
{
    public class AtlasObject : ScriptableObject
    {
        public string psdFile;
        public Vector2 psdSize;
        public GroupNode[] groups;
    }
}
