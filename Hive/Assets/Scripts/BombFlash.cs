using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BombFlash : MonoBehaviour
{
	public float countdown = 10f;
	public bool held = false;

	public Color start_color;
	public Color end_color;
	public float color_change_speed = 0.5f;

	public SpriteRenderer glow;

	private AudioSource beeper;
	private AudioSource aus;

	private float cur_time;

	// Speed of the ball's light fade in seconds
	public float fade_speed = 2f;
	public float min_fade_val = 0.3f;
	private float cur_fade;
	private bool fading = false;

	// for tutorial
	private bool in_tutorial;
	public bool tutorial_trigger = false;


	// Start is called before the first frame update
	void Start()
    {
		in_tutorial = (SceneManager.GetActiveScene().name == "LAB_Ian2");
		beeper = GetComponent<AudioSource>();
        beeper.volume = SoundLevels.effects;
		// aus = _AudioMaster.inst.gameObject.GetComponent<AudioSource>();

		if (glow == null)
		{
			try
			{
				glow = transform.Find("Visual").Find("Glow").GetComponent<SpriteRenderer>();
			}
			catch
			{
				Debug.Log("Ian, this is why we can't have nice things.");
				return;
			}
		}
		cur_time = countdown;
		cur_fade = fade_speed;

	}

    // Update is called once per frame
    void Update()
    {
		// tutorial update
		if (in_tutorial && !tutorial_trigger) return;

		// normal stuff
		if(held){
			cur_time -= Time.deltaTime;
			// Debug.Log(cur_time);
		}
		// Debug.Log(glow.color);
		glow.color = color_correction();
		Color tmp = glow.color;

		float percent_time_left = cur_time / countdown;
		float cur_fade_speed = fade_speed * percent_time_left; // ranges from 0 to 2
        //beeper.pitch = 1.1f - (cur_fade_speed / 5);
		float lerp_to = fading ? 1 : min_fade_val;
		float lerp_from = fading ? min_fade_val : 1;
		tmp.a = Mathf.Lerp(lerp_to, lerp_from, cur_fade/cur_fade_speed);
		cur_fade -= Time.deltaTime;
		if (cur_fade <= 0.01){
			fading = !fading;
			cur_fade = cur_fade_speed;
			if(held) beeper.Play();
		}
		glow.color = tmp;
    }

	// Interpolate ball glow color by first adding all of the end color
	// then removing all the start color
	// at rate specified by color_change_speed
	private Color color_correction(){

		float percent_time_left = cur_time / countdown;
		bool add_end_color = (percent_time_left >= color_change_speed);
		Color cur_color = Color.Lerp(end_color, start_color, percent_time_left);
		if (add_end_color)
		{
			cur_color.r = (cur_color.r < start_color.r) ? start_color.r : cur_color.r * (1/color_change_speed);
			cur_color.g = (cur_color.g < start_color.g) ? start_color.g : cur_color.g * (1/color_change_speed);
			cur_color.b = (cur_color.b < start_color.b) ? start_color.b : cur_color.b * (1/color_change_speed);
		}
		else
		{
			cur_color.r = (cur_color.r < end_color.r) ? end_color.r : cur_color.r;
			cur_color.g = (cur_color.g < end_color.g) ? end_color.g : cur_color.g;
			cur_color.b = (cur_color.b < end_color.b) ? end_color.b : cur_color.b;
		}
		return cur_color;
	}

	public void reset_countdown() { cur_time = countdown; }
	public bool time_to_explode() { return cur_time <= 0; }
}
