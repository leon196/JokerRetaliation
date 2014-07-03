using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controls : MonoBehaviour
{
	public Sprite spriteStand;
	public Sprite spriteDuck;
	public Sprite spriteAttack;
	public Sprite spriteDead;
	public GameObject bulletPrefab;
	public GameObject explosionPrefab;

	public bool player1 = true;
	public bool horizontal = false;
	public bool snap = false;
	public bool attack = false;

	private List<GameObject> blocs;
	public List<GameObject> Blocs { set { blocs = value; } }
	private BoxCollider playerCollider;
	private SpriteRenderer playerSprite;
	private SpriteRenderer attackSprite;
	private BoxCollider attackCollider;
	private GameObject blocSnapped;

	private GameObject opponent;
	private List<GameObject> opponentBullets;
	public List<GameObject> OppenentBullets { set { opponentBullets = value; } }

	// Victory condition
	private float timeStarted = 0.0f;
	private float timeLeft = 30.0f;
	private Vector3 playerStartPosition;
	private Vector3 playerEndPosition;

	// BATMAN !
	private Batman batman;

	// Collisions
	private bool collisionDown = false;
	private bool collisionLeft = false;
	private bool collisionRight = false;
	private bool collisionUp = false;
	private bool ducking = false;
	private Rect rectStand = new Rect(0f, 0f, 1.28f, 2.04f);
	private Rect rectDuck = new Rect(0f, -0.3839f, 1.28f, 1.272f);

	// Velocity
	private Vector3 velocity;
	private float speed = 4.5f;
	private float speedAutoScroll = 0.40f;
	private float jump = 40.5f;
	private float dragAir = 0.95f;
	private float dragGround = 0.75f;
	private float gravity = 0.0038f;
	private const float VELOCITY_MAX = 0.098f;
	private const float JUMP_MAX = 0.158f;
	private const float GRAVITY_MAX = 0.268f;
	private const float VELOCITY_COLLISION_OFFSET = 0.5f; // like a bias : distance of the overlap between player bounds and bloc bounds
	private const float VELOCITY_COLLISION_ATTENUATION = 0.33f; // the ratio of returning force when collision

	// Attack
	private bool attacking = false;
	private float attackDelay = 0.5f;
	private float attackLast = 0.0f;
	private float attackForce = 10.0f;
	private float bulletForce = 10.0f;

	// Hitted
	private float deadDelay = 0.5f;
	private float deadLast = 0.0f;
	private bool dead = false;

	// Inputs
	private Vector3 input;
	private string inputHorizontalName = "Horizontal";
	private string inputVerticalName = "Vertical";
	private KeyCode inputAttack = KeyCode.F;
	private KeyCode inputAttackBullet = KeyCode.R;

	public bool freeze = false;

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

		opponent = Manager.Instance.GetOpponent(player1);

		if (!player1) {
			inputHorizontalName = "Horizontal2";
			inputVerticalName = "Vertical2";
			inputAttack = KeyCode.K;
			inputAttackBullet = KeyCode.L;
		}
		
		if (!spriteStand || !spriteDuck || !spriteAttack || !spriteDead) {
			Debug.Log("*Woops* public links broken");
		}

		playerStartPosition = transform.position;
		playerEndPosition = transform.position;
		playerEndPosition.x = Manager.ScreenRight;

		batman = Manager.Instance.Batman;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		// Escape Menu
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.LoadLevel(0);
		}
		
		// Main Loop
		if (!freeze)
		{
			// Victory
			CheckCollisionBatman();

			// Game Over
			if (transform.position.x < Manager.ScreenLeft) {
				GameOver(false);
			}

			// Quick Debug Game over
			//if (transform.position.y < Manager.ScreenBottom) {
				//RespawnPlayer();
			//}

			// Collision
			CheckCollisionBlocs();

			// Movement
			UpdateMovement();

			// Attack
			if (attack && !dead) Attack();

			// Restore player
			if (dead && deadLast + deadDelay < Time.time) {
				dead = false;
				playerSprite.sprite = spriteStand;
			}

			// Get hitted by bullets
			if (opponentBullets != null) {
				for (int i = 0; i < opponentBullets.Count; i++) {
					GameObject bullet = opponentBullets[i];
					if (bullet == null) {
						opponentBullets.RemoveAt(i);
						continue;
					}
					if (bullet.collider.bounds.Intersects(collider.bounds)) {
						Vector3 direction = Vector3.up + Vector3.right * transform.localScale.x;
						direction.Normalize();
						Push(direction * attackForce);
						opponentBullets.RemoveAt(i);
						break;
					}
				}
			}
		}
	}

	void RespawnPlayer ()
	{
		transform.localPosition = Manager.PlayerSpawn;
		velocity = new Vector3();
		collisionDown = false;
		collisionUp = false;
		collisionRight = false;
		collisionLeft = false;
		freeze = false;
	}

	void GameOver (bool batmanGotCaught)
	{
	
		freeze = true;

		if (batmanGotCaught)
		{
			StartAnimationAttack();
			batman.Push();	
			Explosion(new Vector3(transform.position.x, collider.bounds.min.y, 0f));
		}

		StartCoroutine(Cinematic(batmanGotCaught));
	}

	IEnumerator Cinematic (bool batmanGotCaught)
	{
		// Init
		SpriteRenderer screen = Manager.Instance.GetScreenGameOver(batmanGotCaught);

		float x = 0.0f;
		Color screenColor = screen.color;
		screenColor.a = 0.0f;
		screenColor = screenColor;
		screen.enabled = true;

		// Processing
		float delay = batmanGotCaught ? 10.0f : 1.0f;

		if (batmanGotCaught) {
			while (x < delay)
			{
				// Boum !
				Explosion(new Vector3(Random.Range(Manager.ScreenLeft, Manager.ScreenRight), Random.Range(Manager.ScreenBottom, Manager.ScreenTop), 0f));
				
				// Fade in
				screenColor.a = x / delay;
				screen.color = screenColor;

				// Motor
				x += Time.deltaTime;
				yield return null;
			}
		} else {
			while (x < delay)
			{
				// Fade in
				screenColor.a = x / delay;
				screen.color = screenColor;

				// Motor
				x += Time.deltaTime;
				yield return null;
			}
		}

		// Wait for it ...
		yield return new WaitForSeconds(1.5f);

		// Go for Menu
		Application.LoadLevel(0);
	}

	void CheckCollisionBatman ()
	{
		Bounds bounds = batman.collider.bounds;
		if (playerCollider.bounds.Intersects(bounds)) {
			GameOver(true);
		}
	}

	void CheckCollisionBlocs ()
	{
		collisionDown = false;
		collisionUp = false;
		collisionRight = false;
		collisionLeft = false;
		foreach (GameObject bloc in Manager.Instance.Blocs) {
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

		if (!dead) {
			input.x = Input.GetAxis(inputHorizontalName) * Time.deltaTime * speed;
			input.y = Input.GetAxis(inputVerticalName) * Time.deltaTime * speed;
		} else {
			input.x = 0f;
			input.y = 0f;
		}

		// Velocity Horizontal
		if (horizontal) {
			// Add velocity with clamp and collision
			velocity.x = Mathf.Max(collisionLeft ? 0.0f : -VELOCITY_MAX, Mathf.Min(velocity.x + input.x, collisionRight ? 0.0f : VELOCITY_MAX));
			// Change Orientation
			if (velocity.x < 0 && transform.localScale.x > 0) {
				transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
			} else if (velocity.x > 0 && transform.localScale.x < 0) {
				transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			} 
		}
		// Auto Scroll Horizontal
		else {
			/* Animation incompatible with collisions ?
			float ratioPosition = (Time.time - timeStarted) / timeLeft;
			Vector3 position = transform.position;
			position.x = Mathf.Lerp(playerStartPosition.x, playerEndPosition.x, ratioPosition);
			transform.position = position;
			*/
			// Add velocity with clamp and collision
			velocity.x = Mathf.Min(Time.deltaTime * speedAutoScroll, collisionRight ? 0.0f : Mathf.Infinity);
		}

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
			// Jump
			if (!dead && !collisionUp && input.y > 0.0f) {
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
	}

	void StopAttack() {
		attacking = false;
		attackLast = Time.time-attackDelay + 0.1f;
	}

	void StartAnimationAttack ()
	{
		playerSprite.sprite = spriteAttack;
		attackSprite.enabled = true;
		attackLast = Time.time;
		attacking = true;
	}

	void Attack ()
	{

		// Bullets
		if (Input.GetKeyDown(inputAttackBullet)) {
			FireBullet();
		}

		// Attack
		if (Input.GetKeyDown(inputAttack) && !ducking) {
			StartAnimationAttack();
		}

		// Stop Attacking
		if (attackLast + attackDelay < Time.time) {
			playerSprite.sprite = ducking ? spriteDuck : spriteStand;
			attackSprite.enabled = false;
			StopAttack();
			return;
		}
		// Attacking
		if (attacking) {

			//
			Vector3 direction = Vector3.up + Vector3.right * transform.localScale.x;
			direction.Normalize();

			// Opponent
			if (opponent) {
				if (opponent.collider.bounds.Intersects(collider.bounds)) {
					opponent.GetComponent<Controls>().Push(direction * 10.0f);
					StopAttack();
					return;
				}
			} 

			// Helicopter
			/*
			RocketLauncher helicopter = Manager.Instance.RocketLauncher;
			if (helicopter.collider.bounds.Intersects(playerCollider.bounds)) {
				Explosion((helicopter.transform.position + transform.position) / 2.0f);
				StopAttack();
				return;
			}
			*/

			// Rockets
			List<GameObject> rockets = Manager.Instance.RocketLauncherRockets;
			for (int i = 0; i < rockets.Count; i++) {
				GameObject rocket = rockets[i];
				if (attackCollider.bounds.Intersects(rocket.collider.bounds)) {
					Manager.Instance.DestroyRocket(rocket, i);
					Explosion(rocket.transform.position);
					StopAttack();
					return;
				}
			}

			// Blocs
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
					bloc.GetComponent<Rigidbody>().AddForce(direction * attackForce, ForceMode.Impulse);
					bloc.GetComponent<Rigidbody>().AddTorque(Vector3.forward * Random.Range(-attackForce, attackForce), ForceMode.Impulse);

					// Kill
					Destroy(bloc, 5.0f);
					blocs.Remove(bloc);
					StopAttack();
					return;
				}
			}
		}
	}

	public void Push(Vector3 direction) {

		collisionDown = false;
		collisionUp = false;
		collisionRight = false;
		collisionLeft = false;
		transform.position += new Vector3(0f, VELOCITY_COLLISION_OFFSET, 0f);
		velocity = direction;

		dead = true;
		deadLast = Time.time;
		playerSprite.sprite = spriteDead;

		Explosion(new Vector3(transform.position.x, collider.bounds.min.y, 0f));
	}

	public void Explosion (Vector3 position) {
		GameObject explosion = Instantiate(explosionPrefab) as GameObject;
		explosion.transform.position = position;
	}

	private void FireBullet () {
		GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
		bullet.transform.localScale = transform.localScale;

		Vector3 direction = Vector3.right * transform.localScale.x;
		direction.Normalize();
		bullet.rigidbody.AddForce(direction * bulletForce, ForceMode.Impulse);

		if (player1) {
			List<GameObject> playerBullets = Manager.Instance.BulletsPlayer1;
			playerBullets.Add(bullet);
			Manager.Instance.BulletsPlayer1 = playerBullets;
		} else {
			List<GameObject> playerBullets = Manager.Instance.BulletsPlayer2;
			playerBullets.Add(bullet);
			Manager.Instance.BulletsPlayer2 = playerBullets;
		}

		Destroy(bullet, 5.0f);
	}
}
