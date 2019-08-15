using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageDealer_Base : MonoBehaviour
{
	// could potentially add a hit-list to prevent multihit on the same character
	public int damage = 10;
	public float knockback = 100f;
	public float hitstun = .25f;
	public float duration = .5f;

	// sets damage, knockback, and hitstun
	public void set_vals(int _damage, float _knockback, float _hitstun, float _duration)
	{
		damage = _damage;
		knockback = _knockback;
		hitstun = _hitstun;
		duration = _duration;
	}

	// Destroy this object once duration runs out
	private void Update()
	{
		duration -= Time.deltaTime;
		if (duration <= 0)
			Destroy(gameObject);
	}

	// deals damage to other object
	protected virtual void deal_damage(GameObject other)
	{
		Health_Base other_health = other.GetComponent<Health_Base>();
		if (other_health)
		{
			Debug.Log("[From DamageDealer_Base] I Hit: " + other.name);
			other_health.take_hit(damage, transform.position, hitstun, knockback);
		}
	}

	// must implement some trigger behavior
	protected abstract void trigger_hit(Collider other);
	private void OnTriggerEnter(Collider other)
	{
		trigger_hit(other);
	}
	private void OnTriggerStay(Collider other)
	{
		trigger_hit(other);
	}
}
