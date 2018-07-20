using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using System;
using System.Linq;
using UnityEditorInternal;

namespace PSDUnity
{

    public class ExporterTreeView : TreeView
    {
        static class Styles
        {
            public static GUIStyle background = "RL Background";
            public static GUIStyle headerBackground = "RL Header";
        }

        private GroupNodeItem _root;
        public GroupNodeItem root
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
                Reload();
            }
        }

        public Data.RuleObject ruleObj { get; internal set; }

        private Dictionary<int, bool> imgDic = new Dictionary<int, bool>();
        private List<GroupNodeItem> rows = new List<GroupNodeItem>();
        private Dictionary<int, ReorderableList> imgListDic = new Dictionary<int, ReorderableList>();
        private const float imgNodeHeight = 40;
        public List<GroupNodeItem> selected = new List<GroupNodeItem>();

        public ExporterTreeView(TreeViewState state) : base(state)
        {
            customFoldoutYOffset = 3f;
            showBorder = true;
        }
        protected override TreeViewItem BuildRoot()
        {
            if (root == null)
            {
                root = new GroupNodeItem(new Rect(), 0, -1);
            }
            return root;
        }
        protected override IList<TreeViewItem> BuildRows(TreeViewItem rootItem)
        {
            if (rootItem == null)
            {
                Debug.LogError("tree model root is null. did you call SetData()?");
            }
            rows.Clear();
            AddChildrenRecursive<GroupNodeItem>(rootItem as GroupNodeItem, 0, rows, (x) => { return new GroupNodeItem(x.rect, x.id, x.depth); });
            var list = rows.ConvertAll<TreeViewItem>(x => x);
            return list;
        }
        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            if (imgDic.ContainsKey(item.id) && imgDic[item.id])
            {
                return GetImgsHeight((item as GroupNodeItem).images);
            }
            return 30f;
        }

        private void AddChildrenRecursive<T>(T parent, int depth, IList<T> newRows, System.Func<T, T> Copy) where T : TreeViewItem
        {
            if (parent.children == null) return;

            foreach (T child in parent.children)
            {
                var item = Copy(child);
                newRows.Add(child);

                if (child.hasChildren)
                {
                    if (IsExpanded(child.id))
                    {
                        AddChildrenRecursive(child, depth + 1, newRows, Copy);
                    }
                    else
                    {
                        item.children = CreateChildListForCollapsedParent();
                    }
                }
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            selected.Clear();
            foreach (var item in selectedIds)
            {
                selected.Add(rows.Find(x => x.id == item));
            }
        }

        #region Rename
        protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
        {
            var contentIndent = GetContentIndent(item);
            var bgRect = rowRect;
            bgRect.x = contentIndent;
            bgRect.width = Mathf.Max(bgRect.width - contentIndent, 155f) - 5f;
            bgRect.yMin += 2f;
            bgRect.yMax -= 2f;

            var headerRect = bgRect;
            headerRect.xMin += 5f;
            headerRect.xMax -= 10f;
            headerRect.height = Styles.headerBackground.fixedHeight;

            return headerRect;
        }
        //--------
        protected override bool CanRename(TreeViewItem item)
        {
            // Only allow rename if we can show the rename overlay with a certain width (label might be clipped by other columns)
            Rect renameRect = GetRenameRect(treeViewRect, 0, item);
            return renameRect.width > 32;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            // Set the backend name and reload the tree to reflect the new model
            if (args.acceptedRename)
            {
                var element = rows.Find(x => x.id == args.itemID);
                element.displayName = args.newName;
                Reload();
            }
        }
        #endregion

        #region GUI
        protected override void RowGUI(RowGUIArgs args)
        {
            if (Event.current.type == EventType.MouseDown && args.rowRect.Contains(Event.current.mousePosition))
                SelectionClick(args.item, false);

            var item = (GroupNodeItem)args.item;
            var contentIndent = GetContentIndent(item);

            // Background
            var bgRect = args.rowRect;
            bgRect.x = contentIndent;
            bgRect.width = Mathf.Max(bgRect.width - contentIndent, 155f) - 5f;
            bgRect.yMin += 2f;
            bgRect.yMax -= 2f;
            DrawItemBackground(bgRect);

            // Custom label
            var headerRect = bgRect;
            headerRect.xMin += 5f;
            headerRect.xMax -= 10f;
            headerRect.height = Styles.headerBackground.fixedHeight;
            HeaderGUI(headerRect, args.label, item);

            // Controls
            var controlsRect = headerRect;
            controlsRect.xMin += 20f;
            controlsRect.y += headerRect.height;
            if (imgDic[item.id])
                ControlsGUI(controlsRect, item);
        }
        private void ControlsGUI(Rect controlsRect, GroupNodeItem item)
        {
            var rectRect = new Rect(controlsRect.x, controlsRect.y, 120, controlsRect.width * 0.2f);
            item.rect = EditorGUI.RectField(rectRect, item.rect);
            if (!imgListDic.ContainsKey(item.id))
            {
                var reorder = new ReorderableList(item.images, typeof(Data.ImgNode), true, true, true, true);
                reorder.elementHeight = imgNodeHeight;
                reorder.drawHeaderCallback = (r) => { EditorGUI.LabelField(r, "Contents"); };
                reorder.drawElementCallback = (rect, index, isActive, isFocused) => DrawElementCallBack(item.images, rect, index, isActive, isFocused);
                reorder.onChangedCallback = (x) => { Reload(); };
                imgListDic.Add(item.id, reorder);
            }
            var imgsRect = new Rect(controlsRect.x + 120, controlsRect.y, controlsRect.width - 120, controlsRect.height);
            imgListDic[item.id].DoList(imgsRect);
        }
        void HeaderGUI(Rect headerRect, string label, GroupNodeItem item)
        {
            headerRect.y += 1f;
            HeaderInfos(headerRect, item);

            // Do toggle
            Rect toggleRect = headerRect;
            toggleRect.width = 16;
            EditorGUI.BeginChangeCheck();
            if (!imgDic.ContainsKey(item.id)) imgDic[item.id] = false;
            imgDic[item.id] = EditorGUI.Toggle(toggleRect, imgDic[item.id]); // hide when outside cell rect
            if (EditorGUI.EndChangeCheck())
                RefreshCustomRowHeights();

            Rect labelRect = headerRect;
            labelRect.xMin += toggleRect.width + 2f;
            GUI.Label(labelRect, label);
        }
        void HeaderInfos(Rect rect, GroupNodeItem item)
        {
            var typeRect = new Rect(EditorGUIUtility.currentViewWidth - 110, rect.y, 100, rect.height);
            AnalysisUtility.InitEnviroment(ruleObj);
            UGUI.LayerImportGUI layerImportEditor = AnalysisUtility.GetLayerEditor(item.data.suffix);
            //Debug.Log(layerImportEditor);

            var index = Array.IndexOf(AnalysisUtility.layerImportEditorOptions, item.data.suffix);

            index = EditorGUI.Popup(typeRect, index, AnalysisUtility.layerImportEditorOptions, EditorStyles.miniLabel);
            if (index >= 0)
            {
                item.data.suffix = AnalysisUtility.layerImportEditorOptions[index];
            }

            var dirRect = new Rect(EditorGUIUtility.currentViewWidth - 210, rect.y, 100, rect.height);
            layerImportEditor.HeadGUI(dirRect,item.data);
        }
        void DrawItemBackground(Rect bgRect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var rect = bgRect;
                rect.height = Styles.headerBackground.fixedHeight;
                Styles.headerBackground.Draw(rect, false, false, false, false);

                rect.y += rect.height;
                rect.height = bgRect.height - rect.height;
                Styles.background.Draw(rect, false, false, false, false);
            }
        }
        public override void OnGUI(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
                DefaultStyles.backgroundOdd.Draw(rect, false, false, false, false);

            base.OnGUI(rect);
        }
        private float GetImgsHeight(List<Data.ImgNode> imgList)
        {
            return imgNodeHeight * imgList.Count + 70;
        }
        private void DrawElementCallBack(List<Data.ImgNode> imgs, Rect rect, int index, bool isActive, bool isFocused)
        {
            var img = imgs[index];
            var nameRect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
            img.Name = EditorGUI.TextField(nameRect, img.Name);

            var typeRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, 100, EditorGUIUtility.singleLineHeight);
            img.type = (ImgType)EditorGUI.EnumPopup(typeRect, img.type);

            var rectRect = new Rect(rect.x + 100, rect.y, 200, rect.height);
            img.rect = EditorGUI.RectField(rectRect, img.rect);

            var colorRect = new Rect(rect.x + 300, rect.y, (rect.width - 300) * 0.5f, EditorGUIUtility.singleLineHeight);
            var textRect = colorRect;
            textRect.x += (rect.width - 300) * 0.5f;

            var fontRect = colorRect;
            fontRect.y += EditorGUIUtility.singleLineHeight;

            var fontSizeRect = textRect;
            fontSizeRect.y += EditorGUIUtility.singleLineHeight;

            var sourceRect = colorRect;
            var spriteRect = textRect;
            var textureRect = spriteRect;
            switch (img.type)
            {
                case ImgType.Label:
                    img.font = EditorGUI.ObjectField(fontRect, img.font, typeof(Font), false) as Font;
                    img.fontSize = EditorGUI.IntSlider(fontSizeRect, img.fontSize, 1, 100);
                    img.text = EditorGUI.TextField(textRect, img.text);
                    img.color = EditorGUI.ColorField(colorRect, img.color);
                    break;
                case ImgType.Image:
                case ImgType.AtlasImage:
                    img.source = (ImgSource)EditorGUI.EnumPopup(sourceRect, img.source);
                    img.sprite = EditorGUI.ObjectField(spriteRect, img.sprite, typeof(Sprite), false) as Sprite;
                    break;
                case ImgType.Texture:
                    img.source = (ImgSource)EditorGUI.EnumPopup(sourceRect, img.source);
                    img.texture = EditorGUI.ObjectField(textureRect, img.texture, typeof(Texture2D), false) as Texture2D;
                    break;
                case ImgType.Color:
                    img.color = EditorGUI.ColorField(colorRect, img.color);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Drag
        // Dragging
        //-----------

        const string k_GenericDragID = "GenericDragColumnDragging";

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            if (hasSearch)
                return;

            DragAndDrop.PrepareStartDrag();
            var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
            DragAndDrop.SetGenericData(k_GenericDragID, draggedRows);
            DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
            string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
            DragAndDrop.StartDrag(title);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            // Check if we can handle the current drag data (could be dragged in from other areas/windows in the editor)
            var draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<TreeViewItem>;
            if (draggedRows == null)
                return DragAndDropVisualMode.None;

            // Parent item is null when dragging outside any tree view items.
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                    {
                        bool validDrag = ValidDrag(args.parentItem, draggedRows);
                        if (args.performDrop && validDrag)
                        {
                            var parentData = (GroupNodeItem)args.parentItem;
                            OnDropDraggedElementsAtIndex(draggedRows, parentData, args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
                        }
                        return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
                    }

                case DragAndDropPosition.OutsideItems:
                    {
                        if (args.performDrop)
                            OnDropDraggedElementsAtIndex(draggedRows, root, root.children.Count);

                        return DragAndDropVisualMode.Move;
                    }
                default:
                    Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
                    return DragAndDropVisualMode.None;
            }
        }

        public virtual void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, GroupNodeItem parent, int insertIndex)
        {
            var draggedElements = new List<TreeViewItem>();
            foreach (var x in draggedRows)
                draggedElements.Add(((GroupNodeItem)x));

            var selectedIDs = draggedElements.Select(x => x.id).ToArray();
            MoveElements(parent, insertIndex, draggedElements);
            SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
        }

        bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
        {
            TreeViewItem currentParent = parent;
            while (currentParent != null)
            {
                if (draggedItems.Contains(currentParent))
                    return false;
                currentParent = currentParent.parent;
            }
            return true;
        }
        public void MoveElements(TreeViewItem parentElement, int insertionIndex, List<TreeViewItem> elements)
        {
            if (insertionIndex < 0)
                throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");

            // Invalid reparenting input
            if (parentElement == null)
                return;

            // We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
            if (insertionIndex > 0)
                insertionIndex -= parentElement.children.GetRange(0, insertionIndex).Count(elements.Contains);

            // Remove draggedItems from their parents
            foreach (var draggedItem in elements)
            {
                draggedItem.parent.children.Remove(draggedItem);    // remove from old parent
                draggedItem.parent = parentElement;                 // set new parent
            }

            if (parentElement.children == null)
                parentElement.children = new List<TreeViewItem>();

            // Insert dragged items under new parent
            parentElement.children.InsertRange(insertionIndex, elements);

            TreeViewUtility.UpdateDepthValues(root);
            TreeViewUtility.TreeToList(root, rows);
        }
        #endregion
    }

}