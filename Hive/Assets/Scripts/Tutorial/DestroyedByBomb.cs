using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedByBomb : MonoBehaviour
{
	// only activate destroy script once
	private bool marked = false;

	// check for collision with bomb 
	private void OnTriggerStay2D(Collider2D collision)
	{
		trigger_hit(collision);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		trigger_hit(collision);
	}
	private void trigger_hit(Collider2D other)
	{
		if (other.tag == "Ball")
		{
			destroy_this(other.gameObject);
		}
	}

	// colliders
	private void OnCollisionEnter2D(Collision2D collision)
	{
		collider_hit(collision);
	}
	private void OnCollisionStay2D(Collision2D collision)
	{
		collider_hit(collision);
	}
	private void collider_hit(Collision2D other)
	{
		if (other.gameObject.tag == "Ball")
		{
			destroy_this(other.gameObject);
		}
	}

	// destroyer
	private void destroy_this(GameObject other)
	{
		if (!marked)
		{
			marked = true;
			_NewTutorialManager.Instance.BrokeTarget();
			other.GetComponent<Ball_Behavior>().reset_ball();
			Destroy(gameObject);
		}
	}
}
