using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class Layer
    {
        public string name;
        public LayerType type;
        public Layer[] layers;
        public string[] arguments;
        public PsImage[] images;
        public Size size;
        public Position position;
    }
}

