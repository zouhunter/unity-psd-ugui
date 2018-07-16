#define UNITY_2017
using Ntreev.Library.Psd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


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
            var exporter = ScriptableObject.CreateInstance<Data.Exporter>();
            exporter.psdFile = "";
            ProjectWindowUtil.CreateAsset(exporter, "exporter.asset");
        }
        #endregion

        private static Vector2 maxSize { get; set; }
        public static Data.RuleObject RuleObj { get { return exporter.ruleObj; } }
        public static Data.Exporter exporter { get; set; }
        private static Vector2 rootSize { get; set; }

        private static string exportPath
        {
            get
            {
                if (exporter != null)
                {
                    var assetFolder = AssetDatabase.GetAssetPath(exporter).Replace("/" + exporter.name + ".asset", "");
                    var _exportPath = System.IO.Path.Combine(assetFolder, exporter.ruleObj.subFolder);
                    if (!Directory.Exists(_exportPath))
                    {
                        Directory.CreateDirectory(_exportPath);
                    }
                    return _exportPath;
                }
                return null;
            }
        }
        public static void InitPsdExportEnvrioment(Data.Exporter obj, Vector2 rootSize0)
        {
            exporter = obj;
            rootSize = rootSize0;
            idSpan = 1;
        }

        private static int idSpan;
        public static GroupNodeItem[] CreatePictures(IPsdLayer[] rootLayers, Vector2 rootSize, Vector2 uiSize, bool forceSprite = false)
        {
            maxSize = uiSize;
            List<GroupNodeItem> nodes = new List<GroupNodeItem>();
            foreach (PsdLayer rootLayer in rootLayers)
            {
                if (rootLayer.IsGroup)
                {
                    var groupnode = new GroupNodeItem(GetRectFromLayer(rootLayer), idSpan++, 0);
                    groupnode.Analyzing(ExportUtility.RuleObj, rootLayer.Name);// (rootLayer,rootLayer));
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
            var simplyed = new List<Data.ImgNode>();
            foreach (var item in pictureData)
            {
                if (simplyed.Find(x => x.TextureName == item.TextureName) == null)
                {
                    simplyed.Add(item);
                }
            }
            #endregion

            SwitchCreateTexture(simplyed);
            return nodes.ToArray();
        }

        private static void SwitchCreateTexture(List<Data.ImgNode> pictureData)
        {
            //创建atlas(非全局)
            var atlasTextures = pictureData.FindAll(x => x.type == ImgType.AtlasImage && x.source != ImgSource.Globle);
            SaveToAtlas(atlasTextures.ToArray(), exporter);
            //创建atlas（全局）
            var globleAtlas = pictureData.FindAll(x => x.type == ImgType.AtlasImage && x.source == ImgSource.Globle);
            SaveToTextures(ImgType.Image, globleAtlas.ToArray(), exporter);
            //创建Sprites
            var singleNodes = pictureData.FindAll(x => x.type == ImgType.Image);
            SaveToTextures(ImgType.Image, singleNodes.ToArray(), exporter);
            //创建Textures
            singleNodes = pictureData.FindAll(x => x.type == ImgType.Texture);
            SaveToTextures(ImgType.Texture, singleNodes.ToArray(), exporter);
        }

        public static void ChargeTextures(Data.Exporter exporter, GroupNodeItem groupnode)
        {
            //重新加载
            var atlaspath = string.Format(exportPath + "/{0}.png", exporter.name);
            Texture2D[] globleTextures = LoadAllObjectFromDir<Texture2D>(exporter.ruleObj.globalTexture);
            Sprite[] globleSprites = LoadAllObjectFromDir<Sprite>(exporter.ruleObj.globalSprite);
            Sprite[] fileSprites = AssetDatabase.LoadAllAssetsAtPath(atlaspath).OfType<Sprite>().ToArray();
            Texture2D[] fileTextures = LoadAllObjectFromDir<Texture2D>(exportPath);// AssetDatabase.LoadAllAssetsAtPath(pictureInfo.exportPath).OfType<Texture2D>().ToArray();
            Sprite[] fileSingleSprites = LoadAllObjectFromDir<Sprite>(exportPath);// AssetDatabase.LoadAllAssetsAtPath(pictureInfo.exportPath).OfType<Sprite>().ToArray();

            var pictureData = new List<Data.ImgNode>();
            groupnode.GetImgNodes(pictureData);

            foreach (var item in pictureData)
            {
                if (item.source == ImgSource.Globle)
                {
                    switch (item.type)
                    {
                        case ImgType.Image:
                        case ImgType.AtlasImage:
                            item.sprite = Array.Find(globleSprites, x => x.name == item.TextureName);
                            break;
                        case ImgType.Texture:
                            item.texture = Array.Find(globleTextures, x => x.name == item.TextureName);
                            break;
                        default:
                            break;
                    }
                }
                else
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

        }

        private static T[] LoadAllObjectFromDir<T>(string dirName) where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();

            if (System.IO.Directory.Exists(dirName))
            {
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
            }

            return assets.ToArray();
        }

        /// <summary>
        /// 将一组图片保存为atlas
        /// </summary>
        /// <param name="imgNodes"></param>
        /// <param name="exporter"></param>
        /// <param name="atlasName"></param>
        /// <returns></returns>
        public static void SaveToAtlas(Data.ImgNode[] imgNodes, Data.Exporter exporter)
        {
            if (imgNodes.Length == 0) return;
            var textures = imgNodes.Where(x => x.texture != null).Select(x => x.texture).ToArray();
            // The output of PackTextures returns a Rect array from which we can create our sprites
            Rect[] rects;
            Texture2D atlas = new Texture2D(exporter.ruleObj.maxSize, exporter.ruleObj.maxSize);
            rects = atlas.PackTextures(textures, 2, exporter.ruleObj.maxSize);
            List<SpriteMetaData> Sprites = new List<SpriteMetaData>();

            // For each rect in the Rect Array create the sprite and assign to the SpriteMetaData
            for (int i = 0; i < rects.Length; i++)
            {
                // add the name and rectangle to the dictionary
                SpriteMetaData smd = new SpriteMetaData();
                smd.name = textures[i].name;
                smd.rect = new Rect(rects[i].xMin * atlas.width, rects[i].yMin * atlas.height, rects[i].width * atlas.width, rects[i].height * atlas.height);
                smd.pivot = new Vector2(0.5f, 0.5f); // Center is default otherwise layers will be misaligned
                smd.alignment = (int)SpriteAlignment.Center;
                Sprites.Add(smd);
            }
            // Need to load the image first
            byte[] buf = EncordToPng(atlas);

            var atlaspath = string.Format(exportPath + "/{0}.png", exporter.name);
            File.WriteAllBytes(Path.GetFullPath(atlaspath), buf);
            AssetDatabase.Refresh();
            // Get our texture that we loaded
            TextureImporter textureImporter = AssetImporter.GetAtPath(atlaspath) as TextureImporter;

            // Make sure the size is the same as our atlas then create the spritesheet
            textureImporter.maxTextureSize = exporter.ruleObj.maxSize;
            textureImporter.spritesheet = Sprites.ToArray();
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritePivot = new Vector2(0.5f, 0.5f);

            ChargeTextureImportRule(textureImporter, exporter.ruleObj);

            AssetDatabase.ImportAsset(atlaspath, ImportAssetOptions.ForceUpdate);

            foreach (var node in imgNodes)
            {
                UnityEngine.Object.DestroyImmediate(node.texture);
            }
        }

        /// <summary>
        /// 将图片分别保存到本地
        /// </summary>
        /// <param name="imgType"></param>
        /// <param name="singleNodes"></param>
        /// <param name="pictureInfo"></param>
        public static void SaveToTextures(ImgType imgType, Data.ImgNode[] singleNodes,Data. Exporter pictureInfo)
        {
            foreach (var node in singleNodes)
            {
                byte[] buf = EncordToPng(node.texture);

                var rootPath = exportPath;

                if (node.source == ImgSource.Globle)
                {
                    if (node.type == ImgType.Image || node.type == ImgType.AtlasImage)
                    {
                        rootPath = pictureInfo.ruleObj.globalSprite;
                    }
                    else
                    {
                        rootPath = pictureInfo.ruleObj.globalTexture;
                    }

                    if (!Directory.Exists(rootPath))
                        Directory.CreateDirectory(rootPath);
                }

                var atlaspath = rootPath + "/" + string.Format("{0}.png", node.texture.name);
                File.WriteAllBytes(Path.GetFullPath(atlaspath), buf);
                AssetDatabase.Refresh();

                // Get our texture that we loaded
                TextureImporter textureImporter = AssetImporter.GetAtPath(atlaspath) as TextureImporter;

                // Make sure the size is the same as our atlas then create the spritesheet
                textureImporter.maxTextureSize = pictureInfo.ruleObj.maxSize;
                textureImporter.spritePackingTag = pictureInfo.name;
                switch (imgType)
                {
                    case ImgType.Image:
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.alphaIsTransparency = true;
                        textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
                        break;
                    case ImgType.Texture:
                        textureImporter.textureType = TextureImporterType.Default;
                        break;
                    default:
                        break;
                }
                ChargeTextureImportRule(textureImporter, exporter.ruleObj);
                AssetDatabase.ImportAsset(atlaspath, ImportAssetOptions.ForceUpdate);
            }


            foreach (Data.ImgNode node in singleNodes)
            {
                UnityEngine.Object.DestroyImmediate(node.texture);
            }

        }

        /// <summary>
        /// 加载图片导入规则
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="rule"></param>
        private static void ChargeTextureImportRule(TextureImporter textureImporter, Data.RuleObject rule)
        {
            textureImporter.spritePixelsPerUnit = rule.spritePixelsPerUnit;
            //textureImporter.textureCompression = rule.textureCompression;
            textureImporter.mipmapEnabled = rule.mipmapEnabled;
            textureImporter.isReadable = rule.isReadable;
            textureImporter.wrapMode = rule.wrapMode;
            textureImporter.filterMode = rule.filterMode;
        }

        /// <summary>
        /// 将图层解析为imgNode
        /// </summary>
        /// <param name="rootSize"></param>
        /// <param name="layer"></param>
        /// <param name="forceSprite"></param>
        /// <returns></returns>
        public static Data.ImgNode AnalysisLayer(string baseName, Vector2 rootSize, PsdLayer layer, bool forceSprite = false)
        {
            Data.ImgNode data = null;
            var texture = CreateTexture(layer);
            var rect = GetRectFromLayer(layer);
            switch (layer.LayerType)
            {
                case LayerType.Normal:
                    data = new Data.ImgNode(baseName, rect, texture).SetIndex(CalcuteLayerID(layer)).Analyzing(ExportUtility.RuleObj, layer.Name);
                    break;
                case LayerType.Color:
                    if (forceSprite)
                    {
                        data = new Data.ImgNode(baseName, rect, texture).SetIndex(CalcuteLayerID(layer)).Analyzing(ExportUtility.RuleObj, layer.Name);
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
                    data = new Data.ImgNode(baseName, rect, texture).SetIndex(CalcuteLayerID(layer)).Analyzing(ExportUtility.RuleObj, layer.Name);
                    break;
                default:
                    break;
            }
            return data;
        }

        /// <summary>
        /// 计算layer的id
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 从layer解析图片
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
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

            //Channel mask = Array.Find(layer.Channels, i => i.Type == ChannelType.Mask);

            //if (layer.HasMask && alpha != null && alpha.Data != null)
            //{
            //    Debug.Log(mask.Data.Length + ":" + alpha.Data.Length);
            //}
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
        public static void RetriveLayerToSwitchModle(Vector2 rootSize, PsdLayer layer, GroupNodeItem group, bool forceSprite = false)
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
                        GroupNodeItem childNode = new GroupNodeItem(GetRectFromLayer(child), idSpan++, group.depth + 1);
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

        /// <summary>
        /// 兼容unity2017和unity5.6
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        private static byte[] EncordToPng(this Texture2D texture)
        {
            try
            {
                var assemble = System.Reflection.Assembly.Load("UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
                if (assemble != null)
                {
                    var imageConvention = assemble.GetType("UnityEngine.ImageConversion");
                    if (imageConvention != null)
                    {
                        return imageConvention.GetMethod("EncodeToPNG", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod).Invoke(null, new object[] { texture }) as byte[];
                    }
                }
            }
            catch (Exception)
            {
                return texture.GetType().GetMethod("EncodeToPNG").Invoke(texture, null) as byte[];
            }

            return new byte[0];
        }
    }
}