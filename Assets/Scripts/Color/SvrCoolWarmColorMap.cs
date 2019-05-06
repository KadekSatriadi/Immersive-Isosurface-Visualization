using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;



public class SvrCoolWarmColorMap {
	List<SvColorItemRGB> colors = new List<SvColorItemRGB>();
	public SvrCoolWarmColorMap (string path) {
		if (System.IO.File.Exists (path)) {
			try
			{   
				using (StreamReader sr = new StreamReader(@path))
				{
					string line = sr.ReadToEnd();
					string[,] grid = SvrCSVReader.SplitCsvGrid(line);
					for (int y = 1; y < grid.GetUpperBound(1); y++) {	
						float s = float.Parse(grid[0,y]);
						float r = float.Parse(grid[1,y]);
						float g = float.Parse(grid[2,y]);
						float b = float.Parse(grid[3,y]);
						colors.Add(new SvColorItemRGB(s,r,g,b));
					}
				}
			}
			catch (Exception e)
			{
				
			}

		} else {
			throw new UnityException ("Color map not found");
		}
	}

	public Color[] ArrayToColor(float[] values, float max, float min){
		List<Color> colors = new List<Color> ();
		foreach (float value in values) {
			float norm = (value - min)/ (max - min);
			colors.Add (GetColor (norm));
		}
		return colors.ToArray ();
	}

	public Color GetColor(float value){
		SvColorItemRGB closest = colors [0];
		float closestValue = Mathf.Abs(closest.scalar - value);
		foreach (SvColorItemRGB color in colors) {
			if (closestValue > Mathf.Abs(color.scalar - value)) {
				closest = color;
				closestValue = Mathf.Abs (color.scalar - value);
			}
		}
		float r =  closest.r;
		float g =  closest.g;
		float b =  closest.b;
		return new Color(r, g, b, 1f);
	}
}
