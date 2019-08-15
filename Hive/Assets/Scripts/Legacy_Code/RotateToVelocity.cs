using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class RotateToVelocity : MonoBehaviour
{

    public Vector3 direction;
    public float lerpSpeed = 0.7f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody localBody = GetComponent<Rigidbody>();
        Vector3 velocity = Vector3.Normalize(direction);
        float angle = Vector3.SignedAngle(velocity, Vector3.up, Vector3.forward);
        if (velocity != Vector3.zero)
            localBody.rotation = Quaternion.Lerp(Quaternion.Euler(Vector3.back * angle), localBody.rotation, lerpSpeed);
        
    }
}
