using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Jump : MonoBehaviour
{
	// collision variable
	private BoxCollider2D bc;
	private bool touching_wall = false;
	private int dir = 1; // 1 = left, -1 = right
	private ContactFilter2D cf2;

	// get box collider
	private void Start()
	{
		bc = GetComponent<BoxCollider2D>();
	}

	// check for collision
	public bool touching_ground()
	{
		Collider2D[] temp_list = new Collider2D[10];
		bc.OverlapCollider(cf2, temp_list);
		foreach (Collider2D col2 in temp_list)
		{
			if (col2 == null) continue;
			if (col2.tag == "Ground")
			{
				dir = (col2.transform.position.x > transform.position.x) ? -1 : 1;
				return true;
			}
		}
		return false;
	}

	// get direction (must run touching_ground first)
	public int get_dir() { return dir; }
}
