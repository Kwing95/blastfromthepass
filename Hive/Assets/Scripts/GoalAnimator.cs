using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAnimator : MonoBehaviour
{
    // Start is called before the first frame update

    public float rotationSpeed = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed);
    }

    // Called by ScoreGoal when a goal is scored
    public void PlayGoalAnimation(){
        return;
    }
}
