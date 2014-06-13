using UnityEngine;
using System.Collections;

public class SpriteAnimation : MonoBehaviour {

	public float delay = 0.5f;
	public bool once = true;
	private float last = 0.0f;
	private string animationName;
	private int current = 0;
	private SpriteRenderer spriteRenderer;
	public Sprite[] sprites;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animationName = spriteRenderer.sprite.name.Split('_')[0];
	}
	
	// Update is called once per frame
	void Update () {
		if (last + delay < Time.time) {

			if (once && current + 1 >= sprites.Length) {
				Destroy(gameObject);
			}

			last = Time.time;
			current = (current + 1) % sprites.Length;
			spriteRenderer.sprite = sprites[current];
			//spriteRenderer.sprite = animationName + current;
		}
	}
}
