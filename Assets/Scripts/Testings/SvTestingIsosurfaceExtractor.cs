using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CielaSpike;
using System.Diagnostics;

public class SvTestingIsosurfaceExtractor : MonoBehaviour {
	SvrIsosurfaceExtractor isoExtractor;
	Stopwatch watch = new Stopwatch ();
	public string activeScalar;
	public string colorScalar;
	public float isovalue;
	Task task;
	void Start () {
		isoExtractor = GetComponent<SvrIsosurfaceExtractor> ();
		//StartCoroutine (ReadingTestSingle());
		StartCoroutine (ExtractTest());
	}
	
	IEnumerator ExtractTest(){

		string dataset = "C:\\Users\\Kadek\\Documents\\MinorThesis\\Dataset\\VTR\\re950pipi2.1042.uvw.h5.vtr";

		//Dataset 1
		print("Reading dataset -> " + dataset);
		watch.Start ();
		isoExtractor.SetDatasetPath (dataset);
		this.StartCoroutineAsync (isoExtractor.ReadData (), out task);
		yield return StartCoroutine (task.Wait ());
		watch.Stop ();
		print ("Reading done -> " + watch.ElapsedMilliseconds);
		watch.Reset ();

		SvrColorBar bar = GameObject.FindObjectOfType<SvrColorBar> ();
		//bar.SetLabels ((float)isoExtractor.GetRange (colorScalar)[0],(float)isoExtractor.GetRange (colorScalar)[1]);
		bar.SetTitle (colorScalar);

		GameObject parent = new GameObject ();
		parent.name = "isosurface";

		print ("Constructing isosurface");
		print ("Contour -> " + activeScalar);
		print ("Color -> " + colorScalar);
		print ("Isovalue -> " + isovalue);
		isoExtractor.SetActiveScalar (activeScalar);
		isoExtractor.SetActiveColor (colorScalar);
		isoExtractor.SetIsovalue (isovalue);
		isoExtractor.StartConstructIsosurfaceGameobject (parent);

	}
}
