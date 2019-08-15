using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageScript : MonoBehaviour
{
	// get ref to sprite renderer
	private SpriteRenderer[] sr_list;
	public float max_scale = 2.0f;
	private Vector3 target_scale;
	private Vector3 start_scale;

    // Start is called before the first frame update
    void Start()
    {
		sr_list = GetComponentsInChildren<SpriteRenderer>();
		target_scale = transform.localScale * (max_scale - 1.0f);
		start_scale = transform.localScale;
    }

    // fadeout script
	public void FadeOut()
	{
		BoxCollider2D[] bc_list = GetComponentsInChildren<BoxCollider2D>();
		foreach (BoxCollider2D bc in bc_list) { bc.enabled = false; };
		StartCoroutine(ExpandAndFade(1f));
	}
	IEnumerator ExpandAndFade(float time)
	{
		Color temp_color = sr_list[0].color;
		float timestep = .1f;
		float timer = .0f;
		while (timer < time)
		{
			timer += timestep;
			yield return new WaitForSeconds(timestep);
			transform.localScale = start_scale + (target_scale * (timer / time));
			temp_color.a = (1 - (timer / time));
			foreach (SpriteRenderer sr in sr_list) { sr.color = temp_color; };
		}
	}
}
