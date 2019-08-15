using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Better_Throw_Projection : MonoBehaviour
{
	// keep a record of positions
	Rigidbody2D rb2;
	List<Vector3> pos_rec = new List<Vector3>();
	public float time_max = 1.5f;
	public float timestep = .1f;
	private float sim_step = .01f;

    // Must be awake since this gets used immediately
    void Awake()
    {
		rb2 = GetComponent<Rigidbody2D>();
    }

	// get a new pos_rec
	public List<Vector3> get_pred(Vector3 start_pos, Vector3 add_force)
	{
		Debug.Log("Projection Scene: " + gameObject.scene.name);
		PhysicsScene2D phys2d = gameObject.scene.GetPhysicsScene2D();
		Physics2D.autoSimulation = false;

		// init vars
		pos_rec.Clear();
		transform.position = start_pos;
		rb2.velocity = Vector2.zero;
		rb2.AddForce(add_force);
		pos_rec.Add(transform.position);
		float time_count = 0f;
		int time_scale = (int)(timestep / sim_step);

		// begin prediction
		while (time_count <= time_max)
		{
			// do simulation
			for (int a = 0; a < time_scale; a++)
			{
				phys2d.Simulate(sim_step);
			}
			// add to list
			time_count += timestep;
			pos_rec.Add(transform.position);
		}
		return pos_rec;
	}
}
