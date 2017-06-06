using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Drawing;

public class RectUtility {
    public static Rect GetRectByRectRange(Rectangle rect0)
    {
        return new Rect(rect0.X, rect0.Height - rect0.Y, rect0.Width, rect0.Height);
    }
}
