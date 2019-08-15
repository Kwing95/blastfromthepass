using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowText : MonoBehaviour
{
	// text targets
	public GameObject[] text_targets;
	private bool finished = false;

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (finished) return;
		if (collision.tag != "Player") return;
		for (int a=0; a<text_targets.Length; a++)
		{
			text_targets[a].SetActive(true);
		}
	}
}
