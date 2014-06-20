using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenerationScroll : MonoBehaviour {

	public GameObject blocPrefab;
	private List<GameObject> _blocs;
	private float posYTop = 0f;
	private float posYBottom = -1.28f;

	public float scroolSpeed = 3.0f;
	private float blocSize = 1.28f;

	public bool freeze = false;

	private void InstanciateBlocs()
	{		
		_blocs = new List<GameObject>();
		
		for(int i = 0; i < 14; i++)
		{
			GameObject blocTemp = (GameObject) Instantiate(blocPrefab, new Vector3(Manager.ScreenLeft + (i * blocSize), Manager.Ground + posYBottom, 0.0f), Quaternion.identity);
			blocTemp.SetActive(true);
			blocTemp.name = "Bloc";
			blocTemp.transform.parent = transform;
			_blocs.Add(blocTemp);
		}
		
		Manager.Instance.Blocs = _blocs;
	}

	// Use this for initialization
	void Start () {
		
		InstanciateBlocs();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.LoadLevel(0);
		}

		if (!freeze) {
			foreach (GameObject bloc in _blocs) {
				bloc.transform.Translate(Vector3.right * Time.deltaTime * -scroolSpeed);
				if (bloc.transform.position.x < Manager.ScreenLeft) {
					//float randomY = Random.Range(0.0f, randomRange);

					int randomNumber = Random.Range(0, 4);
					
					switch(randomNumber)
					{
					case 0:
					{
						bloc.transform.position = new Vector3(Manager.ScreenRight, Manager.Ground + posYTop, 0f);
						bloc.SetActive(true);
						break;
					}
						
					case 1:
					{
						bloc.transform.position = new Vector3(Manager.ScreenRight, Manager.Ground + posYTop, 0f);
						bloc.SetActive(false);
						break;
					}
						
					case 2:
					{
						bloc.transform.position = new Vector3(Manager.ScreenRight, Manager.Ground + posYBottom, 0f);
						bloc.SetActive(true);
						break;
					}
						
					case 3:
					{
						bloc.transform.position = new Vector3(Manager.ScreenRight, Manager.Ground + posYBottom, 0f);
						bloc.SetActive(false);
						break;
					}
					}
				}
			}
		}
	}
}
