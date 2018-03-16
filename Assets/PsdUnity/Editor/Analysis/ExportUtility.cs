using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using Ntreev.Library.Psd;
using System.Linq;
using System.IO;
using System;
using PSDUnity.Data;
namespace PSDUnity.Analysis
{
    public static class ExportUtility
    {
        #region 菜单
        [MenuItem("Window/Psd-Preview")]
        static void OpenPSDConfigWindow()
        {
            var window = EditorWindow.GetWindow<PsdPreviewWindow>();
            window.position = new Rect(100, 100, 800, 600);
            window.titleContent = new GUIContent("预览PSD文件");
            window.Repaint();
        }

        [MenuItem("Assets/Create/Psd-Exporter")]
        static void CreateTemp()
        {
            var exporter = ScriptableObject.CreateInstance<Exporter>();
            exporter.psdFile = "";
            ProjectWindowUtil.CreateAsset(exporter, "exporter.asset");
        }
        #endregion

        private static Vector2 maxSize { get; set; }
        public static RuleObject RuleObj { get { return exporter.ruleObj; } }
        public static RuleObject  settingObj { get { return exporter.ruleObj; } }
        public static Exporter exporter { get; set; }
        private static Vector2 rootSize { get; set; }

        private static string exportPath
        {
            get
            {
                if (exporter != null)
                {
                    var _exportPath = AssetDatabase.GetAssetPath(exporter).Replace("/" + exporter.name + ".asset", "");
                    return _exportPath;
                }
                return null;
            }
        }
        public static void InitPsdExportEnvrioment(Exporter obj, Vector2 rootSize0)
        {
            exporter = obj;
            rootSize = rootSize0;
            idSpan = 1;
        }

        private static int idSpan;
        public static GroupNode[] CreatePictures(IPsdLayer[] rootLayers, Vector2 rootSize, Vector2 uiSize, bool forceSprite = false)
        {
            maxSize = uiSize;
            List<GroupNode> nodes = new List<GroupNode>();
            foreach (PsdLayer rootLayer in rootLayers)
            {
                if (rootLayer.IsGroup)
                {
                    var groupnode = new GroupNode(GetRectFromLayer(rootLayer), idSpan++, 0);
                    groupnode.Analyzing(ExportUtility.RuleObj, rootLayer.Name);// (rootLayer,rootLayer));
                    RetriveLayerToSwitchModle(rootSize, rootLayer, groupnode, forceSprite);
                    nodes.Add(groupnode);
                }
            }

            var pictureData = new List<ImgNode>();

            foreach (var groupnode in nodes)
            {
                groupnode.GetImgNodes(pictureData);
            }
            SwitchCreateTexture(pictureData);

            return nodes.ToArray();
        }

        private static void SwitchCreateTexture(List<ImgNode> pictureData)
        {
            //创建atlas
            var textures = new List<Texture2D>();
            foreach (var item in pictureData)
            {
                if (item.type == ImgType.AtlasImage)
                {
                    var tx = textures.Find(x => x.name == item.TextureName);
                    if (tx == null)
                    {
                        textures.Add(item.texture);
                    }
                    else
                    {
                        item.texture = tx;
                    }
                }
            }
            SaveToAtlas(textures.ToArray(), exporter);

            //创建Sprites
            var pictures = pictureData.FindAll(x => x.type == ImgType.Image).ConvertAll<Texture2D>(x => x.texture);
            SaveToTextures(ImgType.Image, pictures.ToArray(), exporter);
            //创建Sprites
            pictures = pictureData.FindAll(x => x.type == ImgType.Texture).ConvertAll<Texture2D>(x => x.texture);
            SaveToTextures(ImgType.Texture, pictures.ToArray(), exporter);

        }

        public static void ChargeTextures(Exporter pictureInfo, GroupNode groupnode)
        {
            //重新加载
            var atlaspath = exportPath + "/" + pictureInfo.CalcAtlasName();
            Sprite[] fileSprites = AssetDatabase.LoadAllAssetsAtPath(atlaspath).OfType<Sprite>().ToArray();

            Texture2D[] fileTextures = LoadAllObjectFromDir<Texture2D>(exportPath);// AssetDatabase.LoadAllAssetsAtPath(pictureInfo.exportPath).OfType<Texture2D>().ToArray();
            Sprite[] fileSingleSprites = LoadAllObjectFromDir<Sprite>(exportPath);// AssetDatabase.LoadAllAssetsAtPath(pictureInfo.exportPath).OfType<Sprite>().ToArray();

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

        private static T[] LoadAllObjectFromDir<T>(string dirName) where T : UnityEngine.Object
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
        public static void SaveToAtlas(Texture2D[] textureArray, Exporter pictureInfo)
        {
            if (textureArray.Length == 0) return;
            // The output of PackTextures returns a Rect array from which we can create our sprites
            Rect[] rects;
            Texture2D atlas = new Texture2D(pictureInfo.ruleObj.maxSize, pictureInfo.ruleObj.maxSize);
            rects = atlas.PackTextures(textureArray, 2, pictureInfo.ruleObj.maxSize);
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
            var atlaspath = exportPath + "/" + pictureInfo.CalcAtlasName();
            File.WriteAllBytes(Path.GetFullPath(atlaspath), buf);
            AssetDatabase.Refresh();
            // Get our texture that we loaded
            TextureImporter textureImporter = AssetImporter.GetAtPath(atlaspath) as TextureImporter;

            // Make sure the size is the same as our atlas then create the spritesheet
            textureImporter.maxTextureSize = pictureInfo.ruleObj.maxSize;
            textureImporter.spritesheet = Sprites.ToArray();
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
            textureImporter.spritePixelsPerUnit = pictureInfo.ruleObj.pixelsToUnitSize;
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
        public static void SaveToTextures(ImgType imgType, Texture2D[] textureArray, Exporter pictureInfo)
        {
            foreach (var texture in textureArray)
            {
                byte[] buf = texture.EncodeToPNG();
                var atlaspath = exportPath + "/" + string.Format(pictureInfo.ruleObj.picNameTemp, texture.name);
                File.WriteAllBytes(Path.GetFullPath(atlaspath), buf);
                AssetDatabase.Refresh();

                // Get our texture that we loaded
                TextureImporter textureImporter = AssetImporter.GetAtPath(atlaspath) as TextureImporter;

                // Make sure the size is the same as our atlas then create the spritesheet
                textureImporter.maxTextureSize = pictureInfo.ruleObj.maxSize;

                switch (imgType)
                {
                    case ImgType.Image:
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
                        textureImporter.spritePixelsPerUnit = pictureInfo.ruleObj.pixelsToUnitSize;
                        break;
                    case ImgType.Texture:
                        textureImporter.textureType = TextureImporterType.Default;
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
        public static ImgNode AnalysisLayer(Vector2 rootSize, PsdLayer layer, bool forceSprite = false)
        {
            ImgNode data = null;
            var texture = CreateTexture(layer);
            var rect = GetRectFromLayer(layer);
            switch (layer.LayerType)
            {
                case LayerType.Normal:
                    data = new ImgNode(rect, texture).SetIndex(GetLayerID(layer)).Analyzing(ExportUtility.RuleObj, layer.Name);
                    break;
                case LayerType.SolidImage:
                    if (forceSprite)
                    {
                        data = new ImgNode(rect, texture).SetIndex(GetLayerID(layer)).Analyzing(ExportUtility.RuleObj, layer.Name);
                    }
                    else
                    {
                        data = new ImgNode(layer.Name, rect, GetLayerColor(layer)).SetIndex(GetLayerID(layer));
                    }
                    break;
                case LayerType.Text:
                    var textInfo = layer.Records.TextInfo;
                    var color = new Color(textInfo.color[0], textInfo.color[1], textInfo.color[2], textInfo.color[3]);
                    data = new ImgNode(layer.Name, rect, textInfo.fontName, textInfo.fontSize, textInfo.text, color);
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

        private static int GetLayerID(PsdLayer layer)
        {
            int id = 0;
            var parent = layer.Parent;
            if (parent != null)
            {
                id = Array.IndexOf(parent.Childs, layer);
                id += 10 * GetLayerID(parent);
            }
            else
            {
                id = Array.IndexOf(layer.Document.Childs, layer);
            }
            return id;
        }
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
        /// <summary>
        /// 转换为组
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="group"></param>
        /// <param name="OnRetrive"></param>
        public static void RetriveLayerToSwitchModle(Vector2 rootSize, PsdLayer layer, GroupNode group, bool forceSprite = false)
        {
            if (!layer.IsGroup)
            {
                return;
            }
            else
            {
                float index = 0;
                foreach (var child in layer.Childs)
                {
                    var progress = ++index / layer.Childs.Length;
                    EditorUtility.DisplayProgressBar(layer.Name, "转换进度:" + progress, progress);

                    if (child.IsGroup)
                    {
                        GroupNode childNode = new GroupNode(GetRectFromLayer(child), idSpan++, group.depth + 1);
                        childNode.Analyzing(RuleObj, child.Name);
                        group.AddChild(childNode);

                        if (childNode != null)
                        {
                            RetriveLayerToSwitchModle(rootSize, child, childNode, forceSprite);
                        }
                    }
                    else
                    {
                        ImgNode imgnode = AnalysisLayer(rootSize, child, forceSprite);
                        if (imgnode != null)
                        {
                            group.images.Add(imgnode);
                        }
                    }

                }
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// 解析Layer中的尺寸信息
        /// </summary>
        /// <param name="psdLayer"></param>
        /// <returns></returns>
        public static Rect GetRectFromLayer(IPsdLayer psdLayer)
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
            return new Rect(xMin, yMin, width, height);
        }

    }
}