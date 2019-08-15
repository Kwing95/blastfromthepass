using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pulse_Base : MonoBehaviour
{
	// Set pulse vars
	private float bpm = 120f; // beats per minute
	private float pulse_rate; // per second
	protected float pulse_interval;
	private float next_pulse;
	private float offset = .2f; // timing offset of song beat (pulse_interval - offset)

	// get ref to audiosource player
	private AudioSource aus;

	// stay inactive until finished loading
	private bool active = false;

	// call in awake
	private void Awake()
	{

	}

	// calculate starting vars
	protected virtual void Start()
	{
		// get audiosource
		if (_AudioMaster.inst == null)
		{
			//Debug.Log("Audiomaster is null");
		}
		else
		{
			//Debug.Log("Audiomaster: " + _AudioMaster.inst.name);
		}
		aus = _AudioMaster.inst.gameObject.GetComponent<AudioSource>();
		// get_stats();
	}

	// recalculates vars
	private void get_stats()
	{
		// set up pulse timing
		bpm = _AudioMaster.inst.bpm;
		offset = _AudioMaster.inst.offset;
		pulse_rate = bpm / 60.0f;
		pulse_interval = 1.0f / pulse_rate;
		next_pulse = .0f;
		get_next_pulse();
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		if (!active)
		{
			active = !_AudioMaster.inst.inactive;
			if (!active) return;
			else get_stats();
		}
		// check for pulse
		if (aus.time + offset >= next_pulse)
		{
			// pulse
			get_next_pulse();
			pulse();
		}
		// check for new song // incompatible with scale_bpm
		if (bpm != _AudioMaster.inst.bpm)
		{
			//get_stats();
		}
		// check for loop
		if (aus.time * 2 < next_pulse)
		{
			next_pulse = aus.time;
			get_next_pulse();
		}
	}

	// pulse behavior
	protected abstract void pulse();

	// adjust bpm
	protected void scale_bpm(float scale_factor)
	{
		//Debug.Log("Rescaling BPM!");
		bpm *= scale_factor;
		pulse_rate = bpm / 60.0f;
		pulse_interval = 1.0f / pulse_rate;
	}

	// adjust offset
	protected void set_offset(float _offset)
	{
		//Debug.Log("Changing Offset!");
		offset = _offset;
	}

	// find next pulse time
	private void get_next_pulse()
	{
		while (aus.time + offset > next_pulse)
		{
			next_pulse += pulse_interval;
		}
	}
}
