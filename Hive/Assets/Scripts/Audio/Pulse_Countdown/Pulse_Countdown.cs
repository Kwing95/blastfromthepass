using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse_Countdown : Pulse_Base
{
	// countdown
	protected int countdown = 10;
	public int start_countdown = 10;
	public bool use = true;

	protected override void Start()
	{
		base.Start();
		if (!use) return;
		reset_countdown();
	}

	public void reset_countdown() { countdown = start_countdown; }

	protected override void pulse()
	{
		countdown--;
	}
}