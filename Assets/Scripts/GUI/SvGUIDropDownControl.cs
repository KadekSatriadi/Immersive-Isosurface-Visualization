using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SvGUIDropDownControl : MonoBehaviour {
	public void SetValue(string s){
		Dropdown dd = GetComponent<Dropdown> ();
		int idx = 0;
		foreach (Dropdown.OptionData d in dd.options) {
			if (d.text.Equals (s)) {
				break;
			}
			idx++;
		}
		dd.value = idx;
		EventSystem.current.SetSelectedGameObject(null);

	}

}