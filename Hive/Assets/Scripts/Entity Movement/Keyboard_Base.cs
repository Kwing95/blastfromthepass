using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Keyboard_Base : MonoBehaviour
{
    // debug stuff
    //public bool is_keyboard

    // share stats
    protected float move_spd = 4.0f;
	public float max_spd = 4.0f;
	public float jump_spd = 8.0f;

	// get ref to rigidbody
	protected Rigidbody2D rb;

	// grounded check
	protected bool grounded = true;
	protected float wiley_factor = .1f;
	protected float wiley_timer = .0f;

	// ball behavior
	protected float no_regrab = .0f;
	protected float regrab_time = .5f;
	protected bool has_ball = false;
	protected GameObject ball;

	// throwing behavior
	protected bool charging = false;
	protected float charge_mult = 1000.0f;
	protected float charge_max = 1.0f;
	protected float charge = 0.0f;
	protected float charge_spd = 1.0f; // units per second
	protected float direction;
	private GameObject[] trail;

	// projection behavior
	private List<GameObject> proj_trail = new List<GameObject>();
	public GameObject projection_dot;

	// for new projection behavior
	//public GameObject ghost_ball;
	//public string projection_scene_name;
	//private PhysicsScene2D alt_physics;
	//public float time_max = 1.5f;
	//public float timestep = .1f;
	//private float sim_step = .01f;

	// Start is called before the first frame update
	protected virtual void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		//SceneManager.LoadScene(projection_scene_name, LoadSceneMode.Additive);
		//alt_physics = SceneManager.GetSceneByName(projection_scene_name).GetPhysicsScene2D();
	}

	// try trigger hit
	protected virtual void trigger_hit(Collider2D other)
	{
		if (other.tag == "Ground")
			wiley_timer = wiley_factor;
		if (other.tag == "Ball" && no_regrab < 0 && !has_ball)
		{
			has_ball = true;
			//Debug.Log("Ball name: " + other.name);
			if (other.transform.parent != null)
				ball = other.transform.parent.gameObject;
			else
				ball = other.gameObject;
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		trigger_hit(collision);
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		trigger_hit(collision);
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		// reduce movespeed while charging
		move_spd = (charging) ? max_spd / 2 : max_spd;

		//gamepad controls
		float horiz = 0;
		float vert = 0;
		if (Input.GetKey("w")) vert = 1;
		else if (Input.GetKey("s")) vert = -1;
		if (Input.GetKey("d")) horiz = 1;
		else if (Input.GetKey("a")) horiz = -1;
		apply_input(horiz, vert);

		// test out dropdown (9 = Player, 10 = PlayerDropDown)
		gameObject.layer = (vert < 0) ? 10 : 9;

		// apply special ability
		horiz = Input.GetAxis("Horizontal2");
		vert = Input.GetAxis("Vertical2");
		apply_special(horiz, vert);

		// check grounded statement
		grounded = wiley_timer > 0;
		wiley_timer -= Time.deltaTime;
		// test out jumping
		if (Input.GetKeyDown("space") && grounded)
		{
			jump();
		}

		// countdown regrab time
		no_regrab -= Time.deltaTime;

		// move the ball with the player
		if (has_ball)
		{
			// move ball to player and zero velocity
			ball.transform.position = transform.position;
			ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			// get throwing input
			horiz = Input.GetAxis("Horizontal2");
			vert = Input.GetAxis("Vertical2");
			if ((horiz != 0 || vert != 0) && !charging)
			{
				charging = true;
				charge = 0;
				direction = angle(horiz, vert);
			}
			if (charging)
			{
				// release ball
				if (horiz == 0 && vert == 0)
				{
					throw_ball();
					clear_projection();
				}
				else
				{
					charge += charge_spd * Time.deltaTime;
					charge = (charge > charge_max) ? charge_max : charge;
					direction = angle(horiz, vert);
					no_regrab = regrab_time;
					// new projection system
					//List<Vector3> projection = get_projection();
					// projection
					List<Vector3> projection = GetComponent<Projectile_Projection>().get_prediction(transform.position, -throw_force() * Time.fixedDeltaTime);
					make_projection(projection);
				}
			}
		}
	}

	// angle given two numbers
	protected float angle(float horiz, float vert)
	{
		return Mathf.Atan2(vert, horiz);
	}

	// get the x_force and y_force
	private Vector3 throw_force()
	{
		float x_force = Mathf.Cos(direction) * charge * charge_mult;
		float y_force = Mathf.Sin(direction) * charge * charge_mult;
		Vector3 force = new Vector3(x_force, y_force, 0);
		return force;
	}

	// clear projection
	private void clear_projection()
	{
		while (proj_trail.Count > 0)
		{
			Destroy(proj_trail[0]);
			proj_trail.RemoveAt(0);
		}
	}

	// get projection (this is the new experimental version)
	/*
	private List<Vector3> get_projection()
	{
		// make ball
		GameObject trail_maker = Instantiate(ghost_ball, transform.position, Quaternion.identity);
		// move ball to the physics scene
		SceneManager.MoveGameObjectToScene(trail_maker, SceneManager.GetSceneByName(projection_scene_name));
		List<Vector3> projection = new List<Vector3>();

		Rigidbody2D rb2 = trail_maker.GetComponent<Rigidbody2D>();
		rb2.velocity = Vector2.zero;
		rb2.AddForce(-throw_force());
		projection.Add(transform.position);
		float time_count = 0f;
		int time_scale = (int)(timestep / sim_step);

		// begin prediction
		Physics2D.autoSimulation = false;
		while (time_count <= time_max)
		{
			// do simulation
			for (int a = 0; a < time_scale; a++)
			{
				alt_physics.Simulate(sim_step);
			}
			// add to list
			time_count += timestep;
			projection.Add(transform.position);
		}
		Physics2D.autoSimulation = true;

		Destroy(trail_maker); // to avoid collisions with ghost ball
		return projection;
	}
	*/

	// create projection
	private void make_projection(List<Vector3> projection)
	{
		clear_projection();
		GameObject temp;
		for (int a = 0; a < projection.Count; a++)
		{
			temp = Instantiate(projection_dot, projection[a], Quaternion.identity);
			proj_trail.Add(temp);
		}
	}

	// throw implementation
	protected virtual void throw_ball()
	{
		charging = false;
		has_ball = false;
		Vector3 force = throw_force();
		ball.GetComponent<Rigidbody2D>().AddForce(-force);
		//Debug.Log("Angle: " + correct_angle(direction*Mathf.Rad2Deg+180) + " || Charge: " + charge);
	}

	// debug angle corrector
	private float correct_angle(float angle)
	{
		return (angle < 0) ? angle + 360 : (angle > 360) ? angle - 360 : angle;
	}

	// separate input from application
	protected virtual void apply_input(float horiz, float vert)
	{
		// only accept horizontal movement
		Vector3 vel = rb.velocity;
		vel.x = horiz * move_spd;
		rb.velocity = vel;
	}

	// abstract, must be implemented by children
	protected abstract void apply_special(float horiz, float vert);

	// makes player jump
	protected virtual void jump()
	{
		Vector3 vel = rb.velocity;
		vel.y = jump_spd;
		rb.velocity = vel;
	}
}
