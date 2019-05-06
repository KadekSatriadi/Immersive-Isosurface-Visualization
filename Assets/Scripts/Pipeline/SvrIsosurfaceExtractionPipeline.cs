using System.Collections;
using System.Collections.Generic;
using Kitware.VTK;
using CielaSpike;
using UnityEngine;

public class SvrIsosurfaceExtractionPipeline {
	public static int VERTICESMAX = 65000;
	public static int LODTHRESHOLD = 1000;
	public static float DECIMATEREDUCTION = 0.9f;
	public List<SvrRegion> regions;
	public int elapsed;

	public static List<vtkPolyData> CreateGroups(vtkPolyData poly)
    {
		vtkOBBDicer dicer = vtkOBBDicer.New ();
		vtkThreshold th = vtkThreshold.New ();
		vtkGeometryFilter geo = vtkGeometryFilter.New ();
		List<vtkPolyData> polys = new List<vtkPolyData> ();

		dicer.SetInput (poly);
		dicer.SetDiceModeToSpecifiedNumberOfPieces ();
		dicer.SetNumberOfPointsPerPiece((int) VERTICESMAX);
		dicer.Update ();
		th.SetInput (dicer.GetOutput ());
		th.AllScalarsOff ();
        th.SetInputArrayToProcess(0, 0, 0, 0, "vtkOBBDicer_GroupIds");
        geo.SetInputConnection (th.GetOutputPort ());

		for(int i = 0; i < dicer.GetNumberOfActualPieces(); i ++){
            th.ThresholdBetween (i, i);	
			th.Update ();
			geo.Update ();
			vtkPolyData pol = vtkPolyData.New();
            pol.DeepCopy(geo.GetOutput());
            th.ThresholdBetween(i, i);
            th.Update();
            geo.Update();
            polys.Add (pol);
		}

		return polys;
	}

	public static List<SvrRegion> DecimateRegionList(List<SvrRegion> regions, string colorScalar){
		vtkDecimatePro decimate = vtkDecimatePro.New ();


		for (int i = 0; i <  regions.Count;i++) {
			List<vtkPolyData> polyLow = new List<vtkPolyData> ();
			foreach (vtkPolyData poly in regions[i].getPoly()) {
				if (poly.GetNumberOfPoints () <= LODTHRESHOLD)
					continue;
				decimate.SetInput (poly);
				decimate.SetInputArrayToProcess (0, 0, 0, 0, colorScalar);
				decimate.SetTargetReduction (DECIMATEREDUCTION);
				decimate.Update ();
				vtkPolyData o = vtkPolyData.New ();
				o.DeepCopy (decimate.GetOutput ());
				polyLow.Add (o);
			}
			regions[i].setPolyLow (polyLow);
		}
		return regions;
	}

	public IEnumerator GenerateRegions(vtkDataObject volume, float isovalue, string arrayName, string colorScalar){
		elapsed = 0;
		vtkContourFilter contour = vtkContourFilter.New ();
		vtkPolyDataConnectivityFilter connect = vtkPolyDataConnectivityFilter.New ();
		vtkThreshold th = vtkThreshold.New ();
		vtkGeometryFilter geo = vtkGeometryFilter.New ();

		contour.SetInput (volume);
		contour.SetInputArrayToProcess (0, 0, 0, 0, arrayName);
		contour.ComputeNormalsOff ();
		contour.SetValue (0, isovalue);
		contour.Update ();

		connect.SetInput (contour.GetOutput ());
		connect.SetExtractionModeToAllRegions ();
		connect.ColorRegionsOn ();
		connect.Update ();


		regions =new List<SvrRegion>();
		System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch ();
		w.Start ();
		if (connect.GetNumberOfExtractedRegions () > 0) {
			

			th.SetInput (connect.GetOutput ());
			th.SetInputArrayToProcess (0, 0, 0, 0, "RegionId");
			geo.SetInputConnection(th.GetOutputPort());

			int nregions =  connect.GetNumberOfExtractedRegions();
			for(int i = 0; i < nregions; i++){
				SvrRegion region = new SvrRegion ();

				List<vtkPolyData> polyList = new List<vtkPolyData> ();
				th.ThresholdBetween (i, i);
				th.Update ();
				geo.Update();

				vtkPolyData poly = vtkPolyData.New ();
				poly.DeepCopy (geo.GetOutput ());
				poly.GetPointData().SetActiveScalars(colorScalar);
				long npoints = (int)poly.GetNumberOfPoints();

				if(npoints> VERTICESMAX){
					region.poly.AddRange(CreateGroups(poly));
				}else{
					region.poly.Add(poly);
				}

				regions.Add(region);
			}

			if (contour.GetOutput ().GetNumberOfPoints () > LODTHRESHOLD) {
				regions = DecimateRegionList (regions, colorScalar);
			}

		}

		w.Stop ();
		elapsed = (int) w.ElapsedMilliseconds;

		yield return null;
	}

}
