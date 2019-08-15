using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rise_and_Fall : MonoBehaviour
{
	// up/down distance
	public AnimationCurve ac;
	public float speed_scale = .5f;
	public float curve_scale = .5f;
	private float timer = 0;
	private Vector3 start_pos;

    // Start is called before the first frame update
    void Start()
    {
		start_pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		timer += Time.unscaledDeltaTime * speed_scale;
		if (timer >= 1.0f) timer = 0;
		transform.position = start_pos + (Vector3.up * ac.Evaluate(timer) * curve_scale);
    }
}
