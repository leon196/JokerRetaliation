using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {

	private static Manager _instance;

	private Manager() {}

	void Awake () {
		_instance = this;
	}

	// Singleton
	public static Manager Instance {
		get  {
			return _instance;
		}
	}

	private Controls[] _controls;
	public Controls[] Controls { 
		get {
			if (_controls == null) {
				_controls = GetComponentsInChildren<Controls>();
			}
			return _controls;
		}
	}

	private Scroll _scroll;
	public Scroll Scroll {
		get {
			if (_scroll == null) {
				_scroll = GetComponentInChildren<Scroll>();
			}
			return _scroll;
		}
	}

	// Get Blocs Bounds
	private List<GameObject> _blocs;
	public List<GameObject> Blocs {
		get {
			if (_blocs == null) {
				_blocs = new List<GameObject>();
				SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer sprite in sprites) {
					if (sprite.name == "Bloc") {
						_blocs.Add(sprite.gameObject);
					}
				}
			}
			return _blocs;
		}
		set {
			_blocs = value;
			if (Controls != null) {
				foreach (Controls controls in Controls) {
					controls.Blocs = _blocs;
				}
			}
			if (MiniIA != null) {
				MiniIA.Blocs = _blocs;
			}
		}
	}

	private MiniIA _miniIA;
	public MiniIA MiniIA { 
		get {
			if (_miniIA == null) {
				_miniIA = GetComponentInChildren<MiniIA>();
			}
			return _miniIA;
		}
	}

	public void Pause ()
	{
		if (Controls != null) {
			foreach (Controls controls in Controls) {
				controls.freeze = true;
			}
		}
		if (Scroll != null) {
			Scroll.freeze = true;
		}
	}

	public void Resume ()
	{
		if (Controls != null) {
			foreach (Controls controls in Controls) {
				controls.freeze = false;
			}
		}
		if (Scroll != null) {
			Scroll.freeze = false;
		}
	}

	public GameObject GetOpponent (bool isPlayer1) {
		GameObject opponent = null;
		if (Controls != null) {
			foreach (Controls controls in Controls) {
				if (isPlayer1 && !controls.player1) {
					opponent = controls.gameObject;
				} else if (!isPlayer1 && controls.player1) {
					opponent = controls.gameObject;
				}
			}
		}
		return opponent;
	}
}