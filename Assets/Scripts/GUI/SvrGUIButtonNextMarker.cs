using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvrGUIButtonNextMarker : MonoBehaviour {

	public void Next(){
		GameObject.FindObjectOfType<Svr3DPointer> ().NextMarker ();
	}
}
