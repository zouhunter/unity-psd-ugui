using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using Ntreev.Library.Psd;

public class AnalysisUtility
{
    public static Dictionary<LayerType, GUIContent> previewIcons = new Dictionary<LayerType, GUIContent>();

    static AnalysisUtility()
    {
        InitPreviewIcons();
    }

    static void InitPreviewIcons()
    {
        previewIcons.Add(LayerType.Group, EditorGUIUtility.IconContent("AssetLabelPartial@2x"));
        previewIcons.Add(LayerType.Normal, EditorGUIUtility.IconContent("ControlHighlight"));
        previewIcons.Add(LayerType.SolidImage, EditorGUIUtility.IconContent("iconselector back"));
        previewIcons.Add(LayerType.Text, EditorGUIUtility.IconContent("eventpin"));
        previewIcons.Add(LayerType.Divider, EditorGUIUtility.IconContent("Favorite Icon"));
    }
}
