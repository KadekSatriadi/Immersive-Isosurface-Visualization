using System.Collections;
using System.Collections.Generic;
using Kitware.VTK;

public class SvrRegion {
	public List<vtkPolyData> poly = new List<vtkPolyData> ();
	public List<vtkPolyData> polyLow = new List<vtkPolyData> ();
	

	public List<vtkPolyData> getPoly(){
		return poly;
	}

	public List<vtkPolyData> getPolyLow(){
		return polyLow;
	}

	public void setPoly(List<vtkPolyData> pol){
		poly = pol;
	}

	public void setPolyLow(List<vtkPolyData> polLow){
		polyLow = polLow;
	}
}
