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
	private const float VELOCITY_COLLISION_OFFSET = 0.5f;

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

		CheckCollisions();

		UpdateMovement();

		if (transform.position.y < -7.0f) {
			RespawnPlayer();
		}
	}

	void RespawnPlayer ()
	{
		transform.position = new Vector3();
		velocity = new Vector3();
		collisionDown = false;
		collisionUp = false;
		collisionRight = false;
		collisionLeft = false;
	}

	void CheckCollisions ()
	{
		collisionDown = false;
		collisionUp = false;
		collisionRight = false;
		collisionLeft = false;
		foreach (GameObject bloc in blocs) {
			Bounds bounds = bloc.renderer.bounds;
			bool intersectsCollider = playerCollider.bounds.Intersects(bounds);
			if (intersectsCollider) {
				float collisionDownDistance = (playerCollider.bounds.min.y - bounds.max.y);
				float collisionLeftDistance = (playerCollider.bounds.min.x - bounds.max.x);
				float collisionRightDistance = (playerCollider.bounds.max.x - bounds.min.x);
				if (collisionDownDistance >= -VELOCITY_COLLISION_OFFSET) {
					collisionDown = true;
					transform.position += new Vector3(0f, -collisionDownDistance, 0f);
				} else if (collisionLeftDistance >= -VELOCITY_COLLISION_OFFSET) {
					collisionLeft = true;
					transform.position += new Vector3(-collisionLeftDistance, 0f, 0f);
				} else if (collisionRightDistance <= VELOCITY_COLLISION_OFFSET) {
					collisionRight = true;
					transform.position += new Vector3(-collisionRightDistance, 0f, 0f);
				}
			}
			bool intersectsSprite = playerSprite.bounds.Intersects(bounds);
			if (intersectsSprite) {
				if (playerCollider.bounds.max.y <= bounds.min.y + VELOCITY_COLLISION_OFFSET) {
					collisionUp = true;
				} 
			}
		}
	}

	void UpdateMovement ()
	{

		// Inputs
		input.x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
		input.y = Input.GetAxis("Vertical") * Time.deltaTime * speed;

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

		// Apply Transformations
		transform.position += velocity;

		// Change Orientation of Batman
		if (velocity.x < 0 && transform.localScale.x > 0) {
			transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
		} else if (velocity.x > 0 && transform.localScale.x < 0) {
			transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		} 
	}
}
