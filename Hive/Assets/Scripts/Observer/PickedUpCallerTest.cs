using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedUpCallerTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
		{
			Observer.Instance.PickedUpBall(1);
		}
		if (Input.GetKeyDown("2"))
		{
			Observer.Instance.PickedUpBall(0);
		}
		if (Input.GetKeyDown("3"))
		{
			Observer.Instance.PickedUpBall(-1);
		}
	}
}
