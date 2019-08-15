using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessage : MonoBehaviour
{
    public enum TutorialEvent { Collider, Tackle, Goal }

    public TutorialEvent tutorialEvent;

    public GameObject oldPanel;
    public GameObject newPanel;

    public Movement2D_Dash tackleMovementDash;
    private bool dummyGotBall;

    public TutorialGoal aimTutorialGoal;

    private bool hasBeenHit;

    // Start is called before the first frame update
    void Start()
    {
        hasBeenHit = false;
        dummyGotBall = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialEvent == TutorialEvent.Tackle)
        {
            if (!dummyGotBall && tackleMovementDash.has_ball)
            {
                dummyGotBall = true;
            }
            if (!hasBeenHit && dummyGotBall && !tackleMovementDash.has_ball)
            {
				Debug.Log("Dummy Got hit in Scene: " + tackleMovementDash.gameObject.transform.parent.transform.parent.name);
                ChangeUI();
            }
        }
        else if (tutorialEvent == TutorialEvent.Goal)
        {
            if (!hasBeenHit && aimTutorialGoal.numScores == 1)
            {
                ChangeUI();
            }
        }
    }

    private void ChangeUI()
    {
        hasBeenHit = true;
        oldPanel.SetActive(false);
        if (newPanel != null)
        {
            //Debug.Log("I, " + gameObject.name + " am activating " + newPanel.gameObject.name);
            newPanel.SetActive(true);
        }
        else
        {
            _TutorialManager.Instance.PlayerDone();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenHit && other.gameObject.CompareTag("Player"))
        {
            ChangeUI();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!hasBeenHit && other.gameObject.CompareTag("Player"))
        {
            ChangeUI();
        }
    }
}
