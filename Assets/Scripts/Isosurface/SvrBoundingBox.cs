using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SvrBoundingBox : MonoBehaviour {
	public Vector3 a,b,c,d,e,f,g,h;
	public Vector3 size;
	public Vector3 center;
	public bool active = true;
	Color color;
	public Material mat;
	Canvas canvas;
	static float  MINSCALE = 0.0001f;
	public Text topText;
	public Text aPointText;
	public Text bPointText;
	public Text cPointText;
	public Text dPointText;
	public Text ePointText;
	public Text fPointText;
	public Text gPointText;
	public Text hPointText;
	public GameObject orientation;
	public SvrBoundingBox parentBound;
	public static float relativeVolumeForLineWidth = 6.7f;
	public static float relativeLineWidth = 3.5f;
	public static float minLineWidth = 0.35f;
	public static float maxLineWidth = 10f;
	public bool centerLocal = false;

	Text CreateText(string name, GameObject parent, string t){
		GameObject g = new GameObject ();
		g.name = name;
		g.AddComponent<Text> ();
		g.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 100f);
		g.AddComponent<SvrBillboard> ();
		g.transform.SetParent (parent.transform);
		Text text = g.GetComponent<Text> ();
		text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		text.fontSize = 30;
		text.alignment = TextAnchor.MiddleCenter;
		text.text = t;
		return text;
	}


	public void SetCenterLocalOn(){
		center = transform.InverseTransformPoint (center);
		centerLocal = true;
	}

	public void SetCenterLocalOff(){
		center = transform.TransformPoint (center);
		centerLocal = false;
	}

	void CreateUI(){
		
		GameObject canvasGameobject = new GameObject ();
		canvasGameobject.name = "Canvas";
		canvasGameobject.transform.SetParent (transform);
		canvasGameobject.AddComponent<Canvas> ();
		canvas = canvasGameobject.GetComponent<Canvas> ();
		canvas.renderMode = RenderMode.WorldSpace;

		topText = CreateText ("TopText", canvasGameobject, "Size: ");
		aPointText = CreateText ("A", canvasGameobject, "A");
		bPointText = CreateText ("B", canvasGameobject, "B");
		cPointText = CreateText ("C", canvasGameobject, "C");
		dPointText = CreateText ("D", canvasGameobject, "D");
		ePointText = CreateText ("E", canvasGameobject, "E");
		fPointText = CreateText ("F", canvasGameobject, "F");
		gPointText = CreateText ("G", canvasGameobject, "G");
		hPointText = CreateText ("H", canvasGameobject, "H");
	}

	void Start(){
		Init ();
	}

	public void Init(){
		color = Color.green;


		CreateUI ();
		CalculatePoints ();
		UpdatePointLabelsPosition ();
		//	orientation =   Instantiate (Resources.Load ("3DOrientation")) as GameObject;
		//	orientation.transform.position = transform.TransformPoint(center);
		//	orientation.transform.localRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
		//	orientation.transform.SetParent (transform);
		DrawLines();
	}

	void UpdatePointLabelsPosition(){
		aPointText.rectTransform.position = transform.TransformPoint(a);
		bPointText.rectTransform.position = transform.TransformPoint(b);;
		cPointText.rectTransform.position = transform.TransformPoint(c);
		dPointText.rectTransform.position = transform.TransformPoint(d);
		ePointText.rectTransform.position = transform.TransformPoint(e);
		fPointText.rectTransform.position = transform.TransformPoint(f);
		gPointText.rectTransform.position = transform.TransformPoint(g);
		hPointText.rectTransform.position = transform.TransformPoint(h);


	}

	string FormatPointText(Vector3 x){
		string s = x.ToString ("G4");
		return s;
	}

	void UpdatePointLabelsText(){
		if (parentBound == null) {
			aPointText.text = a.ToString ("G4");
			bPointText.text = b.ToString ("G4");
			cPointText.text = c.ToString ("G4");
			dPointText.text = d.ToString ("G4");
			ePointText.text = e.ToString ("G4");
			fPointText.text = f.ToString ("G4");
			gPointText.text = g.ToString ("G4");
			hPointText.text = h.ToString ("G4");
		} else {
			aPointText.text = FormatPointText (alignLocalCoordinate (a));
			bPointText.text = FormatPointText (alignLocalCoordinate (b));
			cPointText.text = FormatPointText (alignLocalCoordinate (c));
			dPointText.text =  FormatPointText (alignLocalCoordinate (d));
			ePointText.text = FormatPointText (alignLocalCoordinate (e));
			fPointText.text = FormatPointText (alignLocalCoordinate (f));
			gPointText.text = FormatPointText (alignLocalCoordinate (g));
			hPointText.text = FormatPointText (alignLocalCoordinate (h));
		}
	}

	public Vector3 alignLocalCoordinate(Vector3 p){
		return parentBound.transform.InverseTransformPoint (transform.TransformPoint (p));
	}

	public void ToggleActive(){
		if (active)
			active = false;
		else
			active = true;
	}
		
	public void CalculateSizeAndCenter(){
		size = new Vector3 ( Mathf.Abs(a.x - b.x),  Mathf.Abs(e.y - a.y),  Mathf.Abs(d.z - a.z));

		if (centerLocal && parentBound != null) {
			center = Vector3.zero;
			CalculatePoints ();
		} else {
			center = new Vector3 ((b.x + a.x) / 2f, (e.y + a.y) / 2f, (d.z + a.z) / 2f);
		}


	}

	public void CalculatePoints(){
		if (size == null)
			return;
		if (center == null)
			return;



		float length =  size.x;
		float height =  size.y;
		float width =  size.z;
		float xm = center.x - (length / 2f);
		float xp = center.x + (length / 2f);
		float ym = center.y - (height / 2f);
		float yp = center.y + (height / 2f);
		float zm = center.z - (width / 2f);
		float zp = center.z + (width / 2f);

		a = new Vector3 (xm, yp, zm);
		b = new Vector3 (xp, yp, zm);
		c = new Vector3 (xp, yp, zp);
		d = new Vector3 (xm, yp, zp);

		e = new Vector3 (xm, ym, zm);
		f = new Vector3 (xp, ym, zm);
		g = new Vector3 (xp, ym, zp);
		h = new Vector3 (xm, ym, zp);
	}

	void Update(){
		GetComponent<LineRenderer> ().enabled = active;

		if(size.x / 1000f > MINSCALE)
			canvas.GetComponent<RectTransform> ().localScale = new Vector3 (size.x / 1000f, size.x / 1000f, size.x / 1000f);
		else
			canvas.GetComponent<RectTransform> ().localScale = new Vector3 (MINSCALE, MINSCALE, MINSCALE);
		topText.text = size.x + "x" + size.y + "x" + size.z;
		UpdatePointLabelsText ();
		UpdatePointLabelsPosition ();
		topText.rectTransform.position = transform.TransformPoint(new Vector3(center.x,  (size.y/2f) + center.y + (size.y/3f) , center.z));

		//orientation.transform.position = transform.TransformPoint(center);
		float avg = (size.x + size.y + size.z)/3f;
		//orientation.transform.localScale = new Vector3 (avg, avg, avg);
	}

	void DrawLines(){
		float linewidth = (size.x * size.y * size.z / relativeVolumeForLineWidth) * relativeLineWidth;
		if (linewidth < minLineWidth)
			linewidth = minLineWidth;
		if (linewidth > maxLineWidth)
			linewidth = maxLineWidth;
		mat = new Material (Shader.Find ("Hidden/Internal-Colored"));
		gameObject.AddComponent<LineRenderer> ();
		LineRenderer line1 = gameObject.GetComponents<LineRenderer> () [0];
		line1.useWorldSpace = false;
		line1.startWidth = 0.002f;
		line1.endWidth =  0.002f;
		line1.widthMultiplier = linewidth;
		line1.numCornerVertices = 90;
		line1.material = mat;
		line1.positionCount = 32;
		line1.SetPosition (0, a);
		line1.SetPosition (1,b);
		line1.SetPosition (2,b);
		line1.SetPosition (3,f);
		line1.SetPosition (4,f);
		line1.SetPosition (5,e);
		line1.SetPosition (6,e);
		line1.SetPosition (7,a);
		line1.SetPosition (8,a);
		line1.SetPosition (9,d);
		line1.SetPosition (10,d);
		line1.SetPosition (11,c);
		line1.SetPosition (12,c);
		line1.SetPosition (13,b);
		line1.SetPosition (14,b);
		line1.SetPosition (15,f);
		line1.SetPosition (16,f);
		line1.SetPosition (17,g);
		line1.SetPosition (18,g);
		line1.SetPosition (19,h);
		line1.SetPosition (20,h);
		line1.SetPosition (21,d);
		line1.SetPosition (22,d);
		line1.SetPosition (23,h);
		line1.SetPosition (24,h);
		line1.SetPosition (25,e);
		line1.SetPosition (26,e);
		line1.SetPosition (27,h);
		line1.SetPosition (28,h);
		line1.SetPosition (29,g);
		line1.SetPosition (30,g);
		line1.SetPosition (31,c);
	}

	public void NoLongerUsed (){
		if (!active)
			return;
		
		//CalculatePoints ();
		mat = new Material (Shader.Find ("Hidden/Internal-Colored"));
	
		mat.SetColor(0,Color.yellow);
		mat.SetPass (0);
		GL.PushMatrix ();
		GL.MultMatrix (transform.localToWorldMatrix);
		GL.Begin (GL.LINES);
		//line 1
		GL.Vertex (a);
		GL.Vertex (d);
		//line 2
		GL.Vertex (a);
		GL.Vertex (b);
		//line 3
		GL.Vertex (b);
		GL.Vertex (c);
		//line 4
		GL.Vertex (c);
		GL.Vertex (d);
		//line 5
		GL.Vertex (e);
		GL.Vertex (h);
		//line 6
		GL.Vertex (e);
		GL.Vertex (f);
		//line 7
		GL.Vertex (f);
		GL.Vertex (g);
		//line 8
		GL.Vertex (g);
		GL.Vertex (h);
		//line 9
		GL.Vertex (d);
		GL.Vertex (h);
		//line 10
		GL.Vertex (a);
		GL.Vertex (e);
		//line 11
		GL.Vertex (b);
		GL.Vertex (f);
		//line 12
		GL.Vertex (c);
		GL.Vertex (g);

		GL.End ();
		GL.PopMatrix ();
	}

	string VectorToJsonArray(Vector3 v){
		return "[" + v.x + "," + v.y + "," + v.z + "]";
	}

	public string ToString(){
		string s = "{\"Size\": " + VectorToJsonArray(size) + ",\n";
		if (parentBound == null) {
			s += "\"Center\": " + VectorToJsonArray (center) + ",\n";
			s += "\"Points\":{ \n";
			s += "\"A\": " + VectorToJsonArray (a) + ",\n";
			s += "\"B\": " + VectorToJsonArray (b) + ",\n";
			s += "\"C\": " + VectorToJsonArray (c) + ",\n";
			s += "\"D\": " + VectorToJsonArray (d) + ",\n";
			s += "\"E\": " + VectorToJsonArray (e) + ",\n";
			s += "\"F\": " + VectorToJsonArray (f) + ",\n";
			s += "\"G\": " + VectorToJsonArray (g) + ",\n";
			s += "\"H\": " + VectorToJsonArray (h) + "\n";
			s += "}}";
		} else {
			s += "\"Center\": " + VectorToJsonArray (alignLocalCoordinate(center)) + ",\n";
			s += "\"Points\":{ \n";
			s += "\"A\": " + VectorToJsonArray (alignLocalCoordinate(a)) + ",\n";
			s += "\"B\": " + VectorToJsonArray (alignLocalCoordinate(b)) + ",\n";
			s += "\"C\": " + VectorToJsonArray (alignLocalCoordinate(c)) + ",\n";
			s += "\"D\": " + VectorToJsonArray (alignLocalCoordinate(d)) + ",\n";
			s += "\"E\": " + VectorToJsonArray (alignLocalCoordinate(e)) + ",\n";
			s += "\"F\": " + VectorToJsonArray (alignLocalCoordinate(f)) + ",\n";
			s += "\"G\": " + VectorToJsonArray (alignLocalCoordinate(g)) + ",\n";
			s += "\"H\": " + VectorToJsonArray (alignLocalCoordinate(h)) + "\n";
			s += "}}";
		}
		return s;
	}
}