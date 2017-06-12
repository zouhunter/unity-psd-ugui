using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace PSDUnity
{
    public class AtlasObject : ScriptableObject, ISerializationCallbackReceiver
    {
        public string psdFile;
        public string exportPath;
        public bool maskAsColor;
        public Vector2 psdSize;
        public List<GroupNode> groups;

        public void OnBeforeSerialize()
        {
           
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}
