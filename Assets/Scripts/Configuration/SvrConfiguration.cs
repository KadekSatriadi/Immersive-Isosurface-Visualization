using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monash;

public class SvrConfiguration : MonoBehaviour {
	public string datasetpath;
	public string loadscenepath;
	public string scenefolderpath;
	public GameObject normalCamera;
	public GameObject stereoCamera;
    public SvrFileInputReaderManager readerManaganer;
	SvrColorBar bar;
	SvrIsosurfaceObjLoader loader;
	SvrIsosurfaceExtractorControl isoControl;
	Svr3DPointer pointer;
	string modestring;
	Dictionary<string,string> otherIniFiles = new Dictionary<string,string>();

	public enum Mode{
		normal, stereo, vr
	}
	public Mode mode;
	// Use this for initialization
	void Start () {
		isoControl = GameObject.FindObjectOfType<SvrIsosurfaceExtractorControl> ();
		loader = GameObject.FindObjectOfType<SvrIsosurfaceObjLoader> ();
		bar = GameObject.FindObjectOfType<SvrColorBar> ();
		pointer = GameObject.FindObjectOfType<Svr3DPointer> ();
		LoadConfiguration ();
		SetMode ();

		bar.CreateBar ();
		LoadOtherConfiguration ();

		if (System.IO.File.Exists(loadscenepath)) {
			loader.Init (loadscenepath);
			loader.isLoading = true;
			loader.LoadIsosurfaceOBJ (loadscenepath);

		}
		if (System.IO.File.Exists (datasetpath) || System.IO.Directory.Exists (datasetpath)) {
            readerManaganer.filepath = datasetpath;
            isoControl.reader = readerManaganer.GetReader();
            isoControl.Init (datasetpath);
			StartCoroutine(isoControl.ReadData ());
		} else {
			GameObject.FindObjectOfType<SvrGUITextOutputConsole> ().SetText ("No dataset is loaded");
		}

	}


	void SetMode(){
		if (mode == Mode.normal)	ModeNormal ();
		if (mode == Mode.stereo)	ModeStereo ();
		if (mode == Mode.vr)	ModeVR ();
	}

	void ModeNormal(){
		normalCamera.SetActive (true);
		stereoCamera.SetActive (false);
		pointer.mode = Svr3DPointer.Mode.nonvr;
	}

	void ModeStereo(){
		normalCamera.SetActive (false);
		stereoCamera.SetActive (true);
		pointer.mode = Svr3DPointer.Mode.nonvr;

	}

	void ModeVR(){
		normalCamera.SetActive (false);
		stereoCamera.SetActive (false);
		pointer.mode = Svr3DPointer.Mode.vr;
	}

	public void RemoveOther(string name){
		otherIniFiles.Remove (name);
		RedrawDropDownOther ();
	}

	public void RedrawDropDownOther(){
		Dropdown dd = GameObject.FindObjectOfType<SvGUIDropDownOtherIsosurface> ().gameObject.GetComponent<Dropdown> ();
		dd.options.Clear ();
		dd.AddOptions (new List<string> (otherIniFiles.Keys));
		dd.RefreshShownValue ();
	}

	public void LoadOther(){
		Dropdown dd = GameObject.FindObjectOfType<SvGUIDropDownOtherIsosurface> ().gameObject.GetComponent<Dropdown> ();
		string name = dd.options [dd.value].text;
		string path = "";
		otherIniFiles.TryGetValue (name, out path);
		if (System.IO.File.Exists (path)) {
			loader.LoadIsosurfaceOBJ (path);
		}
		RemoveOther (name);
	}

	void LoadOtherConfiguration(){
		if(loadscenepath == "") return;
		string folder = System.IO.Path.GetDirectoryName (loadscenepath);
		if (System.IO.Directory.Exists (folder)) {
			System.IO.DirectoryInfo parentFolderPath = System.IO.Directory.GetParent (folder);
			System.IO.DirectoryInfo[] directories = parentFolderPath.GetDirectories ();
			List<string> inifiles = new List<string> ();
			foreach (System.IO.DirectoryInfo dir in directories) {
				foreach (System.IO.FileInfo f in dir.GetFiles()) {
					if(f.Name.Contains("config.ini")) inifiles.Add(f.FullName);
				}
			}
			SvGUIDropDownOtherIsosurface other = GameObject.FindObjectOfType<SvGUIDropDownOtherIsosurface> ();
			if (other != null) {
				Dropdown dd = other.gameObject.GetComponent<Dropdown> ();
				dd.options.Clear ();
				foreach (string ini in inifiles) {
					string name = SvrIsosurfaceObjLoader.GetNameFromConfig (ini);
					dd.options.Add (new Dropdown.OptionData (name));
					otherIniFiles.Add (name, ini);
				}
				dd.RefreshShownValue ();
			}

		}
	}

	void LoadConfiguration(){
		string datapath = Application.dataPath;
		string apppath = System.IO.Directory.GetParent (datapath).FullName;
		string path = System.IO.Path.Combine (apppath, "config.ini");
		print (path);

		#if UNITY_EDITOR
			path = System.IO.Path.Combine (Application.dataPath, "config.ini");
		#endif

		if (!System.IO.File.Exists (path)) {
			throw new UnityException ("Configuration file not found under " + path);
		}

		INIParser parser = new INIParser ();
		parser.Open (path);
		string pathSection = "Path";
		string modeSection = "Mode";
		string interactionSection = "Interaction";
		loadscenepath = parser.ReadValue (pathSection, "loadscenepath", "");
		datasetpath = parser.ReadValue (pathSection, "datasetpath", "");
		scenefolderpath = parser.ReadValue (pathSection, "scenefolderpath", Application.dataPath);
		modestring = parser.ReadValue (modeSection, "mode", "");

		if (!modestring.Equals ("")) {
			switch (modestring) {
			case "normal":
				mode = Mode.normal;
				break;
			case "stereo":
				mode = Mode.stereo;
				break;
			case "vr":
				mode = Mode.vr;
				break;
			}
		}
			


		float animtranslatespeed = (float) parser.ReadValue (interactionSection, "animationtranslatespeed", 50.0);
		float animrotatespeed = (float) parser.ReadValue (interactionSection, "animationrotatespeed", 50.0);
		GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ().animationTranslateSpeed = animtranslatespeed;
		GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ().animationRotateSpeed = animrotatespeed;
	
	}

	public void TakeScreenShot(string data, string isosurfacename, string boundcode){
		string datapath = System.IO.Path.Combine (GameObject.FindObjectOfType<SvrConfiguration> ().scenefolderpath, isosurfacename);
		string boundcodepath = System.IO.Path.Combine (datapath, boundcode);

		if (!System.IO.Directory.Exists (datapath)) {
			System.IO.Directory.CreateDirectory (datapath);
		}
		System.IO.Directory.CreateDirectory (boundcodepath);

		string filePathImage = System.IO.Path.Combine (boundcodepath,   System.DateTime.Today.ToString("d").Replace("/","") + "_"+ System.DateTime.Now.Second.ToString () + ".png");
		string filePathData = System.IO.Path.Combine (boundcodepath,   System.DateTime.Today.ToString("d").Replace("/","") + "_"+ System.DateTime.Now.Second.ToString () + ".json");

		GameObject.FindObjectOfType<Svr3DPointer> ().pointer.gameObject.SetActive (false);
		ScreenCapture.CaptureScreenshot (filePathImage,5);
		Debug.Log ("Screen captured " + filePathImage);
		System.IO.File.WriteAllText (@filePathData, data);
		GameObject.FindObjectOfType<SvrAudioControl> ().PlayAudioCaptured ();

	}
}
