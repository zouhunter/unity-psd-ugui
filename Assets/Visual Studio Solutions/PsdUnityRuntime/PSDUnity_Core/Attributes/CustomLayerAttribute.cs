using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class CustomLayerAttribute : Attribute {
    public Type type;
    public CustomLayerAttribute(Type type)
    {
        this.type = type;
    }
}
