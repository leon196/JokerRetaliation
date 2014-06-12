using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Generation : MonoBehaviour {

	public GameObject bloc;
	public float delay = 0.2f;
	private float lastUpdate = 0.0f;
	private List<GameObject> _blocs;
	private float posXmin = -6.4f;
	private float posXmax = 6.4f;
	private float posYTop = -1.28f;
	private float posYBottom = -2.56f;
	private bool display = true;


	private struct BlocGen
	{
		public float posY;
		public bool display;
	}

	private List<BlocGen> _blocGen = new List<BlocGen>();

	// Use this for initialization
	void Start() {

		BlocGen bg = new BlocGen();
		bg.posY = posYBottom;
		bg.display = display;

		for(int i = 0; i < 11; i++)
		{
			_blocGen.Add(bg);
		}

		InstanciateBlocs();

		//_blocs = Manager.Instance.Blocs;
	}

	private void InstanciateBlocs()
	{
		float posX = posXmin;

		_blocs = new List<GameObject>();

		foreach(BlocGen bg in _blocGen)
		{
			GameObject blocTemp = (GameObject) Instantiate(bloc, new Vector3(posX, bg.posY, 0.0f), Quaternion.identity);
			blocTemp.SetActive(bg.display);
			blocTemp.name = "Bloc";

			blocTemp.gameObject.transform.parent = this.transform;

			posX += 1.28f;

			_blocs.Add(blocTemp);
		}

		Manager.Instance.Blocs = _blocs;
	}

	private void DisplayBlocs()
	{
		int index = 0;
		foreach(BlocGen bg in _blocGen)
		{
			GameObject blocTemp = _blocs.ElementAt(index);
			blocTemp.transform.position = new Vector3(blocTemp.transform.position.x, bg.posY, blocTemp.transform.position.z);
			blocTemp.SetActive(bg.display);

			index += 1;
		} 
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (lastUpdate + delay < Time.time)
		{
			lastUpdate = Time.time;

			for(int i = 0; i < 10; i++)
			{
				_blocGen[i] = _blocGen[i+1];
			}

			int randomNumber = Random.Range(0, 4);
			
			switch(randomNumber)
			{
				case 0:
				{
					BlocGen bg = new BlocGen();
					bg.posY = posYTop;
					bg.display = true;
					_blocGen[10] = bg;
					break;
				}

				case 1:
				{
					BlocGen bg = new BlocGen();
					bg.posY = posYTop;
					bg.display = false;
					_blocGen[10] = bg;
					break;
				}

				case 2:
				{
					BlocGen bg = new BlocGen();
					bg.posY = posYBottom;
					bg.display = true;
					_blocGen[10] = bg;
					break;
				}

				case 3:
				{
					BlocGen bg = new BlocGen();
					bg.posY = posYBottom;
					bg.display = false;
					_blocGen[10] = bg;
					break;
				}
			}

			DisplayBlocs();

			_blocs = Manager.Instance.Blocs;
		}
	}
}
