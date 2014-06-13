using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scroll : MonoBehaviour {

	public GameObject blocPrefab;
	private List<GameObject> blocsGround;
	private List<GameObject> blocsObstacle;

	public float scroolSpeed = 3.0f;
	private float timeDelay = 3f;
	private float blocSize = 1.28f;
	private float startX = 9.0f;
	private float endX = -9.0f;
	private float groundY = -3.8f;

	private float randomRange = 2.0f;

	// Use this for initialization
	void Start () {
		blocsGround = new List<GameObject>();	
		blocsObstacle = new List<GameObject>();

		// Ground
		for (int i = 0; i < 14; i++) {

			float randomY = Random.Range(0.0f, randomRange);
			GameObject bloc = Instantiate(blocPrefab, new Vector3(endX + i * blocSize, groundY + randomY, 0f), Quaternion.identity) as GameObject;
			bloc.name = "Bloc";
			bloc.transform.parent = transform;
			blocsGround.Add(bloc);
		}

		Manager.Instance.Blocs = blocsGround;
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject blocGround in blocsGround) {
			blocGround.transform.Translate(Vector3.right * Time.deltaTime * -scroolSpeed);
			if (blocGround.transform.position.x < endX) {
				float randomY = Random.Range(0.0f, randomRange);
				blocGround.transform.position = new Vector3(startX, groundY + randomY, 0f);
			}
		}
	}
}
