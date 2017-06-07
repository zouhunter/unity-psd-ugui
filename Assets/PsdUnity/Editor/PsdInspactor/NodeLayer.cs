using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using PhotoshopFile;
using System.Drawing;

public class NodeMask
{
    public PhotoshopFile.Mask mask;
    public bool PositionIsRelative { get; internal set; }
    public Rectangle Rect { get; internal set; }
    public byte[] ImageData { get { return mask.ImageData; }set { mask.ImageData = value; } }

    public NodeMask(PhotoshopFile.Mask mask)
    {
        this.mask = mask;
    }
}
public class NodeLayer
{
    public Layer layer;
    public List<NodeLayer> Children = new List<NodeLayer>();
    public string Name { get { return layer.Name; } set { layer.Name = value; } }
    public System.Drawing.Rectangle Rect { get { return layer.Rect; } }

    public UnityEngine.Color FillColor { get; internal set; }
    public string Text { get; internal set; }
    public bool IsTextLayer { get; internal set; }
    public float FontSize { get; internal set; }
    public TextJustification Justification { get; internal set; }
    public SortedList<short, Channel> SortedChannels { get; internal set; }
    public NodeMask MaskData { get; internal set; }
    public PsdFile PsdFile { get; internal set; }

    public NodeLayer(Layer layer)
    {
        this.layer = layer;
    }
}