using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer_Enemy : DamageDealer_Base
{
	// on collision
	protected override void trigger_hit(Collider other)
	{
		if (other.tag != "Enemy")
		{
			deal_damage(other.gameObject);
		}
	}
}
