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

	// Get Blocs Bounds
	private List<Bounds> _blocsBounds;
	public List<Bounds> BlocsBounds {
		get {
			if (_blocsBounds == null) {
				_blocsBounds = new List<Bounds>();
				SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer sprite in sprites) {
					if (sprite.name == "Bloc") {
						_blocsBounds.Add(sprite.bounds);
					}
				}
			}
			return _blocsBounds;
		}
	}
}