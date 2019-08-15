using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStartSweep : MonoBehaviour
{
    public Vector3 fromPos;
    public Vector3 toPos;
    public AnimationCurve ac;
    public float speed_scale = 0.5f;
    public float curve_scale = 0.5f;

    public _AudioMaster audioMaster;

    private Dynamic_Camera dynCamera;
    

    private float timer = 0;
    private bool isSweeping;
    

    // Start is called before the first frame update
    void Start()
    {
        // Make sure camera is at fromPos
        transform.position = fromPos;

        dynCamera = GetComponent<Dynamic_Camera>();

        // Sweep to toPos
        isSweeping = false;
        StartCoroutine(Wait(1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        if (isSweeping)
        {
            timer += Time.unscaledDeltaTime * speed_scale;
            //transform.position = fromPos + (Vector3.up * ac.Evaluate(timer) * curve_scale);
            if (timer >= 1.0f)
            {
                isSweeping = false;
                dynCamera.enabled = true;
                audioMaster.ArtificialStart();
            }
            Vector3 diff = toPos - fromPos;
            transform.position = fromPos + diff * ac.Evaluate(timer);
        }
        
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        isSweeping = true;
    }
}
