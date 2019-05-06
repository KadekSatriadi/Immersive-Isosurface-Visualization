using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SvrGUIButtonShowHideObject : MonoBehaviour {
	public GameObject[] objs;
	// Use this for initialization
	public void ToggleShowHide(){
		foreach(GameObject o in objs){
			o.SetActive (!o.activeSelf);
		}
        EventSystem.current.SetSelectedGameObject(null);

    }
}
