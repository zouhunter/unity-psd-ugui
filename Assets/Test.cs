using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log(GetComponent<RectTransform>().sizeDelta);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
