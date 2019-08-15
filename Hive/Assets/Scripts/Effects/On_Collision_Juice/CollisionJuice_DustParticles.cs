using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CollisionJuice_DustParticles : CollisionJuice_Rigidbody
{
	// dust particle
	private ParticleSystem ps;
	public int emit_count = 10;

	// override start
	protected override void Start()
	{
		base.Start();
		ps = GetComponent<ParticleSystem>();
	}

	protected override void col_enter(Collision2D col)
	{
		ps.Emit(emit_count);
	}
}
