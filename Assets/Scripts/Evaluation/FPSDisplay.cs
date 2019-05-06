using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
	float sumfps = 0;
	public int count = 0;
	public float avgfps = 0.0f;
	public float fps = 0.0f;
	string text; 
	public bool isActive = true;
	void Update()
	{
		if(!isActive) return;

		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		count++;
		sumfps += fps;
		avgfps = sumfps / count;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		fps = 1.0f / deltaTime;


		text = "fps : " + fps.ToString ("F") + ", frame : " + count + ", avg : " + avgfps.ToString ("F");
		GUI.Label(rect, text, style);
	}
}