using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scroll : MonoBehaviour {

	public GameObject blocPrefab;
	private List<GameObject> blocs;

	public float scroolSpeed = 3.0f;
	private float blocSize = 1.28f;

	private float randomRange = 1.0f;

	public bool freeze = false;

	// Use this for initialization
	void Start () {
        print(this.name);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
