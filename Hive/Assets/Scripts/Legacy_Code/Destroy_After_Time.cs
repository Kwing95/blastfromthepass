using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_After_Time : MonoBehaviour
{
	public float life_time = .2f;

    // Update is called once per frame
    void Update()
    {
		life_time -= Time.deltaTime;
		if (life_time <= 0)
			Destroy(gameObject);
    }
}
