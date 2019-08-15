using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnDestroy : MonoBehaviour
{
	// destroy target trigger
	public BoxCollider2D destroy_trigger;

	// self reference
	private MeshRenderer[] mr_list;

    // Start is called before the first frame update
    void Start()
    {
		mr_list = GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer mr in mr_list)
		{
			mr.enabled = false;
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (!destroy_trigger.enabled)
		{
			foreach (MeshRenderer mr in mr_list)
			{
				mr.enabled = true;
			}
		}
    }
}
