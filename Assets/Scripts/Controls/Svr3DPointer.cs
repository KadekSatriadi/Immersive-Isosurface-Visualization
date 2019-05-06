using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Svr3DPointer : MonoBehaviour {
	public GameObject pointer;
	public GameObject marker;
	public GameObject boundingBox;
	public enum Mode
	{
		nonvr,vr
	};
	public Mode mode;
	public GameObject regionMenu;
	RaycastHit currentHit;
	public const string TAG_BOUND_PIVOT = "BoundaryPivot";
	public const  string TAG_BOUND = "Boundary";
	public const string TAG_MARKER = "Marker";
    public const string TAG_ISOSURFACE = "Isosurface";
    public const string TAG_REGION = "Region";
    public const string BUTTON_MOVE_FORWARD = "XBOX_LTrigger";
	public const string BUTTON_MOVE_BACKWARD = "XBOX_RTrigger";
	public const  string BUTTON_LOCK_BOUND = "XBOX_YButton";
	public const string BUTTON_SHOW_REGION_MENU = "XBOX_YButton";
	public const string BUTTON_DELETE = "XBOX_XButton";
	public const string BUTTON_MOVE_BOUNDING_BOX = "XBOX_BButton";
	public const string BUTTON_NEXT_MARKER = "XBOX_BButton";


    float acceleration = 1f;
    bool isBoundPivotSelected = false;
	bool isBoundPivotLocked = false;
	SvrMarker activeMarker;
	GameObject activePivot;
	SvrBoundingBoxControl activeBoundingBoxController;

    void Start () {
		pointer = Instantiate (pointer) as GameObject;
		pointer.SetActive (false);
		regionMenu = Instantiate (regionMenu) as GameObject;
		regionMenu.SetActive (false);
		if (mode.Equals (Mode.nonvr)) {
			regionMenu.GetComponent<SvGUIPlace> ().ScreenModeOn ();
		} else {
			regionMenu.GetComponent<SvGUIPlace> ().ScreenModeOff ();
		}
	}

	GameObject GetRegion(){	
		if (currentHit.transform.tag == "Region")
			return currentHit.transform.gameObject;
		else if (currentHit.transform.parent.gameObject.tag == "Region")
			return currentHit.transform.parent.gameObject;
		else if (currentHit.transform.parent.transform.parent.gameObject.tag == "Region")
			return currentHit.transform.parent.transform.parent.gameObject;
		else
			return null;
	}

    /*
    * <summary> 
    * Get the dimension or bounds of a given region gameobject
    * </summary>
    */
    public static Bounds GetRegionBounds(GameObject region){
		List<Vector3> centers = new List<Vector3> ();
		float maxx = float.MinValue;
		float maxy = float.MinValue;
		float maxz = float.MinValue;
		float minx = float.MaxValue;
		float miny = float.MaxValue;
		float minz = float.MaxValue;

		foreach (MeshRenderer mf in region.GetComponentsInChildren<MeshRenderer>()){
			if (mf.gameObject.name.Contains ("LOD1_"))
				continue;
			GameObject g = mf.gameObject;
			centers.Add(mf.bounds.center);

		}

		foreach (MeshFilter mc in region.GetComponentsInChildren<MeshFilter>()) {
			if (mc.gameObject.name.Contains ("LOD1_"))
				continue;
			
			Bounds bo = mc.mesh.bounds;

			if (maxx < bo.max.x) maxx = bo.max.x;
			if (maxy < bo.max.y) maxy = bo.max.y;
			if (maxz < bo.max.z) maxz = bo.max.z;

			if (minx > bo.min.x) minx = bo.min.x;
			if (miny > bo.min.y) miny = bo.min.y;
			if (minz > bo.min.z) minz = bo.min.z;
		}

		if(centers.Count > 0){
			float sumx = 0f;
			float sumy = 0f; 
			float sumz = 0f;
			foreach(Vector3 c in centers){
				sumx += c.x;
				sumy += c.y;
				sumz += c.z;
			}
			Vector3 center =  new Vector3(sumx/centers.Count, sumy/centers.Count, sumz/centers.Count);
			Vector3 boundsize = new Vector3 (Mathf.Abs (maxx - minx), Mathf.Abs (maxy - miny), Mathf.Abs (maxz - minz));
		
			Bounds b = new Bounds(center, boundsize);
			return b;
		}else{
			return	new Bounds();
		}


	}

  /*
   * <summary> 
   * Export bounding box data and screenshot
   * </summary>
   */
	public void CaptureBoundingBox(){
		SvrIsosurface iso = currentHit.transform.GetComponentInParent<SvrIsosurface> ();
		if (currentHit.transform.gameObject.tag.Equals (TAG_BOUND)) {
			iso.CaptureBoundingBox (currentHit.transform.gameObject);
		} else if(currentHit.transform.gameObject.tag.Equals (TAG_ISOSURFACE)){
			GameObject reg = GetRegion ();

			if (iso.IsRegionHasBound (reg)) {
				GameObject b = iso.GetBoundingBox (reg);
				iso.CaptureBoundingBox (b);
			}
		}
	}


  /*
   * <summary> 
   * Add bounding box on the selected region
   * </summary>
   */
	public void AddBoundingBox(){
		GameObject parent = currentHit.transform.root.gameObject;
		SvrIsosurface iso = parent.GetComponent<SvrIsosurface> ();
		SvrBoundingBox parentBoundingBox = parent.GetComponent<SvrBoundingBox> ();
		GameObject region = GetRegion ();

		Bounds b = GetRegionBounds (region);
		print (b);

		GameObject bb = Instantiate (boundingBox) as GameObject;
		bb.transform.rotation = region.transform.rotation;
		bb.transform.position = b.center;
		bb.GetComponent<SvrBoundingBox> ().parentBound = parentBoundingBox;
		bb.GetComponent<SvrBoundingBoxControl> ().region = region;
		bb.GetComponent<SvrBoundingBoxControl> ().Init (b);
		bb.transform.SetParent (parent.transform);

		iso.bounds.Add (bb.transform);

		GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioAddBoundingBox ();
		Debug.Log ("Adding bounding box on " + region.name);
		Debug.Log ("Center " +  b.center);
		Debug.Log ("Size " + b.size);
	}


   /*
   * <summary> 
   * Isolate hit region
   * </summary>
   */
	public void IsolateRegion(){
		SvrIsosurface iso = currentHit.transform.gameObject.GetComponentInParent<SvrIsosurface> ();
		if (currentHit.transform.tag == TAG_BOUND) {
			GameObject region = currentHit.transform.GetComponent<SvrBoundingBoxControl> ().region;
			iso.IsolateRegion (region, Svr3DPointer.GetRegionBounds(region).center);
			GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioIsolate ();
		}else{
			GameObject region = GetRegion ();
			iso.IsolateRegion (region, Svr3DPointer.GetRegionBounds(region).center);
            GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioIsolate ();
		}

	}


    /*
    * <summary> 
    * Exit isolation
    * </summary>
    */
    public void ExitIsolation(){
		SvrIsosurface iso = currentHit.transform.gameObject.GetComponentInParent<SvrIsosurface> ();
		iso.ExitIsolation ();
		GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioExitIsolation ();
	}


    /*
    * <summary> 
    * Add marker
    * </summary>
    */
    public void AddMarker(){
		Debug.Log ("Adding marker");
		SvrIsosurface iso = currentHit.transform.gameObject.GetComponentInParent<SvrIsosurface> ();
		GameObject mark = Instantiate (marker, currentHit.point, currentHit.transform.rotation) as GameObject;
		mark.transform.position = currentHit.point;
		mark.transform.SetParent (currentHit.transform.root);
		mark.GetComponent<SvrMarker> ().parentPosition = iso.transform.position;
		mark.GetComponent<SvrMarker> ().parentRotation = iso.transform.rotation;
		mark.GetComponent<SvrMarker> ().isosurface = iso.gameObject;
		mark.GetComponent<SvrMarker> ().text.text = mark.transform.localPosition.ToString("G4");
		iso.markers.Add (mark.transform);
		GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioAddMarker ();
	}

   /*
   * <summary> 
   * Go to next marker
   * </summary>
   */
	public void NextMarker(){
		Debug.Log ("To marker");
		SvrIsosurface iso = GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ().activeIsosurface.GetComponent<SvrIsosurface> ();
		if (iso == null)
			return;
		if (iso.markers.Count == 0)
			return;
		if (activeMarker == null) {
			activeMarker = iso.markers [0].gameObject.GetComponent<SvrMarker>();
		} else {
			int curIdx = iso.markers.IndexOf (activeMarker.gameObject.transform);
			if (curIdx == iso.markers.Count - 1) {
				activeMarker = iso.markers [0].gameObject.GetComponent<SvrMarker>();
			} else {
				activeMarker = iso.markers [curIdx + 1].gameObject.GetComponent<SvrMarker>();
			}
		}
		GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ().SetPositionAndRotation (activeMarker.parentPosition, activeMarker.parentRotation);
	}

    /*
     * <summary> 
     * Show region menu
     * </summary>
     */
    public void ShowRegionMenu( ){
		regionMenu.GetComponent<SvGUIPlace> ().Show (currentHit);
		GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioShowGUIPlace ();
	}

	public void ShowRegionMenu(bool boundmenu){
		if (!boundmenu) {
			regionMenu.GetComponent<SvGUIPlace> ().Show (currentHit);
			GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioShowGUIPlace ();

		}else {
			regionMenu.GetComponent<SvGUIPlace> ().ShowWithBoundMenu (currentHit);
			GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioShowGUIPlace ();

		}
		
	}

	void ManageBoundingBoxController(){
		  if (Input.GetButton (BUTTON_LOCK_BOUND)) {
			if (isBoundPivotLocked) {
				isBoundPivotLocked = false;
			} else {
				isBoundPivotLocked = true;
			}
		}

		if (Input.GetButton (BUTTON_MOVE_FORWARD)) {
			acceleration += 0.1f;
			activePivot.transform.position += new Vector3 (0.001f, 0.001f, 0.001f) * acceleration;
		}
		if (Input.GetButton (BUTTON_MOVE_BACKWARD)) {
			acceleration += 0.1f;
			activePivot.transform.position -= new Vector3 (0.001f, 0.001f, 0.001f) * acceleration;
		} 
		if (!Input.GetButton (BUTTON_MOVE_BACKWARD) && !Input.GetButton (BUTTON_MOVE_FORWARD))
			acceleration = 1f;

	/*implementation canceled	
	 * if (Input.GetButton (MOVE_BOUNDING_BOX_BUTTON)) {
			activeBoundingBoxController.gameObject.transform.position = new Vector3 (
				pointer.transform.position.x, 
				pointer.transform.position.y, 
				activeBoundingBoxController.gameObject.transform.position.z); ;
		}*/
	}

	void ShowPointer(RaycastHit hit){
		pointer.SetActive (true);
		pointer.transform.position = hit.point;
		pointer.transform.rotation.SetLookRotation (hit.transform.position, Vector3.up);
	}

	void ManageIsosurfaceHit(){
		
		if(Input.GetButtonDown(BUTTON_SHOW_REGION_MENU) || Input.GetMouseButtonDown(1) ){
			ShowRegionMenu ();
		}
		if(Input.GetButton (BUTTON_NEXT_MARKER)) {
			NextMarker ();
		}
	}

	void ManageMarkerHit(){
		if(Input.GetButton (BUTTON_DELETE) || Input.GetMouseButtonDown(2)){
			DeleteMarker ();
		}
	}

	public void DeleteMarker(){
		SvrIsosurface iso = currentHit.transform.root.gameObject.GetComponentInParent<SvrIsosurface> ();
		if (currentHit.transform.gameObject.tag == TAG_MARKER) {
			iso.markers.Remove (currentHit.transform);
			Destroy (currentHit.transform.gameObject);
		}	
	}

	public void ShowBoundingBoxPivots(){
		if (currentHit.transform.gameObject.tag == TAG_BOUND) {
			currentHit.transform.gameObject.GetComponent<SvrBoundingBoxControl> ().TogglePivots ();
		} else {
			SvrIsosurface iso = currentHit.transform.root.gameObject.GetComponentInParent<SvrIsosurface> ();
			GameObject region = GetRegion ();
			if (region != null) {
				GameObject control = iso.GetBoundingBox (region);
				if (control != null)
					control.GetComponent<SvrBoundingBoxControl>().TogglePivots ();
			}
		}
	}

	public void DeleteBoundingBox(){
		SvrIsosurface iso = currentHit.transform.root.gameObject.GetComponentInParent<SvrIsosurface> ();
		if (currentHit.transform.gameObject.tag == TAG_BOUND) {
			iso.bounds.Remove (currentHit.transform);
			Destroy (currentHit.transform.gameObject);
			GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioDeletBoundingBox ();

		} else {
			GameObject region = GetRegion ();
			if (iso.GetBoundingBox (region) != null) {
				GameObject b = iso.GetBoundingBox (region);
				iso.bounds.Remove (b.transform);
				Destroy (b);
				GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioDeletBoundingBox ();

			}
		}
	}

	void ManageBoundingBoxHit(){
		SvrIsosurface iso = currentHit.transform.root.gameObject.GetComponentInParent<SvrIsosurface> ();
		activeBoundingBoxController = currentHit.transform.GetComponent<SvrBoundingBoxControl> ();

		if (activeBoundingBoxController == null) {
			activeBoundingBoxController =currentHit.transform.GetComponentInParent<SvrBoundingBoxControl> ();
		}
		if (activeBoundingBoxController != null) {
			activeBoundingBoxController.ApproximateActivePivot (currentHit.point);
		}


		if (Input.GetButton (BUTTON_DELETE)) {
				DeleteBoundingBox ();
		} else if(Input.GetButton (BUTTON_SHOW_REGION_MENU) || Input.GetMouseButtonDown(1)){
			Vector3 offset = new Vector3 (0f, 0.2f, 0f);
			ShowRegionMenu (true);
		}
		else {
			activePivot = activeBoundingBoxController.activePivot;
			//ManageBoundingBoxController ();
		}
	}
		

	void Update () {

	/*	if (isBoundPivotLocked) {
			pointer.SetActive (true);
			pointer.transform.position = activePivot.transform.position;
			ManageBoundingBoxController ();
			return;
		}
        */

		RaycastHit hit;
		Ray ray;
		if (mode.Equals(Mode.vr)) {
			ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
		}
		else{
			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		}

    

        if (Physics.Raycast (ray, out hit)) {
            if (regionMenu.activeSelf) return;

			switch (hit.collider.tag) {
			    case TAG_BOUND:
				    currentHit = hit;
				    ManageBoundingBoxHit ();
				    ShowPointer (hit);
				    break;
			    case TAG_MARKER:
				    currentHit = hit;
				    ManageMarkerHit ();
				    ShowPointer (hit);
				    break;
			    case TAG_BOUND_PIVOT:
				    currentHit = hit;
				    ManageBoundingBoxHit ();
				    ShowPointer (hit);
				    break;
			    case TAG_ISOSURFACE:
				    currentHit = hit;
				    ManageIsosurfaceHit ();
				    ShowPointer (hit);
				    break;
			    default:
				    if (Input.GetButton (BUTTON_NEXT_MARKER)) {
					    currentHit = hit;
					    NextMarker ();
				    }
				    pointer.SetActive (false);
				    break;
			}
		} else {
			pointer.SetActive (false);
		}

	}
}