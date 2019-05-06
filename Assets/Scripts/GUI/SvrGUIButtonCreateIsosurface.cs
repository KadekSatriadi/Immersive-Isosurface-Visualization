using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvrGUIButtonCreateIsosurface : MonoBehaviour {

	public void Create(){
		GameObject.FindObjectOfType<SvrIsosurfaceExtractorControl> ().ConstructIsosurface ();
	}
}
