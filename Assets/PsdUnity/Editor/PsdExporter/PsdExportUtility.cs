using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using Ntreev.Library.Psd;
using System.Linq;
using System.IO;
using System;
using PSDUnity.Data;
namespace PSDUnity.Exprot
{
    public static class PsdExportUtility
    {
        private static Vector2 maxSize { get; set; }
        public static RouleObject rouleObj { get; set; }
        public static PictureExportInfo pictureInfo { get; set; }
        private static Vector2 rootSize { get; set; }
        public static void InitPsdExportEnvrioment(PictureExportInfo pictureInfo0, RouleObject rouleObj0,Vector2 rootSize0)
        {
            pictureInfo = pictureInfo0;
            rouleObj = rouleObj0;
            rootSize = rootSize0;
        }


        public static GroupNode1[] CreatePictures(IPsdLayer[] rootLayers,Vector2 rootSize, Vector2 uiSize, bool forceSprite = false)
        {
            maxSize = uiSize;
            List<GroupNode1> nodes = new List<GroupNode1>();
            foreach (PsdLayer rootLayer in rootLayers)
            {
                if (rootLayer.IsGroup)
                {
                    var groupnode = new GroupNode1(GetRectFromLayer(rootLayer)).Analyzing(PsdExportUtility.rouleObj, rootLayer.Name) as GroupNode1;// (rootLayer,rootLayer));
                    RetriveLayerToSwitchModle(rootSize, rootLayer, groupnode, forceSprite);
                    nodes.Add(groupnode);
                }
            }

            var pictureData = new List<ImgNode>();

            foreach (var groupnode in nodes)
            {
                groupnode.GetImgNodes(pictureData);
            }

            //创建atlas
            var textures = pictureData.FindAll(x => x.type == ImgType.AtlasImage).ConvertAll<Texture2D>(x => x.texture);
            SaveToAtlas(textures.ToArray(), pictureInfo);
            //创建Sprites
            var pictures = pictureData.FindAll(x => x.type == ImgType.Image).ConvertAll<Texture2D>(x => x.texture);
            SaveToTextures(ImgType.Image, pictures.ToArray(), pictureInfo);
            //创建Sprites
            pictures = pictureData.FindAll(x => x.type == ImgType.Texture).ConvertAll<Texture2D>(x => x.texture);
            SaveToTextures(ImgType.Texture, pictures.ToArray(), pictureInfo);

            return nodes.ToArray();
        }

        public static void ChargeTextures(PictureExportInfo pictureInfo, GroupNode1 groupnode)
        {
            //重新加载
            var atlaspath = pictureInfo.exportPath + "/" + pictureInfo.atlasName;
            Sprite[] fileSprites = AssetDatabase.LoadAllAssetsAtPath(atlaspath).OfType<Sprite>().ToArray();

            Texture2D[] fileTextures = LoadAllObjectFromDir<Texture2D>(pictureInfo.exportPath);// AssetDatabase.LoadAllAssetsAtPath(pictureInfo.exportPath).OfType<Texture2D>().ToArray();
            Sprite[] fileSingleSprites = LoadAllObjectFromDir<Sprite>(pictureInfo.exportPath);// AssetDatabase.LoadAllAssetsAtPath(pictureInfo.exportPath).OfType<Sprite>().ToArray();

            var pictureData = new List<ImgNode>();
            groupnode.GetImgNodes(pictureData);

            foreach (var item in pictureData)
            {
                switch (item.type)
                {
                    case ImgType.Image:
                        item.sprite = Array.Find(fileSingleSprites, x => x.name == item.TextureName);
                        break;
                    case ImgType.AtlasImage:
                        item.sprite = Array.Find(fileSprites, x => x.name == item.TextureName);
                        break;
                    case ImgType.Texture:
                        item.texture = Array.Find(fileTextures, x => x.name == item.TextureName);
                        break;
                    default:
                        break;
                }
            }
           
        }

        private static T[] LoadAllObjectFromDir<T>(string dirName) where T:UnityEngine.Object
        {
            List<T> assets = new List<T>();
            if (!string.IsNullOrEmpty(dirName))
            {
                var textures = Directory.GetFiles(dirName, "*.png", SearchOption.TopDirectoryOnly);
                foreach (var item in textures)
                {
                    var assetPath = item.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                    T obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (obj != null)
                    {
                        assets.Add(obj);
                    }
                }
            }
            return assets.ToArray();
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
        /// <summary>
        /// 将图层解析为imgNode
        /// </summary>
        /// <param name="rootSize"></param>
        /// <param name="layer"></param>
        /// <param name="forceSprite"></param>
        /// <returns></returns>
        public static ImgNode AnalysisLayer(Vector2 rootSize,PsdLayer layer, bool forceSprite = false)
        {
            ImgNode data = null;
            var texture = CreateTexture(layer);
            var rect = GetRectFromLayer(layer);
            switch (layer.LayerType)
            {
                case LayerType.Normal:
                    data = new ImgNode(rect, texture).Analyzing(PsdExportUtility.rouleObj,layer.Name);
                    break;
                case LayerType.SolidImage:
                    if (forceSprite)
                    {
                        data = new ImgNode(rect, texture).Analyzing(PsdExportUtility.rouleObj,layer.Name);
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
        /// <summary>
        /// 从layer解析图片
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 计算平均颜色
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
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
                pixels[pixels.Length - n - 1] = new Color(r/255f, g / 255f, b / 255f, a/255f);
            }
            Color color = Color.white;
            foreach (var item in pixels)
            {
                color += item;
                color *= 0.5f;
            }
            return color;
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
                        GroupNode childNode = group.InsertChild(GetRectFromLayer(child)).Analyzing(rouleObj,child.Name);

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
       
        /// <summary>
        /// 解析Layer中的尺寸信息
        /// </summary>
        /// <param name="psdLayer"></param>
        /// <returns></returns>
        private static Rect GetRectFromLayer(PsdLayer psdLayer)
        {
            //rootSize = new Vector2(rootSize.x > maxSize.x ? maxSize.x : rootSize.x, rootSize.y > maxSize.y ? maxSize.y : rootSize.y);

            var left = psdLayer.Left;// psdLayer.Left <= 0 ? 0 : psdLayer.Left;
            var bottom = psdLayer.Bottom;// psdLayer.Bottom <= 0 ? 0 : psdLayer.Bottom;
            var top = psdLayer.Top;// psdLayer.Top >= rootSize.y ? rootSize.y : psdLayer.Top;
            var rigtht = psdLayer.Right;// psdLayer.Right >= rootSize.x ? rootSize.x : psdLayer.Right;
            var width = psdLayer.Width;// psdLayer.Width > rootSize.x ? rootSize.x : psdLayer.Width;
            var height = psdLayer.Height;// psdLayer.Height > rootSize.y ? rootSize.y : psdLayer.Height;

            var xMin = (rigtht + left - rootSize.x) * 0.5f;
            var yMin = -(top + bottom - rootSize.y) * 0.5f;
            return new Rect(xMin,yMin, width,height);
        }
        
    }
}