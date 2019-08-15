using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiseAndFade : MonoBehaviour
{
	// get reference to mesh renderer
	private MeshRenderer mr;
	private TextMesh tm;
	public float lifespan = 2.0f;
	private float timer = .0f;
	private float rise_dist = 2.0f;
	private float rise_step;
	private Color temp_color;

    // Start is called before the first frame update
    void Start()
    {
		// text mesh
		tm = GetComponent<TextMesh>();
		temp_color = tm.color;
		
		// mesh renderer
		mr = GetComponent<MeshRenderer>();
		mr.sortingLayerName = "HUD";
		mr.sortingOrder = 2;

		// rise per step
		rise_step = rise_dist / lifespan;
    }

    // Update is called once per frame
    void Update()
    {
		timer += Time.deltaTime;
		transform.position += Vector3.up * rise_step * Time.deltaTime;
		temp_color.a = 1.0f - (timer / lifespan);
		tm.color = temp_color;
		if (timer > lifespan) Destroy(gameObject);
    }
}
