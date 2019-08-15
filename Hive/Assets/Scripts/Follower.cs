using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Follower : MonoBehaviour
{

    public GameObject subject;
    public float lerpRate = 1;
    public Vector3 offset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().position += ((subject.GetComponent<Rigidbody>().position + offset) - GetComponent<Rigidbody>().position) / lerpRate;
    }
}
