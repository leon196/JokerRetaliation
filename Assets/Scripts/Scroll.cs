using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scroll : MonoBehaviour {

	public GameObject blocPrefab;
	private List<GameObject> blocs;

	public float scroolSpeed = 3.0f;
	private float blocSize = 1.28f;
	private float startX = 9.0f;
	private float endX = -9.0f;
	private float groundY = -3.8f;

	private float randomRange = 2.0f;

	public bool freeze = false;

	// Use this for initialization
	void Start () {
		blocs = new List<GameObject>();	

		// Ground
		for (int i = 0; i < 14; i++) {

			float randomY = Random.Range(0.0f, randomRange);
			GameObject bloc = Instantiate(blocPrefab, new Vector3(endX + i * blocSize, groundY + randomY, 0f), Quaternion.identity) as GameObject;
			bloc.name = "Bloc";
			bloc.transform.parent = transform;
			blocs.Add(bloc);
		}

		Manager.Instance.Blocs = blocs;
	}
	
	// Update is called once per frame
	void Update () {
		if (!freeze) {
			foreach (GameObject blocGround in blocs) {
				blocGround.transform.Translate(Vector3.right * Time.deltaTime * -scroolSpeed);
				if (blocGround.transform.position.x < endX) {
					float randomY = Random.Range(0.0f, randomRange);
					blocGround.transform.position = new Vector3(startX, groundY + randomY, 0f);
				}
			}
		}
	}
}
