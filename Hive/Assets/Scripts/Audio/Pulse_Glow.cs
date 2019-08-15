using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse_Glow : Pulse_Base
{
	// alpha
	private SpriteRenderer sr;
	private Color temp_color;
	public AnimationCurve ac_alpha;

	protected override void Start()
	{
		base.Start();
		sr = GetComponent<SpriteRenderer>();
	}

	protected override void pulse()
	{
		temp_color = sr.color;
		temp_color.a = 1;
		sr.color = temp_color;
		StartCoroutine(reset(.5f * pulse_interval));
	}
	IEnumerator reset(float time)
	{
		float timer = 0;
		int steps = 50; // resolution
		float timestep = time / steps;
		temp_color = sr.color;
		while (timer < time)
		{
			timer += timestep;
			temp_color.a = ac_alpha.Evaluate(timer / time);
			sr.color = temp_color;
			yield return new WaitForSeconds(timestep);
		}
	}
}
