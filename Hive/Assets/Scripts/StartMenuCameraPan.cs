using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuCameraPan : MonoBehaviour
{
    public float hightestX;
    public float lowestX;
    public float distPerFrame;

    private bool movingRight;

    // Start is called before the first frame update
    void Start()
    {
        movingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingRight)
        {
            transform.position = new Vector3(transform.position.x + distPerFrame, transform.position.y, transform.position.z);
            if (transform.position.x > hightestX)
            {
                movingRight = false;
            } 
        }
        else
        {
            transform.position = new Vector3(transform.position.x - distPerFrame, transform.position.y, transform.position.z);
            if (transform.position.x < lowestX)
            {
                movingRight = true;
            }
        }
    }
}
