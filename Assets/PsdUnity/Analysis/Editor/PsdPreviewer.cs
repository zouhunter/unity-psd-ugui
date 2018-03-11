using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using Ntreev.Library.Psd;

namespace PSDUnity.Analysis
{
    public class PsdPreviewer : TreeView
    {
        private PsdDocument psd;
        private static int id;
        List<PreviewItem> m_Rows = new List<PreviewItem>();
        const float kRowHeights = 20f;
        const float kToggleWidth = 18f;

        public PsdPreviewer(TreeViewState state, PsdDocument psd) : base(state)
        {
            this.psd = psd;
            var m_MultiColumnHeaderState = CreateDefaultMultiColumnHeaderState();
            this.multiColumnHeader = new MultiColumnHeader(m_MultiColumnHeaderState);
            this.multiColumnHeader.canSort = false;

            this.rowHeight = kRowHeights;
            this.columnIndexForTreeFoldouts = 1;
            this.showAlternatingRowBackgrounds = true;
            this.showBorder = true;
            this.customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            this.extraSpaceBeforeIconAndLabel = kToggleWidth;

            Reload();

        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new PreviewItem(id, -1, psd);
            GenerateRowsRecursive(root, 0);

            return root;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem rootItem)
        {
            if (rootItem == null){
                Debug.LogError("tree model root is null. did you call SetData()?");
            }
            //Debug.Log(rootItem);

            var root = rootItem as PreviewItem;
            m_Rows.Clear();
            if (!string.IsNullOrEmpty(searchString))
            {
                Search(root, searchString, m_Rows);
            }
            else
            {
                if (root.hasChildren)
                {
                    AddChildrenRecursive(root, 0, m_Rows);
                }
            }

            var list = m_Rows.ConvertAll<TreeViewItem>(x => x);
            return list;
        }
        void Search(PreviewItem searchFromThis, string search, List<PreviewItem> result)
        {
            if (string.IsNullOrEmpty(search))
                throw new ArgumentException("Invalid search: cannot be null or empty", "search");

            const int kItemDepth = 0; // tree is flattened when searching

            Stack<PreviewItem> stack = new Stack<PreviewItem>();
            foreach (var element in searchFromThis.children)
                stack.Push((PreviewItem)element);
            while (stack.Count > 0)
            {
                PreviewItem current = stack.Pop();
                // Matches search?
                if (current.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(new PreviewItem(current.id, kItemDepth, current.layer));
                }

                if (current.children != null && current.children.Count > 0)
                {
                    foreach (var element in current.children)
                    {
                        stack.Push((PreviewItem)element);
                    }
                }
            }
            result.Sort((x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName)); // sort by displayName by default, can be overriden for multicolumn solutions
        }

        void GenerateRowsRecursive(PreviewItem parent, int depth)
        {
            if (parent.layer.Childs == null || parent.layer.Childs.Length == 0)
            {
                return;
            }
            else
            {
                foreach (var layer in parent.layer.Childs)
                {
                    var item = new PreviewItem(++id, depth, layer);
                    parent.AddChild(item);
                    GenerateRowsRecursive(item, depth + 1);
                }
            }
        }

        void AddChildrenRecursive(PreviewItem parent, int depth, IList<PreviewItem> newRows)
        {
            foreach (PreviewItem child in parent.children)
            {
                var item = new PreviewItem(child.id, depth, child.layer);
                newRows.Add(child);

                if (child.hasChildren)
                {
                    if (IsExpanded(child.id))
                    {
                        AddChildrenRecursive(child, depth + 1, newRows);
                    }
                    else
                    {
                        item.children = CreateChildListForCollapsedParent();
                    }
                }
            }
        }

        protected override bool CanBeParent(TreeViewItem item)
        {
            return base.CanBeParent(item);
        }
        protected override void BeforeRowsGUI()
        {
            base.BeforeRowsGUI();
            EditorGUILayout.LabelField("列表");
        }
        protected override void AfterRowsGUI()
        {
            base.AfterRowsGUI();
        }

        protected override bool CanChangeExpandedState(TreeViewItem item)
        {
            return base.CanChangeExpandedState(item);
        }
        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return base.CanMultiSelect(item);
        }
        protected override bool CanRename(TreeViewItem item)
        {
            return base.CanRename(item);
        }
        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return base.CanStartDrag(args);
        }
        protected override void CommandEventHandling()
        {
            base.CommandEventHandling();
        }

        protected override void ContextClicked()
        {
            base.ContextClicked();
        }

        protected override void ContextClickedItem(int id)
        {
            base.ContextClickedItem(id);
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            return base.DoesItemMatchSearch(item, search);
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
        }

        protected override void ExpandedStateChanged()
        {
            base.ExpandedStateChanged();
        }

        protected override IList<int> GetAncestors(int id)
        {
            return base.GetAncestors(id);
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            return base.GetCustomRowHeight(row, item);
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            return base.GetDescendantsThatHaveChildren(id);
        }

        protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
        {
            Rect cellRect = GetCellRectForTreeFoldouts(rowRect);
            CenterRectUsingSingleLineHeight(ref cellRect);
            return base.GetRenameRect(cellRect, row, item);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            return base.HandleDragAndDrop(args);
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
        }

        protected override void RefreshCustomRowHeights()
        {
            base.RefreshCustomRowHeights();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (PreviewItem)args.item;
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                var rect = args.GetCellRect(i);
                CenterRectUsingSingleLineHeight(ref rect);
                    //Debug.Log(rect);
                var clums = args.GetColumn(i);
                if (clums == 0)
                {
                    var texture = AnalysisUtility.previewIcons[item.layerType];
                    GUI.Label(rect, texture);
                    GUILayoutUtility.GetRect(rect.width, rect.height);
                }
                else if (clums == 1)
                {
                    args.rowRect = rect;
                    base.RowGUI(args);
                }
            }
        }
        protected override void RenameEnded(RenameEndedArgs args)
        {
            base.RenameEnded(args);
        }

        protected override void SearchChanged(string newSearch)
        {
            base.SearchChanged(newSearch);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            base.SetupDragAndDrop(args);
        }

        public override IList<TreeViewItem> GetRows()
        {
            return base.GetRows();
        }

        protected override void KeyEvent()
        {
            base.KeyEvent();
        }


        static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByLabel"), "Lorem ipsum dolor sit amet, consectetur adipiscing elit. "),
                    contextMenuText = "Asset",
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 30,
                    minWidth = 30,
                    maxWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true
                }
            };

            var state = new MultiColumnHeaderState(columns);
            return state;
        }
    }
}