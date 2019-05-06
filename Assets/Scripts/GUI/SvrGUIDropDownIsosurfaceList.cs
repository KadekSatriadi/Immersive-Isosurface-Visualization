using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SvrGUIDropDownIsosurfaceList : MonoBehaviour {

	public void ShowIsosurface(){
		GameObject.FindObjectOfType<SvrIsosurfaceExtractorControl> ().ShowIsosurface ();
		EventSystem.current.SetSelectedGameObject(null);

	}

	public void UpdateList(){
		string name = GameObject.FindObjectOfType<SvrIsosurfaceInteractionControl> ().activeIsosurface.name;
		Dropdown dd = GetComponent<Dropdown> ();
		int val = 0;
		foreach (Dropdown.OptionData d in dd.options) {
			if (d.text.Equals (name)) {
				dd.value = val;
				break;
			}
			val++;
		}
	}
}
