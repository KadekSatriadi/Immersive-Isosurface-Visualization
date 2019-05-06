using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SvrGUITextOutputConsole : MonoBehaviour {
	Text text;
	// Use this for initialization
	void Start () {
		text = GetComponentInChildren<Text> ();
	}
	
	public void SetText(string t){
		if(text == null)
			text = GetComponentInChildren<Text> ();
		
		text.text = t;
	}
}
