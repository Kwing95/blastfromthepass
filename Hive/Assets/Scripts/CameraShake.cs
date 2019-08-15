using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    Dynamic_Camera dc;
    public float shakeTime = 1.0f;
    bool shake = false;
    Vector3 originalPosition;
    public float magnitude = 1.0f;

    private void Start()
    {
        dc = GetComponent<Dynamic_Camera>();
        shake = false;
    }
    public void Shake()
    {
        if (shake) return;
        shake = true;

        dc.in_control = false;

        originalPosition = transform.localPosition; 
        StartCoroutine(WaitForShake());
    }
    public void Shake(float newMagnitude)
    {
        if (shake) return;

        magnitude = newMagnitude;
        Shake();
    }
    IEnumerator WaitForShake()
    {
        yield return new WaitForSecondsRealtime(shakeTime);
        shake = false;
        dc.in_control = true;
        dc.reset_lerp_spd();

    }

    private void Update()
    {
        if (!shake) return;

        float x = Random.Range(-1f, 1f) * magnitude + originalPosition.x;
        float y = Random.Range(-1f, 1f) * magnitude + originalPosition.y;

        transform.localPosition = new Vector3(x, y, transform.localPosition.z);
    }
}
