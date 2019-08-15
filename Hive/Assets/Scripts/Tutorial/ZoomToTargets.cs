using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomToTargets : MonoBehaviour
{
	// target list
	public Transform[] target_list;
	public Tutorial_Camera cam;
	public float interval = 2.0f;
	private bool activated = false;
	private int activate_count = 0;
	private List<int> activate_list = new List<int>();

	// text list
	//public GameObject[] text_targets;
	public GameObject wait_for_partner_text;

	// dummy player
	public Movement2D_Base dummy_player;
	public BombFlash bomb;

	private void Start()
	{
		bomb.tutorial_trigger = false;
	}

	// begin zoom sequence
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (activated) return;
		if (collision.tag == "Player")
		{
			int temp_id = collision.gameObject.GetComponent<Dash_separate_Tutorial>().get_id();
			// Debug.Log("Temp_ID: " + temp_id + " || List: " + activate_list + " || Contains: " + activate_list.Contains(temp_id));
			if (!activate_list.Contains(temp_id))
			{
				// Debug.Log("Activated List!: " + collision.gameObject.name);
				activate_count++;
				activate_list.Add(temp_id);
			}
			if (activate_count == 1)
			{
				wait_for_partner_text.SetActive(true);
			}
			if (activate_count == 2)
			{
				activated = true;
				bomb.tutorial_trigger = true;
				wait_for_partner_text.SetActive(false);
				// throw ball first
				StartCoroutine(Zooming());
				GetComponent<BoxCollider2D>().enabled = false;
			}
		}
	}
	IEnumerator Zooming()
	{
		// throw ball and wait for a bit
		dummy_player.lose_ball(new Vector2(5f, 5f), false);
		Debug.Log("Dummy Player threw the ball!");
		yield return new WaitForSeconds(3.0f);

		// zoom to targets
		int index = 0;
		cam.in_control = false;
		cam.target_fov = 45;
		cam.set_lerp_spd(5.0f);
		while (index < target_list.Length)
		{
			cam.target_pos = target_list[index].position;
			index++;
			yield return new WaitForSeconds(interval);
		}
		cam.reset_lerp_spd();
		cam.in_control = true;
	}
}
