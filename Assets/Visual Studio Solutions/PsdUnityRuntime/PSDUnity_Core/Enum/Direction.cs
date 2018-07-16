using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
namespace PSDUnity
{
    public enum DirectionAxis
    {
        Horizontal = 1<<1,
        Vertical = 1<<2,
    }
    public enum Direction
    {
        LeftToRight = 1 << 1,
        BottomToTop = 1 << 2,
        TopToBottom = 1 << 3,
        RightToLeft = 1 << 4
    }
    
}