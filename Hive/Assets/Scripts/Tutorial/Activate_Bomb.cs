using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate_Bomb : MonoBehaviour
{
	// ref to bomb
	public GameObject bomb;
	private Rigidbody2D rb2;
	private float grav_scale;

	// get ref to wall
	public GameObject wall;

    // Start is called before the first frame update
    void Start()
    {
		rb2 = bomb.GetComponent<Rigidbody2D>();
		grav_scale = rb2.gravityScale;
		rb2.gravityScale = 0;
    }

	// check trigger
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			rb2.gravityScale = grav_scale;
			StartCoroutine(changeCountdown());
			StartCoroutine(openWall());
		}
	}
	IEnumerator changeCountdown()
	{
		yield return new WaitForSeconds(1.0f);
		bomb.GetComponent<Pulse_Countdown_Bomb>().start_countdown = 10;
	}
	IEnumerator openWall()
	{
		yield return new WaitForSeconds(5.0f);
		Destroy(wall);
	}
}
