using System;
using UnityEngine;
using UnityEngine.Events;
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
        private Data.RuleObject rule;
        private static int id;
        List<PreviewItem> m_Rows = new List<PreviewItem>();
        const float kRowHeights = 20f;
        const float kToggleWidth = 18f;
        public Texture currentTexture { get; private set; }
        public List<PreviewItem> selected = new List<PreviewItem>();
        public bool autoGroupTexture;
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
                if (root.hasChildren){
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
            Debug.Log(id);
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
        protected override bool CanRename(TreeViewItem item)
        {
            // Only allow rename if we can show the rename overlay with a certain width (label might be clipped by other columns)
            Rect renameRect = GetRenameRect(treeViewRect, 0, item);
            return renameRect.width > 30;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            // Set the backend name and reload the tree to reflect the new model
            if (args.acceptedRename)
            {
                var element = m_Rows.Find(x=>x.id == args.itemID);
                //element.name = args.newName;
                Debug.Log(element.layer + " current can`t rename,sorry !!!");
                this.Repaint();
            }
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

            if (Event.current.type == EventType.MouseDown && args.rowRect.Contains(Event.current.mousePosition))
                SelectionClick(args.item, false);

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                var rect = args.GetCellRect(i);
                CenterRectUsingSingleLineHeight(ref rect);
                    //Debug.Log(rect);
                var clums = args.GetColumn(i);
                if (clums == 0)
                {
                    GUI.Label(rect, args.row.ToString());
                }
                else if (clums == 1)
                {
                    Rect toggleRect = rect;
                    toggleRect.x += GetContentIndent(item);
                    toggleRect.width = kToggleWidth;

                    var texture = AnalysisUtility.GetPreviewIcon(item,rule);
                    GUI.DrawTexture(toggleRect, texture);

                    args.rowRect = rect;
                    base.RowGUI(args);
                }
            }

           
        }
        protected override void SearchChanged(string newSearch)
        {
            base.SearchChanged(newSearch);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            selected.Clear();
            for (int i = 0; i < selectedIds.Count; i++)
            {
                var item = m_Rows.Find(x => x.id == selectedIds[i]);
                if(item != null && item.layer != null)
                {
                    selected.Add(item);
                }
            }
            GenerateTexture(autoGroupTexture);
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
                    width = 300,
                    minWidth = 100,
                    autoResize = false,
                    allowToggleVisibility = true
                }
            };

            var state = new MultiColumnHeaderState(columns);
            return state;
        }
        public void SetRule(Data.RuleObject rule)
        {
            this.rule = rule;
        }
        public void GenerateTexture(bool all)
        {
            if (selected.Count == 0) return;

            selected.Sort((x,y)=> { return -string.Compare(x.depth.ToString(), y.depth.ToString()); });
            List<KeyValuePair<PsdLayer, Texture2D>> textureList = new List<KeyValuePair<PsdLayer, Texture2D>>();
            int maxWidth=0,maxHeight = 0;
            List<PreviewItem> artItems = new List<PreviewItem>();

            for (int i = 0; i < selected.Count; i++)
            {
                var root = selected[i];
                if(all)
                {
                    RetriveArtLayer(root, (x) =>
                    {
                        if (!artItems.Contains(x))
                        {
                            artItems.Add(x);
                        }
                    });
                }
                else if(root.layerType != LayerType.Group && root.layerType != LayerType.Overflow)
                {
                    artItems.Add(root);
                }
            }

            if (artItems.Count == 0) return;

            for (int i = 0; i < artItems.Count; i++)
            {
                var root = artItems[i];
                var titem = ExportUtility.CreateTexture((PsdLayer)root.layer);
                textureList.Add(new KeyValuePair<PsdLayer, Texture2D>((PsdLayer)root.layer, titem));
                maxHeight = titem.height > maxHeight ? titem.height : maxHeight;
                maxWidth = titem.width > maxWidth ? titem.width : maxWidth;
            }

            Texture2D texture = new Texture2D(psd.Width, psd.Height);

            foreach (var titem in textureList)
            {
                for (int x = 0; x < titem.Value.width; x++)
                {
                    for (int y = 0; y < titem.Value.height; y++)
                    {
                        var color = titem.Value.GetPixel(x, y);
                        if (color != Color.clear)
                            texture.SetPixel(x + titem.Key.Left, psd.Height - (titem.Value.height - y + titem.Key.Top), color);
                    }
                }
            }
            texture.Apply();
            currentTexture = texture;

        }

        private void RetriveArtLayer(PreviewItem data, UnityAction<PreviewItem> onRetrive)
        {
            if (data.layerType != LayerType.Group && data.layerType != LayerType.Overflow)
            {
                onRetrive(data);
            }
            else
            {
                if (data.children != null)
                    foreach (var item in data.children)
                    {
                        RetriveArtLayer((PreviewItem)item, onRetrive);
                    }
            }
        }

    }
}