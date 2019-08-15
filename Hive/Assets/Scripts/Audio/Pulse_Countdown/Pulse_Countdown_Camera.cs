using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse_Countdown_Camera : Pulse_Countdown
{
	// get a list of player locations
	private GameObject[] player_locs;
	private int index = 0;

	// target locations
	private Dynamic_Camera dc;

	// get access to the countdown text script
	public Pulse_Countdown_Text pct;

	// find players
	protected override void Start()
	{
		base.Start();
		if (!use) return;
		dc = GetComponent<Dynamic_Camera>();
		player_locs = GameObject.FindGameObjectsWithTag("Player");
		//scale_bpm(.5f);
		//set_offset(.0f);
	}

	// zoom on a different player
	protected override void pulse()
	{
		if (!use) return;
		base.pulse();
		if (index >= player_locs.Length)
		{
			dc.in_control = true;
			pct.show_text();
			Destroy(this);
			return;
		}
		if (countdown > 0)
		{
			dc.in_control = true;
			dc.reset_lerp_spd();
			return;
		}
		dc.in_control = false;
		//dc.set_lerp_spd(.2f); // works in unity, but is way too fast in build
		dc.target_fov = 45;
		_PlayerManager.Instance.RumblePlayer(index);
		dc.target_pos = player_locs[index++].transform.position;
	}
}
