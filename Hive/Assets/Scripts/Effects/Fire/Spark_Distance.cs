using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark_Distance : MonoBehaviour
{
	// velocities
	public bool active = false;
	private float x_vel, y_vel;
	private Vector3 vel;

	// tri color range
	private SpriteRenderer sr;
	public Color[] colors;
	private float[] distances;
	private int index = 0;
	private float dist = 0;

	// ref to pool
	public Fire_Distance parent_script;

	// is active?
	public bool is_available()
	{
		return !active;
	}

	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		sr.enabled = false;
		distances = new float[colors.Length];
	}

	// call this function on activation
	public void init_stats(float[] _distances, float _xvel, float _yvel)
	{
		index = colors.Length - 1;
		for (int a = 0; a < colors.Length; a++)
		{
			distances[a] = _distances[a];
		}
		x_vel = _xvel;
		y_vel = _yvel;
		vel.x = x_vel;
		vel.y = y_vel;
		sr.color = colors[index];
		sr.sortingOrder = index;
		// Debug.Log("Lifespans: " + lifespans[0] + " || " + lifespans[1] + " || " + lifespans[2]);
		active = true;
		sr.enabled = true;
		parent_script.spark_count++;
	}

	// call this to deactivate spark
	public void deactivate()
	{
		sr.enabled = false;
		active = false;
		parent_script.spark_count--;
	}

	// Update is called once per frame
	void Update()
	{
		if (!active) return;
		dist = Vector3.Magnitude(transform.position);
		if (distances[index] <= dist)
		{
			index--;
			if (index < 0)
			{
				deactivate();
				return;
			}
			sr.color = colors[index];
			sr.sortingOrder = index;
		}
		transform.position += vel * Time.deltaTime;
		// maybe add drag?
		// maybe add alpha fade?
	}
}
