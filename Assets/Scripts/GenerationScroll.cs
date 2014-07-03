using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenerationScroll : MonoBehaviour {
	
    public int numberLv = 5;
    private GameObject[] _levels;
    private float _widthLevel = 21.76f;
    private int nbLevelInstantiate = 0;
    private GameObject _currentLv = null;
    private GameObject _nextLv = null;

    private void Instanciate()
    {
        string lvToLoad = string.Format("Prefabs/Levels/Level_{0}", 0);
        GameObject go = (GameObject)Instantiate(Resources.Load(lvToLoad));
        go.name = "Level_0";
        go.transform.parent = this.transform;
        go.transform.localPosition = new Vector3(nbLevelInstantiate++ * _widthLevel, 0.0f, 0.0f);

        _currentLv = go;
        _levels[0] = go;

        for (int i = 1; i < numberLv; i++)
        {
            lvToLoad = string.Format("Prefabs/Levels/Level_{0}", i);
            go = (GameObject)Instantiate(Resources.Load(lvToLoad));
            go.name = string.Format("Level_{0}", i);
            go.transform.parent = this.transform;
            go.transform.localPosition = new Vector3(-50.0f, 0.0f, 0.0f);

            _levels[i] = go;
        }

        _nextLv = _levels[Random.Range(1, 5)];
        _nextLv.transform.localPosition = new Vector3(nbLevelInstantiate++ * _widthLevel, 0.0f, 0.0f);
    }

    // Use this for initialization
    void Awake()
    {
        _levels = new GameObject[numberLv];
		Instanciate();
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.LoadLevel(0);
		}

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + 0.075f,
            Camera.main.transform.position.y, Camera.main.transform.position.z);

        if (_currentLv.transform.position.x + _widthLevel < Camera.main.transform.position.x)
        {
            _currentLv = _nextLv;
            print(_currentLv.name);
            int current = int.Parse(_currentLv.name.Substring(_currentLv.name.Length-1, 1));
            print("current");
            int r = Random.Range(0,5);
            while(r == current)
                r = Random.Range(0,5);
            _nextLv = _levels[r];
            _nextLv.transform.localPosition = new Vector3(nbLevelInstantiate++ * _widthLevel, 0.0f, 0.0f);
        }
	}
}
