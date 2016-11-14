using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PSDUIImporter
{
    public class GridLayerImport : ILayerImport
    {
        PSDImportCtrl ctrl;
        public GridLayerImport(PSDImportCtrl ctrl)
        {
            this.ctrl = ctrl; Debug.Log(ctrl);
        }
        public void DrawLayer(Layer layer, GameObject parent)
        {
            UnityEngine.UI.GridLayoutGroup temp = Resources.Load(PSDImporterConst.PREFAB_PATH_GRID, typeof(UnityEngine.UI.GridLayoutGroup)) as UnityEngine.UI.GridLayoutGroup;
            UnityEngine.UI.GridLayoutGroup gridLayoutGroup = GameObject.Instantiate(temp) as UnityEngine.UI.GridLayoutGroup;
            gridLayoutGroup.transform.SetParent(parent.transform, false);//.parent = parent.transform;

            gridLayoutGroup.padding = new RectOffset();
            gridLayoutGroup.cellSize = new Vector2(System.Convert.ToInt32(layer.arguments[2]), System.Convert.ToInt32(layer.arguments[3]));

            RectTransform rectTransform = gridLayoutGroup.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2(layer.position.x, layer.position.y);

            int cellCount = System.Convert.ToInt32(layer.arguments[0]) * System.Convert.ToInt32(layer.arguments[1]);
            for (int cell = 0; cell < cellCount; cell++)
            {
                UnityEngine.UI.Image pic = Resources.Load(PSDImporterConst.PREFAB_PATH_IMAGE, typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
                pic.sprite = null;
                //            Sprite sprite = Resources.Load (relativeResoucesDirectory + "normal_13", typeof(Sprite)) as Sprite;
                //            pic.sprite = sprite;

                UnityEngine.UI.Image myImage = GameObject.Instantiate(pic) as UnityEngine.UI.Image;
                myImage.transform.SetParent(rectTransform, false); //parent = rectTransform;
            }
        }
    }
}
