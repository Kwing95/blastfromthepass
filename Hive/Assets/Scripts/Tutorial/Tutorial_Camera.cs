using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Camera : MonoBehaviour
{
	// Border Space (how much space to leave at the edges)
	private float border = 1.0f;

	// Pins to space between
	public Transform[] player_pins;
	public Transform ball_pin;

	// distance from planet to moon
	private float moon_dist = 3f;

	// ref to camera component
	private Camera cam;

	// lerp timing
	private float start_lerp_spd = 10.0f;
	private float lerp_spd;

	// override, hand control of the camera to someone else
	public bool in_control = true;
	public Vector3 target_pos;
	private Vector3 start_pos;
	private float minimum_fov = 60f;
	public float target_fov = 45f;
	private float target_y_offset = 3.0f;

	// debug
	public bool debug_mode = false;

	// Start is called before the first frame update
	void Start()
	{
		lerp_spd = start_lerp_spd;
		start_pos = transform.position;
		target_pos = start_pos;
		cam = GetComponent<Camera>();
		//DontDestroyOnLoad(this);
	}

	// Update is called once per frame
	void Update()
	{
		if (in_control)
		{
			// check num alive
			int num = num_alive();
			if (num <= 1)
			{
				// go to last player
				// maybe lerp
				Vector3 pos = Vector3.zero;
				for (int a = 0; a < player_pins.Length; a++)
				{
					if (player_pins[a].gameObject.activeSelf)
						pos = player_pins[a].position;
				}
				target_pos = new Vector3(pos.x, pos.y, start_pos.z);
				target_fov = 30;
			}
			else
			{
				// dynamic zoom, middle of two farthest points
				float high_x = -1000000, low_x = 1000000;
				float high_y = -1000000, low_y = 1000000;
				float pos_x = 0;
				float pos_y = 0;
				for (int a = 0; a < player_pins.Length; a++)
				{
					if (player_pins[a].gameObject.activeSelf)
					{
						if (player_pins[a].position.x > high_x) high_x = player_pins[a].position.x;
						if (player_pins[a].position.x < low_x) low_x = player_pins[a].position.x;
						if (player_pins[a].position.y > high_y) high_y = player_pins[a].position.y;
						if (player_pins[a].position.y < low_y) low_y = player_pins[a].position.y;
					}
				}
				pos_x = (high_x + low_x) / 2.0f;
				pos_y = (high_y + low_y) / 2.0f;
				pos_y += target_y_offset;

				// set max y_height
				//float y_pos_cap = 15.0f;
				//pos_y = (pos_y > y_pos_cap) ? y_pos_cap : pos_y;

				// find the furthest point
				float max_x_dist = 0;
				float max_y_dist = 0;
				float x_dist, y_dist;
				for (int a = 0; a < player_pins.Length; a++)
				{
					if (player_pins[a].gameObject.activeSelf)
					{
						x_dist = Mathf.Abs(player_pins[a].position.x - pos_x) + moon_dist + border;
						y_dist = Mathf.Abs(player_pins[a].position.y - pos_y) + moon_dist + border + 2.0f;
						if (x_dist > max_x_dist)
						{
							max_x_dist = x_dist;
						}
						if (y_dist > max_y_dist)
						{
							max_y_dist = y_dist;
						}
					}
				}

				// put a cap on max x_dist
				float x_cap = 2000.0f;
				float y_cap = 1400.0f;
				max_x_dist = (max_x_dist > x_cap) ? x_cap : max_x_dist;
				max_y_dist = (max_y_dist > y_cap) ? y_cap : max_y_dist;

				// calculate fov
				float dist = Mathf.Max(max_x_dist, max_y_dist);
				float angle = Mathf.Abs(Mathf.Atan(dist / start_pos.z) * Mathf.Rad2Deg);
				//Debug.Log("Max_x_dist: " + max_x_dist + " || Max_y_dist: " + max_y_dist);
				//Debug.Log("Y: " + transform.position.z + " || X: " + dist + " || Angle: " + angle);
				//Debug.Log("Angle: " + angle);
				target_fov = (angle * 2);
				if (in_control) target_fov = (target_fov < minimum_fov) ? minimum_fov : target_fov;
				target_pos = new Vector3(pos_x, pos_y, start_pos.z);
			}
		}
		else
		{
			// Debug.Log("Someone Else is in Control!!!!");
		}
		// lerp to position
		//Debug.Log("target fov: " + target_fov);
		if (!in_control && debug_mode) return;
		target_pos.z = start_pos.z;
		float lerp_factor = (1.0f - Mathf.Exp(-lerp_spd * Time.deltaTime));
		lerp_factor = (lerp_factor > 1.0f) ? 1.0f : (lerp_factor < 0) ? 0 : lerp_factor;
		transform.position = Vector3.Lerp(transform.position, target_pos, lerp_factor);
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, target_fov, lerp_factor);
		// Debug.Log("Lerp Factor: " + lerp_factor + " || Lerp Spd: " + lerp_spd);
	}

	// count number of active players
	private int num_alive()
	{
		int counter = 0;
		for (int a = 0; a < player_pins.Length; a++)
		{
			if (player_pins[a].gameObject.activeSelf)
				counter++;
		}
		return counter;
	}

	// public set lerp_spd
	public void set_lerp_spd(float spd) { lerp_spd = spd; }
	public void reset_lerp_spd() { lerp_spd = start_lerp_spd; }
}

