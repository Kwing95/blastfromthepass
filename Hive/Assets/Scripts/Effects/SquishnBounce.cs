using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishnBounce : MonoBehaviour
{
	// squish and bounce
	private Vector3 orig_scale;
	public AnimationCurve ac_x;
	public AnimationCurve ac_y;
	private float timer = 0;
	public float duration = .2f;
	private bool run_anim = false;

    // Start is called before the first frame update
    void Start()
    {
		orig_scale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
		if (!run_anim) return;
		timer += Time.deltaTime;
		float x_scale = ac_x.Evaluate(timer / duration);
		float y_scale = ac_y.Evaluate(timer / duration);
		Vector3 scale = transform.localScale;
		scale.x = x_scale;
		scale.y = y_scale;
		transform.localScale = scale;
		if (timer >= duration)
		{
			run_anim = false;
			transform.localScale = orig_scale;
		}
    }

	// run animation
	public void squish()
	{
		run_anim = true;
		timer = 0;
	}
}
