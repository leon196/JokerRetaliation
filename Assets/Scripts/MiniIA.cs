using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniIA : MonoBehaviour {

	private List<GameObject> blocs;
	public List<GameObject> Blocs { set { blocs = value; } }

	private float colliderHalf;
	private bool grounded = false;
	private float gravity = 0.123f;

	// Use this for initialization
	void Start () {
		blocs = Manager.Instance.Blocs;
		colliderHalf = collider.bounds.extents.y;
	}
	
	// Update is called once per frame
	void Update ()
	{
		grounded = false;

		foreach (GameObject bloc in blocs) {
			Bounds bounds = bloc.collider.bounds;
			if (collider.bounds.Intersects(bounds)) {
				transform.position = new Vector3(transform.position.x, colliderHalf + bounds.max.y, transform.position.z);
				grounded = true;
			}
		}

		if (!grounded) {
			transform.position -= new Vector3(0f, gravity, 0f);
		}
	}
}
