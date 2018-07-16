using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PSDUnity.UGUI;
using System;

public class RundSliderLayerImporter : LayerImport
{
    public RundSliderLayerImporter()
    {
        _suffix = "RoundSlider";
    }

    public override GameObject CreateTemplate()
    {
        return new GameObject("RoundSlider", typeof(Image));
    }
}
