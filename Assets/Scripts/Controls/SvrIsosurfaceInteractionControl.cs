using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CielaSpike;
using UnityEngine.EventSystems;

public class SvrIsosurfaceInteractionControl: MonoBehaviour {
	public GameObject activeIsosurface;
	public SvrIsosurface iso;
	public float rotationSpeed = 0.05f, zoomingSpeed = 0.05f, panningSpeed = 0.05f;
	public float animationTranslateSpeed = 10f;
	public float animationRotateSpeed = 50f;


	float mouseSpeed = 0.05f;
	Vector3 mouseStart;
	Vector3 mouseDelta;
	Vector3 mouseDeltaChange;

	bool isDraggingLeft = false;
	bool isDraggingMid = false;

	const string LTHUMBSTICKX = "Oculus_GearVR_LThumbstickX";
	const string LTHUMBSTICKY = "Oculus_GearVR_LThumbstickY";
	const string RTHUMBSTICKX = "Oculus_GearVR_RThumbstickX";
	const string RTHUMBSTICKY = "Oculus_GearVR_RThumbstickY";
	const string RTRIGGER = "Oculus_GearVR_LIndexTrigger";
	const string LTRIGGER = "Oculus_GearVR_RIndexTrigger";
	const string ABUTTON = "XBOX_AButton";
	const string SSBUTTON = "XBOX_RStickClick";
    public const string BUTTON_NEXT_ISOSURFACE = "XBOX_LBumper";
    public const string BUTTON_PREV_ISOSURFACE = "XBOX_RBumper";

    float rotationAcc = 0f;
	float zoomingAcc = 0f;
	float panningAcc = 0f;
	float accInc = 0.001f;

	Vector3 animateToPositon;
	Quaternion animateToRotation;
	bool isAnimating = false;


    void Update () {
		Control ();
	}

    /*
      * <summary> 
      * Show next isosurface gameobject
      * </summary>
      */
    public void NextIsosurface()
    {
        GameObject.FindObjectOfType<SvrIsosurfaceExtractorControl>().SetNextActive();
    }

    /*
       * <summary> 
      * Show previous isosurface gameobject
      * </summary>
      */
    public void PrevIsosurface()
    {
        GameObject.FindObjectOfType<SvrIsosurfaceExtractorControl>().SetPrevActive();
    }

    /*
     * <summary> 
     * Toggle active isosurface bounding box
     * </summary>
     */
    public void ToggleBoundingBox(){
		if(activeIsosurface != null)
		activeIsosurface.GetComponent<SvrBoundingBox> ().active = !activeIsosurface.GetComponent<SvrBoundingBox> ().active;
	}

    void Control(){
		if (activeIsosurface == null)
			return;

		if (activeIsosurface.GetComponent<SvrIsosurface> () == null)
			return;

		iso = activeIsosurface.GetComponent<SvrIsosurface> ();

		if (!iso.isReady)
			return;

		GamepadControl ();
		MouseKeyboardControl ();
	}

	void HideGUIPlace(){
		if(GameObject.FindObjectOfType<SvGUIPlace> () != null && GameObject.FindObjectOfType<SvGUIPlace> ().gameObject.activeSelf)
			GameObject.FindObjectOfType<SvGUIPlace> ().gameObject.SetActive (false);
	}

	void LeftMouseControl(){
        if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0)) {

			if (!isDraggingLeft) {
				isDraggingLeft = true;
				mouseStart = Input.mousePosition;

			}
		}
		if (Input.GetMouseButtonUp (0)) {

			isDraggingLeft = false;
			mouseDelta = Vector3.zero;
			mouseStart = Vector3.zero;
		}

		if (isDraggingLeft) {

			Vector3 mouseCurrent = Input.mousePosition;
			mouseDelta =  mouseCurrent - mouseStart;
			if (mouseDeltaChange != mouseDelta && mouseDelta != Vector3.zero) {
				mouseDeltaChange = mouseDelta;
				float rotationX = mouseDelta.x * rotationSpeed * mouseSpeed;
				float rotationY = mouseDelta.y * rotationSpeed * mouseSpeed;

				RotateObject (rotationX, rotationY);

			}

		}
	}

	void ScrollMouseControl(){
		if (Input.GetMouseButtonDown(2)) {
			if (!isDraggingMid) {
				isDraggingMid = true;
				mouseStart = Input.mousePosition;
			}
		}
		if (Input.GetMouseButtonUp (2)) {
			isDraggingMid = false;
			mouseDelta = Vector3.zero;
			mouseStart = Vector3.zero;
		}

		if (isDraggingMid) {
			Vector3 mouseCurrent = Input.mousePosition;
			mouseDelta =  mouseCurrent - mouseStart;
			if (mouseDeltaChange != mouseDelta && mouseDelta != Vector3.zero) {
				mouseDeltaChange = mouseDelta;
				float translationX = mouseDelta.x * panningSpeed * mouseSpeed * 0.05f;
				float translationY = mouseDelta.y * panningSpeed * mouseSpeed * 0.05f;
				PanObject (translationX, translationY);
			}
		}

	}

	void MiddleMouseControl(){
		float mouseScroll =  Input.GetAxis ("Mouse ScrollWheel");
		if ( mouseScroll < 0) {
			ZoomIn ();
		}
		if( mouseScroll > 0)
		{
			ZoomOut ();
		}
	}

	void MouseKeyboardControl(){
		LeftMouseControl ();
		ScrollMouseControl ();
		MiddleMouseControl ();
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextIsosurface();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PrevIsosurface();
        }
    }

	void GamepadControl(){
		float LThumbstickX = Input.GetAxis (LTHUMBSTICKX);
		float LThumbstickY = Input.GetAxis (LTHUMBSTICKY);
		float RThumbstickX = Input.GetAxis (RTHUMBSTICKX);
		float RThumbstickY = Input.GetAxis (RTHUMBSTICKY);
		float LTrigger = Input.GetAxis (LTRIGGER);
		float RTrigger = Input.GetAxis (RTRIGGER);

        if (Input.GetButtonUp(BUTTON_NEXT_ISOSURFACE))
        {
            NextIsosurface();
        }

        if (Input.GetButtonUp(BUTTON_PREV_ISOSURFACE))
        {
            PrevIsosurface();
        }

		if (LThumbstickX != 0 || LThumbstickY != 0) {
			HideGUIPlace ();

			rotationAcc += accInc;
			float rotationX = LThumbstickX * (rotationSpeed + rotationAcc);
			float rotationY = LThumbstickY * (rotationSpeed + rotationAcc);


			if (Input.GetButton (ABUTTON) ) {
				Vector3 pivot = GameObject.FindObjectOfType<Svr3DPointer> ().pointer.transform.position;
				RotateObject (rotationX, rotationY, pivot);
			} else {
				RotateObject (rotationX, rotationY);
			}

		} else {
			rotationAcc = 0f;
		}

		if (RThumbstickX != 0 || RThumbstickY != 0) {
			HideGUIPlace ();
			panningAcc += accInc;
			float translationX = RThumbstickX * (panningSpeed + panningAcc);
			float translationY = RThumbstickY * (panningSpeed + panningAcc);
			PanObject (translationX, translationY);
		} else {
			panningAcc = 0f;
		}

		if (LTrigger != 0 || RTrigger != 0) {
			HideGUIPlace ();
			zoomingAcc += accInc;

			if (LTrigger > 0) {
				ZoomIn ();
			} 
			if (RTrigger > 0) {
				ZoomOut ();
			}
		} else {
			zoomingAcc = 0f;
		}

	}

	void WriteBoundingBoxData(string filepath){
		print (iso.ToString ());
		System.IO.File.WriteAllText(@filepath, iso.ToString ());
		string objPath = GameObject.FindObjectOfType<SvrConfiguration> ().scenefolderpath;
	}

	public IEnumerator StartExportPieces(){
		string datapath = System.IO.Path.Combine (GameObject.FindObjectOfType<SvrConfiguration> ().scenefolderpath, activeIsosurface.name);
		if (!System.IO.Directory.Exists (datapath)) {
			System.IO.Directory.CreateDirectory (datapath);
			System.IO.Directory.CreateDirectory (datapath + "/captures");
			System.IO.Directory.CreateDirectory (datapath + "/objs/lod1");
			System.IO.Directory.CreateDirectory (datapath + "/objs/lod2");
		}

		List<string> mesh = new List<string> ();
		List<string> meshName = new List<string> ();
		List<string> meshLOD1 = new List<string> ();
		List<string> meshLOD1Name = new List<string> ();
	/*	foreach (Transform t in iso.pieces) {
			string filepath = System.IO.Path.Combine (datapath + "/objs/lod1", t.gameObject.name+".obj");
			meshName.Add (filepath);
			mesh.Add(SvObjExporter.MeshToString(t.GetComponent<MeshFilter>(), activeIsosurface.transform));
		}
		foreach (Transform t in iso.piecesLOD1) {
			string filepath = System.IO.Path.Combine (datapath + "/objs/lod2", t.gameObject.name+".obj");
			meshLOD1Name.Add (filepath);
			meshLOD1.Add(SvObjExporter.MeshToString(t.GetComponent<MeshFilter>(), activeIsosurface.transform));
		}*/
		Task task;
		this.StartCoroutineAsync(ExportPieces(mesh, meshName, meshLOD1, meshLOD1Name), out task);
		yield return StartCoroutine(task.Wait());
	}

	IEnumerator ExportPieces(List<string> mesh, List<string> meshName, List<string> meshLOD1, List<string> meshLOD1Name){
		print ("Writting obj ... ");
		for(int i = 0; i < mesh.Count; i++) {
			SvrObjExporter.WriteToFile(mesh[i], meshName[i]);
			SvrObjExporter.WriteToFile(meshLOD1[i], meshLOD1Name[i]);
		}
		print ("Writting obj done");
		return null;
	} 

	public void PanObject(float translationX, float translationY){
		HideGUIPlace ();
		iso.MoveTo(iso.GetPosition() + new Vector3(translationX, translationY,  0f));
	}

	public void ZoomIn(){
		HideGUIPlace ();
		iso.MoveTo(iso.GetPosition() -  Camera.main.transform.forward * (zoomingSpeed + zoomingAcc));
	}

	public void ZoomOut(){
		HideGUIPlace ();
		iso.MoveTo(iso.GetPosition() +  Camera.main.transform.forward * (zoomingSpeed + zoomingAcc));
	}
		

	public void RotateObject(float rotationX, float rotationY){
		HideGUIPlace ();
		iso.Rotate (rotationX, rotationY);
	}

	public void RotateObject(float rotationX, float rotationY, bool anim){
		HideGUIPlace ();
		iso.Rotate (rotationX, rotationY);
	}

	public void RotateObject(float rotationX, float rotationY, Vector3 pivot){
		HideGUIPlace ();
		iso.Rotate (rotationX, rotationY, pivot);
	}

	public void FrontView(){
		iso.FrontView ();
	}

	public void SetPositionAndRotation(Vector3 position, Quaternion rotation){
			animateToPositon = position;
			animateToRotation = rotation;
			activeIsosurface.transform.rotation = rotation;
			activeIsosurface.transform.position = position; 
	}

}
