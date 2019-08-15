using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse_Scale : Pulse_Base
{
	// pulse behavior
	public float pulse_scale = 1.5f;
	public AnimationCurve ac_scale;
	private Vector3 orig_scale;

	// get original scale
	protected override void Start()
	{
		base.Start();
		orig_scale = transform.localScale;
	}

	// pulse behavior
	protected override void pulse()
	{
		//tr.startWidth = pulse_width;
		transform.localScale = orig_scale * pulse_scale;
		StartCoroutine(reset(.5f * pulse_interval));
	}
	private IEnumerator reset(float time)
	{
		float timer = 0;
		int steps = 50; // resolution
		float timestep = time / steps;
		while (timer < time)
		{
			timer += timestep;
			transform.localScale = orig_scale * (ac_scale.Evaluate(timer / time) * (pulse_scale - 1.0f) + 1.0f);
			yield return new WaitForSeconds(timestep);
		}
	}
}
