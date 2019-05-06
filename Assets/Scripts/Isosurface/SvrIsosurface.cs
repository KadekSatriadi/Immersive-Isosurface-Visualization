using System.Collections.Generic;
using UnityEngine;

public class SvrIsosurface : MonoBehaviour {
	public bool isReady = false;
	public GameObject center;
    public GameObject labelName;
    public Vector3 size;
	public double isovalue;
	public List<GameObject> regions = new List<GameObject> ();
	public List<Transform> bounds = new List<Transform>();
	public List<Transform> markers = new List<Transform>();
	Vector3 isolationCenter;
	bool isIsolationMode = false;

    /*
     * <summary> 
     *Isolate a region and set rotation pivot
     * </summary>
     */
    public void IsolateRegion(GameObject region, Vector3 center){
		foreach (GameObject r in regions) {
			if(!r.Equals(region)){
				r.SetActive (false);
			}
		}
		foreach (Transform b in bounds) {
			if(!b.GetComponent<SvrBoundingBoxControl>().region.Equals(region)){
				b.gameObject.SetActive (false);
			}
		}
		foreach (Transform m in markers) {
				m.gameObject.SetActive (false);
		}
		isIsolationMode = true;
		isolationCenter = center;
	}

     /*
     * <summary> 
     *Get bounding box of a region
     * </summary>
     */
	public GameObject GetBoundingBox(GameObject region){
		foreach (Transform b in bounds) {
			if (b.gameObject.GetComponent<SvrBoundingBoxControl>().region.Equals(region)) {
				return b.gameObject;
				break;
			}
		}
		return null;
	}

    /*
     * <summary> 
     *Set all regions posisiton to a proper location after the isosurface gameobject is flipped
     * </summary>
     */
    public void NormaliseRegionPosition(){
		foreach (GameObject region in regions) {
			region.transform.localPosition = new Vector3 (0, 0, size.z);
		}
	}
    
     /*
     * <summary> 
     *Is a given region has a bounding box
     * </summary>
     */
	public bool IsRegionHasBound(GameObject region){
		bool found = false;
		foreach (Transform b in bounds) {
			GameObject r = b.GetComponent<SvrBoundingBoxControl> ().region;
			if (r.Equals (region))  return true;
		}
		return found;
	}

    /*
    * <summary> 
    *Export bounding box data
    * </summary>
    */
    public void CaptureBoundingBox(GameObject bounding){
		foreach (Transform b in bounds) {
			if (b.gameObject.Equals (bounding)) {
				b.GetComponent<SvrBoundingBoxControl> ().Capture ();
			}
		}
	}

    /*
     * <summary> 
     *Activae all regions, all bounding box, and all markers
     * </summary>
     */
	public void ExitIsolation(){
		foreach (GameObject r in regions) {
			r.SetActive (true);
		}
		foreach (Transform b in bounds) {
				b.gameObject.SetActive (true);
		}
		foreach (Transform m in markers) {
			m.gameObject.SetActive (true);
		}
		isIsolationMode = false;
	}

    /*
    * <summary> 
    *Is a given region has a bounding box
    * </summary>
    */
    public void Hide(){
		foreach (Transform  t in GetComponentsInChildren<Transform>()) {
			if(!transform.Equals(t))
				t.gameObject.SetActive (false);
		}
	}

    /*
    * <summary> 
    *Show all regions
    * </summary>
    */
	public void Show(){
		foreach (Transform  t in GetComponentsInChildren<Transform>()) {
			if(!transform.Equals(t))
				t.gameObject.SetActive (true);
		}
	}

    /*
    * <summary> 
    *Rotate object in x and y axis. If in isolation, the pivot is the isolated object, otherwise the isosurface centre is used as the pivot
    * </summary>
    */
    public void Rotate(float rotationX, float rotationY){
		if (!isIsolationMode) {
			transform.RotateAround (center.transform.position, Vector3.down, rotationX);
			transform.RotateAround (center.transform.position, Vector3.left, -rotationY);
		} else {
			Rotate (rotationX, rotationY,isolationCenter);
		}
	}

    /*
    * <summary> 
    *Rotate object in x and y axis
    * </summary>
    */
    public void Rotate(float rotationX, float rotationY, Vector3 pivot){
		transform.RotateAround (pivot, Vector3.down, rotationX);
		transform.RotateAround (pivot, Vector3.left, -rotationY);
	}

    /*
    * <summary> 
    *Move isosurface object to 0,0,0
    * </summary>
    */
    public void ResetPosisiton(){
		MoveTo (Vector3.zero);
	}

    /*
    * <summary> 
    *Move isosurface to 0,0,0 and rotate it to face the user
    * </summary>
    */
    public void FrontView(){
		ResetPosisiton ();
		Rotate (90f, 0f);
	}

    /*
    * <summary> 
    *Get isosurface center position
    * </summary>
    */
    public Vector3 GetPosition(){
		return (transform.position + center.transform.position);
	}

    /*
    * <summary> 
    *Move the isosurface gameobject to given position
    * </summary>
    */
    public void MoveTo(Vector3 position){
		transform.position = position - center.transform.position;
	}

}
