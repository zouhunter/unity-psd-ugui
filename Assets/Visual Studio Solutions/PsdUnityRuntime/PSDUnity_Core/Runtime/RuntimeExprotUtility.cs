#define UNITY_2017
using Ntreev.Library.Psd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace PSDUnity.Runtime
{
    public static class RuntimeExportUtility
    {
        private static Vector2 maxSize { get; set; }
        public static Data.RuleObject RuleObj { get; set; }
        private static Vector2 rootSize { get; set; }
        private static int idSpan;

        public static Data.GroupNode CreateTree(Data.RuleObject rule, IPsdLayer psdLayer,  Vector2 uiSize, bool forceSprite = false)
        {
            RuleObj = rule;
            rootSize = uiSize;
            var rootNode = new Data.GroupNode(new Rect(Vector2.zero, RuleObj.defultUISize), 0, -1);
            rootNode.displayName = psdLayer.Name;
            idSpan = 1;
            maxSize = uiSize;
            List<Data.GroupNode> nodes = new List<Data.GroupNode>();

            foreach (PsdLayer rootLayer in psdLayer.Childs)
            {
                if (rootLayer.IsGroup)
                {
                    var groupnode = new Data.GroupNode(GetRectFromLayer(rootLayer), idSpan++, 0);
                    groupnode.Analyzing(RuleObj, rootLayer.Name);// (rootLayer,rootLayer));
                    RetriveLayerToSwitchModle(rootSize, rootLayer, groupnode, forceSprite);
                    nodes.Add(groupnode);
                }
            }

            var pictureData = new List<Data.ImgNode>();

            foreach (var groupnode in nodes)
            {
                groupnode.GetImgNodes(pictureData);
            }

            #region 去除名称重复
            var imageNodes = new List<Data.ImgNode>();
            foreach (var item in pictureData)
            {
                if (imageNodes.Find(x => x.TextureName == item.TextureName) == null)
                {
                    imageNodes.Add(item);
                }
            }
            #endregion

            SaveToTextures(pictureData.ToArray());

            foreach (var groupData in nodes)
            {
                rootNode.AddChild(groupData);
            }

            return rootNode;
        }

        private static void SaveToTextures(Data.ImgNode[] singleNodes)
        {
            foreach (var node in singleNodes)
            {
                 if(node.type == ImgType.Image || node.type == ImgType.AtlasImage)
                {
                    node.sprite = Sprite.Create(node.texture, new Rect(0, 0, node.texture.width, node.texture.height), new Vector2(node.texture.width, node.texture.height) * .5f);
                }

            }
        }

        
        private static Data.ImgNode AnalysisLayer(string baseName, Vector2 rootSize, PsdLayer layer, bool forceSprite = false)
        {
            Data.ImgNode data = null;
            var texture = CreateTexture(layer);
            var rect = GetRectFromLayer(layer);
            switch (layer.LayerType)
            {
                case LayerType.Normal:
                    data = new Data.ImgNode(baseName, rect, texture).SetIndex(CalcuteLayerID(layer)).Analyzing(RuleObj, layer.Name);
                    break;
                case LayerType.Color:
                    if (forceSprite)
                    {
                        data = new Data.ImgNode(baseName, rect, texture).SetIndex(CalcuteLayerID(layer)).Analyzing(RuleObj, layer.Name);
                    }
                    else
                    {
                        data = new Data.ImgNode(layer.Name, rect, GetLayerColor(layer)).SetIndex(CalcuteLayerID(layer));
                    }
                    break;
                case LayerType.Text:
                    var textInfo = layer.Records.TextInfo;
                    var color = new Color(textInfo.color[0], textInfo.color[1], textInfo.color[2], textInfo.color[3]);
                    data = new Data.ImgNode(layer.Name, rect, textInfo.fontName, textInfo.fontSize, textInfo.text, color);
                    break;
                case LayerType.Complex:
                    if (!RuleObj.forceSprite)
                    {
                        Debug.LogError("you psd have some not supported layer.(defult layer is not supported)! \n make sure your layers is Intelligent object or color lump." + "\n ->Detail:" + layer.Name);
                    }
                    data = new Data.ImgNode(baseName, rect, texture).SetIndex(CalcuteLayerID(layer)).Analyzing(RuleObj, layer.Name);
                    break;
                default:
                    break;
            }
            return data;
        }

        private static int CalcuteLayerID(PsdLayer layer)
        {
            int id = 0;
            var parent = layer.Parent;
            if (parent != null)
            {
                id = Array.IndexOf(parent.Childs, layer);
                id += 10 * CalcuteLayerID(parent);
            }
            else
            {
                id = Array.IndexOf(layer.Document.Childs, layer);
            }
            return id;
        }

        public static Texture2D CreateTexture(PsdLayer layer)
        {
            Debug.Assert(layer.Width != 0 && layer.Height != 0, layer.Name + ": width = height = 0");
            if (layer.Width == 0 || layer.Height == 0) return new Texture2D(layer.Width, layer.Height);

            Texture2D texture = new Texture2D(layer.Width, layer.Height);
            Color32[] pixels = new Color32[layer.Width * layer.Height];

            Channel red = Array.Find(layer.Channels, i => i.Type == ChannelType.Red);
            Channel green = Array.Find(layer.Channels, i => i.Type == ChannelType.Green);
            Channel blue = Array.Find(layer.Channels, i => i.Type == ChannelType.Blue);
            Channel alpha = Array.Find(layer.Channels, i => i.Type == ChannelType.Alpha);

            for (int i = 0; i < pixels.Length; i++)
            {
                var redErr = red == null || red.Data == null || red.Data.Length <= i;
                var greenErr = green == null || green.Data == null || green.Data.Length <= i;
                var blueErr = blue == null || blue.Data == null || blue.Data.Length <= i;
                var alphaErr = alpha == null || alpha.Data == null || alpha.Data.Length <= i;

                byte r = redErr ? (byte)0 : red.Data[i];
                byte g = greenErr ? (byte)0 : green.Data[i];
                byte b = blueErr ? (byte)0 : blue.Data[i];
                byte a = alphaErr ? (byte)255 : alpha.Data[i];

                int mod = i % texture.width;
                int n = ((texture.width - mod - 1) + i) - mod;
                pixels[pixels.Length - n - 1] = new Color32(r, g, b, a);
            }

            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }
      
        public static Color GetLayerColor(PsdLayer layer)
        {
            Channel red = Array.Find(layer.Channels, i => i.Type == ChannelType.Red);
            Channel green = Array.Find(layer.Channels, i => i.Type == ChannelType.Green);
            Channel blue = Array.Find(layer.Channels, i => i.Type == ChannelType.Blue);
            Channel alpha = Array.Find(layer.Channels, i => i.Type == ChannelType.Alpha);
            //Channel mask = Array.Find(layer.Channels, i => i.Type == ChannelType.Mask);

            Color[] pixels = new Color[layer.Width * layer.Height];

            for (int i = 0; i < pixels.Length; i++)
            {
                byte r = red.Data[i];
                byte g = green.Data[i];
                byte b = blue.Data[i];
                byte a = 255;

                if (alpha != null && alpha.Data[i] != 0)
                    a = (byte)alpha.Data[i];
                //if (mask != null && mask.Data[i] != 0)
                //    a *= mask.Data[i];

                int mod = i % layer.Width;
                int n = ((layer.Width - mod - 1) + i) - mod;
                pixels[pixels.Length - n - 1] = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
            }
            Color color = Color.white;
            foreach (var item in pixels)
            {
                color += item;
                color *= 0.5f;
            }
            return color;
        }
      
        public static void RetriveLayerToSwitchModle(Vector2 rootSize, PsdLayer layer, Data.GroupNode group, bool forceSprite = false)
        {
            if (!layer.IsGroup)
            {
                return;
            }
            else
            {
                foreach (var child in layer.Childs)
                {
                    if (child.IsGroup)
                    {
                        Data.GroupNode childNode = new Data.GroupNode(GetRectFromLayer(child), idSpan++, group.depth + 1);
                        childNode.Analyzing(RuleObj, child.Name);
                        group.AddChild(childNode);

                        if (childNode != null)
                        {
                            RetriveLayerToSwitchModle(rootSize, child, childNode, forceSprite);
                        }
                    }

                    else
                    {
                        Data.ImgNode imgnode = AnalysisLayer(group.displayName, rootSize, child, forceSprite);
                        if (imgnode != null)
                        {
                            group.images.Add(imgnode);
                        }
                    }

                }
            }
        }

        public static Rect GetRectFromLayer(IPsdLayer psdLayer)
        {
            var left = psdLayer.Left;// psdLayer.Left <= 0 ? 0 : psdLayer.Left;
            var bottom = psdLayer.Bottom;// psdLayer.Bottom <= 0 ? 0 : psdLayer.Bottom;
            var top = psdLayer.Top;// psdLayer.Top >= rootSize.y ? rootSize.y : psdLayer.Top;
            var rigtht = psdLayer.Right;// psdLayer.Right >= rootSize.x ? rootSize.x : psdLayer.Right;
            var width = psdLayer.Width;// psdLayer.Width > rootSize.x ? rootSize.x : psdLayer.Width;
            var height = psdLayer.Height;// psdLayer.Height > rootSize.y ? rootSize.y : psdLayer.Height;

            var xMin = (rigtht + left - rootSize.x) * 0.5f;
            var yMin = -(top + bottom - rootSize.y) * 0.5f;
            return new Rect(xMin, yMin, width, height);
        }

    }
}