using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace PSDUIImporter
{
    public interface IImageImport
    {
        void DrawImage(PsImage image,GameObject parent);
    }
}
