using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : EntityMovement
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    new void FixedUpdate()
    {
       directionOfTravel = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0));
       base.FixedUpdate();
    }

}
