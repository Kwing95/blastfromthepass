using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _TutorialGameManager : MonoBehaviour
{
	// public static
	public static _TutorialGameManager Instance;

	// ref to players
	public List<GameObject> players;

	// get players
	private void Awake()
	{
		Instance = this;
		GameObject[] temp_list = GameObject.FindGameObjectsWithTag("Player");
		for (int a=0; a<temp_list.Length; a++)
		{
			players.Add(temp_list[a]);
		}
		// Debug.Log("Players: " + players);
	}
}
