using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CielaSpike;
using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class SvrIsosurfaceExtractorControl : MonoBehaviour {
	SvrIsosurfaceExtractor isoExtractor;
    public SvrReader reader;
	public Slider isoValueSlider;
	public Text isoValueSliderRange;
	public SvrGUITextOutputConsole consoleText;
	public Dropdown contourNamesDropDown;
	public Dropdown colorNamesDropDown;
	public Dropdown isosurfaceListDropDown;
	public List<GameObject> isoGameobjects;

	string datasetpath;
	bool isPartitions;
	Task task;

	void Awake(){
		isoExtractor = GameObject.FindObjectOfType<SvrIsosurfaceExtractor> ();
		isoValueSlider = GameObject.FindObjectOfType<SvrGUISliderIsovalue> ().gameObject.GetComponent<Slider> ();
		isoValueSliderRange = GameObject.FindObjectOfType<SvrGUITextIsovalueRangeLabel> ().gameObject.GetComponent<Text> ();
		consoleText = GameObject.FindObjectOfType<SvrGUITextOutputConsole> ();
		contourNamesDropDown = GameObject.FindObjectOfType<SvrGUIDropDownContour> ().GetComponent<Dropdown> ();
		colorNamesDropDown = GameObject.FindObjectOfType<SvrGUIDropDownColor> ().GetComponent<Dropdown> ();
		isosurfaceListDropDown = GameObject.FindObjectOfType<SvrGUIDropDownIsosurfaceList> ().GetComponent<Dropdown> ();
		isoGameobjects = new List<GameObject> ();
	}


    /*
    * <summary> 
    *Initialise the object by giving a path to either vtr file or partitions folder
    * </summary>
    */
    public void Init (string path) {
        reader.filepath = path;
		datasetpath = path;

        print(reader.GetType().Name);
		if (System.IO.File.Exists (datasetpath) || reader.GetType().Name == "SvrDICOMSeriesReader")
			isPartitions = false;
		else if (System.IO.Directory.Exists (datasetpath) && reader.GetType().Name != "SvrDICOMSeriesReader")
			isPartitions = true;
		else
			throw new UnityException ("Active dataset not found");	
		isoExtractor.SetDatasetPath (datasetpath);
	}

    /*
    * <summary> 
    * Non-coroutine API call for read data
    * </summary>
    */
    public void Read(){
        StartCoroutine (ReadData ());
    }

    /*
    * <summary> 
    * Hide all isosurfaces gameobject in the isosurfaces list
    * </summary>
    */
    public void HideAll(){
		foreach (GameObject g in isoGameobjects) {
			g.SetActive (false);
		}
	}

    public void SetNextActive()
    {
        int idx = 0;
        foreach(GameObject iso in isoGameobjects)
        {
            if (iso.activeSelf)
            {
                if(idx == isoGameobjects.Count - 1)
                {
                    SetActiveIsosurface(isoGameobjects[0]);
                }
                else
                {
                    SetActiveIsosurface(isoGameobjects[idx + 1]);
                }
                return;
            }
            idx++;
        }
    }

    public void SetPrevActive()
    {
        int idx = 0;
        foreach (GameObject iso in isoGameobjects)
        {
            if (iso.activeSelf)
            {
                if (idx == 0)
                {
                    SetActiveIsosurface(isoGameobjects[isoGameobjects.Count -1]);
                }
                else
                {
                    SetActiveIsosurface(isoGameobjects[idx - 1]);
                }
                return;
            }
            idx++;
        }
    }

    /*
    * <summary> 
    * Set active isosurface gameobject
    * </summary>
    */
    public static void SetActiveIsosurface(GameObject parent){
		SvrIsosurfaceInteractionControl control = GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ();
		if (control != null) {
			if (control.activeIsosurface != null) {
				parent.transform.position = control.activeIsosurface.transform.position;
				parent.transform.rotation = control.activeIsosurface.transform.rotation;
				control.activeIsosurface = parent;
			} else {
				control.activeIsosurface = parent;
			}

		}
		SvrObject3DOrientation miniorientation = GameObject.FindObjectOfType<SvrObject3DOrientation> ();
		if(miniorientation != null)
			miniorientation.go = parent;

		GameObject.FindObjectOfType<SvrGUIDropDownIsosurfaceList> ().UpdateList ();
	}

   /*
   * <summary> 
   * Hide all isosurfaces except one
   * </summary>
   */
    public void HideAllExcept(string name){
		
		foreach (GameObject g in isoGameobjects) {
			if (g.name == name) {
				g.SetActive (true);
				SetActiveIsosurface (g);
			}
			else
				g.SetActive (false);
		}
	}


	public IEnumerator ReadData(){
		Stopwatch sw = new Stopwatch ();
		sw.Start ();
		Console ("Reading dataset: " + System.IO.Path.GetFileName(datasetpath));

        if (isPartitions)
        {
            DirectoryInfo d = new DirectoryInfo(reader.filepath);
            FileInfo[] Files = d.GetFiles("*.vtr");
            string[] partitionpaths = new string[Files.Length];
            int i = 0;
            foreach (FileInfo file in Files)
            {
                print("Reading partition " + file.FullName);
                reader.filepath = file.FullName;
                reader.Read();
                isoExtractor.AddPartition(reader.GetOutput());
                i++;
            }
            isoExtractor.SetPartitionMode(true);
            isoExtractor.InitVerticesBoundPartition();
        }
        else
        {
            reader.Read();
            isoExtractor.SetInput(reader.GetOutput());
            isoExtractor.InitVerticesBound();

        }

        //this.StartCoroutineAsync(isoExtractor.ReadData (), out task);
        // yield return StartCoroutine (task.Wait ());
        sw.Stop ();
        isoExtractor.isReady = true;

        Console("Reading done ("+ sw.ElapsedMilliseconds + ")");

			
        if (isoValueSlider != null)
        {
            isoValueSlider.minValue = (float)reader.range[0];
            isoValueSlider.maxValue = (float)reader.range[1];
            isoValueSlider.value = Mathf.Abs((float)(reader.range[0] - reader.range[1])) / 2f;
        }


        if (isoValueSliderRange != null)
        {
            isoValueSliderRange.text = "Min: " + reader.range[0] + " \nMax: " + reader.range[1];
        }

        if (contourNamesDropDown != null)
        {
            contourNamesDropDown.options.Clear();
            colorNamesDropDown.options.Clear();

            foreach (string s in reader.scalarNames)
            {
                contourNamesDropDown.options.Add(new Dropdown.OptionData(s));
                colorNamesDropDown.options.Add(new Dropdown.OptionData(s));
            }
            contourNamesDropDown.RefreshShownValue();
            colorNamesDropDown.RefreshShownValue();
        }

        UpdateGUIAsConfig ();
        yield return null;
	}

  /*
   * <summary> 
   * Update the GUI as specified in the configuration file (for pre-extracted isosurface object)
   * </summary>
   */
    public void UpdateGUIAsConfig(){
		SvrIsosurfaceObjLoader loader = GameObject.FindObjectOfType<SvrIsosurfaceObjLoader> ();
		if (loader == null || loader.isLoading == false)
			return;

		SvrGUISliderIsovalue isoslider = GameObject.FindObjectOfType<SvrGUISliderIsovalue> ();
		if (isoslider != null) {
			isoValueSlider.value = loader.isovalue;
			isoValueSlider.minValue = loader.scalarmin;
			isoValueSlider.maxValue = loader.scalarmax;
			float s = loader.isovalue;
			isoslider.SetValue(s) ;
		}
		SvrGUIDropDownContour ddcontour = GameObject.FindObjectOfType<SvrGUIDropDownContour> ();
		if (ddcontour != null) {
			ddcontour.SetValue (loader.contour);
		}
		SvrGUIDropDownColor ddcolor = GameObject.FindObjectOfType<SvrGUIDropDownColor> ();
		if (ddcolor != null) {
			ddcolor.SetValue (loader.color);
		}
	}


    /*
   * <summary> 
   * Update the isosurface slider range
   * </summary>
   */
    public void UpdatIsoValueSliderRange(){
		if (contourNamesDropDown == null || colorNamesDropDown == null) {
			contourNamesDropDown = GameObject.FindObjectOfType<SvrGUIDropDownContour> ().GetComponent<Dropdown> ();
			colorNamesDropDown = GameObject.FindObjectOfType<SvrGUIDropDownColor> ().GetComponent<Dropdown> ();
		}
		string scalarname = contourNamesDropDown.options [contourNamesDropDown.value].text;
		string colorname = colorNamesDropDown.options [colorNamesDropDown.value].text;
		isoExtractor.SetActiveScalar(scalarname);
		isoExtractor.SetActiveColor (colorname);

		if (isoValueSlider != null) {
			isoValueSlider.minValue = (float)isoExtractor.minIsovalue;
			isoValueSlider.maxValue = (float)isoExtractor.maxIsovalue;
			isoValueSlider.value = Mathf.Abs( (float)(isoExtractor.minIsovalue - isoExtractor.maxIsovalue))/ 2f;
		}

		if (isoValueSliderRange != null) {
			isoValueSliderRange.text = "Min: "+isoExtractor.minIsovalue + " \n Max: "+ isoExtractor.maxIsovalue;
		}

		EventSystem.current.SetSelectedGameObject(null);

	}

   /*
   * <summary> 
   * Hide all isosurface expect the one selected in the drop down menu
   * </summary>
   */
    public void ShowIsosurface(){
		string name = isosurfaceListDropDown.options [isosurfaceListDropDown.value].text;
		HideAllExcept (name);
	}


   /*
   * <summary> 
   * Update the isosurface drop down menu
   * </summary>
   */
    public void UpdateIsosurfaceListDropDown(){
		if (isosurfaceListDropDown == null)
			return;
		
		isosurfaceListDropDown.options.Clear ();

		foreach (GameObject go in isoGameobjects) {
			isosurfaceListDropDown.options.Add (new Dropdown.OptionData (go.name));
		}
		isosurfaceListDropDown.RefreshShownValue ();
	}


  /*
   * <summary> 
   * Construct isosurface
   * </summary>
   */
    public void ConstructIsosurface(){
		if (!isoExtractor.isReady)
			return;
		float isovalue = isoValueSlider.value;
		string scalarname = contourNamesDropDown.options [contourNamesDropDown.value].text;
		string colorname = colorNamesDropDown.options [colorNamesDropDown.value].text;
		isoExtractor.SetIsovalue (isovalue,scalarname);
		isoExtractor.SetActiveColor (colorname);
		GameObject obj = new GameObject ();
		if (!isPartitions) {
			obj.name = System.IO.Path.GetFileName (datasetpath) + "_"+scalarname+"_"+"_iso_" + isovalue + "_color_" + colorname;
		} else {
			string name = new DirectoryInfo (@datasetpath).Name;
			obj.name = name + "_" + scalarname + "_" + "_iso_" + isovalue +"_color_" + colorname;
		}

		isoExtractor.StartConstructIsosurfaceGameobject (obj);
		SvrColorBar bar = GameObject.FindObjectOfType<SvrColorBar> ();
		if (bar != null) {
			bar.SetTitle (colorname);
		//	double[] range = isoExtractor.GetRange (colorname);
	//		bar.SetLabels((float)range[0], (float)range[1]);
		}

		EventSystem.current.SetSelectedGameObject(null);

	}

    void Console(string t)
    {
        UnityEngine.Debug.Log(t);
        if (consoleText != null)
            consoleText.SetText(t);
    }

    void OnApplicationQuit(){
		if (isoExtractor.task != null) {
			UnityEngine.Debug.Log ("Canceling task");
			isoExtractor.task.Cancel ();
			UnityEngine.Debug.Log ("Task canceled");
		}
		if (task != null) {
			task.Cancel ();
		}
	}
}
