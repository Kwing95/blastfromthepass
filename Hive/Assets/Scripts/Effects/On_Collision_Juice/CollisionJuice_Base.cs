using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionJuice_Base : MonoBehaviour
{
	protected abstract void OnCollisionEnter2D(Collision2D collision);
	protected abstract void OnCollisionExit2D(Collision2D collision);
	protected abstract void OnCollisionStay2D(Collision2D collision);
}
