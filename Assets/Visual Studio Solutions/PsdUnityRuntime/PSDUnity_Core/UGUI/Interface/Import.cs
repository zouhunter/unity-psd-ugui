using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PSDUnity;
namespace PSDUnity.UGUI
{
    public abstract class Import:ScriptableObject
    {
        protected PSDImportCtrl ctrl { get; private set; }
        public virtual void InitEnviroment(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }

        /// <summary>
        /// 根UGUI节点
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent"></param>
        protected UGUINode CreateRootNode(string name,Rect rect, UGUINode parent)
        {
            var go = CreateTemplate();
            go.name = name;
            go.layer = LayerMask.NameToLayer("UI");
            var node = CreateNormalNode(go, rect,  parent);
            node.inversionReprocess += AfterGenerate;
            return node;
        }

        /// <summary>
        /// 普通UGUI节点
        /// </summary>
        /// <param name="go"></param>
        /// <param name="rect"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public UGUINode CreateNormalNode(GameObject go, Rect rect, UGUINode parent)
        {
            go.transform.SetParent(ctrl.uinode.transform, false);
            SetRectTransform(rect, go.transform as RectTransform);
            UGUINode node = new UGUINode(go.transform, parent);
            return node;
        }

        /// <summary>
        /// 控件模板
        /// 不进行绘制也可直接使用
        /// </summary>
        /// <returns></returns>
        public abstract GameObject CreateTemplate();

        /// <summary>
        /// 在进行父子级关联后
        /// 需要对部分层级进行微调
        /// </summary>
        /// <param name="node"></param>
        public virtual void AfterGenerate(UGUINode node) {
        }

        /// <summary>
        /// 设置目标对象的尺寸
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rectTransform"></param>
        public static void SetRectTransform(Rect rect, RectTransform rectTransform)
        {
            rectTransform.pivot = Vector2.one * 0.5f;
            rectTransform.anchorMin = rectTransform.anchorMax = Vector2.one * 0.5f;
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
        }

        /// <summary>
        /// 前缀匹配(address 是小写)
        /// </summary>
        /// <returns></returns>
        protected bool MatchAddress(string name,params string[] address)
        {
            var lowerName = name.ToLower();
            var match = false;
            foreach (var item in address)
            {
                match |= lowerName.StartsWith(item,StringComparison.CurrentCulture);
            }
            return match;
        }
        
        protected bool MatchIDAddress(string name,int id, params string[] address)
        {
            var lowerName = name.ToLower();
            var match = false;
            foreach (var item in address)
            {
                if (item.Contains("{0}"))
                {
                    match |= lowerName.StartsWith(string.Format(item,id), StringComparison.CurrentCulture);
                }
            }
            return match;
        }

      
    }
}
