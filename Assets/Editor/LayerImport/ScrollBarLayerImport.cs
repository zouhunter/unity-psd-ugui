using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class ScrollBarLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public ScrollBarLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }
        public void DrawLayer(Layer layer, GameObject parent)
        {
            throw new NotImplementedException();
        }
    }
}
