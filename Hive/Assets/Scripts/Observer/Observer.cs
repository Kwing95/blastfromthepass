using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Observer : MonoBehaviour
{
	// Set up this observer as a singleton
	public static Observer Instance;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(this);
		}
	}

	// Delegate handler
	public event Action LeftPressDelegate;
	public void LeftWasPressed()
	{
		if (LeftPressDelegate != null) LeftPressDelegate();
	}

	// Delegate handler
	public event Action<int> TeamHasBall;
	public void PickedUpBall(int team_id)
	{
		if (TeamHasBall != null) TeamHasBall(team_id);
		Debug.Log("Observer: PickedUpBall(int) has been called!: " + team_id);
	}

	// Cleaner
	private void OnDestroy()
	{
		// apparently this works properly
		LeftPressDelegate = null;
		TeamHasBall = null;
	}
}
