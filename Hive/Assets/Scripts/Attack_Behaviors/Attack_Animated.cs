using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Plays a custom beetle mandible rotation
// could generalize this script via animations
[RequireComponent(typeof(Animator))]
public class Attack_Animated : Attack_Base
{
	// get ref to animation
	private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
		anim = GetComponent<Animator>();
		//anim.AddClip(animClip, "attack");
		anim.SetTrigger("Attack");
		//anim.Play("Attack", 0, 0);
    }

	// modify on_attack
	protected override void on_attack()
	{
		base.on_attack();
		anim.SetTrigger("Attack");
	}
}
