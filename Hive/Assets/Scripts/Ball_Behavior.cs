using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
public class Ball_Behavior : MonoBehaviour
{

    // commentator
    private Commentator commentator;

	// get ref to rigidbody and trailrenderer
	private Rigidbody2D rb2;
	private TrailRenderer tr;
	private CircleCollider2D trigger;
	private CircleCollider2D col;
	private Vector3 start_pos;
	private GameObject vis;
	private GameObject[] numbers;

    // shakes the camera
    private CameraShake cameraShake;

    // charged effect on trailrenderer
    private Color orig_start_color, orig_end_color;
	public Color alt_start_color;
	public Color alt_end_color;
	private float speed_thresh = 10.0f;
	private float last_yvel = 0f;

	// reset timestep
	private float timestep = .01f;

	// animation values
	public bool animate = true;
	public float pauseAnimation = 1f;
	public float animTime = 0.5f;
	public float vibIntensity = 1f;
	private float timeToAnim;
	private float animRemaining;
	private bool vibDir = false;
	public bool can_be_grabbed = true;

	// check if the ball is held
	public BombFlash bomb;
	private bool is_held = false;
	private int player_id = -1;
    private int team_id = -1; // for goal text
	public int last_team_id = -1; // for checking which team last held the ball

    // Zoom collider
    GoalZoom goalZoom;
    // explosion effect
    public GameObject explosion_prefab;

	// lerp to target value
	private bool lerping = false;
	private Transform lerp_target;
	private GameObject lerp_owner;
	private float lerp_new_factor = -2.0f;

	// fire particle trail
	private ParticleSystem fire_particles;
	private bool particle_lock = false;

	// trying to detect the direction passing through a drop through platform
	private float last_last_y_pos = 0;
	private float last_y_pos = 0;

	// save last held position
	public Vector3 last_held_pos;

	// get refs at start
	private void Start()
	{
        commentator = _AudioMaster.inst.gameObject.transform.GetChild(1).GetComponent<Commentator>();

		fire_particles = transform.Find("Particle_System_Fire").GetComponent<ParticleSystem>();
		fire_particles.Play();
		bomb = GetComponent<BombFlash>();

		rb2 = GetComponent<Rigidbody2D>();
		tr = GetComponent<TrailRenderer>();
		trigger = GetComponent<CircleCollider2D>();
		numbers = GameObject.FindGameObjectsWithTag("Numbers");
		vis = transform.Find("Visual").gameObject;
		col = vis.GetComponent<CircleCollider2D>();
		if (GameObject.FindGameObjectWithTag("MainCamera") != null)
			cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
		start_pos = transform.position;
		orig_start_color = tr.startColor;
		orig_end_color = tr.endColor;
		timeToAnim = pauseAnimation;
		animRemaining = animTime;
        if(transform.Find("Zoom Collider") != null)
        {
            goalZoom = transform.Find("Zoom Collider").GetComponent<GoalZoom>();
        }
		team_id = -1;
        if(Observer.Instance != null)
		    Observer.Instance.PickedUpBall(team_id);
	}

	private void Update(){
		// to manage dropdowns
		last_last_y_pos = last_y_pos;
		last_y_pos = transform.position.y;
		// manage fire effect
		if (fire_particles.isPlaying)
		{
			if (!particle_lock && is_held)
			{
				// Debug.Log("held unlock");
				// Debug.Log("FIRE STOPPED BY PLAYER");
				fire_particles.Stop();
			}
		}

		// if lerping, don't do other stuff
		if (lerping)
		{
			Vector3 save_rot = transform.localEulerAngles;
			transform.LookAt(lerp_target);
			rb2.velocity = transform.forward * 15.0f;
			transform.localEulerAngles = save_rot;
			last_yvel = rb2.velocity.y;
			// Debug.Log("Transform.right: " + transform.forward);
			// Debug.Log("Velocity: " + rb2.velocity);
			return;
		}

		if (transform.parent == null && rb2.velocity == Vector2.zero && animate)
		{
			IdleAnimation();
		}
		// check the countdown
		if (is_held)
		{
			// Debug.Log("Held by Player: " + transform.parent.name);
			if (bomb != null && bomb.time_to_explode())
			{
				ball_explosion();
			}
		}
	}

	// turn off charged_trail on collision
	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Debug.Log("I Hit Something: " + collision.gameObject.name);
		if (collision.gameObject.tag == "Ground"
		|| collision.gameObject.tag == "Player")
		{
			// special condition for dropdowns
			if (collision.gameObject.layer == 11 && last_yvel > 0)
				return;
			tr.startColor = orig_start_color;
			tr.endColor = orig_end_color;
			if (lerping)
				stop_lerping(collision.gameObject);
		}
		// fire effect stop
		if (!particle_lock && collision.gameObject.tag == "Ground")
		{
			if (collision.gameObject.layer == 11)
			{
				if (last_last_y_pos > collision.transform.position.y)
				{
					// Debug.Log("FIRE STOPPED BY DROPDOWN");
					fire_particles.Stop();
				}
			}
			else
			{
				// Debug.Log("FIRE STOPPED BY GROUND");
				fire_particles.Stop();
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Debug.Log("Stop Lerping: ");
		if (collision.gameObject.tag == "Player")
		{
			if (lerping)
			{
				stop_lerping(collision.gameObject);
			}
		}
	}
	private void stop_lerping(GameObject other)
	{
		// if (other == lerp_owner || other.layer == 11) return;
		if (other == lerp_owner) return;
		// check if the ball was intercepted by the other team
		if (other.tag == "Player")
		{
			// COMMENTARY CHECK
			if (other.GetComponent<Movement2D_Base>().team_id != lerp_owner.GetComponent<Movement2D_Base>().team_id)
			{
                commentator.Comment(Commentator.Trigger.Intercept, true);
				Debug.Log("BALL WAS INTERCEPTED BY THE OTHER TEAM");
			}
			else
			{
                //commentator.Comment(Commentator.Trigger.Intercept, true);
                //Debug.Log("BALL WAS SUCCESSFULLY CAUGHT!");
			}
		}

        if(other.GetComponent<Movement2D_QuickPass>() != null)
        {
            // insert code for pass and interception comments
        }
		lerping = false;
		ReactivateRigid();
	}

	// ball explosion
	public void ball_explosion()
	{
        commentator.Comment(Commentator.Trigger.BlowUp, true);
		Debug.Log("KABOOM!!!");
		if (cameraShake != null) cameraShake.Shake();
        transform.parent.gameObject.GetComponent<Movement2D_Base>().player_death();
		reset_ball();
	}

	// reset
	public void reset_ball()
	{
        // insert code for goal comments

		fire_particles.Stop();
		if (transform.parent != null){
			transform.parent.gameObject.GetComponent<Movement2D_Base>().has_ball = false;
			transform.parent = null;
		}
		Instantiate(explosion_prefab, transform.position, Quaternion.identity);
		rb2.velocity = Vector2.zero;
		rb2.angularVelocity = 0f;
		rb2.isKinematic = true;
		trigger.enabled = false;
		col.enabled = false;
		is_held = false;
		team_id = -1;
		last_team_id = -1;
        if (Observer.Instance != null)
            Observer.Instance.PickedUpBall(team_id);
        if(goalZoom != null)
        {
            //goalZoom.SetActive(true);
            goalZoom.Regular();
        }
        if (bomb != null) bomb.reset_countdown();
		if (bomb != null) bomb.held = is_held;
		player_id = -1;
		StartCoroutine(BallNoGrab(.1f));
		StartCoroutine(finish_reset(0.5f));
		//tr.Clear();
	}

	public void explode(){
		Debug.Log("Ball Explosion!!!");
		Instantiate(explosion_prefab, transform.position, Quaternion.identity);
		vis.GetComponent<SpriteRenderer>().enabled = false;
		tr.enabled = false;

		foreach(GameObject obj in numbers){
			obj.GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	public void ians_code_is_weird_reset_ball(){
		fire_particles.Stop();
		if (transform.parent != null){
			transform.parent.gameObject.GetComponent<Movement2D_Base>().has_ball = false;
			transform.parent = null;
		}

		rb2.velocity = Vector2.zero;
		rb2.angularVelocity = 0f;
		rb2.isKinematic = true;
		trigger.enabled = false;
		col.enabled = false;
		is_held = false;
		team_id = -1;
		last_team_id = -1;
		if (Observer.Instance != null)
            Observer.Instance.PickedUpBall(team_id);
        if(goalZoom != null)
        {
            //goalZoom.SetActive(true);
            goalZoom.Regular();
        }
        if (bomb != null) bomb.reset_countdown();
		if (bomb != null) bomb.held = is_held;
		player_id = -1;
		vis.GetComponent<SpriteRenderer>().enabled = true;
		tr.enabled = true;
		tr.Clear();
		foreach(GameObject obj in numbers){
			obj.GetComponent<SpriteRenderer>().enabled = true;
		}
		StartCoroutine(BallNoGrab(.1f));
		StartCoroutine(finish_reset(0.5f));
		tr.Clear();
	}

	/*
	float lerp_factor = (1.0f - Mathf.Exp(-lerp_spd * Time.deltaTime));
	lerp_factor = (lerp_factor > 1.0f) ? 1.0f : (lerp_factor < 0) ? 0 : lerp_factor;
	transform.position = Vector3.Lerp(transform.position, target_pos, lerp_factor);
	*/
	// coroutine reset
	IEnumerator finish_reset(float time)
	{
		float lerp_factor = (1.0f - Mathf.Exp(-8.0f * Time.deltaTime));
		lerp_factor = (lerp_factor > 1.0f) ? 1.0f : (lerp_factor < 0) ? 0 : lerp_factor;
		float timer = 0;
		while (timer < time)
		{
			timer += timestep;
			yield return new WaitForSeconds(timestep);
			transform.position = Vector3.Lerp(transform.position, start_pos, lerp_factor);
		}
		transform.position = start_pos;
		transform.rotation = Quaternion.identity;
		trigger.enabled = true;
		col.enabled = true;
		rb2.isKinematic = false;
	}

	// Changes color of trail renderer
	public void charged()
	{
		tr.startColor = alt_start_color;
		tr.endColor = alt_end_color;
		fire_particles.Play();
		StartCoroutine(fire_lock(.1f));
	}
	IEnumerator fire_lock(float time)
	{
		particle_lock = true;
		yield return new WaitForSeconds(time);
		particle_lock = false;
	}


	public IEnumerator BallNoGrab(float time){
		can_be_grabbed = false;
		yield return new WaitForSeconds(time);
		can_be_grabbed = true;
	}

	// Entrypoint for animation for ball
	private void IdleAnimation()
	{
		// Pause between animation cycles
		if (timeToAnim > 0){
			timeToAnim -= Time.unscaledDeltaTime;
		}

		// Reset animation cycle
		else if (animRemaining <= 0)
		{
			timeToAnim = pauseAnimation;
			animRemaining = animTime;
			vis.transform.localPosition = Vector3.zero;
		}

		// Play animation
		else
		{
			Vibrate(); // replace with whatever animation is appropriate
			animRemaining -= Time.unscaledDeltaTime;
		}
	}

	private void Vibrate()
	{
		float vib = ((vibDir) ? 1f : -1f) * vibIntensity;
		vis.transform.localPosition += new Vector3(vib, 0f, 0f);
		vibDir = !vibDir;
	}

	public void picked_up(int id, int team)
	{
		is_held = true;
        if(goalZoom != null)
        {
            //goalZoom.SetActive(false);
            goalZoom.Shrink();
            goalZoom.ZoomOut();
        }

        if (bomb != null) bomb.held = is_held;
		if (id != player_id) if (bomb != null) bomb.reset_countdown();
		player_id = id;
        team_id = team;
		last_team_id = team;
		if (Observer.Instance != null)
            Observer.Instance.PickedUpBall(team_id);
	}

	public void dropped()
	{
		is_held = false;
		team_id = -1;
		last_held_pos = transform.position;
        if (Observer.Instance != null)
            Observer.Instance.PickedUpBall(team_id);
		if (goalZoom != null)
        {
            //goalZoom.SetActive(true);
            goalZoom.Regular();
        }

        if (bomb != null) bomb.held = is_held;
	}

    public int getTeamId()
    {
        return team_id;
    }

    public bool getIsHeld()
    {
        return is_held;
    }

    public bool CanMagnetize(int player, int team)
    {
        return can_be_grabbed && !is_held && team_id == team && player_id != player;
    }

	public void LerpToTarget(Transform target, GameObject owner)
	{
		// Debug.Log("START LERPING");
		// set lerping var
		lerp_target = target;
		lerp_owner = owner;
		// Debug.Log("Target Pos: " + target.position);
		lerping = true;

		// freeze ball rigidbody
		rb2.angularVelocity = 0f;
		// DeactivateRigid();
	}

	private void DeactivateRigid()
	{
		rb2.velocity = Vector2.zero;
		rb2.angularVelocity = 0f;
		rb2.isKinematic = true;
	}
	private void ReactivateRigid()
	{
		rb2.isKinematic = false;
	}
}
