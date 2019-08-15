using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard_Dash : Keyboard_Base
{
	// special ability vars
	private TrailRenderer tr;
	private float dash_spd;
	private float dash_time = .25f;
	private float cooldown_time = 5.0f;
	private float cooldown_timer = .0f;
	private bool dashing = false;

	// get ref to resource bar
	public Transform bar;

	// set up dash_spd
	protected override void Start()
	{
		base.Start();
		dash_spd = move_spd * 5;
		tr = GetComponent<TrailRenderer>();
		tr.emitting = false;
	}

	// cooldown timer
	protected override void Update()
	{
		base.Update();
		cooldown_timer -= Time.deltaTime;
		cooldown_timer = (cooldown_timer > 0) ? cooldown_timer : 0;
		Vector2 temp = bar.localScale;
		temp.x = ((cooldown_time - cooldown_timer) / cooldown_time);
		bar.localScale = temp;
	}

	// implement special ability
	protected override void apply_special(float horiz, float vert)
	{
		if (has_ball || cooldown_timer > 0)
			return;
		// dash
		if (horiz != 0 || vert != 0)
		{
			dashing = true;
			cooldown_timer = cooldown_time;
			tr.emitting = true;
			rb.gravityScale = 0;
			float dir = angle(horiz, vert);
			rb.velocity = new Vector2(Mathf.Cos(dir) * dash_spd, Mathf.Sin(dir) * dash_spd);
			StartCoroutine(reset_from_dash(dash_time));
		}
	}

	// reset after dashing
	IEnumerator reset_from_dash(float time)
	{
		yield return new WaitForSeconds(time);
		rb.gravityScale = 1;
		tr.emitting = false;
		rb.velocity = Vector2.zero;
		dashing = false;
	}

	// modify input
	protected override void apply_input(float horiz, float vert)
	{
		if (dashing)
			return;
		base.apply_input(horiz, vert);
	}
}
