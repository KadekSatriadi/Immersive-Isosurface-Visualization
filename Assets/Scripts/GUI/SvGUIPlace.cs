using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SvGUIPlace : MonoBehaviour {
	Canvas canvas;
	bool screenMode = false;
	Vector3 worldPostition = new Vector3 (-0.35f, 0f, -2f);
	Vector3 worldScale = new Vector3(0.005f,0.005f, 0.005f);
	Vector2 worldSize = new Vector2(95f,50f);
	public List<GameObject> boundMenu;

	public void Show(RaycastHit hit){
		Vector3 offset = new Vector3 (0f, 0.2f, 0f);
		gameObject.SetActive (true);
		HideBoundingMenu ();
		if (!screenMode) {
			Vector3 camerapos = Camera.main.transform.position;
			Vector3 point = hit.point * 0.3f;
			//Vector3 pos = camerapos + worldPostition + point;
			Vector3 pos = camerapos + Camera.main.transform.forward * 1.5f;
			gameObject.transform.position = pos;
			//gameObject.transform.position = Camera.main.transform.forward;
			gameObject.transform.rotation.SetLookRotation (hit.point);
			gameObject.GetComponent<LineRenderer> ().SetPosition (0, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 1f, Camera.main.transform.position.z));

		} 
		gameObject.GetComponent<LineRenderer> ().SetPosition (1, hit.point);
	}
		

	public void HideBoundingMenu(){
		if (boundMenu.Count > 0) {
			foreach (GameObject menu in boundMenu) {
				menu.SetActive (false);
			}
		}
	}

	public void ShowBoundingMenu(){
		if (boundMenu.Count > 0) {
			foreach (GameObject menu in boundMenu) {
				menu.SetActive (true);
			}
		}
	}

	public void ShowWithBoundMenu(RaycastHit hit){
		Vector3 offset = new Vector3 (0f, 0.2f, 0f);
		gameObject.SetActive (true);
		ShowBoundingMenu ();
		if (!screenMode) {
			Vector3 camerapos = Camera.main.transform.position;
			Vector3 pos = camerapos + Camera.main.transform.forward * 1.5f;
			gameObject.transform.position = pos;
			//gameObject.transform.position = Camera.main.transform.forward;
			gameObject.transform.rotation.SetLookRotation (Camera.main.transform.position);
			gameObject.GetComponent<LineRenderer> ().SetPosition (0, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 1f, Camera.main.transform.position.z));

		} 
		gameObject.GetComponent<LineRenderer> ().SetPosition (1, hit.point);
	}

	public void ScreenModeOn(){
		screenMode = true;
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.scaleFactor = 2.5f;
	}

	public void ScreenModeOff(){
		screenMode = false;
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.scaleFactor = 1f;
		if (canvas.gameObject.GetComponent<Rect> () != null) {
			canvas.gameObject.GetComponent<Rect> ().size.Set(worldSize.x, worldSize.y);
			Vector3 camerapos = Camera.main.transform.position;
			Vector3 pos = new Vector3 (camerapos.x  + worldPostition.x, camerapos.y + worldPostition.y, camerapos.z + worldPostition.z);
			canvas.gameObject.GetComponent<Transform> ().position = worldPostition;
			canvas.gameObject.GetComponent<Transform> ().localScale = worldScale;
		}
	}

	void Awake(){
		GetComponent<Canvas> ().worldCamera = Camera.main;
		canvas = GetComponent<Canvas> ();

	}


	public void CaptureBoundingBox(){
		GameObject.FindObjectOfType<Svr3DPointer> ().CaptureBoundingBox ();
		Close ();
	}

	public void PlaceBoundingBox(){
		GameObject.FindObjectOfType<Svr3DPointer> ().AddBoundingBox ();
		Close ();
	}

	public void PlaceMarker(){
		GameObject.FindObjectOfType<Svr3DPointer> ().AddMarker ();
		Close ();
	}

	public void DeleteBounds(){
		GameObject.FindObjectOfType<Svr3DPointer> ().DeleteBoundingBox ();
		Close ();
	}

	public void DeleteMarker(){
		GameObject.FindObjectOfType<Svr3DPointer> ().DeleteMarker ();
		Close ();
	}

	public void ResizeBound(){
		GameObject.FindObjectOfType<Svr3DPointer> ().ShowBoundingBoxPivots();
		Close ();
	}

	public void ExitIsolation(){
		GameObject.FindObjectOfType<Svr3DPointer> ().ExitIsolation ();
		Close ();
	}

	public void IsolateRegion(){
		GameObject.FindObjectOfType<Svr3DPointer> ().IsolateRegion ();
		Close ();
	}

	public void Close(){
		gameObject.SetActive (false);
		EventSystem.current.SetSelectedGameObject(null);

	}

}
