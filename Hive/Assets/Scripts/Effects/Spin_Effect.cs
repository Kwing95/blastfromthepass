using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin_Effect : MonoBehaviour
{
	// store start transform
	private float start_xscale;
	public float duration = .5f;
	private float timer = .0f;
	private int dir = -1;

    // Start is called before the first frame update
    void Start()
    {
		start_xscale = transform.localScale.x;
		timer = duration;
    }

    // Update is called once per frame
    void Update()
    {
		timer += Time.deltaTime * dir;
		Vector3 temp = transform.localScale;
		temp.x = (timer / duration);
		transform.localScale = temp;

		// change direction
		if (timer < 0 && dir == -1)
			dir = 1;
		if (timer > duration && dir == 1)
			dir = -1;
    }
}
