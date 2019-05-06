using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SvrGUIDropDownContour : SvGUIDropDownControl {

	public void UpdateValue(){
		GameObject.FindObjectOfType<SvrIsosurfaceExtractorControl> ().UpdatIsoValueSliderRange ();
		EventSystem.current.SetSelectedGameObject(null);

	}


}
