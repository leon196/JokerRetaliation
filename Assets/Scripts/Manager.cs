﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {

	private static Manager _instance;

	public static Vector3 PlayerSpawn = new Vector3(0.0f, 0.0f, 0.0f);

	public static float ScreenTop = 7.0f;
	public static float ScreenBottom = -7.0f;
    public static float ScreenRight { get { return Camera.main.transform.position.x + Camera.main.orthographicSize; } }
    public static float ScreenLeft { get { return Camera.main.transform.position.x - Camera.main.orthographicSize; } }
	public static float Ground = -3.0f;

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

	// Batman
	private Batman _batman;
	public Batman Batman { 
		get {
			if (_batman == null) {
				_batman = GetComponentInChildren<Batman>();
			}
			return _batman;
		}
	}

	// Control Module
	private Controls[] _controls;
	public Controls[] Controls { 
		get {
			if (_controls == null) {
				_controls = GetComponentsInChildren<Controls>();
			}
			return _controls;
		}
	}

	// Player Game Object
	public GameObject GetPlayer(bool isPlayer1) {
		GameObject player = null;
		Controls[] controls = this.Controls;
		foreach (Controls control in controls) {
			if (isPlayer1 && control.player1) {
				player = control.gameObject;
				break;
			} else if (!isPlayer1 && !control.player1) {
				player = control.gameObject;
				break;
			}
		}
		return player;
	}

	private GenerationScroll _scroll;
    public GenerationScroll Scroll
    {
		get {
			if (_scroll == null) {
                _scroll = GetComponentInChildren<GenerationScroll>();
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

    private List<GameObject> _bombs;
    public List<GameObject> Bombs
    {
        get
        {
            if (_bombs == null)
            {
                _bombs = new List<GameObject>();
                SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sprite in sprites)
                {
                    if (sprite.name == "Bomb")
                    {
                        _bombs.Add(sprite.gameObject);
                    }
                }
            }
            return _bombs;
        }
    }

	private RocketLauncher _rocketLauncher;
	public RocketLauncher RocketLauncher {
		get {
			if (_rocketLauncher == null) {
				_rocketLauncher = GetComponentInChildren<RocketLauncher>();
			}
			return _rocketLauncher;
		}
	}
	public List<GameObject> RocketLauncherRockets {
		get {
			return RocketLauncher.Rockets;
		}
	}
	public void DestroyRocket (GameObject rocket, int index) {
		RocketLauncher.DestroyRocket(rocket, index);
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

	private SpriteRenderer _screenFail;
	public SpriteRenderer ScreenFail { get { if (_screenFail == null) { _screenFail = transform.Find("[ HUD ]/fail").GetComponent<SpriteRenderer>(); } return _screenFail; } }
	private SpriteRenderer _screenWin;
	public SpriteRenderer ScreenWin { get { if (_screenWin == null) { _screenWin = transform.Find("[ HUD ]/win").GetComponent<SpriteRenderer>(); } return _screenWin; } }
	public SpriteRenderer GetScreenGameOver (bool batmanGotCaught) {
		return batmanGotCaught ? ScreenWin : ScreenFail;
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

	private List<GameObject> _bulletsPlayer1;
	public List<GameObject> BulletsPlayer1 {
		get {
			if (_bulletsPlayer1 == null) {
				_bulletsPlayer1 = new List<GameObject>();
			}
			return _bulletsPlayer1;
		}
		set {
			_bulletsPlayer1 = value;
			GetOpponent(true).GetComponent<Controls>().OppenentBullets = _bulletsPlayer1;
		}
	}

	private List<GameObject> _bulletsPlayer2;
	public List<GameObject> BulletsPlayer2 {
		get {
			if (_bulletsPlayer2 == null) {
				_bulletsPlayer2 = new List<GameObject>();
			}
			return _bulletsPlayer2;
		}
		set {
			_bulletsPlayer2 = value;
			GetOpponent(false).GetComponent<Controls>().OppenentBullets = _bulletsPlayer2;
		}
	}

    private bool _gameOver = false;
    public bool IsGameOver
    {
        get
        {
            return _gameOver;
        }
        set
        {
            _gameOver = value;
        }
    }
}