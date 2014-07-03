using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenerationScroll : MonoBehaviour {
	
    public int numberLv = 5;
    private GameObject[] _levels;
    private float _widthLevel = 21.76f;
    private int nbLevelInstantiate = 0;

    private void Instanciate()
    {
        for (int i = 0; i < numberLv; i++)
        {
            string lvToLoad = string.Format("Prefabs/Levels/Level_{0}", i);
            print(lvToLoad);
            GameObject go = (GameObject)Instantiate(Resources.Load(lvToLoad));
            go.transform.parent = this.transform;
            go.transform.localPosition = new Vector3(nbLevelInstantiate++ * _widthLevel, 0.0f, 0.0f);

            _levels[i] = go;
        }
    }

    // Use this for initialization
    void Awake()
    {
        _levels = new GameObject[numberLv];
		Instanciate();
	}

	
	
	// Update is called once per frame
	void FixedUpdate () {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.LoadLevel(0);
		}

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + 0.05f,
            Camera.main.transform.position.y, Camera.main.transform.position.z);

        
	}
}
