using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controls : MonoBehaviour
{
	public Sprite spriteStand;
	public Sprite spriteDuck;
	public Sprite spriteAttack;

	public bool player1 = true;
	public bool snap = false;
	public bool attack = false;

	private List<GameObject> blocs;
	public List<GameObject> Blocs { set { blocs = value; } }
	private BoxCollider playerCollider;
	private SpriteRenderer playerSprite;
	private SpriteRenderer attackSprite;
	private BoxCollider attackCollider;
	private GameObject blocSnapped;

	private bool collisionDown = false;
	private bool collisionLeft = false;
	private bool collisionRight = false;
	private bool collisionUp = false;
	private bool ducking = false;
	private Vector3 input;

	private Vector3 velocity;
	private float speed = 4.5f;
	private float jump = 40.5f;
	private float dragAir = 0.95f;
	private float dragGround = 0.75f;
	private float gravity = 0.0038f;
	private const float VELOCITY_MAX = 0.098f;
	private const float JUMP_MAX = 0.158f;
	private const float GRAVITY_MAX = 0.268f;
	private const float VELOCITY_COLLISION_OFFSET = 0.5f; // like a bias : distance of the overlap between player bounds and bloc bounds
	private const float VELOCITY_COLLISION_ATTENUATION = 0.33f; // the ratio of returning force when collision

	private Rect rectStand = new Rect(0f, 0f, 1.28f, 2.04f);
	private Rect rectDuck = new Rect(0f, -0.3839f, 1.28f, 1.272f);

	private float attackDelay = 0.5f;
	private float attackLast = 0.0f;
	private float attackForce = 10.0f;

	private string inputHorizontalName = "Horizontal";
	private string inputVerticalName = "Vertical";
	private string inputAttackName = "Fire1";

	// Use this for initialization
	void Start ()
	{
		input = new Vector3();
		velocity = new Vector3();

		blocs = Manager.Instance.Blocs;

		playerCollider = GetComponent<BoxCollider>();
		playerSprite = GetComponent<SpriteRenderer>();
		attackSprite = transform.Find("Attack").GetComponent<SpriteRenderer>();
		attackCollider = attackSprite.GetComponent<BoxCollider>();

		if (!player1) {
			inputHorizontalName = "Horizontal2";
			inputVerticalName = "Vertical2";
			inputAttackName = "Fire2";
		}
		
		if (spriteStand == null || spriteDuck == null || spriteAttack == null) {
			Debug.Log("*Woops* public links broken");
		}
	}
	
	// Update is called once per frame
	void Update ()
	{

		CheckCollisions();

		UpdateMovement();

		if (attack) Attack();

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
			Bounds bounds = bloc.collider.bounds;
			bool intersectsCollider = playerCollider.bounds.Intersects(bounds);
			if (intersectsCollider) {
				float collisionDownDistance = playerCollider.bounds.min.y - bounds.max.y;
				float collisionLeftDistance = playerCollider.bounds.min.x - bounds.max.x;
				float collisionRightDistance = playerCollider.bounds.max.x - bounds.min.x;
				// Collision Down
				if (collisionDownDistance >= -VELOCITY_COLLISION_OFFSET)
				{
					collisionDown = true;
					transform.position += new Vector3(0f, -collisionDownDistance * VELOCITY_COLLISION_ATTENUATION, 0f);
					// Snap Bloc
					if (snap && (blocSnapped == null || blocSnapped.GetInstanceID() != bloc.GetInstanceID())) {
						blocSnapped = bloc;
						transform.parent = blocSnapped.transform;
					}
				}
				// Collision Left
				else if (collisionLeftDistance >= -VELOCITY_COLLISION_OFFSET)
				{
					collisionLeft = true;
					transform.position += new Vector3(-collisionLeftDistance, 0f, 0f);
				} 
				// Collision Right
				else if (collisionRightDistance <= VELOCITY_COLLISION_OFFSET)
				{
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

		// Unsnap if no collision down
		if (snap && !collisionDown && blocSnapped != null) 
		{
			transform.parent = Manager.Instance.transform;
			blocSnapped = null;
		}
	}

	void UpdateMovement ()
	{

		// Inputs
		input.x = Input.GetAxis(inputHorizontalName) * Time.deltaTime * speed;
		input.y = Input.GetAxis(inputVerticalName) * Time.deltaTime * speed;

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
			if (Mathf.Abs(Input.GetAxis(inputHorizontalName)) < 0.1f) {
				velocity.x *= dragGround;
			}
		}

		// Duck
		if (input.y < 0.0f && playerCollider.size.y > rectDuck.height)
		{
			playerSprite.sprite = spriteDuck;
			playerCollider.center = new Vector2(rectDuck.x, rectDuck.y);
			playerCollider.size = new Vector2(rectDuck.width, rectDuck.height);
			ducking = true;
		}
		// Stand
		else if (!collisionUp && input.y >= 0.0f && playerCollider.size.y < rectStand.height )
		{
			playerSprite.sprite = spriteStand;
			playerCollider.center = new Vector2(rectStand.x, rectStand.y);
			playerCollider.size = new Vector2(rectStand.width, rectStand.height);
			ducking = false;
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

	void Attack ()
	{

		// Attack
		if (Input.GetButtonDown(inputAttackName) && !ducking) {
			playerSprite.sprite = spriteAttack;
			attackSprite.enabled = true;
			attackLast = Time.time;
		}

		// Stop Attacking
		if (attackLast + attackDelay < Time.time) {
			playerSprite.sprite = ducking ? spriteDuck : spriteStand;
			attackSprite.enabled = false;
		}
		// Attacking
		else {
			for (int i = 0; i < blocs.Count; i++)
			{
				GameObject bloc = blocs[i];
				if (attackCollider.bounds.Intersects(bloc.collider.bounds)) {

					// Remove Animation 
					if (bloc.GetComponent<Animation>() != null) {
						Destroy(bloc.GetComponent<Animation>());
					}

					// Add Physics
					if (bloc.GetComponent<Rigidbody>() == null) {
						bloc.AddComponent<Rigidbody>();
						bloc.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
					}

					// Push
					//Vector3 direction = bloc.transform.position - attackCollider.transform.position;
					Vector3 direction = Vector3.up + Vector3.right * transform.localScale.x;
					direction.Normalize();
					bloc.GetComponent<Rigidbody>().AddForce(direction * attackForce, ForceMode.Impulse);
					bloc.GetComponent<Rigidbody>().AddTorque(Vector3.forward * Random.Range(-attackForce, attackForce), ForceMode.Impulse);

					// Kill
					Destroy(bloc, 5.0f);
					blocs.Remove(bloc);
				}
			}
		}
	}
}
