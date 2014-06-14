using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RocketLauncher : MonoBehaviour {

	public GameObject rocketPrefab;
	private GameObject target;

	public float fireRate = 3.0f;
	private float fireLast = 0.0f;
	private float speedRocket = 0.1f;

	private float lastAngle = 0.0f;

	private List<GameObject> rockets;
	public List<GameObject> Rockets { get { return rockets; } }

	// Use this for initialization
	void Start () {
		target = Manager.Instance.GetPlayer(true);	
		rockets = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Fire Rocket
		if (fireLast + fireRate < Time.time) {
			GameObject rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity) as GameObject;
			rockets.Add(rocket);
			fireLast = Time.time;
		}

		// Update Rockets
		for (int i = 0; i < rockets.Count; i++) {
			GameObject rocket = rockets[i];
			Vector3 dir = new Vector3(target.transform.position.x - rocket.transform.position.x, target.transform.position.y - rocket.transform.position.y, 0f);

			// Find target
			if (dir.magnitude < 1.0f) {
				// Push player
				target.GetComponent<Controls>().Push(dir);

				DestroyRocket(rocket, i);
				continue;
			}
			dir.Normalize();

			float angle = Mathf.Atan2(dir.y, dir.x) * 180.0f / 3.14f;
			angle = Mathf.Lerp(lastAngle, angle, 0.5f);
			//float angle = Mathf.Max(-45.0f, Mathf.Min(Mathf.Atan2(dir.y, dir.x) * 180.0f / 3.14f), 45.0f);
			rocket.transform.rotation = Quaternion.Euler(0f, 0f, angle);
			lastAngle = angle;

			rocket.transform.position += rocket.transform.right * speedRocket;
		}
	}

	public void DestroyRocket(GameObject rocket, int index) {
		rockets.RemoveAt(index);
		ParticleSystem rocketParticle = rocket.GetComponentInChildren<ParticleSystem>();
		rocketParticle.transform.parent = null;
		rocketParticle.Stop();
		Destroy(rocketParticle, 3.0f);
		Destroy(rocket);
	}
}
