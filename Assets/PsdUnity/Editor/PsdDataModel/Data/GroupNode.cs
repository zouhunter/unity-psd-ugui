using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSDUnity
{
    [Serializable]
    public class GroupNode
    {
        public string      name;
        public ControlType controltype;
        public List<GroupNode> groups = new List<GroupNode>();
        public List<ImgNode> images = new List<ImgNode>();
        public Rect        rect;
        public List<string> arguments = new List<string>();

        [OnSerializing]
        private void OnSerializingStarted(StreamingContext context)
        {
            Debug.LogError("OnSerializing" + context);
        }

        //restore info about the children
        [OnDeserialized]
        private void OnDeserialized(object o)
        {
            Debug.LogError("OnDeserialized" + o);
           
        }
    }
}

