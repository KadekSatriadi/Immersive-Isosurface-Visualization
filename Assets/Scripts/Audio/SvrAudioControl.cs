using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SvrAudioControl : MonoBehaviour {

	public AudioClip addmarker;
	public AudioClip addbox;
	public AudioClip isosurfaceloaded;
	public AudioClip datacapture;
	public AudioClip showguiplace;
	public AudioClip isolate;
	public AudioClip exitIsolation;
	public AudioClip deleteboundingbox;

	public void PlayAudio(AudioClip clip){
		if (clip == null) return;
		GetComponent<AudioSource> ().Stop ();
		GetComponent<AudioSource> ().clip = clip;
		GetComponent<AudioSource> ().Play ();
	}

	public void PlayAudioIsolate(){
		PlayAudio (isolate);
	}

	public void PlayAudioExitIsolation(){
		PlayAudio (exitIsolation);
	}

	public void PlayAudioDeletBoundingBox(){
		PlayAudio (deleteboundingbox);
	}

	public void PlayAudioAddMarker(){
		PlayAudio (addmarker);
	}

	public void PlayAudioAddBoundingBox(){
		PlayAudio (addbox);
	}

	public void PlayAudioIsosurfaceLoaded(){
		PlayAudio (isosurfaceloaded);
	}

	public void PlayAudioCaptured(){
		PlayAudio (datacapture);
	}

	public void PlayAudioShowGUIPlace(){
		PlayAudio (showguiplace);
	}



}
