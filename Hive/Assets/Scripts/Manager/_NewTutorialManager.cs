using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.SceneManagement;

public class _NewTutorialManager : MonoBehaviour
{
	// find all players in scene
	private GameObject[] players;

	public static _NewTutorialManager Instance;

	// count the number of targets broken
	private int target_count = 0;
	private int player_count = 0;

	// to prevent multi call
	private bool called_end = false;

	// find on awake
	private void Awake()
	{
		Instance = this;
		Time.timeScale = 2.0f;
		players = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] temps = new GameObject[players.Length];
		// reverse
		for (int a=0; a<players.Length; a++)
		{
			temps[a] = players[a];
		}
		for (int a=0; a<players.Length; a++)
		{
			players[players.Length - 1 - a] = temps[a];
		}
		sort_players();
		AssignControllers();
	}

	// check for level completion
	private void Update()
	{ 

        if (all_finished())
		{
			if (called_end) return;
			called_end = true;
			Transitioner.instance.Transition("StartMenu");
		}
		/*
		if (target_count >= player_count * 3)
		{
            // As of right now, TransitionScene goes right into BugbyScene
			SceneManager.LoadScene("TransitionScene");
		}
		*/
	}

	// assign controllers, deactivate players that don't have controllers
	private void AssignControllers()
	{
		// For Debugging
		//players[0].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(0));
		//players[1].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(1));
		//players[2].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(0));
		//players[3].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(1));

		// Actual code to use
		players[0].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(0));
		players[1].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(2));
		players[2].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(1));
		players[3].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(3));
		player_count = 4;
		/*
		for (int a=0; a<Gamepad.all.Count; a+=2)
		{
			players[a].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(a));
			player_count++;
		}
		for (int a = 1; a < Gamepad.all.Count; a += 2)
		{
			players[a].GetComponent<Movement2D_Base>().AssignController(_GameControls.All.GetGamePad(a));
			player_count++;
		}
		for (int a=Gamepad.all.Count; a<players.Length; a++)
		{
			// players[a].SetActive(false);
		}
		*/
	}

	// respawn player
	public void RespawnPlayer(int index)
	{
		players[index].GetComponent<Dash_separate_Tutorial>().respawn();
		return;
	}

	// increase target count
	public void BrokeTarget() { target_count++; }

	// check if all player have finished the tutorial
	public bool all_finished()
	{
		for (int a=0; a<players.Length; a++)
		{
			if (!players[a].GetComponent<Dash_separate_Tutorial>().finished) return false;
		}
		return true;
	}

	// sorts the players list by team_id then player_id
	private void sort_players()
	{
		GameObject[] temp_player_list = new GameObject[players.Length];
		Movement2D_Base[] temp_move_list = new Movement2D_Base[players.Length];
		for (int a=0; a<players.Length; a++)
		{
			temp_move_list[a] = players[a].GetComponent<Movement2D_Base>();
		}
		int counter = 0;
		for (int a = 0; a < 2; a++) // team_id
		{
			for (int b = 0; b < 2; b++) // player_id
			{
				for(int c=0; c<players.Length; c++)
				{
					if (temp_move_list[c].team_id == a+1 && temp_move_list[c].player_id == b+1)
					{
						temp_player_list[counter] = players[c];
						counter++;
					}
				}
			}
		}
		for (int a=0; a<players.Length; a++)
		{
			players[a] = temp_player_list[a];
		}
	}
}
