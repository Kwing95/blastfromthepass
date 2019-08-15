using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class EntityMovement : MonoBehaviour
{

    public RotateToVelocity view;
    protected Vector3 directionOfTravel;
    public float baseSpeed = 3;
    private float speedModifier = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        view.direction = directionOfTravel;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = directionOfTravel * baseSpeed * speedModifier;
    }

    // Set intensity to 0 to freeze movement; set to 0.5 to slow movement; set to 1.5 to increase movement
    public void ChangeSpeed(float intensity, float duration)
    {
        speedModifier = intensity;
        StartCoroutine(SpeedChange(duration));
    }

    public IEnumerator SpeedChange(float duration)
    {
        yield return new WaitForSeconds(duration);
        speedModifier = 1;
    }
}
