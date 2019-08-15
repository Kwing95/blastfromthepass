using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrigger : MonoBehaviour
{
	// remove target
	public GameObject[] targets;

	// destroy together
	private void OnDestroy()
	{
		for (int a=0; a<targets.Length; a++)
		{
			Destroy(targets[a]);
		}
	}
}
