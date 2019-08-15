using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
	// set a random rotation speed
	private float rot_spd;

    // Start is called before the first frame update
    void Start()
    {
		rot_spd = Random.Range(90.0f, 120.0f);
    }

    // Update is called once per frame
    void Update()
    {
		transform.localEulerAngles += Vector3.forward * rot_spd * Time.deltaTime;
    }
}
