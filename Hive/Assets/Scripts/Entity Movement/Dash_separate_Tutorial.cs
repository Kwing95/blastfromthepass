using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash_separate_Tutorial : Movement2D_QuickPass
{
	// public on/off switches for certain actions
	public bool move_on = false;
	public bool jump_on = false;
	public bool dash_on = false;
	public bool tackle_on = false;
	public bool throw_on = false;
	public bool aim_on = false;
	public bool dropdown_on = false;
	public bool quick_toss_on = false;

	// get messsages
	private List<GameObject> messages = new List<GameObject>();
	private GameObject[] temps;
	private int message_index = 0;
	private int stored_index = -1;

	// respawn position
	private Vector3 respawn_pos;

	// check for three broken targets
	public GameObject[] targets;
	public bool finished = false;

	// portal target
	public Transform portal_target;
	public bool finished_tutorial = false;
	protected Dash_separate_Tutorial tutorial_buddy;

	// dummy stats
	private bool tackled_by_player = false;

	// pass flag message
	private bool pass_flag = true;

	// dash error message
	public GameObject dash_error_prefab;

	protected override void Start()
	{
		base.Start();
		if (team_id == -1 && player_id == -1) return;
		// Debug.Log(transform.name + " || Tutorial Buddy: " + teammate.name);
		tutorial_buddy = teammate.GetComponent<Dash_separate_Tutorial>();
		respawn_pos = transform.position;
		temps = GameObject.FindGameObjectsWithTag("Message");
		for (int a=0; a<temps.Length; a++)
		{
			// only add it to messages if it's in the same parent object
			if (temps[a].transform.parent.parent == transform)
				messages.Add(temps[a]);
			// Debug.Log("Parent: " + temps[a].transform.parent.parent);
			// Debug.Log("Messages: " + messages.Count);
		}
		messages.Sort((x, y) => string.Compare(x.name, y.name));
		update_message();
		/*
		Debug.Log("Tutorial Messages: ");
		for (int a=0; a<messages.Count; a++)
		{
			Debug.Log(messages[a].name);
		}
		*/
	}

	// tutorial triggers
	protected override void trigger_hit(Collider2D other)
	{
		if (!is_dummy || !tackled_by_player) base.trigger_hit(other);
		if (other.tag == "Dash")
		{
			// Debug.Log("Hit by Dash: " + other.transform.parent.name);
			// tackled_by_player = true;
		};
		//if (is_dummy) Debug.Log("Dummy Hit by: " + other.tag);
		if (is_dummy) return;
		// tutorials
		if (other.tag == "Tutorial")
		{
			// Debug.Log(gameObject.name + ": Hit Tutorial Trigger: " + other.name);
			if (other.name == "TutorialTrigger1")
			{
				jump_on = true;
				message_index = 1;
			}
			if (other.name == "TutorialTrigger2")
			{
				dropdown_on = true;
				message_index = 2;
			}
			if (other.name == "TutorialTrigger3")
			{
				dash_on = true;
				// throw_on = true;
				// tackle_on = true;
				message_index = 5;
			}
			if (other.name == "TutorialTrigger4")
			{
				// if (!dash_on) cooldown_timer = 0;
				// dash_on = true;
				message_index = 4;
				pass_flag = true;
				// Debug.Log("Message Index: " + message_index);
			}
			if (other.name == "TutorialTrigger5")
			{
				// tackle_on = true;
				quick_toss_on = true;
				message_index = 5;
				pass_flag = true;
			}
			if (other.name == "TutorialTrigger6")
			{
				throw_on = true;
				aim_on = true;
				message_index = 5;
				pass_flag = false;
			}
			if (other.name == "TutorialTrigger7")
			{
				message_index = -1;
				pass_flag = false;
				// Debug.Log("Message Index is -1: " + message_index);
			}
			if (other.name == "TutorialTrigger8")
			{
				pass_flag = true;
			}
		}
		// respawn locations
		if (other.tag == "Respawn")
		{
			// Debug.Log("Hit respawn trigger");
			respawn_pos = other.transform.position;
		}
		// kill trigger
		if (other.tag == "Enemy")
		{
			respawn();
		}
		// goal trigger
		if (other.tag == "Portal")
		{
			freeze_player = true;
			finished_tutorial = true;
			message_index = 8;
			foreach(SpriteRenderer sr in transform.GetComponentsInChildren<SpriteRenderer>())
			{
				sr.enabled = false;
			}
		}
	}

	// Order of Messages:
	/*
	 * message_index = 0 : Move
	 * message_index = 1 : Jump
	 * message_index = 2 : Dropdown
	 * message_index = 3 : Dash
	 * message_index = 4 : Jump + Dash
	 * message_index = 5 : Dash (no direction)
	 * message_index = 6 : Aim + Throw
	 * message_index = 7 : Pass
	 * message_index = 8 : Waiting for Players
	*/
	// update
	protected override void Update()
	{
		base.Update();
		if (is_dummy) return;
		if (finished) return;
		if (has_ball && message_index != 8)
		{
			if (message_index != 6 && message_index != 7) stored_index = message_index;
			message_index = (!pass_flag)? 6 : 7;
		}
		else if (message_index == 6 || message_index == 7)
		{
			message_index = stored_index;
		}
		update_message();
		// if (!finished) check_finished();
		if (finished_tutorial && tutorial_buddy != null && tutorial_buddy.finished_tutorial) teleport_to_waiting();
	}

	// check for finished
	private void check_finished()
	{
		return;
		finished = true;
		for (int a=0; a<targets.Length; a++)
		{
			if (targets[a] != null) finished = false;
		}
		if (finished) message_index = 8;
		if (finished) update_message();
	}

	// respawn
	public void respawn()
	{
		freeze_player = true;
		foreach(SpriteRenderer sr in transform.GetComponentsInChildren<SpriteRenderer>(true))
		{
			sr.enabled = false;
		}
		StartCoroutine(respawn_now(.5f));
	}
	IEnumerator respawn_now(float time)
	{
		yield return new WaitForSeconds(time);
		transform.position = respawn_pos;
		freeze_player = false;
		foreach (SpriteRenderer sr in transform.GetComponentsInChildren<SpriteRenderer>(true))
		{
			sr.enabled = true;
		}
	}

	// update tutorial message
	private void update_message()
	{
		// if (messages.Count == 0) return;
		/*
		if(message_index < 0 || message_index >= messages.Count)
		{
			Debug.Log("OUT OF BOUNDS || " + gameObject.name + ": " + message_index);
			return;
		}
		*/
		for (int a=0; a<messages.Count; a++)
		{
			// if (a != message_index) messages[a].SetActive(false);
			messages[a].SetActive(false);
		}
		return; // not doing messages anymore
		if (message_index == -1) return;
		messages[message_index].SetActive(true);
		//Debug.Log("message_index: " + message_index);
		//Debug.Log("Activated: " + messages[message_index].name + " || " + messages[message_index].activeSelf);
	}

	// move
	protected override void apply_input(float horiz, float vert)
	{
		// Debug.Log("Move_on: " + move_on);
		if (!move_on) return;
		base.apply_input(horiz, vert);
	}

	// jump
	protected override void jump()
	{
		if (!jump_on) return;
		base.jump();
	}

	// dash
	protected override void apply_special(float horiz, float vert)
	{
		if (!dash_on) return;
		// flash an error message when players try to dash with the ball
		if (has_ball)
		{
			Instantiate(dash_error_prefab, transform.position + Vector3.up * 1.0f, Quaternion.identity);
			return;
		}
		base.apply_special(horiz, vert);
	}

	// tackle
	protected override void tackle(float horiz, float vert)
	{
		if (!tackle_on) return;
		base.tackle(horiz, vert);
	}

	// throw
	public override void throw_ball()
	{
		if (!throw_on) return;
		base.throw_ball();
	}

	// aim
	protected override Vector2 get_throwing_input()
	{
		if (!aim_on) return Vector2.zero;
		return base.get_throwing_input();
	}

	// dropdown
	protected override void dropdown()
	{
		if (!dropdown_on) return;
		base.dropdown();
	}

	// quick pass
	protected override void quick_toss()
	{
		if (!quick_toss_on) return;
		base.quick_toss();
	}

	// override player death
	public override void player_death()
	{
		base.player_death();
		if (is_dummy)
		{
			tackled_by_player = false;
			transform.position = spawn_pos + Vector3.up * 1f;
			StartCoroutine(dummy_reset(1.5f));
		}
		else respawn();
	}

	// teleport to waiting area
	public void teleport_to_waiting()
	{
		transform.position = portal_target.position;
		freeze_player = false;
		StartCoroutine(wait_to_flip());
		// finished_tutorial = false;
		foreach (SpriteRenderer sr in transform.GetComponentsInChildren<SpriteRenderer>())
		{
			sr.enabled = true;
		}
	}
	// hack
	IEnumerator wait_to_flip()
	{
		yield return new WaitForSeconds(.01f);
		finished_tutorial = false;
		finished = true;
		respawn_pos = portal_target.position;
	}
	// returns player_id
	public int get_id() { return player_id; }
}
