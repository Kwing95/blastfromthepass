using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class Movement2D_Dash_separate : Movement2D_Base
{
	// special ability vars
	public GameObject dash_box_prefab;
	public float lossDir = 10f;
	private TrailRenderer tr;
	protected float dash_spd = 18.0f;
	private float dash_time = .3f;
	protected float cooldown_time = 2.5f;
	protected float cooldown_timer = .0f;
	private bool dashing = false;
	private GameObject dash_box;
    private bool has_puffed = true;

    // get ref to resource bar
    public Transform bar;
	private Vector3 bar_scale;

	// set up dash_spd
	protected override void Start()
	{
		base.Start();
		tr = GetComponent<TrailRenderer>();
		tr.emitting = false;
		dash_box = Instantiate(dash_box_prefab, transform.position, Quaternion.identity);
		dash_box.transform.parent = transform;
		dash_box.SetActive(false);
		bar_scale = bar.localScale;
		// Debug.Log("Player ID: " + player_id + " || is_dummy: " + is_dummy);
	}

	// cooldown timer
	protected override void Update()
	{
		base.Update();
		cooldown_timer -= Time.deltaTime;
		cooldown_timer = (cooldown_timer > 0) ? cooldown_timer : 0;
        if (cooldown_timer <= 0 && !has_puffed)
        {
            stacker.PlaySound(reload, 0.25f);
            GetComponent<ParticleSystem>().Emit(30);
            has_puffed = true;
        }
        Vector2 temp = bar.localScale;
		temp.y = bar_scale.y * ((cooldown_time - cooldown_timer) / cooldown_time);
		// Debug.Log("Bar_Scale: " + bar_scale.x);
		bar.localScale = temp;
	}

	// implement special ability
	protected override void apply_special(float horiz, float vert)
	{
		if (has_ball || cooldown_timer > 0)
			return;
		// apply deadzone
		// horiz = (Mathf.Abs(horiz) > deadzone * 1) ? horiz : 0;
		// vert = (Mathf.Abs(vert) > deadzone * 1) ? vert : 0;
		// dash
		Vector2 mag_test = new Vector2(horiz, vert);
		if (mag_test.magnitude > .0f) // no long doing that here
		{
            stacker.PlaySound(dashSound, 0.5f, 0.8f);
            has_puffed = false;
            dashing = true;
			dash_box.SetActive(true);
			cooldown_timer = cooldown_time;
			tr.emitting = true;
			rb.gravityScale = 0;
			float dir = angle(horiz, vert);
			//Debug.Log("Dash Dir: " + (dir * Mathf.Rad2Deg));
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
		dash_box.SetActive(false);
	}

	// modify input
	protected override void apply_input(float horiz, float vert)
	{
		if (dashing)
			return;
		base.apply_input(horiz, vert);
	}

	protected override void trigger_hit(Collider2D other)
	{
		base.trigger_hit(other);
		if (other.CompareTag("Dash") && has_ball)
		{
			Rigidbody2D rb_temp = other.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
			float x_vel, y_vel;
			x_vel = rb_temp.velocity.x / 3.0f;
			float y_div = 1.5f;
			y_vel = (rb_temp.velocity.y / y_div < lossDir) ? lossDir : rb_temp.velocity.y / y_div;
			Vector2 vel = new Vector2(-x_vel, y_vel);
            bool sameTeam = other.gameObject.transform.parent.GetComponent<Movement2D_Dash_separate>().team_id == team_id;
			lose_ball(vel, sameTeam);
		}
	}

	public override void throw_ball()
	{
		base.throw_ball();
		cooldown_timer = cooldown_time;
	}

	protected override void pickup_ball(Collider2D other)
	{
		if (other.gameObject.GetComponent<Ball_Behavior>().can_be_grabbed)
		{
			base.pickup_ball(other);
			// Debug.Log("grabbed ball!");
		}
	}

    public bool GetDashing()
    {
        return dashing;
    }

}
