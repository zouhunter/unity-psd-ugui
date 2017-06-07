
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PhotoshopFile;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Drawing;
using Image = UnityEngine.UI.Image;
using Font = UnityEngine.Font;
using Color = UnityEngine.Color;
public enum TextJustification
{
    /// <summary>
    /// The text is left justified.
    /// </summary>
    Left,

    /// <summary>
    /// The text is right justified.
    /// </summary>
    Right,

    /// <summary>
    /// The text is center justified.
    /// </summary>
    Center
}

/// <summary>
/// Handles all of the importing for a PSD file (exporting textures, creating prefabs, etc).
/// </summary>
public static class PsdImporter
{
    /// <summary>
    /// The current file path to use to save layers as .png files
    /// </summary>
    private static string currentPath;

    /// <summary>
    /// The <see cref="GameObject"/> representing the root PSD layer.  It contains all of the other layers as children GameObjects.
    /// </summary>
    private static GameObject rootPsdGameObject;

    /// <summary>
    /// The <see cref="GameObject"/> representing the current group (folder) we are processing.
    /// </summary>
    private static GameObject currentGroupGameObject;

    /// <summary>
    /// The current depth (Z axis position) that sprites will be placed on.  It is initialized to the MaximumDepth ("back" depth) and it is automatically
    /// decremented as the PSD file is processed, back to front.
    /// </summary>
    private static float currentDepth;

    /// <summary>
    /// The amount that the depth decrements for each layer.  This is automatically calculated from the number of layers in the PSD file and the MaximumDepth.
    /// </summary>
    private static float depthStep;

    /// <summary>
    /// Initializes static members of the <see cref="PsdImporter"/> class.
    /// </summary>
    static PsdImporter()
    {
        MaximumDepth = 10;
        PixelsToUnits = 100;
    }

    /// <summary>
    /// Gets or sets the maximum depth.  This is where along the Z axis the back will be, with the front being at 0.
    /// </summary>
    public static float MaximumDepth { get; set; }

    /// <summary>
    /// Gets or sets the number of pixels per Unity unit value.  Defaults to 100 (which matches Unity's Sprite default).
    /// </summary>
    public static float PixelsToUnits { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use the Unity 4.6+ UI system or not.
    /// </summary>
    public static bool UseUnityUI { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the import process should create <see cref="GameObject"/>s in the scene.
    /// </summary>
    private static bool LayoutInScene { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the import process should create a prefab in the project's assets.
    /// </summary>
    private static bool CreatePrefab { get; set; }

    /// <summary>
    /// Gets or sets the size (in pixels) of the entire PSD canvas.
    /// </summary>
    private static Vector2 CanvasSize { get; set; }

    /// <summary>
    /// Gets or sets the name of the current 
    /// </summary>
    private static string PsdName { get; set; }

    /// <summary>
    /// Gets or sets the Unity 4.6+ UI canvas.
    /// </summary>
    private static GameObject Canvas { get; set; }

    /// <summary>
    /// Gets or sets the current <see cref="PsdFile"/> that is being imported.
    /// </summary>
    ////private static PsdFile CurrentPsdFile { get; set; }

    /// <summary>
    /// Exports each of the art layers in the PSD file as separate textures (.png files) in the project's assets.
    /// </summary>
    /// <param name="assetPath">The path of to the .psd file relative to the project.</param>
    public static void ExportLayersAsTextures(string assetPath)
    {
        LayoutInScene = false;
        CreatePrefab = false;
        Import(assetPath);
    }

    /// <summary>
    /// Lays out sprites in the current scene to match the PSD's layout.  Each layer is exported as Sprite-type textures in the project's assets.
    /// </summary>
    /// <param name="assetPath">The path of to the .psd file relative to the project.</param>
    public static void LayoutInCurrentScene(string assetPath)
    {
        LayoutInScene = true;
        CreatePrefab = false;
        Import(assetPath);
    }

    /// <summary>
    /// Generates a prefab consisting of sprites laid out to match the PSD's layout. Each layer is exported as Sprite-type textures in the project's assets.
    /// </summary>
    /// <param name="assetPath">The path of to the .psd file relative to the project.</param>
    public static void GeneratePrefab(string assetPath)
    {
        LayoutInScene = false;
        CreatePrefab = true;
        Import(assetPath);
    }

    /// <summary>
    /// Imports a Photoshop document (.psd) file at the given path.
    /// </summary>
    /// <param name="asset">The path of to the .psd file relative to the project.</param>
    private static void Import(string asset)
    {
        currentDepth = MaximumDepth;
        string fullPath = Path.Combine(GetFullProjectPath(), asset.Replace('\\', '/'));

        PsdFile psd = new PsdFile();
        psd.Load(fullPath, Encoding.Default);
        var baseLayer = psd.BaseLayer;
        CanvasSize = new Vector2(baseLayer.Rect.Width, baseLayer.Rect.Height);

        // Set the depth step based on the layer count.  If there are no layers, default to 0.1f.
        depthStep = psd.Layers.Count != 0 ? MaximumDepth / psd.Layers.Count : 0.1f;

        int lastSlash = asset.LastIndexOf('/');
        string assetPathWithoutFilename = asset.Remove(lastSlash + 1, asset.Length - (lastSlash + 1));
        PsdName = asset.Replace(assetPathWithoutFilename, string.Empty).Replace(".psd", string.Empty);

        currentPath = GetFullProjectPath() + "Assets";
        currentPath = Path.Combine(currentPath, PsdName);
        Directory.CreateDirectory(currentPath);

        if (LayoutInScene || CreatePrefab)
        {
            if (UseUnityUI)
            {
                CreateUIEventSystem();
                CreateUICanvas();
                rootPsdGameObject = Canvas;
            }
            else
            {
                rootPsdGameObject = new GameObject(PsdName);
            }

            currentGroupGameObject = rootPsdGameObject;
        }

        List<NodeLayer> tree = BuildLayerTree(psd.Layers);

        ExportTree(tree);

        if (CreatePrefab)
        {
            UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(asset.Replace(".psd", ".prefab"));
            PrefabUtility.ReplacePrefab(rootPsdGameObject, prefab);

            if (!LayoutInScene)
            {
                // if we are not flagged to layout in the scene, delete the GameObject used to generate the prefab
                UnityEngine.Object.DestroyImmediate(rootPsdGameObject);
            }
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Constructs a tree collection based on the PSD layer groups from the raw list of layers.
    /// </summary>
    /// <param name="flatLayers">The flat list of all layers.</param>
    /// <returns>The layers reorganized into a tree structure based on the layer groups.</returns>
    private static List<NodeLayer> BuildLayerTree(List<Layer> flatLayers)
    {
        // There is no tree to create if there are no layers
        if (flatLayers == null)
        {
            return null;
        }

        // PSD layers are stored backwards (with End Groups before Start Groups), so we must reverse them
        flatLayers.Reverse();

        List<NodeLayer> tree = new List<NodeLayer>();
        NodeLayer currentGroupLayer = null;
        Stack<NodeLayer> previousLayers = new Stack<NodeLayer>();

        foreach (Layer layer in flatLayers)
        {
            var rect = PsdUtility.GetRectByRectRange(layer.Rect);

            if (IsEndGroup(layer))
            {
                if (previousLayers.Count > 0)
                {
                    NodeLayer previousLayer = previousLayers.Pop();
                    previousLayer.Children.Add(currentGroupLayer);
                    currentGroupLayer = previousLayer;
                }
                else if (currentGroupLayer != null)
                {
                    tree.Add(currentGroupLayer);
                    currentGroupLayer = null;
                }
            }
            else if (IsStartGroup(layer))
            {
                // push the current layer
                if (currentGroupLayer != null)
                {
                    previousLayers.Push(currentGroupLayer);
                }

                currentGroupLayer = new NodeLayer(layer);
            }
            else if (rect.width != 0 && rect.height != 0)
            {
                // It must be a text layer or image layer
                if (currentGroupLayer != null)
                {
                    currentGroupLayer.Children.Add(new NodeLayer(layer));
                }
                else
                {
                    tree.Add(new NodeLayer(layer));
                }
            }
        }

        // if there are any dangling layers, add them to the tree
        if (tree.Count == 0 && currentGroupLayer != null && currentGroupLayer.Children.Count > 0)
        {
            tree.Add(currentGroupLayer);
        }

        return tree;
    }

    /// <summary>
    /// Fixes any layer names that would cause problems.
    /// </summary>
    /// <param name="name">The name of the layer</param>
    /// <returns>The fixed layer name</returns>
    private static string MakeNameSafe(string name)
    {
        // replace all special characters with an underscore
        Regex pattern = new Regex("[/:&.<>,$¢;+]");
        string newName = pattern.Replace(name, "_");

        if (name != newName)
        {
            Debug.Log(string.Format("Layer name \"{0}\" was changed to \"{1}\"", name, newName));
        }

        return newName;
    }

    /// <summary>
    /// Returns true if the given <see cref="Layer"/> is marking the start of a layer group.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to check if it's the start of a group</param>
    /// <returns>True if the layer starts a group, otherwise false.</returns>
    private static bool IsStartGroup(Layer layer)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);
        return layer.Name.Contains("<Layer set>") ||
            layer.Name.Contains("<Layer group>") ||
            (layer.Name == " copy" && rect.height == 0);
    }

    /// <summary>
    /// Returns true if the given <see cref="Layer"/> is marking the end of a layer group.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to check if it's the end of a group.</param>
    /// <returns>True if the layer ends a group, otherwise false.</returns>
    private static bool IsEndGroup(Layer layer)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);
        return layer.Name.Contains("</Layer set>") ||
            layer.Name.Contains("</Layer group>") ||
            (layer.Name == " copy" && rect.height == 0);
    }

    /// <summary>
    /// Gets full path to the current Unity project. In the form "C:/Project/".
    /// </summary>
    /// <returns>The full path to the current Unity project.</returns>
    private static string GetFullProjectPath()
    {
        string projectDirectory = Application.dataPath;

        // remove the Assets folder from the end since each imported asset has it already in its local path
        if (projectDirectory.EndsWith("Assets"))
        {
            projectDirectory = projectDirectory.Remove(projectDirectory.Length - "Assets".Length);
        }

        return projectDirectory;
    }

    /// <summary>
    /// Gets the relative path of a full path to an asset.
    /// </summary>
    /// <param name="fullPath">The full path to the asset.</param>
    /// <returns>The relative path to the asset.</returns>
    private static string GetRelativePath(string fullPath)
    {
        return fullPath.Replace(GetFullProjectPath(), string.Empty);
    }

    #region Layer Exporting Methods

    /// <summary>
    /// Processes and saves the layer tree.
    /// </summary>
    /// <param name="tree">The layer tree to export.</param>
    private static void ExportTree(List<NodeLayer> tree)
    {
        // we must go through the tree in reverse order since Unity draws from back to front, but PSDs are stored front to back
        for (int i = tree.Count - 1; i >= 0; i--)
        {
            ExportLayer(tree[i]);
        }
    }

    /// <summary>
    /// Exports a single layer from the tree.
    /// </summary>
    /// <param name="layer">The layer to export.</param>
    private static void ExportLayer(NodeLayer layer)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);
        layer.Name = MakeNameSafe(layer.Name);
        if (layer.Children.Count > 0 || rect.width == 0)
        {
            ExportFolderLayer(layer);
        }
        else
        {
            ExportArtLayer(layer);
        }
    }

    /// <summary>
    /// Exports a <see cref="Layer"/> that is a folder containing child layers.
    /// </summary>
    /// <param name="layer">The layer that is a folder.</param>
    private static void ExportFolderLayer(NodeLayer layer)
    {
        if (layer.Name.ContainsIgnoreCase("|Button"))
        {
            layer.Name = layer.Name.ReplaceIgnoreCase("|Button", string.Empty);

            if (UseUnityUI)
            {
                CreateUIButton(layer);
            }
            else
            {
                ////CreateGUIButton(layer);
            }
        }
        else if (layer.Name.ContainsIgnoreCase("|Animation"))
        {
            layer.Name = layer.Name.ReplaceIgnoreCase("|Animation", string.Empty);

            string oldPath = currentPath;
            GameObject oldGroupObject = currentGroupGameObject;

            currentPath = Path.Combine(currentPath, layer.Name.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
            Directory.CreateDirectory(currentPath);

            if (UseUnityUI)
            {
                ////CreateUIAnimation(layer);
            }
            else
            {
                CreateAnimation(layer);
            }

            currentPath = oldPath;
            currentGroupGameObject = oldGroupObject;
        }
        else
        {
            // it is a "normal" folder layer that contains children layers
            string oldPath = currentPath;
            GameObject oldGroupObject = currentGroupGameObject;

            currentPath = Path.Combine(currentPath, layer.Name);
            Directory.CreateDirectory(currentPath);

            if (LayoutInScene || CreatePrefab)
            {
                currentGroupGameObject = new GameObject(layer.Name);
                currentGroupGameObject.transform.parent = oldGroupObject.transform;
            }

            ExportTree(layer.Children);

            currentPath = oldPath;
            currentGroupGameObject = oldGroupObject;
        }
    }

    /// <summary>
    /// Checks if the string contains the given string, while ignoring any casing.
    /// </summary>
    /// <param name="source">The source string to check.</param>
    /// <param name="toCheck">The string to search for in the source string.</param>
    /// <returns>True if the string contains the search string, otherwise false.</returns>
    private static bool ContainsIgnoreCase(this string source, string toCheck)
    {
        return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    /// <summary>
    /// Replaces any instance of the given string in this string with the given string.
    /// </summary>
    /// <param name="str">The string to replace sections in.</param>
    /// <param name="oldValue">The string to search for.</param>
    /// <param name="newValue">The string to replace the search string with.</param>
    /// <returns>The replaced string.</returns>
    private static string ReplaceIgnoreCase(this string str, string oldValue, string newValue)
    {
        StringBuilder sb = new StringBuilder();

        int previousIndex = 0;
        int index = str.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
        while (index != -1)
        {
            sb.Append(str.Substring(previousIndex, index - previousIndex));
            sb.Append(newValue);
            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
        }

        sb.Append(str.Substring(previousIndex));

        return sb.ToString();
    }

    /// <summary>
    /// Exports an art layer as an image file and sprite.  It can also generate text meshes from text layers.
    /// </summary>
    /// <param name="layer">The art layer to export.</param>
    private static void ExportArtLayer(NodeLayer layer)
    {
        if (!layer.IsTextLayer)
        {
            if (LayoutInScene || CreatePrefab)
            {
                // create a sprite from the layer to lay it out in the scene
                if (!UseUnityUI)
                {
                    CreateSpriteGameObject(layer);
                }
                else
                {
                    CreateUIImage(layer);
                }
            }
            else
            {
                // it is not being laid out in the scene, so simply save out the .png file
                CreatePNG(layer);
            }
        }
        else
        {
            // it is a text layer
            if (LayoutInScene || CreatePrefab)
            {
                // create text mesh
                if (!UseUnityUI)
                {
                    CreateTextGameObject(layer);
                }
                else
                {
                    CreateUIText(layer);
                }
            }
        }
    }

    /// <summary>
    /// Saves the given <see cref="Layer"/> as a PNG on the hard drive.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to save as a PNG.</param>
    /// <returns>The filepath to the created PNG file.</returns>
    private static string CreatePNG(NodeLayer layer)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);

        string file = string.Empty;

        if (layer.Children.Count == 0 && rect.width > 0)
        {
            // decode the layer into a texture
            Texture2D texture = ImageDecoder.DecodeImage(layer);

            file = Path.Combine(currentPath, layer.Name + ".png");

            File.WriteAllBytes(file, texture.EncodeToPNG());
        }

        return file;
    }

    /// <summary>
    /// Creates a <see cref="Sprite"/> from the given <see cref="Layer"/>.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to use to create a <see cref="Sprite"/>.</param>
    /// <returns>The created <see cref="Sprite"/> object.</returns>
    private static Sprite CreateSprite(NodeLayer layer)
    {
        return CreateSprite(layer, PsdName);
    }

    /// <summary>
    /// Creates a <see cref="Sprite"/> from the given <see cref="Layer"/>.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to use to create a <see cref="Sprite"/>.</param>
    /// <param name="packingTag">The tag used for Unity's atlas packer.</param>
    /// <returns>The created <see cref="Sprite"/> object.</returns>
    private static Sprite CreateSprite(NodeLayer layer, string packingTag)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);

        Sprite sprite = null;

        if (layer.Children.Count == 0 && rect.width > 0)
        {
            string file = CreatePNG(layer);
            sprite = ImportSprite(GetRelativePath(file), packingTag);
        }

        return sprite;
    }

    /// <summary>
    /// Imports the <see cref="Sprite"/> at the given path, relative to the Unity project. For example "Assets/Textures/texture.png".
    /// </summary>
    /// <param name="relativePathToSprite">The path to the sprite, relative to the Unity project "Assets/Textures/texture.png".</param>
    /// <param name="packingTag">The tag to use for Unity's atlas packing.</param>
    /// <returns>The imported image as a <see cref="Sprite"/> object.</returns>
    private static Sprite ImportSprite(string relativePathToSprite, string packingTag)
    {
        AssetDatabase.ImportAsset(relativePathToSprite, ImportAssetOptions.ForceUpdate);

        // change the importer to make the texture a sprite
        TextureImporter textureImporter = AssetImporter.GetAtPath(relativePathToSprite) as TextureImporter;
        if (textureImporter != null)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.mipmapEnabled = false;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
            textureImporter.maxTextureSize = 2048;
            textureImporter.spritePixelsPerUnit = PixelsToUnits;
            textureImporter.spritePackingTag = packingTag;
        }

        AssetDatabase.ImportAsset(relativePathToSprite, ImportAssetOptions.ForceUpdate);

        Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(relativePathToSprite, typeof(Sprite));
        return sprite;
    }

    /// <summary>
    /// Creates a <see cref="GameObject"/> with a <see cref="TextMesh"/> from the given <see cref="Layer"/>.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to create a <see cref="TextMesh"/> from.</param>
    private static void CreateTextGameObject(NodeLayer layer)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);

        Color color = layer.FillColor;

        float x = rect.x / PixelsToUnits;
        float y = rect.y / PixelsToUnits;
        y = (CanvasSize.y / PixelsToUnits) - y;
        float width = rect.width / PixelsToUnits;
        float height = rect.height / PixelsToUnits;

        GameObject gameObject = new GameObject(layer.Name);
        gameObject.transform.position = new Vector3(x + (width / 2), y - (height / 2), currentDepth);
        gameObject.transform.parent = currentGroupGameObject.transform;

        currentDepth -= depthStep;

        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = font.material;

        TextMesh textMesh = gameObject.AddComponent<TextMesh>();
        textMesh.text = layer.Text;
        textMesh.font = font;
        textMesh.fontSize = 0;
        textMesh.characterSize = layer.FontSize / PixelsToUnits;
        textMesh.color = color;
        textMesh.anchor = TextAnchor.MiddleCenter;

        switch (layer.Justification)
        {
            case TextJustification.Left:
                textMesh.alignment = TextAlignment.Left;
                break;
            case TextJustification.Right:
                textMesh.alignment = TextAlignment.Right;
                break;
            case TextJustification.Center:
                textMesh.alignment = TextAlignment.Center;
                break;
        }
    }

    /// <summary>
    /// Creates a <see cref="GameObject"/> with a sprite from the given <see cref="Layer"/>
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to create the sprite from.</param>
    /// <returns>The <see cref="SpriteRenderer"/> component attached to the new sprite <see cref="GameObject"/>.</returns>
    private static SpriteRenderer CreateSpriteGameObject(NodeLayer layer)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);
        float x = rect.x / PixelsToUnits;
        float y = rect.y / PixelsToUnits;
        y = (CanvasSize.y / PixelsToUnits) - y;
        float width = rect.width / PixelsToUnits;
        float height = rect.height / PixelsToUnits;

        GameObject gameObject = new GameObject(layer.Name);
        gameObject.transform.position = new Vector3(x + (width / 2), y - (height / 2), currentDepth);
        gameObject.transform.parent = currentGroupGameObject.transform;

        currentDepth -= depthStep;

        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSprite(layer);
        return spriteRenderer;
    }

    /// <summary>
    /// Creates a Unity sprite animation from the given <see cref="Layer"/> that is a group layer.  It grabs all of the children art
    /// layers and uses them as the frames of the animation.
    /// </summary>
    /// <param name="layer">The group <see cref="Layer"/> to use to create the sprite animation.</param>
    private static void CreateAnimation(NodeLayer layer)
    {
        float fps = 30;

        string[] args = layer.Name.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string arg in args)
        {
            if (arg.ContainsIgnoreCase("FPS="))
            {
                layer.Name = layer.Name.Replace("|" + arg, string.Empty);

                string[] fpsArgs = arg.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (!float.TryParse(fpsArgs[1], out fps))
                {
                    Debug.LogError(string.Format("Unable to parse FPS: \"{0}\"", arg));
                }
            }
        }

        List<Sprite> frames = new List<Sprite>();

        var firstChild = layer.Children.First<NodeLayer>();
        SpriteRenderer spriteRenderer = CreateSpriteGameObject(firstChild);
        spriteRenderer.name = layer.Name;

        foreach (var child in layer.Children)
        {
            frames.Add(CreateSprite(child, layer.Name));
        }

        spriteRenderer.sprite = frames[0];

#if UNITY_5
        // Create Animator Controller with an Animation Clip
        UnityEditor.Animations.AnimatorController controller = new UnityEditor.Animations.AnimatorController();
        controller.AddLayer("Base Layer");

        UnityEditor.Animations.AnimatorControllerLayer controllerLayer = controller.layers[0];
        UnityEditor.Animations.AnimatorState state = controllerLayer.stateMachine.AddState(layer.Name);
        state.motion = CreateSpriteAnimationClip(layer.Name, frames, fps);

        AssetDatabase.CreateAsset(controller, GetRelativePath(currentPath) + "/" + layer.Name + ".controller");
#else // Unity 4
            // Create Animator Controller with an Animation Clip
            AnimatorController controller = new AnimatorController();
            AnimatorControllerLayer controllerLayer = controller.AddLayer("Base Layer");

            State state = controllerLayer.stateMachine.AddState(layer.Name);
            state.SetAnimationClip(CreateSpriteAnimationClip(layer.Name, frames, fps));

            AssetDatabase.CreateAsset(controller, GetRelativePath(currentPath) + "/" + layer.Name + ".controller");
#endif

        // Add an Animator and assign it the controller
        Animator animator = spriteRenderer.gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
    }

    /// <summary>
    /// Creates an <see cref="AnimationClip"/> of a sprite animation using the given <see cref="Sprite"/> frames and frames per second.
    /// </summary>
    /// <param name="name">The name of the animation to create.</param>
    /// <param name="sprites">The list of <see cref="Sprite"/> objects making up the frames of the animation.</param>
    /// <param name="fps">The frames per second for the animation.</param>
    /// <returns>The newly constructed <see cref="AnimationClip"/></returns>
    private static AnimationClip CreateSpriteAnimationClip(string name, IList<Sprite> sprites, float fps)
    {
        float frameLength = 1f / fps;

        AnimationClip clip = new AnimationClip();
        clip.name = name;
        clip.frameRate = fps;
        clip.wrapMode = WrapMode.Loop;

        // The AnimationClipSettings cannot be set in Unity (as of 4.6) and must be editted via SerializedProperty
        // from: http://forum.unity3d.com/threads/can-mecanim-animation-clip-properties-be-edited-in-script.251772/
        SerializedObject serializedClip = new SerializedObject(clip);
        SerializedProperty serializedSettings = serializedClip.FindProperty("m_AnimationClipSettings");
        serializedSettings.FindPropertyRelative("m_LoopTime").boolValue = true;
        serializedClip.ApplyModifiedProperties();

        EditorCurveBinding curveBinding = new EditorCurveBinding();
        curveBinding.type = typeof(SpriteRenderer);
        curveBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Count];

        for (int i = 0; i < sprites.Count; i++)
        {
            ObjectReferenceKeyframe kf = new ObjectReferenceKeyframe();
            kf.time = i * frameLength;
            kf.value = sprites[i];
            keyFrames[i] = kf;
        }

#if UNITY_5
        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
#else // Unity 4
            AnimationUtility.SetAnimationType(clip, ModelImporterAnimationType.Generic);
            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

            clip.ValidateIfRetargetable(true);
#endif

        AssetDatabase.CreateAsset(clip, GetRelativePath(currentPath) + "/" + name + ".anim");

        return clip;
    }

    #endregion

    #region Unity UI
    /// <summary>
    /// Creates the Unity UI event system game object that handles all input.
    /// </summary>
    private static void CreateUIEventSystem()
    {
        if (!GameObject.Find("EventSystem"))
        {
            GameObject gameObject = new GameObject("EventSystem");
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
        }
    }

    /// <summary>
    /// Creates a Unity UI <see cref="Canvas"/>.
    /// </summary>
    private static void CreateUICanvas()
    {
        Canvas = new GameObject(PsdName);

        Canvas canvas = Canvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform transform = Canvas.GetComponent<RectTransform>();
        Vector2 scaledCanvasSize = new Vector2(CanvasSize.x / PixelsToUnits, CanvasSize.y / PixelsToUnits);
        transform.sizeDelta = scaledCanvasSize;

        CanvasScaler scaler = Canvas.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = PixelsToUnits;
        scaler.referencePixelsPerUnit = PixelsToUnits;

        Canvas.AddComponent<GraphicRaycaster>();
    }

    /// <summary>
    /// Creates a Unity UI <see cref="UnityEngine.UI.Image"/> <see cref="GameObject"/> with a <see cref="Sprite"/> from a PSD <see cref="Layer"/>.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to use to create the UI Image.</param>
    /// <returns>The newly constructed Image object.</returns>
    private static Image CreateUIImage(NodeLayer layer)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);

        float x = rect.x / PixelsToUnits;
        float y = rect.y / PixelsToUnits;

        // Photoshop increase Y while going down. Unity increases Y while going up.  So, we need to reverse the Y position.
        y = (CanvasSize.y / PixelsToUnits) - y;

        // Photoshop uses the upper left corner as the pivot (0,0).  Unity defaults to use the center as (0,0), so we must offset the positions.
        x = x - ((CanvasSize.x / 2) / PixelsToUnits);
        y = y - ((CanvasSize.y / 2) / PixelsToUnits);

        float width = rect.width / PixelsToUnits;
        float height = rect.height / PixelsToUnits;

        GameObject gameObject = new GameObject(layer.Name);
        gameObject.transform.position = new Vector3(x + (width / 2), y - (height / 2), currentDepth);
        gameObject.transform.parent = currentGroupGameObject.transform;

        // if the current group object actually has a position (not a normal Photoshop folder layer), then offset the position accordingly
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + currentGroupGameObject.transform.position.x, gameObject.transform.position.y + currentGroupGameObject.transform.position.y, gameObject.transform.position.z);

        currentDepth -= depthStep;

        Image image = gameObject.AddComponent<Image>();
        image.sprite = CreateSprite(layer);

        RectTransform transform = gameObject.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(width, height);

        return image;
    }

    /// <summary>
    /// Creates a Unity UI <see cref="UnityEngine.UI.Text"/> <see cref="GameObject"/> with the text from a PSD <see cref="Layer"/>.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> used to create the <see cref="UnityEngine.UI.Text"/> from.</param>
    private static void CreateUIText(NodeLayer layer)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);

        Color color = layer.FillColor;

        float x = rect.x / PixelsToUnits;
        float y = rect.y / PixelsToUnits;

        // Photoshop increase Y while going down. Unity increases Y while going up.  So, we need to reverse the Y position.
        y = (CanvasSize.y / PixelsToUnits) - y;

        // Photoshop uses the upper left corner as the pivot (0,0).  Unity defaults to use the center as (0,0), so we must offset the positions.
        x = x - ((CanvasSize.x / 2) / PixelsToUnits);
        y = y - ((CanvasSize.y / 2) / PixelsToUnits);

        float width = rect.width / PixelsToUnits;
        float height = rect.height / PixelsToUnits;

        GameObject gameObject = new GameObject(layer.Name);
        gameObject.transform.position = new Vector3(x + (width / 2), y - (height / 2), currentDepth);
        gameObject.transform.parent = currentGroupGameObject.transform;

        currentDepth -= depthStep;

        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Text textUI = gameObject.AddComponent<Text>();
        textUI.text = layer.Text;
        textUI.font = font;
        textUI.rectTransform.sizeDelta = new Vector2(width, height);

        float fontSize = layer.FontSize / PixelsToUnits;
        float ceiling = Mathf.Ceil(fontSize);
        if (fontSize < ceiling)
        {
            // Unity UI Text doesn't support floating point font sizes, so we have to round to the next size and scale everything else
            float scaleFactor = ceiling / fontSize;
            textUI.fontSize = (int)ceiling;
            textUI.rectTransform.sizeDelta *= scaleFactor;
            textUI.rectTransform.localScale /= scaleFactor;
        }
        else
        {
            textUI.fontSize = (int)fontSize;
        }

        textUI.color = color;
        textUI.alignment = TextAnchor.MiddleCenter;

        switch (layer.Justification)
        {
            case TextJustification.Left:
                textUI.alignment = TextAnchor.MiddleLeft;
                break;
            case TextJustification.Right:
                textUI.alignment = TextAnchor.MiddleRight;
                break;
            case TextJustification.Center:
                textUI.alignment = TextAnchor.MiddleCenter;
                break;
        }
    }

    /// <summary>
    /// Creates a <see cref="UnityEngine.UI.Button"/> from the given <see cref="Layer"/>.
    /// </summary>
    /// <param name="layer">The Layer to create the Button from.</param>
    private static void CreateUIButton(NodeLayer layer)
    {
        // create an empty Image object with a Button behavior attached
        Image image = CreateUIImage(layer);
        Button button = image.gameObject.AddComponent<Button>();

        // look through the children for a clip rect
        ////Rectangle? clipRect = null;
        ////foreach (Layer child in layer.Children)
        ////{
        ////    if (child.Name.ContainsIgnoreCase("|ClipRect"))
        ////    {
        ////        clipRect = child.Rect;
        ////    }
        ////}

        // look through the children for the sprite states
        foreach (var child in layer.Children)
        {
            var rect = PsdUtility.GetRectByRectRange(child.Rect);
            if (child.Name.ContainsIgnoreCase("|Disabled"))
            {
                child.Name = child.Name.ReplaceIgnoreCase("|Disabled", string.Empty);
                button.transition = Selectable.Transition.SpriteSwap;

                SpriteState spriteState = button.spriteState;
                spriteState.disabledSprite = CreateSprite(child);
                button.spriteState = spriteState;
            }
            else if (child.Name.ContainsIgnoreCase("|Highlighted"))
            {
                child.Name = child.Name.ReplaceIgnoreCase("|Highlighted", string.Empty);
                button.transition = Selectable.Transition.SpriteSwap;

                SpriteState spriteState = button.spriteState;
                spriteState.highlightedSprite = CreateSprite(child);
                button.spriteState = spriteState;
            }
            else if (child.Name.ContainsIgnoreCase("|Pressed"))
            {
                child.Name = child.Name.ReplaceIgnoreCase("|Pressed", string.Empty);
                button.transition = Selectable.Transition.SpriteSwap;

                SpriteState spriteState = button.spriteState;
                spriteState.pressedSprite = CreateSprite(child);
                button.spriteState = spriteState;
            }
            else if (child.Name.ContainsIgnoreCase("|Default") ||
                     child.Name.ContainsIgnoreCase("|Enabled") ||
                     child.Name.ContainsIgnoreCase("|Normal") ||
                     child.Name.ContainsIgnoreCase("|Up"))
            {
                child.Name = child.Name.ReplaceIgnoreCase("|Default", string.Empty);
                child.Name = child.Name.ReplaceIgnoreCase("|Enabled", string.Empty);
                child.Name = child.Name.ReplaceIgnoreCase("|Normal", string.Empty);
                child.Name = child.Name.ReplaceIgnoreCase("|Up", string.Empty);

                image.sprite = CreateSprite(child);

                float x = rect.x / PixelsToUnits;
                float y = rect.y / PixelsToUnits;

                // Photoshop increase Y while going down. Unity increases Y while going up.  So, we need to reverse the Y position.
                y = (CanvasSize.y / PixelsToUnits) - y;

                // Photoshop uses the upper left corner as the pivot (0,0).  Unity defaults to use the center as (0,0), so we must offset the positions.
                x = x - ((CanvasSize.x / 2) / PixelsToUnits);
                y = y - ((CanvasSize.y / 2) / PixelsToUnits);

                float width = child.Rect.Width / PixelsToUnits;
                float height = child.Rect.Height / PixelsToUnits;

                image.gameObject.transform.position = new Vector3(x + (width / 2), y - (height / 2), currentDepth);

                RectTransform transform = image.gameObject.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(width, height);

                button.targetGraphic = image;
            }
            else if (child.Name.ContainsIgnoreCase("|Text") && !child.IsTextLayer)
            {
                child.Name = child.Name.ReplaceIgnoreCase("|Text", string.Empty);

                GameObject oldGroupObject = currentGroupGameObject;
                currentGroupGameObject = button.gameObject;

                // If the "text" is a normal art layer, create an Image object from the "text"
                CreateUIImage(child);

                currentGroupGameObject = oldGroupObject;
            }

            if (child.IsTextLayer)
            {
                // TODO: Create a child text game object
            }
        }
    }
    #endregion
}
