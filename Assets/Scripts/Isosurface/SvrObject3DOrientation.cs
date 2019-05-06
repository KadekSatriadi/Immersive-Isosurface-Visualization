using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvrObject3DOrientation : MonoBehaviour {
	public GameObject go;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (go != null) {
			transform.rotation = go.transform.rotation;
		}
	}
}
