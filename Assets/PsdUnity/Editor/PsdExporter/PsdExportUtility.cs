using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using Ntreev.Library.Psd;
using System.Linq;
using System.IO;
using System;

namespace PSDUnity
{
    public static class PsdExportUtility
    {
        public static Vector2 CreateAtlas(PsdLayer psd,out GroupNode1 group, float pixelsToUnitSize, int atlassize, string releativePath,bool forceSprite = false)
        {
            string fileName = Path.GetFileNameWithoutExtension(releativePath);

            List<Texture2D> textures = new List<Texture2D>();

            group = new GroupNode1();

            RetriveChild(ref group,psd, (item) =>
             {
                 if (!item.IsGroup)
                 {
                    
                     switch (item.LayerType)
                     {
                         case LayerType.Normal:
                             Texture2D tex = CreateTexture(item);
                             tex.name = item.Name;
                             textures.Add(tex);
                             break;
                         case LayerType.SolidImage:

                             break;
                         case LayerType.Text:

                             break;
                         case LayerType.Group:
                             break;
                         case LayerType.Divider:
                             break;
                         default:
                             break;
                     }
                 }
                 else
                 {

                 }
             });

            // The output of PackTextures returns a Rect array from which we can create our sprites
            Rect[] rects;
            Texture2D atlas = new Texture2D(atlassize, atlassize);
            Texture2D[] textureArray = textures.ToArray();
            rects = atlas.PackTextures(textureArray, 2, atlassize);
            List<SpriteMetaData> Sprites = new List<SpriteMetaData>();

            // For each rect in the Rect Array create the sprite and assign to the SpriteMetaData
            for (int i = 0; i < rects.Length; i++)
            {
                // add the name and rectangle to the dictionary
                SpriteMetaData smd = new SpriteMetaData();
                smd.name = textureArray[i].name;
                smd.rect = new Rect(rects[i].xMin * atlas.width, rects[i].yMin * atlas.height, rects[i].width * atlas.width, rects[i].height * atlas.height);
                smd.pivot = new Vector2(0.5f, 0.5f); // Center is default otherwise layers will be misaligned
                smd.alignment = (int)SpriteAlignment.Center;
                Sprites.Add(smd);
            }

            // Need to load the image first

            byte[] buf = atlas.EncodeToPNG();
            File.WriteAllBytes(Path.GetFullPath(releativePath), buf);
            AssetDatabase.Refresh();

            // Get our texture that we loaded
            atlas = (Texture2D)AssetDatabase.LoadAssetAtPath(releativePath, typeof(Texture2D));
            TextureImporter textureImporter = AssetImporter.GetAtPath(releativePath) as TextureImporter;
            // Make sure the size is the same as our atlas then create the spritesheet
            textureImporter.maxTextureSize = atlassize;
            textureImporter.spritesheet = Sprites.ToArray();
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
            textureImporter.spritePixelsPerUnit = pixelsToUnitSize;
            AssetDatabase.ImportAsset(releativePath, ImportAssetOptions.ForceUpdate);

            foreach (Texture2D tex in textureArray)
            {
               UnityEngine.Object.DestroyImmediate(tex);
            }

            return new Vector2(psd.Width, psd.Height);
        }
        public static PsdLayerData AnalysisLayer(PsdLayer layer)
        {
            PsdLayerData data = null;
            if (!layer.HasMask)
            {
                data = new PsdLayerData(CreateTexture(layer));
            }
            else
            {
                data = new PsdLayerData(GetLayerColor(layer));
            }
            return data;
        }
        public static Texture2D CreateTexture(PsdLayer layer)
        {
            Texture2D texture = new Texture2D(layer.Width, layer.Height);
            Color32[] pixels = new Color32[layer.Width * layer.Height];
            Channel red = Array.Find(layer.Channels, i => i.Type == ChannelType.Red);
            Channel green = Array.Find(layer.Channels, i => i.Type == ChannelType.Green);
            Channel blue = Array.Find(layer.Channels, i => i.Type == ChannelType.Blue);
            Channel alpha = Array.Find(layer.Channels, i => i.Type == ChannelType.Alpha);
            //Channel mask = Array.Find(layer.Channels, i => i.Type == ChannelType.Mask);
            for (int i = 0; i < pixels.Length; i++)
            {
                byte r = red.Data[i];
                byte g = green.Data[i];
                byte b = blue.Data[i];
                byte a = 255;

                if (alpha != null)
                    a = alpha.Data[i];
                //if (mask != null)
                //    a *= mask.Data[i];

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
            Channel mask = Array.Find(layer.Channels, i => i.Type == ChannelType.Mask);

            byte r = red.Data[0];
            byte g = green.Data[0];
            byte b = blue.Data[0];
            byte a = alpha.Data[0];

            return new Color32(r, g, b, a);
        }
        public static PsdLayerData GetText(PsdLayer layer)
        {
            return new PsdLayerData(0, "", "", Color.white);
        }
        public static void RetriveChild(ref GroupNode1 group,PsdLayer layer, UnityAction<PsdLayer> OnRetrive)
        {
            OnRetrive(layer);

            if (!layer.IsGroup)
            {
                return;
            }
            else
            {
                group = new GroupNode1();
                group.name = layer.Name;
                group.controltype = ControlType.Button;
                group.rect = new Rect(layer.Left, layer.Bottom, layer.Width, layer.Height);

                foreach (var child in layer.Childs)
                {
                    GroupNode1 childNode = null;
                    RetriveChild(ref childNode, child, OnRetrive);
                    if(childNode != null)
                    {
                        group.groups.Add(childNode);
                    }
                    else
                    {
                        var imgNode = new ImgNode();
                        imgNode.name = child.Name;
                        group.images.Add(imgNode);
                    }
                }
            }
        }
    }
}