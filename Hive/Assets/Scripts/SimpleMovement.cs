using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleMovement : MonoBehaviour
{
	public bool freeze = false;
	public float move_spd = 4.0f;
	private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
		rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
		if (freeze)
			return;
		float horiz = Input.GetAxis("Horizontal");
		float vert = Input.GetAxis("Vertical");
		rb.velocity = new Vector3(horiz * move_spd, vert * move_spd, 0);
    }
}
