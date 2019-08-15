using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Base : MonoBehaviour
{
	// public damage can be changed per prefab
	public int damage = 10;
	public float knockback = 100.0f;
	public float hitstun = .25f;
	public float duration = .25f;
	public GameObject hurtBox;
	public Vector3 offset;

	// attack behavior always spawns hurtbox
	protected virtual void on_attack()
	{
		GameObject col = Instantiate(hurtBox, transform.position + offset, Quaternion.identity);
		col.GetComponent<DamageDealer_Base>().set_vals(damage, knockback, hitstun, duration);
	}
}
