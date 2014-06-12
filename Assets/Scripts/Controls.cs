using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {

	private Vector3 input;
	private Vector3 velocity;
	private float speed = 1.5f;
	private float drag = 0.75f;
	private float gravity = 0.38f;
	private const float VELOCITY_MAX = 0.1f;

	// Use this for initialization
	void Start ()
	{
		input = new Vector3();
		velocity = new Vector3();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		input.x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
		input.y = Input.GetAxis("Vertical") * Time.deltaTime * speed;

		velocity.x = Mathf.Max(-VELOCITY_MAX, Mathf.Min(velocity.x + input.x, VELOCITY_MAX));

		if (Mathf.Abs(velocity.x) < 0.1) {
			velocity.x *= drag;
		}

		//velocity.y += input.y;

		transform.position += velocity;
	}

	void OnCollisionEnter (Collision other)
	{

	}
}
