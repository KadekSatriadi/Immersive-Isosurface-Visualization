using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SvrGUIButtonCloseDropDown : MonoBehaviour {
	public Dropdown d;

	public void Close(){
		d.Hide ();
		EventSystem.current.SetSelectedGameObject(null);

	}
}
