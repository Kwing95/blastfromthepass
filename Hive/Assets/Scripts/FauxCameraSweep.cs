using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxCameraSweep : MonoBehaviour
{
    public _AudioMaster am;
    // Start is called before the first frame update
    void Start()
    {
        am.ArtificialStart();
    }

    // Update is called once per frame
    void Update()
    {
        // quit build of game
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
