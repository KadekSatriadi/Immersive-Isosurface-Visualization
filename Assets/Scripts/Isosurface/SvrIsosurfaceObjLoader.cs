using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SvrIsosurfaceObjLoader: MonoBehaviour {
	Material material;
	SvrCoolWarmColorMap colorMap;
	SvrGUITextOutputConsole consoleText;
	public string contour ;
	public string color ;
	public float isovalue;
	public float scalarmax;
	public float scalarmin;
	public bool isLoading = false;
	public string configpath;
	public bool useColor = false;
    string datasetpath;
    int nregion;
    string section;
    string name;
    float colormax;
    float colormin;
    float minvx;
    float maxvx;
    float minvy;
    float maxvy;
    float minvz;
    float maxvz;
    string parentpath;
    string objspath;

    public void Init(string p){
		material = Resources.Load ("VertexColorMaterial", typeof(Material)) as Material;
		string path = System.IO.Path.Combine(Application.streamingAssetsPath, "CoolWarmFloat257.csv");
		colorMap = new SvrCoolWarmColorMap (path);
		consoleText = GameObject.FindObjectOfType<SvrGUITextOutputConsole> ();
		configpath = p;
	}

	public  Color[] GetColorArray(string path, float max, float min){
		float[] values = GetColorFileContent (path);
		return colorMap.ArrayToColor (values, max, min);
	}

	public  float[] GetColorFileContent(string path){
		string text = GetTextContent (path);
		string[] stringvalues = text.Split(',');
		float[] values = new float[stringvalues.Length];
		for(int i =0; i < stringvalues.Length; i++) {
			values[i] = float.Parse(stringvalues[i]);
		}
		return values;
	}

	public static string GetNameFromConfig(string configpath){
		INIParser ini = new INIParser ();
		ini.Open (configpath);
		string section = "Isosurface";
		string name = ini.ReadValue (section, "name", "isosurface");
		return name;
	}

    void LoadMetaData()
    {
        parentpath = System.IO.Path.GetDirectoryName(configpath);
        objspath = System.IO.Path.Combine(parentpath, "objs");
        INIParser ini = new INIParser();
        ini.Open(configpath);
        section = "Isosurface";
        name = ini.ReadValue(section, "name", "isosurface");
        isovalue = (float)ini.ReadValue(section, "value", 0f);
        nregion = ini.ReadValue(section, "nregions", 0);
        datasetpath = ini.ReadValue(section, "dataset", "");
        contour = ini.ReadValue(section, "contour", "");
        color = ini.ReadValue(section, "color", "");
        colormax = (float)ini.ReadValue(section, "colormax", 0f);
        colormin = (float)ini.ReadValue(section, "colormin", 0f);
        scalarmax = (float)ini.ReadValue(section, "scalarmax", 0f);
        scalarmin = (float)ini.ReadValue(section, "scalarmin", 0f);
        minvx = (float)ini.ReadValue(section, "minvx", 0f);
        maxvx = (float)ini.ReadValue(section, "maxvx", 0f);
        minvy = (float)ini.ReadValue(section, "minvy", 0f);
        maxvy = (float)ini.ReadValue(section, "maxvy", 0f);
        minvz = (float)ini.ReadValue(section, "minvz", 0f);
        maxvz = (float)ini.ReadValue(section, "maxvz", 0f);

    }
    public void LoadIsosurfaceOBJ(string configpath){
		System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch ();
		w.Start ();

        LoadMetaData();

		GameObject parent = new GameObject();
		parent.name = name;
		parent.tag = "Isosurface";
		parent.AddComponent <SvrIsosurface> ();
		parent.GetComponent<SvrIsosurface> ().isovalue = isovalue;
		SvrIsosurface iso = parent.GetComponent<SvrIsosurface> ();

		List<GameObject> regions = new List<GameObject>();
		for (int i = 0; i < nregion; i++) {
			GameObject region = new GameObject ();
			region.name = "Region" + i;
			region.tag = "Region";

			region.transform.SetParent (parent.transform);
			iso.regions.Add (region);
			string regionfolderpath = System.IO.Path.Combine (objspath, "region"+i);
			DirectoryInfo regiondir = new DirectoryInfo (@regionfolderpath);
			DirectoryInfo[] groupdirs = regiondir.GetDirectories ();
			for (int j = 0; j < groupdirs.Length; j++) {
				DirectoryInfo groupdir = groupdirs [j];
				FileInfo[] objfiles = groupdir.GetFiles ("*.obj");
				FileInfo[] mtlfiles = groupdir.GetFiles ("*.mtl");
				FileInfo[] colorfiles = groupdir.GetFiles ("*.txt");
				GameObject groupGO = new GameObject ();
				groupGO.name = "Group_" + j;
				groupGO.transform.SetParent (region.transform);
				GameObject LOD0 = null;
				GameObject LOD1 = null;
				for (int k = 0; k < objfiles.Length; k++) {
					string objpath = objfiles [k].FullName;
					string mtlpath = mtlfiles [k].FullName;
					string colorpath = "";
					if(k < colorfiles.Length)
						 colorpath = colorfiles [k].FullName;
					string objcontent = GetTextContent (objpath);
					string mtlcontent = GetTextContent (mtlpath);
					string fname = objfiles[k].Name;
					if(fname.Contains("LOD0_")){ 
						LOD0 = ObjImporter.Import(objcontent, mtlcontent, new Texture2D[0]);
						//yield return StartCoroutine(ObjImporter.ImportInBackground(objcontent, mtlcontent, null, retval => LOD1 = retval));

						if (LOD0 != null) {
							LOD0 = SvrIsosurfaceExtractor.FlipZ (LOD0);

							LOD0.transform.SetParent (groupGO.transform);
							LOD0.GetComponent<MeshRenderer> ().material = material;
							if (useColor && colorpath != "") {
								Color[] colors = GetColorArray (colorpath, colormax, colormin);
								LOD0.GetComponent<MeshFilter> ().mesh.colors = colors;
							}
							LOD0.name = "LOD0";
							LOD0 = ProcessLoadedGameobject (LOD0);
						}
					}else if(fname.Contains("LOD1_")){
						LOD1 = ObjImporter.Import(objcontent, mtlcontent, new Texture2D[0]);
						//yield return StartCoroutine(ObjImporter.ImportInBackground(objcontent, mtlcontent, null, retval => LOD2 = retval));

						if (LOD1 != null) {
							LOD1 = SvrIsosurfaceExtractor.FlipZ (LOD1);

							LOD1.transform.SetParent (groupGO.transform);
							LOD1.GetComponent<MeshRenderer> ().material = material;
							if (useColor) {
								Color[] colors = GetColorArray (colorpath, colormax, colormin);
								LOD1.GetComponent<MeshFilter> ().mesh.colors = colors;
							}
							LOD1.name = "LOD1";
							LOD1 = ProcessLoadedGameobject (LOD1);
						}
					}

				}
				if (LOD1 != null) {
					groupGO = SvrIsosurfaceExtractor.InitLODGroupGOComponents (groupGO, LOD0, LOD1);
				}
			}
		}

		Vector3 boundSize = SvrIsosurfaceExtractor.CalculateSize (minvx, maxvx, minvy, maxvy, minvz, maxvz);
		Vector3 boundCenter = SvrIsosurfaceExtractor.CalculateCenter (minvx, maxvx, minvy, maxvy, minvz, maxvz);
		SvrIsosurfaceExtractor.ParentPostProcess (parent, boundSize, boundCenter);
		w.Stop ();
		print ("Loading success (elapsed " + w.ElapsedMilliseconds + "ms)");

		/*
		string path = @System.IO.Path.Combine (Application.dataPath, "Benchmark_Loading_" + parent.name + ".txt");
		string content = parent.name + "," + w.ElapsedMilliseconds;
		GameObject.FindObjectOfType<SvrBenchmark> ().WriteBenchmarkText (path, content);

		// Starting benchmark
		print("Benchmarking");
		GameObject.FindObjectOfType<SvrBenchmark> ().Benchmark ();
		*/
	}

	GameObject ProcessLoadedGameobject(GameObject g){
		g.tag = "Isosurface";
		g.AddComponent<MeshCollider> ();
		return g;
	}

	public  string GetTextContent(string path){
		string lines = System.IO.File.ReadAllText (@path);
		return lines;
	}

	private IEnumerator LoadLOD1(string url, GameObject importedObject) {
		string objString = null;
		string mtlString = null;
		Hashtable textures = null;
			
		if(objString!=null && objString.Length>0) {
			yield return StartCoroutine(ObjImporter.ImportInBackground(objString, mtlString, textures, retval => importedObject = retval));
		}
	}

	void print(string t){
		if (consoleText != null) {
			consoleText.SetText (t);
		}
		Debug.Log (t);
	}
}
