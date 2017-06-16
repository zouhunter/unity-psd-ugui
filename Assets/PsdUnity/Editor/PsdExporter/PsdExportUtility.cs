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
        public static Rect GetRectFromLayer(Vector2 rootSize,PsdLayer psdLayer)
        {
            var xMin = (psdLayer.Right + psdLayer.Left - rootSize.x) * 0.5f;
            var yMin = -(psdLayer.Top + psdLayer.Bottom - rootSize.y) * 0.5f;
            return new Rect(xMin,yMin, psdLayer.Width,psdLayer.Height);
        }
        public static GroupNode1 CreatePictures(PsdLayer rootLayer, PictureExportInfo pictureInfo, bool forceSprite = false)
        {
            if (!rootLayer.IsGroup){
                Debug.LogWarning("[rootLayer不是组]");
                return null;

            }
            var rootSize = new Vector2(rootLayer.Width, rootLayer.Height);

            var groupnode = new GroupNode1(rootLayer.Name, GetRectFromLayer(rootSize, rootLayer));// (rootLayer,rootLayer));

            RetriveLayerToSwitchModle(rootSize,rootLayer, groupnode,forceSprite);

            var pictureData = new List<ImgNode>();
            groupnode.GetImgNodes(pictureData);


            //创建atlas
            var textures = pictureData.FindAll(x => x.type == ImgType.AtlasImage).ConvertAll<Texture2D>(x => x.texture);
            SaveToAtlas(textures.ToArray(), pictureInfo);
            //创建Sprites
            var pictures = pictureData.FindAll(x => x.type == ImgType.Image).ConvertAll<Texture2D>(x => x.texture);
            SaveToTextures(ImgType.Image, pictures.ToArray(), pictureInfo);
            //创建Sprites
            pictures = pictureData.FindAll(x => x.type == ImgType.Texture).ConvertAll<Texture2D>(x => x.texture);
            SaveToTextures(ImgType.Texture, pictures.ToArray(), pictureInfo);

            return groupnode;
        }

        public static void ChargeTextures(PictureExportInfo pictureInfo, GroupNode1 groupnode)
        {
            //重新加载
            var atlaspath = pictureInfo.exportPath + "/" + pictureInfo.atlasName;
            Sprite[] fileSprites = AssetDatabase.LoadAllAssetsAtPath(atlaspath).OfType<Sprite>().ToArray();
            Texture2D[] fileTextures = AssetDatabase.LoadAllAssetsAtPath(pictureInfo.exportPath).OfType<Texture2D>().ToArray();
            Sprite[] fileSingleSprites = AssetDatabase.LoadAllAssetsAtPath(pictureInfo.exportPath).OfType<Sprite>().ToArray();

            var pictureData = new List<ImgNode>();
            groupnode.GetImgNodes(pictureData);

            foreach (var item in pictureData)
            {
                switch (item.type)
                {
                    case ImgType.Image:
                        item.sprite = Array.Find(fileSingleSprites, x => x.name == item.picturename);
                        break;
                    case ImgType.AtlasImage:
                        item.sprite = Array.Find(fileSprites, x => x.name == item.picturename);
                        break;
                    case ImgType.Texture:
                        item.texture = Array.Find(fileTextures, x => x.name == item.picturename);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 将一组图片保存为atlas
        /// </summary>
        /// <param name="textureArray"></param>
        /// <param name="pictureInfo"></param>
        /// <param name="atlasName"></param>
        /// <returns></returns>
        public static void SaveToAtlas(Texture2D[] textureArray, PictureExportInfo pictureInfo)
        {
            if (textureArray.Length == 0) return;
            // The output of PackTextures returns a Rect array from which we can create our sprites
            Rect[] rects;
            Texture2D atlas = new Texture2D(pictureInfo.atlassize, pictureInfo.atlassize);
            rects = atlas.PackTextures(textureArray, 2, pictureInfo.atlassize);
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
            var atlaspath = pictureInfo.exportPath + "/" + pictureInfo.atlasName;
            File.WriteAllBytes(Path.GetFullPath(atlaspath), buf);
            AssetDatabase.Refresh();
            // Get our texture that we loaded
            TextureImporter textureImporter = AssetImporter.GetAtPath(atlaspath) as TextureImporter;

            // Make sure the size is the same as our atlas then create the spritesheet
            textureImporter.maxTextureSize = pictureInfo.atlassize;
            textureImporter.spritesheet = Sprites.ToArray();
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
            textureImporter.spritePixelsPerUnit = pictureInfo.pixelsToUnitSize;
            AssetDatabase.ImportAsset(atlaspath, ImportAssetOptions.ForceUpdate);

            foreach (Texture2D tex in textureArray)
            {
                UnityEngine.Object.DestroyImmediate(tex);
            }
        }
        /// <summary>
        /// 将图片分别保存到本地
        /// </summary>
        /// <param name="imgType"></param>
        /// <param name="textureArray"></param>
        /// <param name="pictureInfo"></param>
        public static void SaveToTextures(ImgType imgType, Texture2D[] textureArray, PictureExportInfo pictureInfo)
        {
            foreach (var texture in textureArray)
            {
                byte[] buf = texture.EncodeToPNG();
                var atlaspath = pictureInfo.exportPath + "/" + string.Format(pictureInfo.picNameTemp, texture.name);
                File.WriteAllBytes(Path.GetFullPath(atlaspath), buf);
                AssetDatabase.Refresh();

                // Get our texture that we loaded
                TextureImporter textureImporter = AssetImporter.GetAtPath(atlaspath) as TextureImporter;

                // Make sure the size is the same as our atlas then create the spritesheet
                textureImporter.maxTextureSize = pictureInfo.atlassize;

                switch (imgType)
                {
                    case ImgType.Image:
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
                        textureImporter.spritePixelsPerUnit = pictureInfo.pixelsToUnitSize;
                        break;
                    case ImgType.Texture:
                        textureImporter.textureType = TextureImporterType.Image;
                        break;
                    default:
                        break;
                }

                AssetDatabase.ImportAsset(atlaspath, ImportAssetOptions.ForceUpdate);
            }


            foreach (Texture2D tex in textureArray)
            {
                UnityEngine.Object.DestroyImmediate(tex);
            }

        }

        public static ImgNode AnalysisLayer(Vector2 rootSize,PsdLayer layer, bool forceSprite = false)
        {
            ImgNode data = null;
            var rect = GetRectFromLayer(rootSize,layer);
            switch (layer.LayerType)
            {
                case LayerType.Normal:
                    data = new ImgNode(layer.Name, rect, CreateTexture(layer));
                    break;
                case LayerType.SolidImage:
                    if (forceSprite)
                    {
                        data = new ImgNode(layer.Name, rect, CreateTexture(layer));
                    }
                    else
                    {
                        data = new ImgNode(layer.Name, rect, GetLayerColor(layer));
                    }
                    break;
                case LayerType.Text:
                    var textInfo = layer.Records.TextInfo;
                    data = new ImgNode(layer.Name, rect, textInfo.fontName, textInfo.fontSize, textInfo.text, textInfo.color);
                    break;
                case LayerType.Group:
                    break;
                case LayerType.Divider:
                    break;
                default:
                    break;
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
            texture.name = layer.Name;
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
        /// <summary>
        /// 转换为组
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="group"></param>
        /// <param name="OnRetrive"></param>
        public static void RetriveLayerToSwitchModle(Vector2 rootSize,PsdLayer layer, GroupNode group, bool forceSprite = false)
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
                        GroupNode childNode = group.InsertChild(child.Name, GetRectFromLayer(rootSize, child));

                        if (childNode != null)
                        {
                            group.groups.Add(childNode);
                            RetriveLayerToSwitchModle(rootSize,child, childNode,forceSprite);
                        }
                    }
                   else
                    {
                        ImgNode imgnode = AnalysisLayer(rootSize,child, forceSprite);
                        if (imgnode != null)
                        {
                            group.images.Add(imgnode);
                        }
                    }

                }
            }
        }
    }
}