using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using CielaSpike;

public class SvTestingDatasetReading : MonoBehaviour {
	SvrIsosurfaceExtractor isoExtractor;
	Stopwatch watch = new Stopwatch ();
	Task task;
	void Start () {
		isoExtractor = GetComponent<SvrIsosurfaceExtractor> ();
		//StartCoroutine (ReadingTestSingle());
		StartCoroutine (ReadingTestPartition());
	}

	IEnumerator ReadingTestPartition(){
		string partition1 = @"C:\\Users\\Kadek\\Documents\\MinorThesis\\Dataset\\VTR Partitions\\3GB";
		string partition2 = @"C:\\Users\\Kadek\\Documents\\MinorThesis\\Dataset\\VTR Partitions\\8GB";

		watch.Start ();
		isoExtractor.SetPartitionMode (true);
		isoExtractor.SetPartitionFolder (partition1);
		this.StartCoroutineAsync (isoExtractor.ReadData (), out task);
		yield return StartCoroutine (task.Wait ());
		watch.Stop ();
		print ("Paritions1 -> " + watch.ElapsedMilliseconds);
		watch.Reset ();

		watch.Start ();
		isoExtractor.SetPartitionMode (true);
		isoExtractor.SetPartitionFolder (partition2);
		this.StartCoroutineAsync (isoExtractor.ReadData (), out task);
		yield return StartCoroutine (task.Wait ());
		watch.Stop ();
		print ("Paritions2 -> " + watch.ElapsedMilliseconds);
		watch.Reset ();
	}


	IEnumerator ReadingTestSingle(){

		string dataset1 = "C:\\Users\\Kadek\\Documents\\MinorThesis\\Dataset\\VTR\\re950pipi2.1042.u.h5.vtr";
		string dataset2 = "C:\\Users\\Kadek\\Documents\\MinorThesis\\Dataset\\VTR\\re950pipi2.1042.v.h5.vtr";
		string dataset3 = "C:\\Users\\Kadek\\Documents\\MinorThesis\\Dataset\\VTR\\re950pipi2.1042.w.h5.vtr";
		string dataset4 = "C:\\Users\\Kadek\\Documents\\MinorThesis\\Dataset\\VTR\\re950pipi2.1042.u.v.w.h5.vtr";
		string dataset5 = "C:\\Users\\Kadek\\Documents\\MinorThesis\\Dataset\\VTR Partitions\\3GB\\tbl.258.phys.uvd.h5__0.vtr";
		string dataset6 = "C:\\Users\\Kadek\\Documents\\MinorThesis\\Dataset\\VTR Partitions\\8GB\\tbl.258.phys.Qd.h5__0.vtr";

		//Dataset 1
		watch.Start ();
		isoExtractor.SetDatasetPath (dataset1);
		this.StartCoroutineAsync (isoExtractor.ReadData (), out task);
		yield return StartCoroutine (task.Wait ());
		watch.Stop ();
		print ("Dataset1 -> " + watch.ElapsedMilliseconds);
		watch.Reset ();

		//Dataset 2
		watch.Start ();
		isoExtractor.SetDatasetPath (dataset2);
		this.StartCoroutineAsync (isoExtractor.ReadData (), out task);
		yield return StartCoroutine (task.Wait ());
		watch.Stop ();
		print ("Dataset2 -> " + watch.ElapsedMilliseconds);
		watch.Reset ();

		//Dataset 3
		watch.Start ();
		isoExtractor.SetDatasetPath (dataset3);
		this.StartCoroutineAsync (isoExtractor.ReadData (), out task);
		yield return StartCoroutine (task.Wait ());
		watch.Stop ();
		print ("Dataset3 -> " + watch.ElapsedMilliseconds);
		watch.Reset ();


		//Dataset 4
		watch.Start ();
		isoExtractor.SetDatasetPath (dataset4);
		this.StartCoroutineAsync (isoExtractor.ReadData (), out task);
		yield return StartCoroutine (task.Wait ());
		watch.Stop ();
		print ("Dataset4 -> " + watch.ElapsedMilliseconds);
		watch.Reset ();

		//Dataset 5
		watch.Start ();
		isoExtractor.SetDatasetPath (dataset5);
		this.StartCoroutineAsync (isoExtractor.ReadData (), out task);
		yield return StartCoroutine (task.Wait ());
		watch.Stop ();
		print ("Dataset5 -> " + watch.ElapsedMilliseconds);
		watch.Reset ();

		//Dataset 6
		watch.Start ();
		isoExtractor.SetDatasetPath (dataset6);
		this.StartCoroutineAsync (isoExtractor.ReadData (), out task);
		yield return StartCoroutine (task.Wait ());
		watch.Stop ();
		print ("Dataset6 -> " + watch.ElapsedMilliseconds);
		watch.Reset ();
	}

}
