using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvrBillboard : MonoBehaviour {
	RectTransform rect;

	void Start () {
		if (GetComponent<RectTransform> () != null) {
			rect = GetComponent<RectTransform> ();
		}
		
	}
	
	void LateUpdate () {
		if (rect != null) {
			
			rect.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
				Camera.main.transform.rotation * Vector3.up);
			rect.rotation = Quaternion.Euler(0f, rect.rotation.eulerAngles.y,0f);

			
		} else {
			transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
				Camera.main.transform.rotation * Vector3.up);
			transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y,0f);
		}
	}
}
