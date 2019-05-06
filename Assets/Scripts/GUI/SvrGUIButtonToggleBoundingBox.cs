using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvrGUIButtonToggleBoundingBox : MonoBehaviour {

	public void Toggle(){
		GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ().ToggleBoundingBox ();
	}
}
