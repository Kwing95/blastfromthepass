using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer_Player : DamageDealer_Base
{
	// on collision
	protected override void trigger_hit(Collider other)
	{
		if (other.tag != "Player" && other.tag != "Friendly")
		{
			deal_damage(other.gameObject);
		}
	}
}
