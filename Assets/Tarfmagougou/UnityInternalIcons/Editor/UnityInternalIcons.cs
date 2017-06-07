/*
 *	Created by Philippe Groarke on 2016-08-28.
 *	Copyright (c) 2016 Tarfmagougou Games. All rights reserved.
 *
 *	Dedication : I dedicate this code to Gabriel, who makes kickass extensions. Now go out and use awesome icons!
 */

namespace tarfmagougou
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    struct BuiltinIcon : System.IEquatable<BuiltinIcon>, System.IComparable<BuiltinIcon>
    {
        public GUIContent icon;
        public GUIContent name;

        public override bool Equals(object o)
        {
            return o is BuiltinIcon && this.Equals((BuiltinIcon)o);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public bool Equals(BuiltinIcon o)
        {
            return this.name.text == o.name.text;
        }

        public int CompareTo(BuiltinIcon o)
        {
            return this.name.text.CompareTo(o.name.text);
        }
    }

    public class UnityInternalIcons : EditorWindow
    {
        List<BuiltinIcon> _icons = new List<BuiltinIcon>();
        Vector2 _scroll_pos;
        GUIContent _refresh_button;

        [MenuItem("Window/Unity Internal Icons")]
        public static void ShowWindow()
        {
            UnityInternalIcons w = EditorWindow.GetWindow<UnityInternalIcons>();
            TarfmagougouHelperUII.SetWindowTitle(w, "Internal Icons");
        }

        void OnEnable()
        {
            _refresh_button = new GUIContent(EditorGUIUtility.IconContent("d_preAudioLoopOff").image,
                "Refresh : Icons are only loaded in memory when the appropriate window is opened.");

            FindIcons();
        }

        /* Find all textures and filter them to narrow the search. */
        void FindIcons()
        {
            _icons.Clear();

            Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D x in t)
            {
                if (x.name.Length == 0)
                    continue;

                if (x.hideFlags != HideFlags.HideAndDontSave && x.hideFlags != (HideFlags.HideInInspector | HideFlags.HideAndDontSave))
                    continue;

                if (!EditorUtility.IsPersistent(x))
                    continue;

                /* This is the *only* way I have found to confirm the icons are indeed unity builtin. Unfortunately
				 * it uses LogError instead of LogWarning or throwing an Exception I can catch. So make it shut up. */
                TarfmagougouHelperUII.DisableLogging();
                GUIContent gc = EditorGUIUtility.IconContent(x.name);
                TarfmagougouHelperUII.EnableLogging();

                if (gc == null)
                    continue;
                if (gc.image == null)
                    continue;

                _icons.Add(new BuiltinIcon()
                {
                    icon = gc,
                    name = new GUIContent(x.name)
                });
            }

            _icons.Sort();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            Repaint();
        }

        void OnGUI()
        {
            _scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button(_refresh_button, EditorStyles.toolbarButton))
            {
                FindIcons();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Found " + _icons.Count + " icons");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Double-click name to copy", TarfmagougouHelperUII.GetMiniGreyLabelStyle());

            EditorGUILayout.Space();

            EditorGUIUtility.labelWidth = 100;

            var k = 0;
            var con = 6;
            var raw = Mathf.CeilToInt(_icons.Count / con);

            for (int j = 0; j < raw; j++)
            {
                using (var hor = new EditorGUILayout.HorizontalScope())
                {
                    for (int i = 0; i < con; ++i)
                    {
                        if (_icons[k].icon.image == null) continue;
                        var btn = GUILayout.Button(_icons[k].icon/*, _icons[i].name*/);
                        if (btn)
                        {
                            EditorGUIUtility.systemCopyBuffer = _icons[k].name.text;
                            Debug.Log(_icons[k].name.text + " copied to clipboard.");
                        }
                        k++;
                        if (k == _icons.Count) break;
                    }
                }

            }


            EditorGUILayout.EndScrollView();
        }
    }
}
