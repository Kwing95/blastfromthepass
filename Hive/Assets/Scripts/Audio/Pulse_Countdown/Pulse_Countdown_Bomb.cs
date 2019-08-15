using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pulse_Countdown_Bomb : Pulse_Countdown
{
	// only countdown if being held
	public bool held = false;

	// numbers
	public SpriteRenderer tens;
	public SpriteRenderer ones;
	private Vector3 tens_offset;
	private Vector3 ones_offset;

    // ring
    // public GameObject ring;

	// load number sprites
	private Sprite[] number_list;

	// requires audiosource
	private AudioSource beeper;

    // debug counter
    private int count = 0;

	// for tutorial
	private bool in_tutorial;
	public bool tutorial_trigger = false;

	// set numbers to start value
	protected override void Start()
	{
		if (!use) return;
		base.Start();
		in_tutorial = SceneManager.GetActiveScene().name == "LAB_Ian2";
		beeper = GetComponent<AudioSource>();
		number_list = Resources.LoadAll<Sprite>("Text");
        tens_offset = tens.transform.position - transform.position;
		ones_offset = ones.transform.position - transform.position;
		update_nums();
	}

	// don't rotate
	protected override void Update()
	{
		if (!use) return;
		base.Update();
		// hacky solution, find a better one later
		ones.transform.position = transform.position + ones_offset;
		tens.transform.position = transform.position + tens_offset;
		// once countdown == 0, kaboom!
		countdown = (countdown < 0) ? 0 : countdown;
	}

	public bool time_to_explode() { return countdown == 0; }

	private void update_nums()
	{
		if (countdown < 0) return;
		tens.sprite = number_list[(countdown / 10) % 10];
		ones.sprite = number_list[countdown % 10];
	}

	protected override void pulse()
	{
		if (transform.parent != null && transform.parent.tag == "Player")
		{
			// Debug.Log("Rumble Rumble: " + transform.parent.name);
			transform.parent.gameObject.GetComponent<Movement2D_Base>().rumble(0.2f);
            // ring.transform.localScale = ring.transform.localScale * 2;
        }

        // Debug.Log("In_Tutorial: " + in_tutorial);
		if (in_tutorial && !tutorial_trigger) return;
		// if (held) Debug.Log("Count: " + count++);
		if (held) base.pulse();
		if (held && 0 < countdown && countdown <= 3) if (beeper != null) beeper.Play();
		update_nums();
	}
}
