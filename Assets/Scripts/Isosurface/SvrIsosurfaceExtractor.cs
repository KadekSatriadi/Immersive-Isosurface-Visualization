using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kitware.VTK;
using CielaSpike;
using System.Diagnostics;
using System.IO;

public class SvrIsosurfaceExtractor : MonoBehaviour {
	public SvrGUITextOutputConsole consoleText;
    public List<string> scalarNames;
    public string activeScalar;
    public string activeColor;
    public Task task;
    public double minIsovalue, maxIsovalue;
    public double minColorvalue, maxColorvalue;
    vtkDataSet volume;
    Stopwatch stopwatch;
    SvrCoolWarmColorMap colorMap;
    int nvertices;
    string datasetPath;
	string[] partitionpaths;
    double isovalue;
	public bool isReady = false;
    bool partitionMode;
	float maxVXValue = float.MinValue;
	float maxVYValue = float.MinValue;
	float maxVZValue = float.MinValue;
	float minVXValue = float.MaxValue;
	float minVYValue = float.MaxValue;
	float minVZValue = float.MaxValue;
	Vector3 boundSize;
	Vector3 boundCenter;
	Material material;
    List<vtkDataSet> volumeParts;

    void Start(){
        stopwatch = new Stopwatch();
        material = Resources.Load ("VertexColorMaterial", typeof(Material)) as Material;
		string path = System.IO.Path.Combine(Application.streamingAssetsPath, "CoolWarmFloat257.csv");
		colorMap = new SvrCoolWarmColorMap (path);
		consoleText = GameObject.FindObjectOfType<SvrGUITextOutputConsole> ();
	}

    public void SetInput(vtkDataObject vol)
    {
        volume = (vtkDataSet)vol;
    }

    public void AddPartition(vtkDataObject vol)
    {
        if(volumeParts == null) volumeParts = new List<vtkDataSet>();


        vtkDataSet voldata = (vtkDataSet)vol;
        volumeParts.Add(voldata);
    }

    /*
     * <summary> 
     *Set partition mode on or off
     * </summary>
     */
    public void SetPartitionMode(bool val){
		partitionMode = val;
	}

    /*
     * <summary> 
     *Set partitions folder path
     * </summary>
     */
    public void SetPartitionFolder(string path){
		datasetPath = path;
	}

    /*
     * <summary> 
     *Set isosurface material (vertex colour)
     * </summary>
     */
    public void SetMaterial(Material mat)
	{
		this.material = mat;
	}

    /*
     * <summary> 
     *Set isovalue and scalar/contour array
     * </summary>
     */
    public void SetIsovalue(double isovalue, string scalar)
	{
		this.isovalue = isovalue;
		this.activeScalar = scalar;
	}

    /*
     * <summary> 
     *Set isovalue 
     * </summary>
     */
    public void SetIsovalue(double isovalue)
	{
		this.isovalue = isovalue;
	}

    /*
     * <summary> 
     *Set volume data path
     * </summary>
     */
    public void SetDatasetPath(string path)
	{
		this.datasetPath = path;
	}

    /*
     * <summary> 
     *Return the active scalar range of the volume (single or partitions)
     * </summary>
     */
    public double[] GetRange()
	{
		double[] r = GetRange (activeScalar);
		return r;
	}

    /*
     * <summary> 
     *Return the range of an array of the volume (single or partitions)
     * </summary>
     */
    public double[] GetRange(string scalar)
	{
		if (!partitionMode) {
			return volume.GetPointData ().GetArray (scalar).GetRange ();
		} else {
			int i = 0;
			double min = double.MaxValue;
			double max = double.MinValue;
			foreach (vtkRectilinearGrid r in volumeParts) {
				double[] isovalueRange = r.GetPointData ().GetArray (scalar).GetRange ();
				if (i == 0) {
					min = isovalueRange [0];
					max = isovalueRange [1];
				} else {
					min = Mathf.Min ((float)min, (float)isovalueRange [0]);
					max = Mathf.Max ((float)max, (float)isovalueRange [1]);
				}
				i++;
			}
			double[] range = {min,max};
			return range;
		}
	} 

    /*
     * <summary> 
     *Set color array
     * </summary>
     */
    public void SetActiveColor(string color){
		activeColor = color;
		double[] range = GetRange (color);
		minColorvalue = (float) range [0];
		maxColorvalue = (float) range [1];
	}

    /*
     * <summary> 
     *Set scalar/contour array
     * </summary>
     */
    public void SetActiveScalar(string name){
		activeScalar = name;
		double[] range = GetRange (name);
		minIsovalue = (float)range [0];
		maxIsovalue = (float)range [1];
	}

	public void InitVerticesBoundPartition(){
		for (int i=0; i < volumeParts.Count; i++) {
			if (i == 0) {
				minVXValue = (float) volumeParts [i].GetBounds () [0];
				maxVXValue = (float) volumeParts [i].GetBounds () [1];
				minVYValue = (float) volumeParts [i].GetBounds () [2];
				maxVYValue = (float) volumeParts [i].GetBounds () [3];
				minVZValue = (float) volumeParts [i].GetBounds () [4];
				maxVZValue = (float) volumeParts [i].GetBounds () [5];
			} else {
				if (minVXValue != (float) volumeParts [i].GetBounds () [0]){ minVXValue = Mathf.Min(minVXValue, (float) volumeParts [i].GetBounds () [0]); }
				if (maxVXValue != (float) volumeParts [i].GetBounds () [1]){ maxVXValue = Mathf.Max(maxVXValue, (float) volumeParts [i].GetBounds () [1]); }
				if (minVYValue != (float) volumeParts [i].GetBounds () [2]){ minVYValue = Mathf.Min(minVYValue, (float) volumeParts [i].GetBounds () [2]); }
				if (maxVYValue != (float) volumeParts [i].GetBounds () [3]){ maxVYValue = Mathf.Max(maxVYValue, (float) volumeParts [i].GetBounds () [3]); }
				if (minVZValue != (float) volumeParts [i].GetBounds () [4]){ minVZValue = Mathf.Min(minVZValue, (float) volumeParts [i].GetBounds () [4]); }
				if (maxVZValue != (float) volumeParts [i].GetBounds () [5]){ maxVZValue = Mathf.Max(maxVZValue, (float) volumeParts [i].GetBounds () [5]); }
			}
		}
	}

	public void InitVerticesBound(){
		minVXValue = (float) volume.GetBounds () [0];
		maxVXValue = (float) volume.GetBounds () [1];
		minVYValue = (float) volume.GetBounds () [2];
		maxVYValue = (float) volume.GetBounds () [3];
		minVZValue = (float) volume.GetBounds () [4];
		maxVZValue = (float) volume.GetBounds () [5];
	}


    void ReadVTRSingle()
    {
        vtkXMLRectilinearGridReader reader = vtkXMLRectilinearGridReader.New();
        reader.SetFileName(datasetPath);
        reader.Update();
        volume = reader.GetOutput();
        minIsovalue = volume.GetPointData().GetArray(0).GetRange()[0];
        maxIsovalue = volume.GetPointData().GetArray(0).GetRange()[1];
        for (int i = 0; i < volume.GetPointData().GetNumberOfArrays(); i++)
        {
            scalarNames.Add(volume.GetPointData().GetArray(i).GetName());
        }
        InitVerticesBound();
        volume.Update();
    }

    void ReadVTRPartitions()
    {
        DirectoryInfo d = new DirectoryInfo(datasetPath);
        FileInfo[] Files = d.GetFiles("*.vtr");
        partitionpaths = new string[Files.Length];
        int i = 0;
        foreach (FileInfo file in Files)
        {
            vtkXMLRectilinearGridReader reader = vtkXMLRectilinearGridReader.New();
            partitionpaths[i] = file.FullName;
            reader.SetFileName(partitionpaths[i]);
            reader.Update();
            vtkRectilinearGrid v = vtkRectilinearGrid.New();
            v.DeepCopy(reader.GetOutput());
            volumeParts.Add(v);
            if (i == 0)
            {
                for (int j = 0; j < v.GetPointData().GetNumberOfArrays(); j++)
                {
                    scalarNames.Add(v.GetPointData().GetArray(j).GetName());
                }
            }
            double[] isovalueRange = reader.GetOutput().GetPointData().GetArray(0).GetRange();
            if (i == 0)
            {
                minIsovalue = isovalueRange[0];
                maxIsovalue = isovalueRange[1];
            }
            else
            {
                minIsovalue = Mathf.Min((float)minIsovalue, (float)isovalueRange[0]);
                maxIsovalue = Mathf.Max((float)maxIsovalue, (float)isovalueRange[1]);
            }
            i++;
        }
        InitVerticesBoundPartition();
    }

    /*
     * <summary> 
     *Read volume data
     * </summary>
     */
    public IEnumerator ReadData()
	{
		if (!partitionMode) {
            ReadVTRSingle();
		} else {
            ReadVTRPartitions();
        }
		isReady = true;
		yield return Ninja.JumpToUnity;
	}


    /*
     * <summary> 
     *Non-coroutine isosurface construction API call
     * </summary>
     */
    public void StartConstructIsosurfaceGameobject(GameObject parent){
		nvertices = 0;
		if(partitionMode)
			StartCoroutine(ConstructIsosurfaceGameObjectRegionBasedPartitions(parent));
		else
			StartCoroutine(ConstructIsosurfaceGameObjectRegionBased(parent));
	}

    void ConstructGameobjects(List<vtkPolyData> polys, List<vtkPolyData> polysLow, GameObject regionGO){
		for(int i = 0; i < polys.Count; i++){
			vtkPolyData poly = polys [i];

			GameObject LODGroupGO = new GameObject ();
			LODGroupGO.name = "Group_"+i;
			//print ("Region " + regionGO.name + " Group " + LODGroupGO.name);
			GameObject LOD0 = new GameObject ();
			LOD0.name = "LOD0_" + i;

			Mesh mesh = PolyDataToMesh (poly);
			nvertices += mesh.vertices.Length;
			LOD0 = InitLODGOComponents (LOD0, mesh);
			LOD0.transform.SetParent (LODGroupGO.transform);
			if ( polysLow != null && polysLow.Count > 0) {
				vtkPolyData polyLow = polysLow [i];

				GameObject LOD1 = new GameObject ();
				Mesh meshLow = PolyDataToMesh (polyLow);
				LOD1 = InitLODGOComponents (LOD1, meshLow);
				LOD1.name = "LOD1_" + i;
				LODGroupGO = InitLODGroupGOComponents (LODGroupGO, LOD0, LOD1);
			}


			LODGroupGO.transform.SetParent (regionGO.transform);
		}
	}

    /*
     * <summary> 
     *Adding created isosurface gameobject into the isosurfaces list
     * </summary>
     */
    public static void AddIsosurfaceToList(GameObject parent){
		SvrIsosurfaceExtractorControl isoControl = GameObject.FindObjectOfType<SvrIsosurfaceExtractorControl> ();
		if (isoControl != null) {
			isoControl.HideAll ();
			isoControl.isoGameobjects.Add (parent);
			isoControl.UpdateIsosurfaceListDropDown ();
		}
	}

    static void InitNameGameObject(GameObject parent, GameObject labelName, Vector3 position){
	    labelName.name = "LabelName";
	    labelName.AddComponent<TextMesh> ();
	    labelName.GetComponent<TextMesh> ().text = parent.name;
	    labelName.GetComponent<TextMesh> ().fontSize = 15;
	    labelName.GetComponent<TextMesh> ().alignment = TextAlignment.Center;
	    labelName.transform.localScale = new Vector3 (0.15f, 0.15f, 0.15f);
	    labelName.transform.SetParent (parent.transform);
	    labelName.transform.position = position; 
    }

    /*
     * <summary> 
     *Adding bounding box, creating centre object, move isosurface to scene centre 
     * add isosurface into the list, create isosurface label, assign isosurface to SvrIsosurfaceExtractorControl
     * </summary>
     */
    public static void ParentPostProcess(GameObject parent, Vector3 boundSize, Vector3 boundCenter){
		GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioIsosurfaceLoaded ();
	    parent.AddComponent<SvrBoundingBox>();
	    parent.GetComponent<SvrBoundingBox> ().size = boundSize;
	    parent.GetComponent<SvrBoundingBox> ().center = boundCenter;

	    parent.transform.position = Vector3.zero;

	    GameObject localCenterPoint = new GameObject();
	    localCenterPoint.name = "CenterPoint";
	    localCenterPoint.transform.position = boundCenter;
	    localCenterPoint.transform.SetParent(parent.transform);

	    parent.GetComponent<SvrIsosurface> ().size = boundSize;
	    parent.GetComponent<SvrIsosurface> ().center = localCenterPoint;
	    parent.GetComponent<SvrIsosurface> ().MoveTo (Vector3.zero);

	    AddIsosurfaceToList (parent);
	    GameObject labelName = new GameObject ();
	    InitNameGameObject(parent, labelName, new Vector3 (localCenterPoint.transform.position.x/2f,   localCenterPoint.transform.position.y - boundSize.y, localCenterPoint.transform.position.z));
		
	    SvrIsosurfaceExtractorControl.SetActiveIsosurface (parent);
	    parent.GetComponent<SvrIsosurface> ().NormaliseRegionPosition ();
	    parent.GetComponent<SvrIsosurface> ().isReady = true;
    }

    void PostProcess(GameObject parent){
        boundSize = CalculateSize(minVXValue, maxVXValue, minVYValue, maxVYValue, minVZValue, maxVZValue);
  	    boundCenter =  CalculateCenter(minVXValue, maxVXValue, minVYValue, maxVYValue, minVZValue, maxVZValue);
        ParentPostProcess(parent, boundSize, boundCenter);
    }

    /*
     * <summary>
     * Construct an isosurface gameobject based on regions. It means it will create a gameobject for each region (individual structure) of the isosurface).
     * The input is a single volume data. 
     * The parent gameobject is an empty gameobject in which the isosurface will be created.
     * </summary>
     */
    public IEnumerator ConstructIsosurfaceGameObjectRegionBased(GameObject parent)
	{
        isReady = false;
        stopwatch.Start ();
		SvrIsosurfaceExtractionPipeline pipeline = new SvrIsosurfaceExtractionPipeline ();
		List<SvrRegion> regions = new List<SvrRegion> (); 

		LogProccess ("Constructing isosurface");
		this.StartCoroutineAsync(pipeline.GenerateRegions(volume, (float) isovalue, activeScalar, activeColor),out task);
		yield return StartCoroutine (task.Wait ());
		regions = pipeline.regions;

		parent.tag = "Isosurface";
		parent.AddComponent <SvrIsosurface> ();
		parent.GetComponent<SvrIsosurface> ().isovalue = isovalue;

		LogProccess ("Constructing gameobjects");
		LogProccess ("Number of regions: " + regions.Count);

	for (int id = 0; id < regions.Count; id++) {
			GameObject regionGO = new GameObject ();
			regionGO.name = "Region_" + id;
			regionGO.tag = "Region";
			List<vtkPolyData> polys = new List<vtkPolyData> ();
			polys = regions[id].getPoly ();
			List<vtkPolyData> polysLow = new List<vtkPolyData> ();
			polysLow = regions[id].getPolyLow ();

			ConstructGameobjects (polys, polysLow, regionGO);

			regionGO.transform.SetParent (parent.transform);
			parent.GetComponent<SvrIsosurface> ().regions.Add (regionGO);
		}
		
		PostProcess (parent);

		stopwatch.Stop ();

		long time =  stopwatch.ElapsedMilliseconds + pipeline.elapsed;
		LogProccess("Number of vertices: " + nvertices );
		LogProccess ("Time lapsed: " + time);
		LogProccess("--------------------");
		LogProccess("Loading success");
		isReady = true;

       // GameObject.FindObjectOfType<SvrMeasureExtraction>().RecordTime(time, isovalue);
		yield return null;
	}


    /*
     * <summary>
     * Construct an isosurface gameobject based on regions. It means it will create a gameobject for each region (individual structure) of the isosurface).
     * The input is partitions of a volume data. 
     * The parent gameobject is an empty gameobject in which the isosurface will be created.
     * </summary>
     */
    public IEnumerator ConstructIsosurfaceGameObjectRegionBasedPartitions(GameObject parent){
        isReady = false;
        parent.tag = "Isosurface";
		parent.AddComponent <SvrIsosurface> ();
		parent.GetComponent<SvrIsosurface> ().isovalue = isovalue;

		LogProccess ("Reading partitions");

		SvrIsosurfaceExtractionPipeline pipeline = new SvrIsosurfaceExtractionPipeline ();

		LogProccess ("Constructing isosurface gameobject");
		for(int p = 0;  p < volumeParts.Count; p++){
			LogProccess ("----------------------------");
			LogProccess (" - Volume partition "+ (p + 1) + " / " + volumeParts.Count);
            vtkDataSet part = volumeParts [p];

			List<SvrRegion> regions = new List<SvrRegion> (); 

			this.StartCoroutineAsync(pipeline.GenerateRegions(part, (float) isovalue, activeScalar, activeColor),out task);
			yield return StartCoroutine (task.Wait ());
			regions = pipeline.regions;

			LogProccess ("Number of regions: " + regions.Count);
			for (int id = 0; id < regions.Count; id++) {
				GameObject regionGO = new GameObject ();
				regionGO.name = "Region_" + id;
				regionGO.tag = "Region";
				List<vtkPolyData> polys = new List<vtkPolyData> ();
				polys = regions[id].getPoly ();
				List<vtkPolyData> polysLow = new List<vtkPolyData> ();
				polysLow = regions[id].getPolyLow ();

				ConstructGameobjects (polys, polysLow, regionGO);

				regionGO.transform.SetParent (parent.transform);
				parent.GetComponent<SvrIsosurface> ().regions.Add (regionGO);
			}
			
			yield return null;
		}

		PostProcess (parent);

		LogProccess("Number of vertices: " + nvertices );
		LogProccess ("Time lapsed: " + stopwatch.ElapsedMilliseconds.ToString ());
		LogProccess("--------------------");
		LogProccess("Loading success");
		isReady = true;

		yield return null;
	}

    /*
     * <summary>
     * Calculate dimension of a box based on given min and max value of each x,y,z axis
     * </summary>
     */
    public static   Vector3 CalculateSize(float minx, float maxx, float miny, float maxy, float minz, float maxz){
	    float w = Mathf.Abs (minx - maxx);
	    float l = Mathf.Abs (minz - maxz);
	    float h = Mathf.Abs (miny - maxy);
	    return new Vector3 (w, h, l); 
    }

    /*
     * <summary>
     * Calculate the center of a box based on given min and max value of each x,y,z axis
     * </summary>
     */
    public static Vector3 CalculateCenter(float minx, float maxx, float miny, float maxy, float minz, float maxz){
	    float wCenter = (minx + maxx) / 2f;
	    float lCenter = (minz + maxz) / 2f;
	    float hCenter = (miny + maxy) / 2f;
	    return new Vector3 (wCenter, hCenter, lCenter);
    }
	
    /*
     * <summary>
     * Convert vtkPolyData to Unity Mesh. The vtkPolyData must not have
     * more than 65553 points
     * </summary>
     */
	public Mesh PolyDataToMesh(vtkPolyData poly){
		vtkPoints points = vtkPoints.New();
		vtkCellArray polysArray = vtkCellArray.New();
		
		points = poly.GetPoints();
		polysArray = poly.GetPolys();
		poly.GetPointData().SetActiveScalars (activeColor);
		List<Vector3> newVertices = new List<Vector3> ();
		List<Color> newColors = new List<Color> ();
		for (int i = 0; i < points.GetNumberOfPoints(); i++)
		{
			double[] pt = new double[3];
			pt = points.GetPoint(i);
			Vector3 vertex = new Vector3 ((float)pt [0] , (float)pt [1],  (float)pt [2]);

			if (poly.GetPointData ().GetArray (activeColor) != null) {
				float value = (float)poly.GetPointData ().GetArray (activeColor).GetTuple1 (i);
				Color color = GetColor (value, (float) minColorvalue, (float) maxColorvalue);
				newColors.Add ( color);
			}	
			newVertices.Add (vertex);			
		}

		int numberOfCells = (int)polysArray.GetNumberOfCells();
		int numberOfPolys = numberOfCells * 3;

		List<int> newTriangles = new List<int> ();
		int triIdx = 0;

		polysArray.InitTraversal();
		for (int i = 0; i < polysArray.GetNumberOfCells(); i++)
		{
			vtkIdList tris = vtkIdList.New();
			polysArray.GetNextCell(tris);
			for (int j = 0; j < tris.GetNumberOfIds(); j++)
			{
				newTriangles.Add ((int)tris.GetId (j));
				triIdx++;
			}
		}

        if(newVertices.Count >  65553)
        print("Vertices size " + newVertices.Count);

		Mesh mesh = new Mesh ();
		mesh.name = "mesh";
		mesh.vertices = newVertices.ToArray ();
	    mesh.colors = newColors.ToArray();
		mesh.triangles = newTriangles.ToArray ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();

		return mesh;
	}

    /*
     * <summary>
     * Flip object in z axis then normalise the position and scale
     * </summary>
     */
    public static GameObject FlipZ(GameObject go){
		Vector3 scale = go.transform.lossyScale;
		Vector3 post = go.transform.localPosition;
		go.transform.localScale = new Vector3 (scale.x, scale.y, -1f * scale.z);
		go.transform.localPosition = new Vector3 (post.x, post.y, -1f * post.z);
		return go;
	}
		
	GameObject InitLODGOComponents(GameObject go, Mesh mesh){
		go.AddComponent<MeshFilter> ();
		go.AddComponent<MeshRenderer> ();
		go.AddComponent<MeshCollider> ();
		go.GetComponent<MeshFilter> ().mesh = mesh;
		go.GetComponent<MeshRenderer> ().material = material;
		go.GetComponent<MeshRenderer> ().receiveShadows = false;
		go.GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		go.GetComponent<MeshFilter> ().mesh.RecalculateBounds ();
		go.GetComponent<MeshCollider> ().sharedMesh = mesh;
		go.tag = "Isosurface";
		go = FlipZ (go);
		return go;
	}

    /*
     * <summary>
     * Assign 2 LOD object into a LOD group
     * </summary>
     */
    public static GameObject InitLODGroupGOComponents(GameObject LODGroupGO, GameObject LOD1GO, GameObject LOD2GO){
		LODGroupGO.AddComponent<LODGroup> ();
		LODGroup groupLod = LODGroupGO.GetComponent<LODGroup> ();
		groupLod.size = 3;
		LOD[] lods = new LOD[3];

		Renderer[] renlod0 = new Renderer[1];
		renlod0 [0] = LOD1GO.GetComponent<MeshRenderer> ();

		Renderer[] renlod1 = new Renderer[1];
		renlod1 [0] = LOD2GO.GetComponent<MeshRenderer> ();

		lods [0] = new LOD(0.05f, renlod0);
		lods [1] = new LOD (0.01f, renlod1);
	    

		groupLod.SetLODs (lods);
		groupLod.RecalculateBounds ();

		LOD1GO.transform.SetParent (LODGroupGO.transform);
		LOD2GO.transform.SetParent (LODGroupGO.transform);

		return LODGroupGO;
	}
		

	Color GetColor(float value, float min, float max)
	{
		float norm = (value - min)/ (max - min);
		return colorMap.GetColor (norm);
	}
		
	void LogProccess(string t){
		if (consoleText != null) {
		    consoleText.SetText(t);
		}
		UnityEngine.Debug.Log (t);

	}
}
