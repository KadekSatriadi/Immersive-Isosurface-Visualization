using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvrGUIButtonLoadOther : MonoBehaviour {

	public void LoadOther(){
		GameObject.FindObjectOfType<SvrConfiguration> ().LoadOther ();
	}
}
