using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SvrGUISliderSpeedControl : MonoBehaviour {
	public Text label;
	public enum Control
	{
		zoom, rotate, pan
	}
	SvrIsosurfaceInteractionControl gc;
	public Control control;
	Slider slider;
	// Use this for initialization
	void Start () {
		slider = GetComponent<Slider> ();
		gc = GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ();
		if (gc != null) {
			if (control.Equals (Control.zoom)) {
				slider.value = gc.zoomingSpeed;
			}
			if (control.Equals (Control.rotate)) {
				slider.value = gc.rotationSpeed;
			}
			if (control.Equals (Control.pan)) {
				slider.value = gc.panningSpeed;
			}
		}
		SetLabel ();

	}

	void SetLabel(){
		if (label == null)
			return;
		
		if (control.Equals (Control.zoom)) {
			label.text = "zoom: ";
		}
		if (control.Equals (Control.rotate)) {
			label.text = "rotate: ";
		}
		if (control.Equals (Control.pan)) {
			label.text = "pan: ";
		}
		label.text += slider.value.ToString ();;

	}

	public void UpdateValue(){
		gc = GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ();
		if (gc != null) {
			SetLabel ();

			if (control.Equals (Control.zoom)) {
				gc.zoomingSpeed = slider.value;
			}
			if (control.Equals (Control.rotate)) {
				gc.rotationSpeed = slider.value;
			}
			if (control.Equals (Control.pan)) {
				gc.panningSpeed = slider.value;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
