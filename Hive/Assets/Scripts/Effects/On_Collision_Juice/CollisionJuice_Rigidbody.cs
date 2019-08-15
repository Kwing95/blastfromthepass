using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
public abstract class CollisionJuice_Rigidbody : CollisionJuice_Base
{
	// ref to rigidbody
	public Rigidbody2D rb2;
	public float speed_threshold = 2.0f;

	// get ref to rigidbody
	protected virtual void Start()
	{
		if (rb2 == null)
			rb2 = GetComponent<Rigidbody2D>();
	}

	// implement abstracts from base
	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		if (impact(collision))
			col_enter(collision);
	}
	protected override void OnCollisionExit2D(Collision2D collision)
	{
		if (impact(collision))
			col_exit(collision);
	}
	protected override void OnCollisionStay2D(Collision2D collision)
	{
		if (impact(collision))
			col_stay(collision);
	}

	// overrides for children
	protected virtual void col_enter(Collision2D col) { }
	protected virtual void col_exit(Collision2D col) { }
	protected virtual void col_stay(Collision2D col) { }

	// personal functions

	// determines if the collision was in the vertical or horizontal direction
	// and checks if the speed in that direction exceeds threshold
	private bool impact(Collision2D col)
	{
		// strange unity bug that's been around since Unity 5
		if (col.contactCount == 0)
			return false;
		// calculate vars
		Vector3 col_pos = col.GetContact(0).point;
		Vector3 this_pos = transform.position;
		float x_diff, y_diff;
		x_diff = Mathf.Abs(col_pos.x - this_pos.x);
		y_diff = Mathf.Abs(col_pos.y - this_pos.y);
		// horizontal collision
		if (x_diff > y_diff && y_diff < .2f) // there's some strangeness to how contact points work
		{
			if (Mathf.Abs(rb2.velocity.x) > speed_threshold)
			{
				//Debug.Log("My Pos: " + this_pos + " || Their Pos: " + col_pos);
				//Time.timeScale = 0;
				return true;
			}
		}
		else // vertical collision
		{
			if (Mathf.Abs(rb2.velocity.y) > speed_threshold)
			{
				//Debug.Log("Y Vel: " + Mathf.Abs(rb2.velocity.y));
				return true;
			}
		}
		return false;
	}
}
