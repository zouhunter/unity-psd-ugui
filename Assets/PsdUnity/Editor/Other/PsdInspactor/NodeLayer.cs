//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections.Generic;
//using PhotoshopFile;
//using System.Drawing;

//public class NodeMask
//{
//    public PhotoshopFile.Mask mask;
//    public bool PositionIsRelative { get { return true; } }
//    public Rectangle Rect { get { return mask.Rect; } set { mask.Rect = value; } }
//    public byte[] ImageData { get { return mask.ImageData; }set { mask.ImageData = value; } }

//    public NodeMask(PhotoshopFile.Mask mask)
//    {
//        this.mask = mask;
//    }
//}
//public class NodeLayer
//{
//    public Layer layer;
//    public List<NodeLayer> Children = new List<NodeLayer>();
//    public string Name { get { return layer.Name; } set { layer.Name = value; } }
//    public System.Drawing.Rectangle Rect { get { return layer.Rect; } }

//    public UnityEngine.Color FillColor { get { return UnityEngine.Color.red; } }
//    public string Text { get { return "sdfsfs"; } }
//    public bool IsTextLayer { get { return _isTextLayer; } }
//    public float FontSize { get { return 20; } }
//    public TextJustification Justification {get { return TextJustification.Center; } }
//    public SortedList<short, Channel> SortedChannels { get; internal set; }
//    public NodeMask MaskData { get{ return _mask; } }
//    public PsdFile PsdFile { get { return _psdFile; } }

//    private PsdFile _psdFile;
//    private NodeMask _mask;
//    private bool _isTextLayer;
//    public NodeLayer(Layer layer, PsdFile psdfile)
//    {
//        this.layer = layer;
//        this._psdFile = psdfile;
//        var mask0 = layer.Masks.LayerMask == null ? layer.Masks.UserMask : layer.Masks.LayerMask;
//        _mask = new NodeMask(mask0);
//        JudgeType();
//        ChannelSort();
//    }
//    public void ChannelSort()
//    {
//        SortedChannels = new SortedList<short, Channel>();
//        for (int index = 0; index < layer.Channels.Count; ++index)
//        {
//            SortedChannels.Add(layer.Channels[index].ID, layer.Channels[index]);
//        }
//    }
//    public void JudgeType()
//    {
//        foreach (var adjustmentLayerInfo in layer.AdditionalInfo)
//        {
//            if (adjustmentLayerInfo.Key == "TySh")//文字层
//            {
//                _isTextLayer = true;
//                Debug.Log(adjustmentLayerInfo +":"+ layer.Name);
//            }
//            else if (adjustmentLayerInfo.Key == "luni")
//            {
             
//            }
//        }
//    }
//}