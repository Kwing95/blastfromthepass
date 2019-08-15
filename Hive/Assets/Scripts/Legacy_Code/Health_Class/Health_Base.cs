using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Base : MonoBehaviour
{
	public int max_health = 100;
	public int health; // public for debugging purposes
    // Start is called before the first frame update
    protected virtual void Start()
    {
		health = max_health;
    }

	// take a hit
	public virtual void take_hit(int damage, Vector3 pos, float hitstun, float knockback_force)
	{
		health -= damage;
		if (health <= 0)
			death();
	}

	// on-death behavior
	protected virtual void death()
	{
		Destroy(gameObject);
	}

	// get health
	public float get_health() { return health; }
}
