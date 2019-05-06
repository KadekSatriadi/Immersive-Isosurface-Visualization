using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]
public class SvrBoundingBoxControl : MonoBehaviour {
	SvrBoundingBox bounding;
	public GameObject region;
	public Text text;
	public Canvas canvas;
	public GameObject lengthPivot;
	public GameObject heigthPivot;
	public GameObject widthPivot;
	public GameObject pivotZMin, pivotZMax, pivotXMin, pivotXMax, pivotYMin, pivotYMax;
	public GameObject a, b, c,d,e,f,g,h;
	List<Transform> points = new List<Transform>();
	List<Transform> pivots = new List<Transform>();
	float distance = 0.1f;
	BoxCollider boxCollider;
	public float maxX, maxY, maxZ, minX, minY, minZ;
	Color DEFAULT_PIVOT_COLOR = Color.white;
	Color ACTIVE_PIVOT_COLOR = Color.blue;
	public GameObject activePivot;
	bool isReady = false;
	public Vector3 size;
	public Vector3 center;

	void Awake () {
		bounding = GetComponent<SvrBoundingBox> ();
		boxCollider = GetComponent<BoxCollider> ();
		boxCollider.isTrigger = true;
		canvas = GetComponentInChildren<Canvas> ();
		text = GetComponentInChildren<Text> ();

	}

	public void TogglePivots(){
		foreach (Transform t in pivots) {
			t.gameObject.SetActive (!t.gameObject.activeSelf);
			isReady = !isReady;
		}
		EventSystem.current.SetSelectedGameObject(null);

	}

	public void Delete(){
		transform.root.GetComponent<SvrIsosurface>().bounds.Remove (transform);
		Destroy (transform.gameObject);
	}


	public void Capture(){
		print (bounding.ToString ());
		GameObject.FindObjectOfType<SvrConfiguration> ().TakeScreenShot (bounding.ToString (), transform.root.name, region.name);
	}




	public void Init(Bounds b){
		this.size = b.size; 
		this.center = b.center;

		bounding.size = b.size;
		bounding.center = b.center;
		bounding.SetCenterLocalOn ();

		minX = b.min.x;
		maxX = b.max.x;
		minY = b.min.y;
		maxY = b.max.y;
		minZ = b.min.z;
		maxZ = b.max.z;

		CreatePivots ();
		CreatePoints ();

		bounding.CalculatePoints ();
		AlignPoints ();

		InitPivotsPosition ();
		TogglePivots ();
		isReady = true;
	}

	public void ApproximateActivePivot(Vector3 cursorPosition){
		if (pivotXMax == null)
			return;
		activePivot = pivotXMax;
		float closestValue = Vector3.Distance (cursorPosition, pivotXMax.transform.position);
		if (Vector3.Distance (cursorPosition, pivotXMin.transform.position) < closestValue) {
			activePivot = pivotXMin;
			closestValue = Vector3.Distance (cursorPosition, pivotXMin.transform.position);
		}
		if (Vector3.Distance (cursorPosition, pivotYMax.transform.position) < closestValue) {
			activePivot = pivotYMax;
			closestValue = Vector3.Distance (cursorPosition, pivotYMax.transform.position);
		}
		if (Vector3.Distance (cursorPosition, pivotYMin.transform.position) < closestValue) {
			activePivot = pivotYMin;
			closestValue = Vector3.Distance (cursorPosition, pivotYMin.transform.position);
		}
		if (Vector3.Distance (cursorPosition, pivotZMax.transform.position) < closestValue) {
			activePivot = pivotZMax;
			closestValue = Vector3.Distance (cursorPosition, pivotZMax.transform.position);
		}
		if (Vector3.Distance (cursorPosition, pivotZMin.transform.position) < closestValue) {
			activePivot = pivotZMin;
			closestValue = Vector3.Distance (cursorPosition, pivotZMin.transform.position);
		}

		SetActiveColor (activePivot, pivotXMax);
		SetActiveColor (activePivot, pivotXMin);
		SetActiveColor (activePivot, pivotYMax);
		SetActiveColor (activePivot, pivotYMin);
		SetActiveColor (activePivot, pivotZMax);
		SetActiveColor (activePivot, pivotZMin);


	}

	void InitPivotsPosition(){
		pivotZMin.transform.localPosition = new Vector3 ((minX + maxX) / 2f, (minY + maxY) / 2f, minZ - distance);
		pivotZMax.transform.localPosition = new Vector3 ((minX + maxX) / 2f, (minY + maxY) / 2f, maxZ + distance);
		pivotYMin.transform.localPosition = new Vector3 ((minX + maxX) / 2f, minY - distance, (minZ + maxZ) / 2f);
		pivotYMax.transform.localPosition = new Vector3 ((minX + maxX) / 2f, maxY + distance , (minZ + maxZ) / 2f);
		pivotXMin.transform.localPosition = new Vector3 (minX - distance, (minY + maxY) / 2f, (minZ + maxZ) / 2f);
		pivotXMax.transform.localPosition = new Vector3 (maxX + distance, (minY + maxY) / 2f, (minZ + maxZ) / 2f);
	}

	void AlignPivotsPosition(){
		UpdateMinMax ();

		pivotZMin.transform.localPosition = new Vector3 ((minX + maxX) / 2f, (minY + maxY) / 2f, pivotZMin.transform.localPosition.z );
		pivotZMax.transform.localPosition = new Vector3 ((minX + maxX) / 2f, (minY + maxY) / 2f, pivotZMax.transform.localPosition.z );
		pivotYMin.transform.localPosition = new Vector3 ((minX + maxX) / 2f, pivotYMin.transform.localPosition.y , (minZ + maxZ) / 2f);
		pivotYMax.transform.localPosition = new Vector3 ((minX + maxX) / 2f, pivotYMax.transform.localPosition.y  , (minZ + maxZ) / 2f);
		pivotXMin.transform.localPosition = new Vector3 (pivotXMin.transform.localPosition.x , (minY + maxY) / 2f, (minZ + maxZ) / 2f);
		pivotXMax.transform.localPosition = new Vector3 (pivotXMax.transform.localPosition.x , (minY + maxY) / 2f, (minZ + maxZ) / 2f);


	}


	public void CreatePoints(){
		a = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		b = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		c = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		d = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		e = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		f = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		g = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		h = GameObject.CreatePrimitive (PrimitiveType.Sphere);

		a.name = "a";
		b.name = "b";
		c.name = "c";
		d.name = "d";
		e.name = "e";
		f.name = "f";
		g.name = "g";
		h.name = "h";

		points.Add (a.transform);
		points.Add (b.transform);
		points.Add (c.transform);
		points.Add (d.transform);
		points.Add (e.transform);
		points.Add (f.transform);
		points.Add (g.transform);
		points.Add (h.transform);

		foreach (Transform t in points) {
			t.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
			t.SetParent (transform);
			t.gameObject.GetComponent<Collider> ().enabled = false;
		}
			
	}

	void SetActiveColor(GameObject active, GameObject pivot){
		if(pivot.Equals(active))
			pivot.GetComponent<MeshRenderer>().material.color = ACTIVE_PIVOT_COLOR;
		else
			pivot.GetComponent<MeshRenderer>().material.color = DEFAULT_PIVOT_COLOR;
	}

	void CreatePivots(){
		pivotZMin = GameObject.CreatePrimitive (PrimitiveType.Cube);
		pivotZMax = GameObject.CreatePrimitive (PrimitiveType.Cube);
		pivotYMin = GameObject.CreatePrimitive (PrimitiveType.Cube);
		pivotYMax = GameObject.CreatePrimitive (PrimitiveType.Cube);
		pivotXMin = GameObject.CreatePrimitive (PrimitiveType.Cube);
		pivotXMax = GameObject.CreatePrimitive (PrimitiveType.Cube);

		pivotZMin.name = "zmin";
		pivotZMax.name = "zmax";
		pivotYMin.name = "ymin";
		pivotYMax.name = "ymax";
		pivotXMin.name = "xmin";
		pivotXMax.name = "xmax";

		string tag = "BoundaryPivot";
		pivotZMin.tag = tag;
		pivotZMax.tag = tag;
		pivotYMin.tag =tag;
		pivotYMax.tag = tag;
		pivotXMin.tag = tag;
		pivotXMax.tag =tag;

		float scale = 0.05f;
		pivotZMin.transform.localScale = new Vector3 (scale, scale, scale);
		pivotZMax.transform.localScale = new Vector3 (scale, scale, scale);
		pivotYMin.transform.localScale = new Vector3 (scale, scale, scale);
		pivotYMax.transform.localScale = new Vector3 (scale, scale, scale);
		pivotXMin.transform.localScale = new Vector3 (scale, scale, scale);
		pivotXMax.transform.localScale = new Vector3 (scale, scale, scale);

		pivotZMin.GetComponent<BoxCollider> ().isTrigger = true;
		pivotZMax.GetComponent<BoxCollider> ().isTrigger = true;
		pivotYMin.GetComponent<BoxCollider> ().isTrigger = true;
		pivotYMax.GetComponent<BoxCollider> ().isTrigger = true;
		pivotXMin.GetComponent<BoxCollider> ().isTrigger = true;
		pivotXMax.GetComponent<BoxCollider> ().isTrigger = true;

		pivotZMin.GetComponent<MeshRenderer>().material.color = DEFAULT_PIVOT_COLOR;
		pivotZMax.GetComponent<MeshRenderer>().material.color = DEFAULT_PIVOT_COLOR;
		pivotYMin.GetComponent<MeshRenderer>().material.color = DEFAULT_PIVOT_COLOR;
		pivotYMax.GetComponent<MeshRenderer>().material.color = DEFAULT_PIVOT_COLOR;
		pivotXMin.GetComponent<MeshRenderer>().material.color = DEFAULT_PIVOT_COLOR;
		pivotXMax.GetComponent<MeshRenderer>().material.color = DEFAULT_PIVOT_COLOR;

		pivotZMin.transform.SetParent (transform);
		pivotZMax.transform.SetParent (transform);
		pivotYMin.transform.SetParent (transform);
		pivotYMax.transform.SetParent (transform);
		pivotXMin.transform.SetParent (transform);
		pivotXMax.transform.SetParent (transform);


		pivots.Add (pivotZMin.transform);
		pivots.Add (pivotZMax.transform);
		pivots.Add(	pivotYMin.transform);
		pivots.Add(	pivotYMax.transform);
		pivots.Add(	pivotXMin.transform);
		pivots.Add(	pivotXMax.transform);


	}

	void UpdateMinMax(){
		minX = pivotXMin.transform.localPosition.x;
		maxX = pivotXMax.transform.localPosition.x;
		minY = pivotYMin.transform.localPosition.y;
		maxY = pivotYMax.transform.localPosition.y;
		minZ = pivotZMin.transform.localPosition.z;
		maxZ = pivotZMax.transform.localPosition.z;
	}

	void InitMinMax(){
		

		/*foreach (Transform t in pivots) {
			if (minX > t.localPosition.x) {
				minX = t.localPosition.x;
			}
			if (minY > t.localPosition.y) {
				minY = t.localPosition.y;
			}
			if (minZ > t.localPosition.z) {
				minZ = t.localPosition.z;
			}
			if (maxX < t.localPosition.x) {
				maxX = t.localPosition.x;
			}
			if (maxY < t.localPosition.y) {
				maxY = t.localPosition.y;
			}
			if (maxZ < t.localPosition.z) {
				maxZ = t.localPosition.z;
			}
		}*/
	}

	public void AlignPivotsReverse(){
		bounding.a = a.transform.localPosition;
		bounding.b = b.transform.localPosition;
		bounding.c = c.transform.localPosition;
		bounding.d = d.transform.localPosition;
		bounding.e = e.transform.localPosition;
		bounding.f = f.transform.localPosition;
		bounding.g = g.transform.localPosition;
		bounding.h = h.transform.localPosition;
		bounding.CalculateSizeAndCenter ();

	}

	public void AlignPoints(){
		a.transform.position = bounding.a;
		b.transform.position = bounding.b;
		c.transform.position = bounding.c;
		d.transform.position = bounding.d;
		e.transform.position = bounding.e;
		f.transform.position = bounding.f;
		g.transform.position = bounding.g;
		h.transform.position = bounding.h;
		InitMinMax ();
	}

	void ZMinConstraint(){
		float z = pivotZMin.transform.localPosition.z + distance;
		a.transform.localPosition = new Vector3 (a.transform.localPosition.x, a.transform.localPosition.y, z);
		b.transform.localPosition = new Vector3 (b.transform.localPosition.x, b.transform.localPosition.y, z);
		e.transform.localPosition = new Vector3 (e.transform.localPosition.x, e.transform.localPosition.y, z);
		f.transform.localPosition = new Vector3 (f.transform.localPosition.x, f.transform.localPosition.y, z);
		AlignPivotsPosition ();
	}
	void ZMaxConstraint(){
		float z = pivotZMax.transform.localPosition.z - distance;
		c.transform.localPosition = new Vector3 (c.transform.localPosition.x, c.transform.localPosition.y, z);
		d.transform.localPosition = new Vector3 (d.transform.localPosition.x, d.transform.localPosition.y, z);
		g.transform.localPosition = new Vector3 (g.transform.localPosition.x, g.transform.localPosition.y, z);
		h.transform.localPosition = new Vector3 (h.transform.localPosition.x, h.transform.localPosition.y, z);
		AlignPivotsPosition ();
	}
	void XMinConstraint(){
		float x = pivotXMin.transform.localPosition.x + distance;
		a.transform.localPosition = new Vector3 (x, a.transform.localPosition.y, a.transform.localPosition.z);
		e.transform.localPosition = new Vector3 (x, e.transform.localPosition.y, e.transform.localPosition.z);
		d.transform.localPosition = new Vector3 (x, d.transform.localPosition.y, d.transform.localPosition.z);
		h.transform.localPosition = new Vector3 (x, h.transform.localPosition.y, h.transform.localPosition.z);
		AlignPivotsPosition ();
	}
	void XMaxConstraint(){
		float x = pivotXMax.transform.localPosition.x - distance;
		c.transform.localPosition = new Vector3 (x, c.transform.localPosition.y, c.transform.localPosition.z);
		b.transform.localPosition = new Vector3 (x, b.transform.localPosition.y, b.transform.localPosition.z);
		f.transform.localPosition = new Vector3 (x, f.transform.localPosition.y, f.transform.localPosition.z);
		g.transform.localPosition = new Vector3 (x, g.transform.localPosition.y, g.transform.localPosition.z);
		AlignPivotsPosition ();
	}
	void YMaxConstraint(){
		float y = pivotYMax.transform.localPosition.y - distance;
		g.transform.localPosition = new Vector3 ( g.transform.localPosition.x,y, g.transform.localPosition.z);
		f.transform.localPosition = new Vector3 ( f.transform.localPosition.x,y, f.transform.localPosition.z);
		e.transform.localPosition = new Vector3 ( e.transform.localPosition.x, y,e.transform.localPosition.z);
		h.transform.localPosition = new Vector3 ( h.transform.localPosition.x,y, h.transform.localPosition.z);
		AlignPivotsPosition ();
	}
	void YMinConstraint(){
		float y = pivotYMin.transform.localPosition.y + distance;
		a.transform.localPosition = new Vector3 ( a.transform.localPosition.x, y, a.transform.localPosition.z);
		b.transform.localPosition = new Vector3 ( b.transform.localPosition.x,y, b.transform.localPosition.z);
		d.transform.localPosition = new Vector3 ( d.transform.localPosition.x,y, d.transform.localPosition.z);
		c.transform.localPosition = new Vector3 ( c.transform.localPosition.x,y, c.transform.localPosition.z);
		AlignPivotsPosition ();
	}



	// Update is called once per frame
	void Update () {
		if (!isReady)
			return;
		
		boxCollider.size = bounding.size;
		boxCollider.center = Vector3.zero;

		AlignPivotsReverse ();

		ZMinConstraint ();
		ZMaxConstraint ();
		XMinConstraint ();
		XMaxConstraint ();
		YMinConstraint ();
		YMaxConstraint ();
		canvas.transform.localPosition = pivotYMax.transform.localPosition + new Vector3(0f, 0.1f, 0f);
		//text.text = "size: " + bounding.size.x + " x " + bounding.size.y + " x " + bounding.size.z;
	}
}
