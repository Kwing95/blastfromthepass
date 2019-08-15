using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTextOnTrigger : MonoBehaviour
{
	// get ref to text
	public GameObject[] Text;
	public float wait_time = 2.0f;
	private float timer;

	// start
	private void Start()
	{
		// disable text
		disable_all();
		timer = wait_time;
		// disable own sprite renderer
		GetComponent<SpriteRenderer>().enabled = false;
	}

	// Disable Text 2 seconds (wait_time) after leaving
	private void Update()
	{
		timer -= Time.unscaledDeltaTime;
		if (timer <= 0) disable_all();
	}

	// activate on trigger
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			enable_all();
			timer = wait_time;
		}
	}

	// disable or enable all text
	private void enable_all()
	{
		foreach (GameObject g in Text)
		{
			if (g != null) g.SetActive(true);
		}
	}
	private void disable_all()
	{
		foreach (GameObject g in Text)
		{
			if (g != null) g.SetActive(false);
		}
	}
}
