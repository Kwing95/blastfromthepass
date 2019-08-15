using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionJuice_Squish : CollisionJuice_Rigidbody
{
	// get original scale
	public Vector3 squish_scale = new Vector3(1.3f, .5f, 0);
	private Vector3 orig_scale;
	public float duration = .2f;
	private float dur_timer = .0f;
	private float x_diff;
	private float y_diff;

	protected override void Start()
	{
		base.Start();
		orig_scale = transform.localScale;
		x_diff = squish_scale.x - orig_scale.x;
		y_diff = squish_scale.y - orig_scale.y;
	}

	// lerp back to position
	private void Update()
	{
		float scale_factor = dur_timer / duration;
		transform.localScale = new Vector3(orig_scale.x + (x_diff * scale_factor), orig_scale.y + (y_diff * scale_factor), 1);
		dur_timer -= Time.deltaTime;
		dur_timer = (dur_timer > 0) ? dur_timer : 0;
	}

	// squish on enter
	protected override void col_enter(Collision2D col)
	{
		transform.localScale = new Vector3(1.3f, .5f, 1);
		dur_timer = duration;
	}
}
