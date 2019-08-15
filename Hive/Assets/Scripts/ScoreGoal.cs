using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreGoal : MonoBehaviour
{

	public int team = 0;
	private bool in_tutorial = false;
	private bool already_scored = false;
    private Commentator commentator;

	private void Start()
	{
        commentator = _AudioMaster.inst.gameObject.transform.GetChild(1).GetComponent<Commentator>();
        if (SceneManager.GetActiveScene().name == "LAB_Ian2")
		{
			in_tutorial = true;
		}
	}

	// trigger behavior
	private void OnCollisionEnter2D(Collision2D collision)
	{
		trigger_hit(collision.gameObject);
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		trigger_hit(other.gameObject);
    }
	private void OnTriggerStay2D(Collider2D other)
	{
		// trigger_hit(other);
	}
	private void trigger_hit(GameObject other)
	{
		if (already_scored) return;
		if (other.CompareTag("Ball"))
		{
			already_scored = true;
			Debug.Log("Goal Hit Ball!!!");


            Transform zoomCollider = other.transform.Find("Zoom Collider");
            if(zoomCollider != null)
            {
                if (zoomCollider.GetComponent<GoalZoom>() != null)
                {
                    StartCoroutine(WaitForZoomOut(zoomCollider.GetComponent<GoalZoom>()));
                }
            }
            else { Debug.Log("Goal can't find the zoomcollider"); }


            _AudioMaster.inst.ScoreSound();
			// if (in_tutorial) return;

			// COMMENTARY CHECK
			if (!in_tutorial)
			{
				if (other.GetComponent<Ball_Behavior>().last_team_id == team)
				{
					float long_throw_threshold = 20.0f;
					float shot_distance = Vector3.Distance(other.GetComponent<Ball_Behavior>().last_held_pos, transform.position);
					if (shot_distance > long_throw_threshold)
					{
						commentator.Comment(Commentator.Trigger.LongGoal, true);
						//Debug.Log("PLAYER SCORED A LONGSHOT!");
					}
					else
					{
						commentator.Comment(Commentator.Trigger.Goal, true);
					}
					//Debug.Log("THE PLAYER SCORED ON THEMSELVES!");
				}
				else
				{
					commentator.Comment(Commentator.Trigger.OwnGoal, true);
				}
			}

            if (!in_tutorial)
            {
				_GameManager.score[team]++;
				if (!_GameManager.Instance.GamePoint() || true)
                {
                    StartCoroutine(PlayTransition(other));
                }
            }
            else
            {
                // reset ball
                other.GetComponent<Ball_Behavior>().reset_ball();
            }


            if (!in_tutorial /*&& _GameManager.Instance.GamePoint()*/)
            {

                StartCoroutine(RestPlayer());

            }

            // GetComponent<GoalAnimator>().PlayGoalAnimation();
        }
	}
    IEnumerator WaitForZoomOut(GoalZoom goalZoom) 
    {
        yield return new WaitForSecondsRealtime(.5f);

        goalZoom.ZoomOut();

    }

    IEnumerator PlayTransition(GameObject other)
    {
		//other.GetComponent<Ball_Behavior>().reset_ball();
		other.GetComponent<Ball_Behavior>().explode();
		other.SetActive(false);
		yield return new WaitForSecondsRealtime(1.0f);
		GameObject restpanel = GameObject.FindWithTag("ResetPanel");
		restpanel.GetComponent<TransitionController>().playTransition(3.8f);
        yield return new WaitForSecondsRealtime(1.2f);
		other.SetActive(true);
		other.GetComponent<Ball_Behavior>().ians_code_is_weird_reset_ball();
	}
    IEnumerator RestPlayer()
    {
        yield return new WaitForSecondsRealtime(2.3f);
        _PlayerManager.Instance.ResetPlayers();
		_PlayerManager.Instance.FreezePlayers();
		StartCoroutine(UnFreeze(.5f));
    }
	IEnumerator UnFreeze(float time)
	{
		yield return new WaitForSecondsRealtime(time);
		_PlayerManager.Instance.UnFreezePlayers();
	}

}
