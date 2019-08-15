using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for testing attack animations
public class Attack_Animated_Debug : Attack_Animated
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
		{
			on_attack();
		}
    }
}
