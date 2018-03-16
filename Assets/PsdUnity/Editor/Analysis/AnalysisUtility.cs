using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using Ntreev.Library.Psd;
using PSDUnity;
using PSDUnity.Analysis;

public class AnalysisUtility
{
    public static Dictionary<string, Texture> previewIcons = new Dictionary<string, Texture>();

    static AnalysisUtility()
    {
        InitPreviewIcons();
    }

    static void InitPreviewIcons()
    {
        previewIcons.Add(LayerType.Group.ToString(), EditorGUIUtility.FindTexture("GameObject Icon"));
        previewIcons.Add(LayerType.Normal.ToString(), EditorGUIUtility.IconContent("Image Icon").image);
        previewIcons.Add(LayerType.Color.ToString(), EditorGUIUtility.FindTexture("Material Icon"));
        previewIcons.Add(LayerType.Text.ToString(), EditorGUIUtility.IconContent("Text Icon").image);
        previewIcons.Add(LayerType.Other.ToString(), EditorGUIUtility.FindTexture("Favorite Icon"));
        previewIcons.Add(GroupType.BUTTON.ToString(), EditorGUIUtility.IconContent("Button Icon").image);
        previewIcons.Add(GroupType.TOGGLE.ToString(), EditorGUIUtility.IconContent("Toggle Icon").image);
        previewIcons.Add(GroupType.SLIDER.ToString(), EditorGUIUtility.IconContent("Slider Icon").image);
        previewIcons.Add(GroupType.SCROLLBAR.ToString(), EditorGUIUtility.IconContent("Scrollbar Icon").image);
        previewIcons.Add(GroupType.DROPDOWN.ToString(), EditorGUIUtility.IconContent("Dropdown Icon").image);
        previewIcons.Add(GroupType.CANVAS.ToString(), EditorGUIUtility.IconContent("Canvas Icon").image);
        previewIcons.Add(GroupType.RawIMAGE.ToString(), EditorGUIUtility.IconContent("RawImage Icon").image);
        previewIcons.Add(GroupType.INPUTFIELD.ToString(), EditorGUIUtility.IconContent("InputField Icon").image);
        previewIcons.Add(GroupType.SCROLLVIEW.ToString(), EditorGUIUtility.IconContent("ScrollRect Icon").image);
        previewIcons.Add(GroupType.GRID.ToString(), EditorGUIUtility.IconContent("GridLayoutGroup Icon").image);
        previewIcons.Add(GroupType.GROUP.ToString(), EditorGUIUtility.IconContent("GridLayoutGroup Icon").image);
    }

    internal static Texture GetPreviewIcon(PreviewItem item, RuleObject rule)
    {
        if(rule == null || item.layerType != LayerType.Group)
        {
            return previewIcons[item.layerType.ToString()];
        }
        else
        {
            GroupType groupType = GroupType.EMPTY;
            string[] args;
            /*string name = */rule.AnalysisGroupName(item.name, out groupType, out args);
            if(previewIcons.ContainsKey(groupType.ToString()) && previewIcons[groupType.ToString()] != null)
            {
                return previewIcons[groupType.ToString()];
            }
            else
            {
                return previewIcons[LayerType.Group.ToString()];
            }
        }
    }
}
