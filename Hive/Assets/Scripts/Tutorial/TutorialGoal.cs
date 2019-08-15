using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGoal : MonoBehaviour
{
    public int numScores;

    // trigger behavior
    private void OnTriggerEnter2D(Collider2D other)
    {
        trigger_hit(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        // trigger_hit(other);
    }
    private void trigger_hit(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(other.gameObject);
            _AudioMaster.inst.ScoreSound();
            GetComponent<GoalAnimator>().PlayGoalAnimation();
            numScores++;
        }
    }
}
