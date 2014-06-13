using UnityEngine;
using System.Collections;

public class Animation : MonoBehaviour {

	public bool turn = true;
	public float radiusTurn = 3.0f;
	public float speedTurn = 1.0f;
	public bool orientation = true;
	private float lastX = 0.0f;

	public bool translate = false;
	public float radiusTranslate = 4.0f;
	public float speedTranslate = 0.7f;

	void Start () {
	
	}
	
	void Update ()
	{
		float x = 0;
		float y = 0;

		if (turn)
		{
			x += Mathf.Cos(Time.time * speedTurn) * radiusTurn;
			y += Mathf.Sin(Time.time * speedTurn) * radiusTurn;
			transform.position = new Vector3(x, y, 0f);
		}

		if (translate) {
			x += Mathf.Cos(Time.time * speedTranslate) * radiusTranslate;
		}

		transform.position = new Vector3(x, y, 0f);

		if (orientation) {
			if (lastX > 0.0f && x < 0.0f) {
				transform.localScale = new Vector3(-1f, 1f, 1f);
			} else if (lastX < 0.0f && x > 0.0f) {
				transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}

		lastX = x;
	}
}
