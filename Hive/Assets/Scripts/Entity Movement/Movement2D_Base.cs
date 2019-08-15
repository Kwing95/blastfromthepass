using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Movement2D_Base : MonoBehaviour
{
    // debugging
    public bool is_keyboard = false;
    public int keyboard_id = -1;
    private static int num_keyboard_players = 0;
    public static int current_keyboard = 1;

    protected bool projectingQuick = false;
    // zoom params
    public float lose_ball_zoom_time = 0.2f;
    public float slow_down_rate = 0.8f;

    // voice work
    public Vocalizer vox;
    protected SoundStacker stacker;
    public AudioClip jumpSound;
    public AudioClip throwSound;
    public AudioClip passSound;
    public AudioClip dashSound;
    public AudioClip tackleSound;
    public AudioClip reload;

    // share stats
    public float move_spd = 4.0f;
	public float max_spd = 4.0f;
	public float jump_spd = 8.0f;
	public float default_charge = 0.5f;
	public int team_id = -2;
	public int player_id = -2;

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
	protected float direction = (float)Math.PI;
	private GameObject[] trail;

	// track throw
	float throw_deadzone = .3f;
	float _last_horiz = 0;
	float _last_vert = 0;
    float last_horiz = 0;
    float last_vert = 0;
	// tackle behavior
	public GameObject tackle_box_prefab;
	protected GameObject tackle_box;
	protected float tackle_cooldown = .5f;
	protected float tackle_time = .1f;
	protected float tackle_spd = .0f;
	protected bool tackling = false;
	protected bool can_tackle = true;
	public SpriteRenderer visual;
	protected Color orig_color;

	// tackle effect
	public GameObject tackle_effect_prefab;
	private GameObject tackle_effect;
	private float facing_angle;

	// stun behavior
	protected float stun_time = 2.0f;
	protected bool stunned = false;
	public GameObject stun_effect_prefab;
	protected GameObject stun_effect;

	// projection behavior
	private List<GameObject> proj_trail = new List<GameObject>();
	public GameObject projection_dot;
	public GameObject projection_triangle;

	// dust particles
	private Dust_Maker dm;
	private float last_dir = 0;

	// quickthrow pointer
	public GameObject pointer_prefab;
	private GameObject pointer;

	// squish effect
	private SquishnBounce squish_script;

	// for debug purposes
	public bool is_dummy = false;
	protected Vector3 spawn_pos;

    // reference the ball, even if player isn't holding it
    private GameObject ball_reference;

	// dropdown timer
	private float dropdown_timer = .0f;

	// freeze bool
	protected bool freeze_player = false;

	// find teammate
	private Transform _teammate;
	private Color indicator = Color.yellow;

	// last input from leftstick
	private float left_horiz = 0;
	private float left_vert = 0;

	// turn walljump on/off
	public bool wall_jump_on = false;
	private Wall_Jump wall_jump_script;
	private float wall_jump_repel_force;
	private bool wall_jumping = false;
	public float wall_jump_slide = -6f;

	// tutorial backup
	public CameraZoom tutorial_cam;
    private CameraZoom cameraZoom;


    // Awake is called before Start
    protected virtual void Awake()
	{
		pointer = Instantiate(pointer_prefab, transform.position, Quaternion.identity);
		pointer.transform.parent = transform;
		spawn_pos = transform.position;
		wall_jump_repel_force = move_spd * 1.32f;
	}

	// Start is called before the first frame update
	protected virtual void Start()
    {
        stacker = GetComponent<SoundStacker>();
        ball_reference = GameObject.Find("Temp_Ball"); // doesn't work in tutorial scene (multiple balls)
		rb = GetComponent<Rigidbody2D>();
        //Debug.Log("start " + rb.velocity);
		dm = GetComponent<Dust_Maker>();
		squish_script = transform.Find("Visual").GetComponent<SquishnBounce>();
		orig_color = visual.color;
		tackle_spd = move_spd * 4.0f;
		if (!is_keyboard)
		{
			if (gp != null)
			{
				gp.SetMotorSpeeds(.5f, 1);
				gp.PauseHaptics();
			}
		}
		if (is_dummy)
		{
			//player_id = -2;
			//team_id = -2;
		}

		if (is_keyboard)
            keyboard_id = ++num_keyboard_players;

		// get teammate
		GameObject[] player_list = GameObject.FindGameObjectsWithTag("Player");
		Movement2D_Base temp_base;
		for (int a=0; a<player_list.Length; a++)
		{
			temp_base = player_list[a].GetComponent<Movement2D_Base>();
			if (temp_base.team_id == team_id && temp_base.player_id != player_id) _teammate = player_list[a].transform;
		}

		// get ref to wall jump if used
		if (wall_jump_on)
		{
			wall_jump_script = transform.Find("Wall_Jump").GetComponent<Wall_Jump>();
		}


    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		trigger_hit(collision);
		if (wiley_timer <= 0 && collision.CompareTag("Ground"))
		{
			//GetComponent<WallJumpScript>().wall_jump(collision, gp);
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		trigger_hit(collision);
		if (wiley_timer <= 0 && collision.CompareTag("Ground"))
		{
			//GetComponent<WallJumpScript>().wall_jump(collision, gp);
		}
	}

	// Check if player has collided with something
	// Can collide with: Ground, Ball, Stunner
	protected virtual void trigger_hit(Collider2D other)
    {
		// ground check
		if (other.tag == "Ground" || other.tag == "Goal")
		{
			float distToGround = GetComponent<Collider2D>().bounds.extents.y;
			if (Physics2D.Raycast(transform.position, -Vector2.up, distToGround + 0.1f)){
				wiley_timer = wiley_factor;
				wall_jumping = false;
			}
            // generate puff if hitting ground with y_vel < -2.0f
            //Debug.Log(gameObject.layer);


            //Debug.Log(other.gameObject.layer);
            //Debug.Log(rb.velocity);
            if (rb.velocity.y <= -5.0f && !(gameObject.layer == 10 && other.gameObject.layer == 11))
			{
				dm.generate_puff(new Vector3(-.3f, -.3f, 0), 30, -.5f, .2f, .3f, .1f, .3f, .1f);
				dm.generate_puff(new Vector3(.3f, -.3f, 0), 30, .5f, .2f, .3f, .1f, .3f, .1f);
				squish_script.squish();
			}
		}

        if (!has_ball && other.tag == "Ball" && ball_reference != null && MagnetPass(other))
        {
            //Debug.Log("time to lerp!");
        }

		// ball check
		if (other.tag == "Ball" && no_regrab <= 0
			&& !has_ball && !other.GetComponent<Ball_Behavior>().getIsHeld()
			&& !stunned)
		{
			//Debug.Log(gameObject.name + ": I grabbed the ball!");
			if (is_dummy)
			{
				// Debug.Log("Ball Team ID: " + other.GetComponent<Ball_Behavior>().last_team_id);
			}
			if (!(is_dummy && other.GetComponent<Ball_Behavior>().last_team_id != -1)) pickup_ball(other);
		}
		/*
		if (other.tag == "Ball")
		{
			// Debug.Log("Ball: " + other.name);
			Debug.Log(gameObject.name + " || no_regrab: " + no_regrab + " || has_ball: " + has_ball + " || other.transform.parent: " + other.transform.parent.name);
		}
		*/
		// tackle_box check
		if (other.tag == "Stunner" && !stunned && other.GetComponent<Stun>().id != team_id)
		{
			// drop ball if you have it
			if (has_ball)
			{

                //Debug.Log(gameObject.name + ": I dropped the ball!");
                drop_ball();
			}
			// stun effect
			stunned = true;
			stun_effect = Instantiate(stun_effect_prefab, transform.position + Vector3.up * 1, Quaternion.identity);
			stun_effect.transform.parent = transform;
			rb.velocity = Vector3.zero;
			clear_projection();
			// for extensability, we can delegate stun_time to a script on the other object
			StartCoroutine(reset_stun(stun_time * cameraZoom.get_orig_timescale()));
		}
	}

    private void GetStunned()
    {
        stunned = true;
        stun_effect = Instantiate(stun_effect_prefab, transform.position + Vector3.up * 1, Quaternion.identity);
        stun_effect.transform.parent = transform;
        rb.velocity = Vector3.zero;
        clear_projection();
        // for extensability, we can delegate stun_time to a script on the other object
        StartCoroutine(reset_stun(stun_time * cameraZoom.get_orig_timescale()));
    }

    private bool MagnetPass(Collider2D other)
    {
		if (ball_reference == null) return false;
		// Debug.Log("In MagnetPass: " + other.name);
        Ball_Behavior b = ball_reference.GetComponent<Ball_Behavior>();

        if (other.CompareTag("PassCollider") && b.CanMagnetize(player_id, team_id))
        {
            //return true;
            LayerMask mask = ~(1 << 12) & ~(1 << 14); // ignore ball layers
            //Debug.Log(ball_reference.layer);
            RaycastHit2D hit;
            hit = Physics2D.Raycast(ball_reference.transform.position, transform.position - ball_reference.transform.position, 5, mask);
            //Debug.Log("Raycast hit " + hit.collider.gameObject.name);
            return hit.collider.gameObject == gameObject; // if raycast goes directly into player, it can be magnetized to them
        }
        return false;
    }

	// fixed update to avoid flooding the scene with particles
	protected virtual void FixedUpdate()
	{
		// making wall jump particles
		if (!wall_jump_on || grounded) return;
		if (wall_jump_script.touching_ground())
		{
			// StartCoroutine(run_puffs(.05f, wall_jump_script.get_dir()));
		}
	}

	// Update is called once per frame
	protected virtual void Update()
    {
		if (freeze_player)
		{
			rb.velocity = Vector3.zero;
			return;
		}
		// move the ball to the player if the player has the ball
		// move ball to player
		if (has_ball)
		{
			if (ball.transform.parent != null)
			{
				ball.transform.localPosition = Vector3.zero;
				ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			}
			Vector3 vel = rb.velocity;
			if (!charging)
			{
				Vector2 input_value_left;
				float horiz_left = 0, vert_left = 0;
				if (is_keyboard)
				{
					if (current_keyboard == keyboard_id)
					{
						if (Input.GetKey("w"))
							vert_left = 1;
						else if (Input.GetKey("s"))
							vert_left = -1;
						if (Input.GetKey("d"))
							horiz_left = 1;
						else if (Input.GetKey("a"))
							horiz_left = -1;
					}
				}
				else
				{
                    if (!is_dummy)
                    {
                        input_value_left = gp.leftStick.ReadValue();
                        horiz_left = input_value_left.x;
                        vert_left = input_value_left.y;
						if (Mathf.Abs(horiz_left) > deadzone
							|| Mathf.Abs(vert_left) > deadzone)
							direction = angle(horiz_left, vert_left); // + (float)Math.PI;
                    }
				}
			}
		}

		// wall jump slide
		if (wall_jump_on && !grounded && rb.velocity.y < -.1f && wall_jump_script.touching_ground())
		{
			if (rb.velocity.y < wall_jump_slide) rb.velocity = Vector2.up * wall_jump_slide;
		}

		// passing pointer
		if (has_ball && !charging && !is_dummy)
		{
            // quick throw direction
            // if (pointer.activeSelf == false) Debug.Log(gameObject.name + ": POINTER IS ACTIVE!!!");

            /*
            pointer.SetActive(true);
			float diffx, diffy;
			diffx = transform.position.x - _teammate.position.x;
			diffy = transform.position.y - _teammate.position.y;
			pointer.transform.localEulerAngles = Vector3.forward * ((angle(diffx,diffy)*Mathf.Rad2Deg) + 135.0f);
            */

            // Debug.Log("Angle: " + pointer.transform.localEulerAngles.z);
            // pointer.transform.localEulerAngles = new Vector3(0, 0, (direction) * Mathf.Rad2Deg + 135.0f + 180.0f);
			if (_teammate != null && _teammate.gameObject.activeSelf){
            	List<Vector3> projection = GetComponent<Projectile_Projection>().quick_prediction(transform.position, _teammate.position);
				pass_indicator(projection);
			}
            //make_projection(_teammate.position);

        }
		else
		{
			// if (pointer.activeSelf == true) Debug.Log(gameObject.name + ": POINTER IS OFF!!!");
			pointer.SetActive(false);
		}

		if (is_dummy) return;

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
            if (gp.dpad.left.isPressed)
                vox.PlayVoice(0);
            else if (gp.dpad.right.isPressed)
                vox.PlayVoice(1);
            else if (gp.dpad.up.isPressed)
                vox.PlayVoice(2);
            else if (gp.dpad.down.isPressed)
                vox.PlayVoice(3);

            input_value = gp.leftStick.ReadValue();
			horiz = input_value.x;
			vert = input_value.y;
		}
		horiz = (Mathf.Abs(horiz) > deadzone) ? horiz : 0;
		vert = (Mathf.Abs(vert) > deadzone * 4) ? vert : 0;
		facing_angle = angle(horiz, vert);

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

		// MOAR PARTICLES!!!
		float horiz_check = (horiz > 0) ? 1 : -1;
		if (Mathf.Abs(horiz) >= .8f && horiz_check != last_dir)
		{
			if (grounded && !charging && !stunned)
			{
				last_dir = horiz_check;
				StartCoroutine(run_puffs(.05f, last_dir));
			}
		}
		if (horiz == 0)
		{
			last_dir = 0;
		}

		// tackle
		if (tackle_input() && false) tackle(horiz, vert);

		// don't allow other actions to interrupt
		if (tackling) return;

		// move
		apply_input(horiz, vert);

		// test out dropdown (9 = Player, 10 = PlayerDropDown)
		if (vert < 0)
		{
			dropdown();
		}
		gameObject.layer = (dropdown_timer > 0) ? 10 : 9;
		dropdown_timer -= Time.deltaTime;

		// apply special ability
		Vector2 special_input = new Vector2();
        if (!is_keyboard)
        {
            special_input = gp.leftStick.ReadValue();
			// THIS CODE ENABLES DASHING WITH FACE BUTTONS [facedash]

			/** /

            if (!has_ball)
            {
                if (gp.buttonWest.wasPressedThisFrame)
                    special_input = Vector3.left;
                else if (gp.buttonNorth.wasPressedThisFrame)
                    special_input = Vector3.up;
                else if (gp.buttonEast.wasPressedThisFrame)
                    special_input = Vector3.right;
                else if (gp.buttonSouth.wasPressedThisFrame)
                    special_input = Vector3.down;
            }

            /**/

			horiz = special_input.x;
            vert = special_input.y;
			horiz = (Mathf.Abs(horiz) > deadzone) ? horiz : 0;
			vert = (Mathf.Abs(vert) > deadzone) ? vert : 0;
		}
        else
        {
            if (keyboard_id == current_keyboard)
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
        }
		if (horiz == 0 && vert == 0)
		{
			horiz = _last_horiz;
			vert = _last_vert;
		}
		else
		{
			_last_horiz = horiz;
			_last_vert = vert;
		}
		if (!is_keyboard && gp.leftTrigger.wasPressedThisFrame) apply_special(horiz, vert);

		// check grounded statement
		grounded = wiley_timer > 0;
		wiley_timer -= Time.deltaTime;
		// test out jumping

		if (is_keyboard && keyboard_id == current_keyboard && Input.GetKey("space"))
		{
			if (grounded) jump();
			else if (wall_jump_on && wall_jump_script.touching_ground()) wall_jump();
		}
		if (!is_keyboard && gp.buttonSouth.wasPressedThisFrame)
		{
			if (grounded) jump();
			else if (wall_jump_on && wall_jump_script.touching_ground()) wall_jump();
		}

		// countdown regrab time
		no_regrab -= Time.deltaTime;

		// move the ball with the player
		if (has_ball)
		{
			// move ball to player and zero velocity
			if (ball.transform.parent != null)
			{
				ball.transform.localPosition = Vector3.up * .25f;
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
				input_value = get_throwing_input();
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

            // quicktoss
			if (gp.buttonEast.wasPressedThisFrame)
			{
				quick_pass();
			}

			if (charging)
			{
                projectingQuick = false;
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
					// projection
					List<Vector3> projection = GetComponent<Projectile_Projection>().get_prediction(transform.position, throw_force()*Time.fixedDeltaTime);
					make_projection(projection, projection_dot);
				}
				// throw ball
				if (gp.rightTrigger.wasPressedThisFrame) throw_ball();
			} else
            {
                // quickthrow projection
				if (gp.rightTrigger.wasPressedThisFrame) quick_toss();
                projectingQuick = true;
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
    }

	// wall jump behavior
	protected virtual void wall_jump()
	{
		if (!wall_jump_on) return;

        stacker.PlaySound(jumpSound, 0.5f);
        // wall jump
        Vector3 vel = rb.velocity;
		vel.y = jump_spd;
		vel.x = wall_jump_script.get_dir() * wall_jump_repel_force;
		rb.velocity = vel;
		// spawn dust particles
		// StartCoroutine(jump_puffs(.05f));
		squish_script.squish();
		wall_jumping = true;
		// StartCoroutine(reset_wall_jump(.5f));
	}
	IEnumerator reset_wall_jump(float time)
	{
		yield return new WaitForSeconds(time);
		wall_jumping = false;
	}

	// quick toss behavior (Right Trigger without Aiming)
	protected virtual void quick_toss()
	{
		if (!charging) charge = default_charge;
		throw_ball();
	}

	// dropdown behavior
	protected virtual void dropdown()
	{
		// dropdown layer
		dropdown_timer = .1f;
		// fast fall
		float fast_fall = -3.0f;
		if (rb.velocity.y > fast_fall && rb.velocity.y < 1.0f && grounded)
		{
			Vector3 temp = rb.velocity;
			temp.y = fast_fall;
			rb.velocity = temp;
		}
	}

	// get throwing input
	protected virtual Vector2 get_throwing_input()
	{
		Vector2 input_value = gp.rightStick.ReadValue();
		return input_value;
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
		// default conditions
		bool input =  can_tackle && !has_ball;
		// for Gamepad
		if (!is_keyboard)
			// input = input && (gp.rightTrigger.wasPressedThisFrame);
			input = input && (gp.rightShoulder.wasPressedThisFrame);

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
		Destroy(tackle_effect);
		tackle_box = null;
	}
	IEnumerator reset_tackle_flag(float time)
	{
		yield return new WaitForSeconds(time);
		tackling = false;
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

	// intermediate for quick_pass indicator
	protected void pass_indicator(List<Vector3> projection)
	{
		// original color
		indicator = Color.yellow;

		// check direction
		bool moving_up = (transform.position.y < _teammate.position.y);

		// get a list of all objects inbetween two teammates
		RaycastHit2D[] hit_list = Physics2D.LinecastAll(transform.position, _teammate.position);
		foreach (RaycastHit2D rc in hit_list)
		{
			if (rc.transform.tag == "Ground")
			{
				if (moving_up && rc.transform.gameObject.layer == 11) continue; // ignore dropdowns
				indicator = Color.red;
				break;
			}
		}
		projection_triangle.GetComponent<SpriteRenderer>().color = indicator;
		make_projection(projection, projection_triangle);
	}

	// create projection
	protected void make_projection(List<Vector3> projection, GameObject template)
	{
		clear_projection();
		if (projection.Count < 2) return;
		GameObject temp;
		// also calculate between two marks
		float proj_dir, xval, yval;
		xval = projection[1].x - projection[0].x;
		yval = projection[1].y - projection[0].y;
		proj_dir = angle(xval, yval) * Mathf.Rad2Deg;
		// Debug.Log("Angle: " + proj_dir + " || Xval,Yval: " + xval + "," + yval);
		for (int a=0; a<projection.Count; a++)
		{
			temp = Instantiate(template, projection[a], Quaternion.identity);
			temp.transform.localEulerAngles = new Vector3(0, 0, proj_dir);
			proj_trail.Add(temp);
		}
	}

	// throw implementation
	public virtual void throw_ball()
	{
        stacker.PlaySound(throwSound, 1);
        // Debug.Log("Throwing ball");
        // Debug.Log(direction);
        // Debug.Log(charge);
        no_regrab = regrab_time;
		charging = false;
		Vector3 force = throw_force();
		ball.GetComponent<Rigidbody2D>().AddForce(force);
		if (charge >= charge_max / 2.0f) ball.GetComponent<Ball_Behavior>().charged();
		drop_ball();
	}

	public virtual void drop_ball()
	{
        clear_projection();
		ball.GetComponent<Ball_Behavior>().dropped();
		has_ball = false;
		charging = false;
		if (ball != null)
			ball.transform.parent = null;
		ball = null;
	}

	public virtual void lose_ball(Vector2 vel, bool sameTeam)
	{
        if (sameTeam)
            return;

        stacker.PlaySound(tackleSound, 1);
        // get camera zoom
        GameObject cameraZoomGO = GameObject.FindGameObjectWithTag("MainCamera");
        if (cameraZoomGO != null)
        {
            cameraZoom = cameraZoomGO.GetComponent<CameraZoom>();
            cameraZoom.ZoomIn(gameObject, lose_ball_zoom_time, slow_down_rate);
        }
		else
		{
			//Debug.Log("Tutorial_Cam: " + tutorial_cam.name);
			cameraZoom = tutorial_cam.GetComponent<CameraZoom>();
			//cameraZoom.ZoomIn(gameObject, lose_ball_zoom_time, slow_down_rate);
		}

        has_ball = false;
		charging = false;
		ball.transform.parent = null;
		ball.GetComponent<Ball_Behavior>().dropped();
		ball.GetComponent<Rigidbody2D>().velocity = vel;
		StartCoroutine(ball.GetComponent<Ball_Behavior>().BallNoGrab(1f));
		// Debug.Log("Ball velocity: " + ball.GetComponent<Rigidbody2D>().velocity);
		ball = null;
        GetStunned();
        Commentator c = _AudioMaster.inst.gameObject.transform.GetChild(1).GetComponent<Commentator>();
        c.Comment(Commentator.Trigger.Tackle, true);
        // Debug.Log(gameObject.name + ": DISPOSSESSED!!!");
    }

	// debug angle corrector
	private float correct_angle(float angle)
	{
		return (angle < 0) ? angle + 360 : (angle > 360) ? angle - 360 : angle;
	}

	// separate input from application
	protected virtual void apply_input(float horiz, float vert)
	{
		if (tackling)
			return;
		if (wall_jumping)
		{
			Vector2 wall_vel = rb.velocity;
			wall_vel.x += 2f * horiz * move_spd * Time.deltaTime;
			if (wall_vel.x > wall_jump_repel_force) wall_vel.x = wall_jump_repel_force;
			else if (wall_vel.x < -wall_jump_repel_force) wall_vel.x = -wall_jump_repel_force;
			rb.velocity = wall_vel;
			// Debug.Log("Horizontal Input: " + .1f * horiz * move_spd * Vector2.right * Time.deltaTime);
			return;
		}
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
        stacker.PlaySound(jumpSound, 0.5f);
        Vector3 vel = rb.velocity;
		vel.y = jump_spd;
		rb.velocity = vel;
		// spawn dust particles
		//Debug.Log("jump");
		StartCoroutine(jump_puffs(.05f));
		squish_script.squish();
	}

	// rumble
	public void rumble(float time)
	{
		StartCoroutine(test_rumble(time));
	}

	// haptic test
	IEnumerator test_rumble(float time)
	{
		if (!is_keyboard)
		{
			gp.ResumeHaptics();
			yield return new WaitForSeconds(time);
			gp.PauseHaptics();
		}
	}

	// makes particles for jumping
	IEnumerator jump_puffs(float time)
	{
		float step = .02f;
		float timer = 0;
		while (timer < time)
		{
			timer += step;
			yield return new WaitForSeconds(step);
			dm.generate_puff(new Vector3(0, -.4f, 0), 30, 0, 0.2f, 1f, .2f, .4f, .2f);
		}
	}

	// makes particles for running
	IEnumerator run_puffs(float time, float last_dir)
	{
		float step = .02f;
		float timer = 0;
		while (timer < time)
		{
			timer += step;
			yield return new WaitForSeconds(step);
			dm.generate_puff(new Vector3(0f * last_dir, -.3f, 0), 30, -.7f * last_dir, .3f, .4f, .1f, .3f, .1f);
		}
	}

	protected virtual void tackle(float horiz, float vert)
	{
		// no more tackle
		return;
		// apply physics
		float dir = angle(horiz, vert);
		rb.velocity = new Vector2(Mathf.Cos(dir) * tackle_spd, Mathf.Sin(dir) * tackle_spd);
		rb.gravityScale = 0;

		// flip flags
		tackling = true;
		can_tackle = false;

		// visual and practical effects
		visual.color = Color.white;
		tackle_box = Instantiate(tackle_box_prefab, transform.position, Quaternion.identity);
		tackle_box.transform.parent = transform;
		tackle_box.GetComponent<Stun>().id = team_id;

		// tackle effect - temporarily removed, it might confuse the players as to where the
		// actual hitbox is. Alternatively, we can give it a tackle_box collider and make it so
		// that the tackle is easier to hit, discuss later

		tackle_effect = Instantiate(tackle_effect_prefab, transform.position, Quaternion.identity);
		tackle_effect.transform.parent = transform;
		tackle_effect.transform.localEulerAngles = new Vector3(0, 0, facing_angle * Mathf.Rad2Deg);


		StartCoroutine(reset_tackle(tackle_time));
		StartCoroutine(reset_tackle_flag(tackle_cooldown));
		StartCoroutine(reset_can_tackle(tackle_cooldown * 2.0f));
	}

    public void AssignController(Gamepad gamepad)
    {
        gp = gamepad;
		// Debug.Log(gameObject.name + ": I HAVE A CONTROLLER || " + gp);
    }

	protected virtual void pickup_ball(Collider2D other)
	{
		//Debug.Log(gameObject.name + ": I grabbed the ball!");
		has_ball = true;
		ball = other.gameObject;
		ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		ball.GetComponent<Rigidbody2D>().angularVelocity = 0f;
		ball.transform.parent = transform;
		ball.transform.localPosition = Vector3.zero;
		ball.GetComponent<Ball_Behavior>().picked_up(player_id, team_id);
	}

	// turns off rumble
	public void RumbleOff()
	{
		gp.PauseHaptics();
	}

	protected virtual void quick_pass(){
		return;
	}

	// player "death"
	public virtual void player_death()
	{
		drop_ball();
		Scene scene = SceneManager.GetActiveScene();
		if (scene.name != "LAB_Ian2") _PlayerManager.Instance.RemovePlayer(player_id - 1);
	}
	public IEnumerator dummy_reset(float time)
	{
		rb.gravityScale = 0;
		yield return new WaitForSeconds(time);
		rb.gravityScale = 1.0f;
	}

	// dropdown timer
	IEnumerator reset_dropdown(float time)
	{
		yield return new WaitForSeconds(time);
		gameObject.layer = 9;
	}

	public Gamepad GetGamepad()
	{
		return gp;
	}

    public bool GetGrounded()
    {
        return grounded;
    }

    public bool GetTackling()
    {
        return tackling;
    }

}
