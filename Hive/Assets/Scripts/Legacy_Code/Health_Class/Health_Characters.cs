using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpriteRenderer))]
// also requires movements script
// [RequireComponent(typeof(Movement_Script))]
public class Health_Characters : Health_Base
{
	// characters can recieve knockback
	// public float knockback_force = 500f;
	// public float knockback_time = .25f;
	protected Rigidbody rb;

	// characters flash when hit and are briefly invincible
	public Color alt_color;
	private Color start_color;
	private SpriteRenderer sr;
	private float invincible_timer = 0;
	private float flash_time = .1f;

	// modify start to get rb and sr component
	protected override void Start()
	{
		base.Start();
		rb = GetComponent<Rigidbody>();
		sr = GetComponent<SpriteRenderer>();
		start_color = sr.color;
	}

	// have an update
	protected void Update()
	{
		// manage timer
		invincible_timer -= Time.deltaTime;
	}

	// modify take_damage
	public override void take_hit(int damage, Vector3 pos, float hitstun, float knockback_force)
	{
		// check invincible_timer
		if (invincible_timer <= 0)
		{
			// apply knockback and flash effect
			invincible_timer = hitstun;
			knockback(pos, knockback_force);
			sr.color = alt_color;
			StartCoroutine(reset_color(flash_time));
			StartCoroutine(stop_knockback(hitstun));
			Debug.Log("Hitstun: " + hitstun);
			// disable movement
			GetComponent<SimpleMovement>().freeze = true; // debug
			// GetComponent<Movement>().freeze_timer(hitstun);
			base.take_hit(damage, pos, hitstun, knockback_force);
		}
	}

	// private knockback func
	private void knockback(Vector3 other_pos, float knockback_force)
	{
		float x_diff = transform.position.x - other_pos.x;
		float y_diff = transform.position.y - other_pos.y;
		float angle = Mathf.Atan2(y_diff, x_diff);
		float x_force = Mathf.Cos(angle) * knockback_force;
		float y_force = Mathf.Sin(angle) * knockback_force;
		rb.AddForce(new Vector3(x_force, y_force, 0));
	}

	// stop knockback
	private IEnumerator stop_knockback(float time)
	{
		yield return new WaitForSeconds(time);
		rb.velocity = Vector3.zero;
		GetComponent<SimpleMovement>().freeze = false; // debug
	}

	// reset back to normal color
	private IEnumerator reset_color(float time)
	{
		yield return new WaitForSeconds(time);
		sr.color = start_color;
	}
}
