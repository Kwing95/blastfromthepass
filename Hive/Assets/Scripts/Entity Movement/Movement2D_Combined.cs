using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Movement2D_Combined : MonoBehaviour
{
    // debugging
    public bool is_keyboard = false;
    public int keyboard_id = -1;
    private static int num_keyboard_players = 0;
    public static int current_keyboard = 1;

    // share stats
    protected float move_spd = 4.0f;
	public float max_spd = 4.0f;
	public float jump_spd = 8.0f;
	public float default_charge = 0.5f;

	// get ref to rigidbody
	protected Rigidbody2D rb;

	// grounded check
	protected bool grounded = true;
	protected float wiley_factor = .1f;
	protected float wiley_timer = .0f;

	// apply controller after _PlayerManager is up
    protected Gamepad gp;
    protected float deadzone = 0.2f;

	// ball behavior
	protected float no_regrab = .0f;
	protected float regrab_time = .25f;
	public bool has_ball = false;
	protected GameObject ball;

	// throwing behavior
	protected bool charging = false;
	protected float charge_mult = 1000.0f;
	protected float charge_max = 1.0f;
	protected float charge = 0.0f;
	protected float charge_spd = 1.0f; // units per second
	protected float direction;
	private GameObject[] trail;

	// track throw
	float throw_deadzone = .3f;
	float last_horiz = 0;
	float last_vert = 0;

	// tackle behavior
	public GameObject tackle_box_prefab;
	protected GameObject tackle_box;
	protected float tackle_cooldown = .5f;
	protected float tackle_time = .1f;
	protected float tackle_spd = .0f;
	//protected bool tackling = false;
	protected bool can_tackle = true;
	public SpriteRenderer visual;
	protected Color orig_color;

	// stun behavior
	protected float stun_time = 2.0f;
	protected bool stunned = false;
	public GameObject stun_effect_prefab;
	protected GameObject stun_effect;

	// projection behavior
	private List<GameObject> proj_trail = new List<GameObject>();
	public GameObject projection_dot;

	// for debug purposes
	public bool is_dummy = false;

    // special ability vars
    private TrailRenderer tr;
    private float dash_spd;
    private float dash_time = .25f;
    private float cooldown_time = 5.0f;
    private float dash_cooldown = .0f;
    private bool dashing = false;

    // get ref to resource bar
    public Transform bar;

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
		orig_color = visual.color;
		tackle_spd = move_spd * 4.0f;

        if (is_keyboard)
            keyboard_id = ++num_keyboard_players;

        //SceneManager.LoadScene(projection_scene_name, LoadSceneMode.Additive);
        //alt_physics = SceneManager.GetSceneByName(projection_scene_name).GetPhysicsScene2D();
        dash_spd = move_spd * 5;
        tr = GetComponent<TrailRenderer>();
        tr.emitting = false;
    }

	// try trigger hit
	protected virtual void trigger_hit(Collider2D other)
	{
		// ground check
		if (other.tag == "Ground")
			wiley_timer = wiley_factor;
		// ball check
		if (other.tag == "Ball" && no_regrab <= 0 && !has_ball && other.transform.parent == null && !stunned)
		{
			//Debug.Log(gameObject.name + ": I grabbed the ball!");
			has_ball = true;
			ball = other.gameObject;
			ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			ball.GetComponent<Rigidbody2D>().angularVelocity = 0f;
			ball.transform.parent = transform;
			ball.transform.localPosition = Vector3.zero;
		}
		// tackle_box check
		if (other.tag == "Stunner" && !stunned)
		{
			// drop ball if you have it
			if (has_ball)
				drop_ball();
			// stun effect
			stunned = true;
			stun_effect = Instantiate(stun_effect_prefab, transform.position + Vector3.up * 1, Quaternion.identity);
			stun_effect.transform.parent = transform;
			rb.velocity = Vector3.zero;
			clear_projection();
			// for extensability, we can delegate stun_time to a script on the other object
			StartCoroutine(reset_stun(stun_time));
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
		// move the ball to the player if the player has the ball
		// move ball to player
		if (has_ball)
		{
			if (ball.transform.parent != null)
			{
				ball.transform.localPosition = Vector3.zero;
			}
			Vector3 vel = rb.velocity;
			if (!charging) {
				Vector2 input_value_left = gp.leftStick.ReadValue();
				float horiz_left = input_value_left.x;
				float vert_left = input_value_left.y;
				direction = angle(horiz_left, vert_left) + 3.14159f;
			}
		}

		if (is_dummy) return;
        // debug with keyboard controls for now
        //float horiz = Input.GetAxis("Horizontal");
        //float vert = Input.GetAxis("Vertical");

        if (is_keyboard)
        {
            if (Input.GetKey("1"))
                current_keyboard = 1;
            else if (Input.GetKey("2"))
                current_keyboard = 2;
            else if (Input.GetKey("3"))
                current_keyboard = 3;
            else if (Input.GetKey("4"))
                current_keyboard = 4;
        }

        if(gp == null && !is_keyboard) return;

		// if stunned, do nothing
		if (stunned) return;

		// reduce movespeed while charging
		move_spd = (charging) ? max_spd / 2 : max_spd;

		// get input
		float horiz = 0, vert = 0;
		Vector2 input_value;
		if (!is_keyboard)
		{
			input_value = gp.leftStick.ReadValue();
			horiz = input_value.x;
			vert = input_value.y;
		}
		horiz = (Mathf.Abs(horiz) > deadzone) ? horiz : 0;
		vert = (Mathf.Abs(vert) > deadzone * 4) ? vert : 0;

		// get keyboard input
		if (is_keyboard && current_keyboard == keyboard_id)
		{
			if (Input.GetKey("w"))
				vert = 1;
			else if (Input.GetKey("s"))
				vert = -1;
			if (Input.GetKey("d"))
				horiz = 1;
			else if (Input.GetKey("a"))
				horiz = -1;
		}

		// tackle
		if (tackle_input())
		{
			// apply physics
			float dir = angle(horiz, vert);
			rb.velocity = new Vector2(Mathf.Cos(dir) * tackle_spd, Mathf.Sin(dir) * tackle_spd);
			rb.gravityScale = 0;
			// flip flags
			//tackling = true;
			can_tackle = false;
			// visual and practical effects
			visual.color = Color.red;
			tackle_box = Instantiate(tackle_box_prefab, transform.position, Quaternion.identity);
			tackle_box.transform.parent = transform;
			// coroutines
			StartCoroutine(reset_tackle(tackle_time));
			StartCoroutine(reset_tackle_flag(tackle_cooldown));
			StartCoroutine(reset_can_tackle(tackle_cooldown * 2.0f));
		}
		// don't allow other actions to interrupt
		//if (tackling) return;

		// move
		apply_input(horiz, vert);

		// test out dropdown (9 = Player, 10 = PlayerDropDown)
		gameObject.layer = (vert < 0) ? 10 : 9;

        // apply special ability
        Vector2 special_input = new Vector2();
        if (!is_keyboard)
            special_input = gp.rightStick.ReadValue();
		horiz = special_input.x;
		vert = special_input.y;
		apply_special(horiz, vert);

		// check grounded statement
		grounded = wiley_timer > 0;
		wiley_timer -= Time.deltaTime;
        // test out jumping

        if (is_keyboard && keyboard_id == current_keyboard && Input.GetKey("space") && grounded)
            jump();
		if (!is_keyboard && gp.rightTrigger.wasPressedThisFrame && grounded)
			jump();

		// countdown regrab time
		no_regrab -= Time.deltaTime;

		// move the ball with the player
		if (has_ball)
		{
			// move ball to player and zero velocity
			if (ball.transform.parent != null)
			{
				ball.transform.localPosition = Vector3.zero;
			}
			ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // get throwing input
            if (is_keyboard)
            {
                if(keyboard_id == current_keyboard)
                {
                    vert = horiz = 0;
                    if (Input.GetKey("up"))
                        vert = 1;
                    else if (Input.GetKey("down"))
                        vert = -1;
                    if (Input.GetKey("right"))
                        horiz = 1;
                    else if (Input.GetKey("left"))
                        horiz = -1;
                }

            } else
            {
                input_value = gp.rightStick.ReadValue();
                horiz = input_value.x;
                vert = input_value.y;
            }

			if (
				(Mathf.Abs(horiz) >= throw_deadzone
				|| Mathf.Abs(vert) >= throw_deadzone)
				&& !charging
			)
			{
				charging = true;
				charge = 0;
			}

			if (gp.leftTrigger.wasPressedThisFrame)
			{
				if (!charging) charge = default_charge;
				throw_ball();
			}
			if (charging)
			{
				// release ball
				if (Mathf.Abs(horiz) <= throw_deadzone
					&& Mathf.Abs(vert) <= throw_deadzone
				)
				{
					clear_projection();
					charge = 0;
					charging = false;
				}
				else
				{
					charge += charge_spd * Time.deltaTime;
					charge = (charge > charge_max) ? charge_max : charge;
					direction = angle(last_horiz, last_vert);
					// new projection system
					//List<Vector3> projection = get_projection();
					// projection
					List<Vector3> projection = GetComponent<Projectile_Projection>().get_prediction(transform.position, -throw_force()*Time.fixedDeltaTime);
					make_projection(projection);
				}
			}
			if (horiz != 0 && vert != 0)
			{
				last_horiz = horiz;
				last_vert = vert;
			}
		}
		else
		{
			clear_projection();
		}

        dash_cooldown -= Time.deltaTime;
        dash_cooldown = (dash_cooldown > 0) ? dash_cooldown : 0;
        Vector2 temp = bar.localScale;
        temp.x = ((cooldown_time - dash_cooldown) / cooldown_time);
        bar.localScale = temp;

    }

	// angle given two numbers
	protected float angle(float horiz, float vert)
	{
		return Mathf.Atan2(vert, horiz);
	}

	// stun coroutines
	IEnumerator reset_stun(float time)
	{
		yield return new WaitForSeconds(time);
		stunned = false;
		Destroy(stun_effect);
		stun_effect = null;
	}

	// input bool for tackle input
	protected bool tackle_input()
	{
        return false;
		// default conditions
		bool input =  can_tackle && !has_ball;
		// for Gamepad
		if (!is_keyboard)
			input = input && (gp.buttonSouth.isPressed || gp.buttonEast.isPressed);
		else // for Keyboard
			input = input
				&& Input.GetKey("t")
				&& (current_keyboard == keyboard_id); 
		return input;
	}

	// tackling coroutines
	IEnumerator reset_tackle(float time)
	{
		yield return new WaitForSeconds(time);
		rb.velocity = Vector3.zero;
		rb.gravityScale = 1.0f;
		visual.color = orig_color;
		Destroy(tackle_box);
		tackle_box = null;
	}
	IEnumerator reset_tackle_flag(float time)
	{
		yield return new WaitForSeconds(time);
		//tackling = false;
	}
	IEnumerator reset_can_tackle(float time)
	{
		yield return new WaitForSeconds(time);
		can_tackle = true;
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
		for (int a=0; a<projection.Count; a++)
		{
			temp = Instantiate(projection_dot, projection[a], Quaternion.identity);
			proj_trail.Add(temp);
		}
	}

	// throw implementation
	public virtual void throw_ball()
	{
		Debug.Log("Throwing ball");
		Debug.Log(direction);
		Debug.Log(charge);
		no_regrab = regrab_time;
		charging = false;
		Vector3 force = throw_force();
		ball.GetComponent<Rigidbody2D>().AddForce(-force);
		drop_ball();
		Debug.Log("Angle: " + correct_angle(direction*Mathf.Rad2Deg+180) + " || Charge: " + charge);
        dash_cooldown = cooldown_time;
    }
	
	public virtual void drop_ball()
	{
		has_ball = false;
		charging = false;
		ball.transform.parent = null;
		ball = null;
	}

	// debug angle corrector
	private float correct_angle(float angle)
	{
		return (angle < 0) ? angle + 360 : (angle > 360) ? angle - 360 : angle;
	}

	// makes player jump
	protected virtual void jump()
	{
		Vector3 vel = rb.velocity;
		vel.y = jump_spd;
		rb.velocity = vel;
	}

    public void AssignController(Gamepad gamepad)
    {
        gp = gamepad;
		Debug.Log(gameObject.name + ": I HAVE A CONTROLLER || " + gp);
    }

    // implement special ability
    protected void apply_special(float horiz, float vert)
    {
        if (has_ball || dash_cooldown > 0)
            return;
        // apply deadzone
        horiz = (Mathf.Abs(horiz) > deadzone * 2) ? horiz : 0;
        vert = (Mathf.Abs(vert) > deadzone * 2) ? vert : 0;
        // dash
        if (horiz != 0 || vert != 0)
        {
            dashing = true;
            dash_cooldown = cooldown_time;
            tr.emitting = true;
            rb.gravityScale = 0;
            float dir = angle(horiz, vert);
            rb.velocity = new Vector2(Mathf.Cos(dir) * dash_spd, Mathf.Sin(dir) * dash_spd);
            StartCoroutine(reset_from_dash(dash_time));
        }
    }

    // reset after dashing
    IEnumerator reset_from_dash(float time)
    {
        yield return new WaitForSeconds(time);
        rb.gravityScale = 1;
        tr.emitting = false;
        rb.velocity = Vector2.zero;
        dashing = false;
    }

    // modify input
    protected void apply_input(float horiz, float vert)
    {
        if (dashing)
            return;
        //if (tackling)
        //    return;
        // only accept horizontal movement
        Vector3 vel = rb.velocity;
        vel.x = horiz * move_spd;
        rb.velocity = vel;
    }

}
