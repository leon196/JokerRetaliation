using UnityEngine;
using System.Collections;
using System;

public class UI : MonoBehaviour {
	bool display_GO = true;
	bool is_pause = false;
	float start_time;
	string timer_text;

	//DEBUG
	public bool you_win = false;
	public bool you_lose = false;
	//DEBUG

	public GUIStyle timer_style;
	public GUIStyle pause_style;
	public GUIStyle main_menu_style;

	public Texture texture_win;
	public Texture texture_lose;

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
			Manager.Instance.Pause();
			is_pause = true;
		}

		if (Input.GetKeyDown(KeyCode.Escape) && is_pause)
		{
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
			timer_text = String.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction); 
			GUI.Label (new Rect (0, 25, 100, 30), timer_text, timer_style);
		}

		if (is_pause) 
		{
			GUI.Label (new Rect (Screen.width/2-50, 200, 100, 30), "Pause", pause_style);
			if (GUI.Button(new Rect(Screen.width/2-60, 300, 100, 30), "Main Menu", main_menu_style))
			{
				Application.LoadLevel("MainMenu");
			}
		}

		if (you_win || you_lose) 
		{
			is_pause = true; // stop timer
			Manager.Instance.Pause(); // stop game

			if (you_win)
				GUI.DrawTexture(new Rect(0, 0, 1024, 768), texture_win, ScaleMode.ScaleToFit, true, 0.0f);
			if (you_lose)
				GUI.DrawTexture(new Rect(0, 0, 1024, 768), texture_lose, ScaleMode.ScaleToFit, true, 0.0f);

			StartCoroutine("backToMainMenu");
		}
	}

	IEnumerator backToMainMenu()
	{
		yield return new WaitForSeconds(2);
		Application.LoadLevel ("MainMenu");
	}
}
