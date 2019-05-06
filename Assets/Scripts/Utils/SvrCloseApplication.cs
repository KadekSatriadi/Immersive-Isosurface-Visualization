using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CielaSpike;


public class SvrCloseApplication : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CloseApp(){
		Task t = GameObject.FindObjectOfType<SvrIsosurfaceExtractor> ().task;
		if(t != null) t.Cancel ();
		Application.Quit ();
	}
}
