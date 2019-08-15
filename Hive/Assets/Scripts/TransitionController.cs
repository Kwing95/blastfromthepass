using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playTransition(float transitionTime)
    {
        animator = GetComponent<Animator>();

        // print("Setting the trigger");
        animator.SetBool("Trigger", true);
        StartCoroutine(StopPlaying(transitionTime));
    }
	IEnumerator StopPlaying(float transitionTime)
	{
		yield return new WaitForSecondsRealtime(transitionTime);
		animator.SetBool("Trigger", false);
	}


}
