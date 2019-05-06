using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SvrMarker : MonoBehaviour {
	public Vector3 parentPosition;
	public Quaternion parentRotation;
	public GameObject isosurface;
	public Text text;

	void Awake(){
		text = GetComponentInChildren<Text> ();
	}
}
