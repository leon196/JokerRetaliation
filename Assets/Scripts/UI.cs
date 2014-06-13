using UnityEngine;
using System.Collections;
using System;

public class UI : MonoBehaviour {
	bool display_GO = true;
	bool is_pause = false;
	float start_time;
	string timer_text;

	public GUIStyle timer_style;

	void Awake() {	
		start_time = Time.time;	
	}

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			print("pause game");
			Manager.Instance.Pause();
			is_pause = true;
		}

		if (Input.GetKeyDown(KeyCode.Escape) && is_pause)
		{
			print("resume game");
			Manager.Instance.Resume();
			is_pause = false;
		}
	}
	
	void OnGUI()
	{
		float guiTime = Time.time - start_time;
		
		float minutes = guiTime / 60f;
		float seconds = guiTime % 60f;
		float fraction = (guiTime * 100f) % 100f;
		
		if (!is_pause) 
		{
			timer_text = String.Format ("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction); 
			GUI.Label (new Rect (Screen.width / 2 - 50, 25, 100, 30), timer_text, timer_style);
		}

		if (is_pause) 
		{
			GUI.Label (new Rect (Screen.width/2, 200, 100, 30), "Pause", timer_style);
			GUI.Label (new Rect (Screen.width/2, 250, 100, 30), "Hit Esc to resume game", timer_style);
			if (GUI.Button(new Rect(Screen.width/2, 300, 100, 30), "Main Menu", timer_style))
			{
				Application.LoadLevel("MainMenu");
			}
		}
					
	}
}
