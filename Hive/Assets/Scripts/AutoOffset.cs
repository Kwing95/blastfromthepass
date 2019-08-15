using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoOffset : MonoBehaviour
{
	// follow a target at positioned offset
	public Transform target;
	private Vector3 offset;

    // Start is called before the first frame update
    void Awake()
    {
		offset = target.position - transform.position;
		// Debug.Log(gameObject.name + " :: Offset: " + offset);
    }

    // Update is called once per frame
    void Update()
    {
		Debug.Log(gameObject.name + ": updating");
		transform.position = target.position - offset;
    }
}
