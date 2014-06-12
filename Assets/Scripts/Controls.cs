using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controls : MonoBehaviour
{
	public Sprite batmanStand;
	public Sprite batmanDuck;

	private List<GameObject> blocs;
	private BoxCollider playerCollider;
	private SpriteRenderer playerSprite;

	private bool collisionDown = false;
	private bool collisionLeft = false;
	private bool collisionRight = false;
	private bool collisionUp = false;
	private Vector3 input;

	private Vector3 velocity;
	private float speed = 4.5f;
	private float jump = 40.5f;
	private float dragAir = 0.95f;
	private float dragGround = 0.75f;
	private float gravity = 0.0038f;
	private const float VELOCITY_MAX = 0.088f;
	private const float JUMP_MAX = 0.158f;
	private const float GRAVITY_MAX = 0.268f;
	private const float VELOCITY_OFFSET = 0.5f;

	private Rect rectStand = new Rect(0f, 0f, 1.28f, 2.04f);
	private Rect rectDuck = new Rect(0f, -0.3839f, 1.28f, 1.272f);

	// Use this for initialization
	void Start ()
	{
		input = new Vector3();
		velocity = new Vector3();

		blocs = Manager.Instance.Blocs;

		playerCollider = GetComponent<BoxCollider>();
		playerSprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
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
			bool intersectsCollider = playerCollider.bounds.Intersects(bounds);
			if (intersectsCollider) {
				if (playerCollider.bounds.min.y >= bounds.max.y - VELOCITY_OFFSET) {
					collisionDown = true;
				} else if (playerCollider.bounds.min.x >= bounds.max.x - VELOCITY_OFFSET) {
					collisionLeft = true;
				} else if (playerCollider.bounds.max.x <= bounds.min.x + VELOCITY_OFFSET) {
					collisionRight = true;

					// Move Player Away From Bloc
					transform.position = new Vector3(transform.position.x - (playerCollider.bounds.max.x - bounds.min.x), transform.position.y, transform.position.z);
				}
			}
			bool intersectsSprite = playerSprite.bounds.Intersects(bounds);
			if (intersectsSprite) {
				if (playerCollider.bounds.max.y <= bounds.min.y + VELOCITY_OFFSET) {
					collisionUp = true;
				} 
			}
		}

		// Velocity Horizontal
		velocity.x = Mathf.Max(collisionLeft ? 0.0f : -VELOCITY_MAX, Mathf.Min(velocity.x + input.x, collisionRight ? 0.0f : VELOCITY_MAX));

		// In Air
		if (!collisionDown)
		{
			velocity.y = Mathf.Max(-GRAVITY_MAX, Mathf.Min(velocity.y - gravity, JUMP_MAX));

			// Velocity Horizontal Air Drag
			velocity.x *= dragAir;
		}
		// On Ground
		else
		{
			if (!collisionUp && input.y > 0.0f) {
				velocity.y =  Mathf.Max(-GRAVITY_MAX, Mathf.Min(jump, JUMP_MAX));
				collisionDown = false;
			} else {
				velocity.y = 0.0f;
			}

			// Velocity Horizontal Ground Drag
			velocity.x *= dragGround;
		}

		// Apply Transformations
		transform.position += velocity;

		// Change Orientation of Batman
		transform.localScale = new Vector3(input.x >= 0.0f ? 1.0f : -1.0f, 1.0f, 1.0f);

		// Duck
		if (input.y < 0.0f && playerCollider.size.y > rectDuck.height)
		{
			playerSprite.sprite = batmanDuck;
			playerCollider.center = new Vector2(rectDuck.x, rectDuck.y);
			playerCollider.size = new Vector2(rectDuck.width, rectDuck.height);
		}
		// Stand
		else if (!collisionUp && input.y >= 0.0f && playerCollider.size.y < rectStand.height )
		{
			playerSprite.sprite = batmanStand;
			playerCollider.center = new Vector2(rectStand.x, rectStand.y);
			playerCollider.size = new Vector2(rectStand.width, rectStand.height);
		}
	}
}
