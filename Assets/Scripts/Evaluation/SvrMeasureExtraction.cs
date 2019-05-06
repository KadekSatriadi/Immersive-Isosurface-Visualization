using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SvrMeasureExtraction : MonoBehaviour {
	SvrIsosurfaceExtractor isoExtractor;
	public string scalarname ;
	public string colorname ;
	public float[] isovalues;
	int activeIndex = 0;
	public void StartExtractions(){
		isoExtractor = GameObject.FindObjectOfType<SvrIsosurfaceExtractor> ();
		isoExtractor.SetIsovalue (isovalues[activeIndex],scalarname);
		isoExtractor.SetActiveColor (colorname);
		GameObject obj = new GameObject ();
		obj.name = System.IO.Path.GetFileName (GameObject.FindObjectOfType<SvrConfiguration>().datasetpath) + "_"+scalarname+"_"+"_iso_" + isovalues[activeIndex] + "_color_" + colorname;
		StartCoroutine (isoExtractor.ConstructIsosurfaceGameObjectRegionBased (obj));
		activeIndex++;
	}

	public void RecordTime(long time, double value){
		print ("time -> " + time);
		string path = @System.IO.Path.Combine (Application.dataPath, "Benchmark_Runtime_Construction_" + value + ".txt");
		string content =   value + "," + time;
		GameObject.FindObjectOfType<SvrBenchmark>().WriteBenchmarkText (path, content);

		if (activeIndex < isovalues.Length)
			StartExtractions ();
	}
}
