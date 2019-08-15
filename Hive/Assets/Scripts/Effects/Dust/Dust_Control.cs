using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust_Control : MonoBehaviour
{
	// lifetime
	private float lifespan = 0.0f;
	private float max_lifespan = 10.0f;
	private float start_scale;
	private float scale = 0;
	private bool run_anim = false;
	public AnimationCurve ac;

    // Start is called before the first frame update
    void Start()
    {
		start_scale = transform.localScale.x;
		transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
		if (!run_anim)
			return;
		// iterate counter
		lifespan -= Time.deltaTime;
		scale = ac.Evaluate((lifespan / max_lifespan));
		scale *= start_scale;
		transform.localScale = new Vector3(scale, scale, 1.0f);
		if (lifespan <= 0)
		{
			Destroy(gameObject);
		}
    }

	// initializes the dust particle's movement and animation
	public void init_dust(Vector2 vel, float life)
	{
		max_lifespan = life;
		lifespan = max_lifespan;
		GetComponent<Rigidbody2D>().velocity = vel;
		run_anim = true;
	}
}
