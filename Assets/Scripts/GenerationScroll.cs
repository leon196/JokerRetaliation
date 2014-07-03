using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenerationScroll : MonoBehaviour {
	
    public int numberLv = 5;
    public float speedScroll = 0.05f;
    private GameObject[] _levels;
    private float _widthLevel = 21.76f;
    private int nbLevelInstantiate = 0;
    private GameObject _currentLv = null;
    private GameObject _nextLv = null;
    public bool freeze = false;
    bool hasGenerate = true;
    List<GameObject> onScreen = new List<GameObject>();

    public GameObject GetNextLv()
    {
        if (!hasGenerate)
        {
            // hasGenerate = true;
            // _currentLv = _nextLv;
            int current = int.Parse(_currentLv.name.Substring(_currentLv.name.Length - 1, 1));
            int next = int.Parse(_nextLv.name.Substring(_nextLv.name.Length - 1, 1));
            int r = Random.Range(0, 5);
            while (r == current || r == next)
                r = Random.Range(0, 5);

            _levels[r].transform.localPosition = new Vector3(nbLevelInstantiate++ * _widthLevel, 0.0f, 0.0f);
            return _levels[r];
             
        }
        else
        {
            hasGenerate = false;
            return _nextLv;
        }
    }

    private void Instanciate()
    {
        string lvToLoad = string.Format("Prefabs/Levels/Level_{0}", 0);
        GameObject go = (GameObject)Instantiate(Resources.Load(lvToLoad));
        go.name = "Level_0";
        go.transform.parent = this.transform;
        go.transform.localPosition = new Vector3(nbLevelInstantiate++ * _widthLevel, 0.0f, 0.0f);

        onScreen.Add(go);
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

        int r = Random.Range(1, 5);
        _nextLv = _levels[r];
        _nextLv.transform.localPosition = new Vector3(nbLevelInstantiate++ * _widthLevel, 0.0f, 0.0f);
    }

    // Use this for initialization
    void Awake()
    {
        _levels = new GameObject[numberLv];
		Instanciate();
        
	}

    void Start()
    {
        
    }

    public void PrepareIA()
    {
        foreach (var lv in _levels)
        {
            List<GameObject> pathList = new List<GameObject>();
            foreach (Transform t in lv.transform)
            {
                if (t.name == "Bloc")
                    pathList.Add(t.gameObject);
            }

            Manager.Instance.Batman.GetComponent<IA>().PathFind(pathList, lv.name);
        }
    }

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.LoadLevel(0);
		}

        if (freeze || Manager.Instance.IsGameOver)
            return;

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + 0.075f,
            Camera.main.transform.position.y, Camera.main.transform.position.z);

        if (_currentLv.transform.position.x + _widthLevel + 2 < Manager.ScreenLeft)
        {
            hasGenerate = true;
            _currentLv = _nextLv;
            int current = int.Parse(_currentLv.name.Substring(_currentLv.name.Length-1, 1));
            print(onScreen.Count);
            onScreen.Remove(_currentLv);
            int r = Random.Range(0,5);
            while (r == current)
                r = Random.Range(0,5);

            _nextLv = _levels[r];
            _nextLv.transform.localPosition = new Vector3(nbLevelInstantiate++ * _widthLevel, 0.0f, 0.0f);
            
        }
	}
}
