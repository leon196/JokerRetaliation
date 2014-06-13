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

	private Controls _controls;
	public Controls Controls { 
		get {
			if (_controls == null) {
				_controls = GetComponentInChildren<Controls>();
			}
			return _controls;
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
				Controls.Blocs = _blocs;
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
}