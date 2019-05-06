using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(FPSDisplay))]
public class SvrBenchmark : MonoBehaviour {
	FPSDisplay fpsDisplay;
	SvrIsosurfaceInteractionControl control;
	bool isRunning = false;
	bool isRotating = false;
	bool isZoomingIn = false;
	bool isZoomingOut = false;
	bool isPanningRight = false;
	bool isPanningLeft = false;
	public float rotationDuration = 5f;
	public float zoomingDuration = 5f;
	public float panningDuration = 5f;
	public float startdelay = 1f;
	void Start () {
		fpsDisplay = GetComponent<FPSDisplay> ();
		control = GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ();
	}

	public void Benchmark(){
		StartCoroutine (Go ());
	}

	IEnumerator Go(){
		yield return new WaitForSecondsRealtime (startdelay);
		isRunning = true;
		isRotating = true;
		yield return new WaitForSecondsRealtime (rotationDuration);
		isRotating = false;
		isZoomingIn = true;
		yield return new WaitForSecondsRealtime (zoomingDuration);
		isZoomingIn = false;
		isZoomingOut = true;
		yield return new WaitForSecondsRealtime (zoomingDuration * 0.5f);
		isZoomingOut = false;
		isPanningRight = true;
		yield return new WaitForSecondsRealtime (panningDuration);
		isPanningRight = false;
		isPanningLeft = true;
		yield return new WaitForSecondsRealtime (panningDuration);
		isPanningLeft = false;
		isRotating = true;
		yield return new WaitForSecondsRealtime (rotationDuration);
		isRotating = false;
		isRunning = false;
		fpsDisplay.isActive = false;
		string path = @System.IO.Path.Combine (Application.dataPath, "Benchmark_Interaction_" + control.activeIsosurface.name + ".txt");
		string content = control.activeIsosurface.name + "," + fpsDisplay.avgfps + "," + fpsDisplay.count;
		WriteBenchmarkText (path, content);
		Application.Quit ();
	}

	public void WriteBenchmarkText(string path, string content){
		File.WriteAllText (path, content);
		print ("File written -> " + path);
	}

	void Update(){
		if (!isRunning)
			return;
		if (isRotating) {
			control.RotateObject (Time.unscaledDeltaTime * 10f, 0);
		}
		if (isZoomingIn) {
			control.ZoomIn ();
		}
		if (isZoomingOut) {
			control.ZoomOut ();
		}
		if (isPanningRight) {
			control.PanObject (Time.unscaledDeltaTime, 0);
		}
		if (isPanningLeft) {
			control.PanObject (-1f * Time.unscaledDeltaTime, 0);
		}
	}
}
