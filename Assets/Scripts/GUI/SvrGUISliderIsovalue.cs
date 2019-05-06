using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SvrGUISliderIsovalue : MonoBehaviour {
	public Text label;
	// Use this for initialization
	public void UpdateLabel () {
		if (label == null)
			label = GameObject.FindObjectOfType<SvrGUITextIsovalueLabel> ().gameObject.GetComponent<Text>();
		if (label != null) {
			label.text = GetComponent<Slider> ().value + "";
		}
	}

	public void SetValue(float v){
		GetComponent<Slider> ().value = v;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
