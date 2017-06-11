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
        public GroupNode[] groups = new GroupNode[0];
        public ImgNode[]   images = new ImgNode[0];
        public Rect        rect;
        public string[] arguments = new string[0];

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

