using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controls : MonoBehaviour
{
	private List<GameObject> blocs;

	private bool collisionDown = false;
	private bool collisionLeft = false;
	private bool collisionRight = false;
	private bool collisionUp = false;
	private Vector3 input;

	private Vector3 velocity;
	private float speed = 1.5f;
	private float jump = 0.275f;
	private float drag = 0.75f;
	private float gravity = 0.0038f;
	private const float VELOCITY_MAX = 0.1f;
	private const float VELOCITY_OFFSET = 0.1f;

	// Use this for initialization
	void Start ()
	{
		input = new Vector3();
		velocity = new Vector3();

		blocs = Manager.Instance.Blocs;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		// Inputs
		input.x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
		input.y = Input.GetAxis("Vertical") * Time.deltaTime * speed;

		// Collision
		collisionDown = false;
		collisionUp = false;
		collisionRight = false;
		collisionLeft = false;
		foreach (GameObject bloc in blocs) {
			Bounds bounds = bloc.renderer.bounds;
			bool intersects = renderer.bounds.Intersects(bounds);
			if (intersects) {
				if (renderer.bounds.min.y >= bounds.max.y - VELOCITY_OFFSET) {
					collisionDown = true;
				} else if (renderer.bounds.max.y <= bounds.min.y + VELOCITY_OFFSET) {
					collisionUp = true;
				} else if (renderer.bounds.min.x >= bounds.max.x - VELOCITY_OFFSET) {
					collisionLeft = true;
				} else if (renderer.bounds.max.x <= bounds.min.x + VELOCITY_OFFSET) {
					collisionRight = true;
				}
			}
		}

		// Velocity Horizontal
		velocity.x = Mathf.Max(collisionLeft ? 0.0f : -VELOCITY_MAX, Mathf.Min(velocity.x + input.x, collisionRight ? 0.0f : VELOCITY_MAX));

		// Velocity Drag
		if (Mathf.Abs(velocity.x) < 0.1) {
			velocity.x *= drag;
		}

		// Velocity Vertical

		// In Air
		if (!collisionDown)
		{
			velocity.y = Mathf.Max(-VELOCITY_MAX, Mathf.Min(velocity.y - gravity, VELOCITY_MAX));
		}
		// On Ground
		else
		{
			if (input.y > 0.0f) {
				velocity.y += jump;
				collisionDown = false;
			} else {
				velocity.y = 0.0f;
			}
		}

		// Apply Transformations
		transform.position += velocity;

		// Change Orientation of Batman
		transform.localScale = new Vector3(input.x >= 0.0f ? 1.0f : -1.0f, 1.0f, 1.0f);
	}
}
