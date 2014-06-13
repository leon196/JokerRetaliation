using UnityEngine;
using System.Collections;
using System;

public class UI : MonoBehaviour {
	bool display_GO = true;
	float startTime;
	string timerText;

	void Awake() {	
		startTime = Time.time;
		
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI()
	{
		float guiTime = Time.time - startTime;
		
		float minutes = guiTime / 60f;
		float seconds = guiTime % 60f;
		float fraction = (guiTime * 100f) % 100f;
		
		timerText = String.Format ("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction); 
		GUI.Label (new Rect (400, 25, 100, 30), timerText);
	}
}
