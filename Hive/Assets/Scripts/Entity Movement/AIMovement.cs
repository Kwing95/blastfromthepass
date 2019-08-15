using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : Movement2D_QuickPass
{
    // Start is called before the first frame update
    public bool throw_ball = false;
    public float time_btwn_throw = 0f;

    public bool move = false;
    public float time_btwn_move = 0f;

    public bool jump = false;
    public float time_btwn_jump = 0f;

    public bool dash = false;
    public float time_btwn_dash = 0f;

    public bool pass = false;
    public float time_btwn_pass = 0f;

    private Vector3 starting_position;

    void Start()
    {
        return;
    }

    // Update is called once per frame
    void Update()
    {
        if (jump && time_btwn_jump <= 0 && grounded)
        {
            jump();
        }
        if (move && time_btwn_move <= 0)
        {

        }
        if (has_ball)
        {

        }
        // else if (dash && time_btwn_dash <= 0 && )
    }
}
