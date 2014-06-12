using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controls : MonoBehaviour
{
	private List<Bounds> blocsBounds;

	private bool ground = false;
	private Vector3 input;

	private Vector3 velocity;
	private float speed = 1.5f;
	private float jump = 0.275f;
	private float drag = 0.75f;
	private float gravity = 0.0038f;
	private const float VELOCITY_MAX = 0.1f;

	// Use this for initialization
	void Start ()
	{
		input = new Vector3();
		velocity = new Vector3();

		blocsBounds = Manager.Instance.BlocsBounds;
		Debug.Log(blocsBounds.Count);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		// Inputs
		input.x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
		input.y = Input.GetAxis("Vertical") * Time.deltaTime * speed;

		// Collision
		ground = false;
		foreach (Bounds bounds in blocsBounds) {
			if (renderer.bounds.Intersects(bounds)) {
				ground = true;
			}
		}

		// Velocity Horizontal
		velocity.x = Mathf.Max(-VELOCITY_MAX, Mathf.Min(velocity.x + input.x, VELOCITY_MAX));

		// Velocity Drag
		if (Mathf.Abs(velocity.x) < 0.1) {
			velocity.x *= drag;
		}

		// Velocity Vertical

		// In Air
		if (!ground)
		{
			velocity.y = Mathf.Max(-VELOCITY_MAX, Mathf.Min(velocity.y - gravity, VELOCITY_MAX));
		}
		// On Ground
		else
		{
			if (input.y > 0.0f) {
				velocity.y += jump;
				ground = false;
			} else {
				velocity.y = 0.0f;
			}
		}

		// Apply Transformations
		transform.position += velocity;
	}
}
