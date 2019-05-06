using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SvrLoadSceneAsync : MonoBehaviour {
	public string scenename;
	public bool loadonstart = true;
	void Start(){
		if (loadonstart)
			StartLoading ();
	}

	public void StartLoading(){
		StartCoroutine (Load());
	}

	IEnumerator Load() {
		yield return new WaitForSeconds (2f);
		AsyncOperation async = SceneManager.LoadSceneAsync(scenename);
		async.allowSceneActivation = false;
		while (!async.isDone && async.progress < 0.9f) {
			yield return null;
		}
		async.allowSceneActivation = true;
		yield return async;
	}
}
