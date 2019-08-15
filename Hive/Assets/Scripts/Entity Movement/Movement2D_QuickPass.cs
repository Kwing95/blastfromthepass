using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement2D_QuickPass : Movement2D_Dash_separate
{
	// get ref to teammate
	protected Transform teammate;
	protected override void Start()
	{
		base.Start();
		// Debug.Log("TRYING TO PULL PLAYERS LIST");
		if (team_id == -1 && player_id == -1) return;
		List<GameObject> temp_list;
		if (SceneManager.GetActiveScene().name == "StartMenu") return;
		if (FindObjectOfType<_PlayerManager>())
		{
			temp_list = _PlayerManager.Instance.players;
		}
		else
		{
			temp_list = _TutorialGameManager.Instance.players;
		}
		Movement2D_Base temp_base;
		for (int a=0; a<temp_list.Count; a++)
		{
			temp_base = temp_list[a].GetComponent<Movement2D_Base>();
			if (temp_base.player_id != player_id)
			{
				if (temp_base.team_id == team_id)
				{
					teammate = temp_list[a].transform;
				}
			}
		}
		if (teammate == null)
		{
			Debug.Log(transform.name + " || Couldn't find Teammate: " + player_id + " || " + team_id);
		}
	}

	// get rid of quick pointer
	protected override void Update()
	{
		base.Update();
		// pointer.SetActive(false);
	}

	// override quick_throw
	protected override void quick_pass()
	{
		if (teammate == null || !teammate.gameObject.activeSelf) base.quick_toss();
		if (!teammate.gameObject.activeSelf) return;
        stacker.PlaySound(passSound, 1);
		no_regrab = regrab_time;
		charging = false;
		ball.GetComponent<Ball_Behavior>().LerpToTarget(teammate, gameObject);
		ball.GetComponent<Ball_Behavior>().charged();
		drop_ball();
	}
}
