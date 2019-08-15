using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
	// set rotation speed
	public float rot_speed = 90.0f;

    // Update is called once per frame
    void Update()
    {
		transform.localEulerAngles += Vector3.forward * rot_speed * Time.deltaTime;
	}
}
