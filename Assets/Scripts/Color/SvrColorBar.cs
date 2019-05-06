using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SvrColorBar : MonoBehaviour {
	public int nscale;
	public float scaleHeight;
	public float scaleWidth;
	public float maxValue = 1f;
	public float minValue = 0f;
	public string title = "Color Map";
	public GameObject maxLabelText;
	public GameObject minLabelText;
	public GameObject midLabelText;
	public GameObject midLowLabelText;
	public GameObject midTopLabelText;
	public GameObject titleText;

	Material material;
	SvrCoolWarmColorMap colorMap;
	// Use this for initialization

	
	public void CreateBar(){
		material = Resources.Load ("VertexColorMaterial", typeof(Material)) as Material;
		string path = System.IO.Path.Combine(Application.streamingAssetsPath, "CoolWarmFloat257.csv");
		colorMap = new SvrCoolWarmColorMap (path);
		float scale = 1f / (nscale+1);
		float currentValue = 0f;
		List<Vector3> vertices = new List<Vector3> ();
		List<Color> colors = new List<Color> ();
		List<int> triangles = new List<int> ();
		for (int i = 0; i < nscale + 1; i++) {
			if (i == 0) {
				Color bottom = colorMap.GetColor (0);
				Color top = colorMap.GetColor (scale);
				Vector3 bl = new Vector3 (0f, 0f , 0f);
				Vector3 br = new Vector3 (scaleWidth, 0f , 0f);
				Vector3 tl = new Vector3 (0, scaleHeight, 0f);
				Vector3 tr = new Vector3 (scaleWidth, scaleHeight, 0f);
				vertices.Add (bl);
				vertices.Add (tl);
				vertices.Add (tr);
				vertices.Add (br);
				triangles.Add (i);
				triangles.Add (i + 1);
				triangles.Add (i + 2);
				triangles.Add (i);
				triangles.Add (i + 2);
				triangles.Add (i + 3);
				colors.Add (bottom);
				colors.Add (top);
				colors.Add (top);
				colors.Add (bottom);
				minLabelText = CreateLabel (minLabelText, "MinValue", minValue, br);
			} else {
				Color top = colorMap.GetColor (currentValue + (scale * (i + 1)));
				Vector3 tl = new Vector3 (0, scaleHeight * (i + 1), 0f);
				Vector3 tr = new Vector3 (scaleWidth, scaleHeight * (i + 1), 0f);
				vertices.Add (tl);
				vertices.Add (tr);

				if (i == 1) {
					triangles.Add (1);
					triangles.Add (4);
					triangles.Add (5);
					triangles.Add (1);
					triangles.Add (5);
					triangles.Add (2);
				} else {
					int p = i * 2;
					triangles.Add (p);
					triangles.Add (p + 2);
					triangles.Add (p + 3);
					triangles.Add (p);
					triangles.Add (p + 3);
					triangles.Add (p + 1);
				}
				colors.Add (top);
				colors.Add (top);
				currentValue = scale * (i+1);
				if ((i + 1) == Mathf.RoundToInt (nscale / 2f))
					midLabelText = CreateLabel (midLabelText, "MidValue", currentValue, tr);
				else if( (i+1) == Mathf.RoundToInt (nscale / 4f) ) 
					midLowLabelText = CreateLabel (midLowLabelText, "MidLowValue", currentValue, tr);
				else if((i+1) == Mathf.RoundToInt (nscale * (3f/ 4f)))
					midTopLabelText = CreateLabel (midTopLabelText, "MidTopValue", currentValue, tr);
				else if (currentValue == maxValue) {
					maxLabelText = CreateLabel (maxLabelText, "MaxValue", currentValue, tr);
					CreateTitle (new Vector3(0,scaleHeight * (i + 1) + 0.05f, 0f ));
				}
			}

		}
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();
		mesh.colors = colors.ToArray ();

		GetComponent<MeshFilter> ().mesh = mesh;
		GetComponent<MeshRenderer> ().material = material;

		foreach (Transform t in gameObject.GetComponentInChildren<Transform>()) {
			if (!t.Equals (transform)) {
				t.localEulerAngles = (Vector3.zero);
			}
		}
	}

	public void SetLabels(float min, float max){
		float mid = (max + min) / 2f;
		float midLow = (max + min) / 4f;
		float midTop = (max + min) * (3f/4);

		maxLabelText.GetComponent<TextMesh> ().text = "-- " + max.ToString ("G4");
		midLabelText.GetComponent<TextMesh> ().text = "-- " + mid.ToString ("G4");
		minLabelText.GetComponent<TextMesh> ().text = "-- " + min.ToString ("G4");
		midLowLabelText.GetComponent<TextMesh> ().text = "-- " + midLow.ToString ("G4");
		midTopLabelText.GetComponent<TextMesh> ().text = "-- " + midTop.ToString ("G4");
	}

	public void SetTitle(string t ){
		title = t;
		titleText.GetComponent<TextMesh> ().text = title;
	}

	void CreateTitle(Vector3 localpos){
		titleText = new GameObject();
		titleText.name = "Title";
		titleText.AddComponent <TextMesh>();
		titleText.GetComponent<TextMesh> ().text = title;
		titleText.GetComponent<TextMesh> ().fontSize =80;
		titleText.GetComponent<TextMesh> ().anchor = TextAnchor.MiddleLeft;
		titleText.transform.SetParent (transform);
		titleText.transform.localPosition = localpos;
		titleText.transform.localScale = new Vector3 (0.005f, 0.005f, 0f);

	}
	
	void CreateLabel(float value, Vector3 localpos){
		GameObject scaleText = new GameObject ();
		scaleText = InitLabel(scaleText, value, localpos);
	}

	GameObject InitLabel(GameObject obj, float value, Vector3 localpos){
		obj.AddComponent <TextMesh>();
		obj.GetComponent<TextMesh> ().text = "-- "+ value.ToString("G4");
		obj.GetComponent<TextMesh> ().fontSize =70;
		obj.GetComponent<TextMesh> ().anchor = TextAnchor.MiddleLeft;
		obj.transform.SetParent (transform);
		obj.transform.localPosition = localpos;
		obj.transform.localScale = new Vector3 (0.005f, 0.005f, 0f);
		return obj;
	}

	GameObject CreateLabel(GameObject obj, string name, float value, Vector3 localpos){
		obj = new GameObject ();
		obj.name = name;
		obj = InitLabel (obj, value, localpos);
		return obj;
	}

	void Update(){

	}
}
