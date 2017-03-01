using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    RectTransform recttransform;
	// Use this for initialization
	void Start () {
        recttransform = GetComponent<RectTransform>();
       

    }
    int count;
	// Update is called once per frame
	void OnGUI () {
        if (GUILayout.Button("Print"))
        {
            count++;
            Debug.Log(count.ToString() + "anchoredPosition" + recttransform.anchoredPosition);
            Debug.Log(count.ToString() + "anchorMin" + recttransform.anchorMin);
            Debug.Log(count.ToString() + "anchorMax" + recttransform.anchorMax);
            Debug.Log(count.ToString() + "sizeDelta" + recttransform.sizeDelta);
        }
       
    }
}
