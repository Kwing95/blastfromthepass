using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement2D_QuickPass))]

public class PlayerAnimator : MonoBehaviour
{

    private Movement2D_QuickPass mover;
    private Rigidbody2D rb;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mover = GetComponent<Movement2D_QuickPass>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.gameObject.GetComponent<SpriteRenderer>().flipX = rb.velocity.x < 0;
        /*if (mover.GetDashing() || mover.GetTackling())
        {
            Debug.Log("to dash");
            animator.SetTrigger("ToDash");
            return;
        }*/
        if (!mover.GetGrounded())
        {
            //Debug.Log("to air");
            animator.SetTrigger("ToAir");
            return;
        }
        if(Mathf.Abs(rb.velocity.x) > 1)
        {
            //Debug.Log("to run");
            animator.SetTrigger("ToRun");
            return;
        }
        //Debug.Log("to stand");
        animator.SetTrigger("ToStand");
    }
}
