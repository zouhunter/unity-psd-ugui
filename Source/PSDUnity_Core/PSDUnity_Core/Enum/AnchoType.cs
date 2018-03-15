using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
namespace PSDUnity
{
    public enum AnchoType
    {
        Custom = 1 << 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Up = 1 << 3,
        Down = 1 << 4,
        XStretch = 1 << 5,
        YStretch = 1 << 6,
        XCenter = 1 << 7,
        YCenter = 1 << 8,
    }
}