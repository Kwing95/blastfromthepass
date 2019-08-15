using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hop : MonoBehaviour
{
    // up/down distance
    public AnimationCurve ac;
    public float speed_scale = .5f;
    public float curve_scale = .5f;
    private float timer = 0;
    private Vector3 start_pos;
    private bool isHopping;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hop is enabled");
        start_pos = transform.position;
        isHopping = false;
        StartCoroutine(Wait(.0f));
    }

    // Update is called once per frame
    void Update()
    {
        if (isHopping)
        {
            Debug.Log("Hopping");
            timer += Time.unscaledDeltaTime * speed_scale;
            if (timer >= 1.0f)
            {
                isHopping = false;
                Debug.Log("Finished Hopping");
            }
            transform.position = start_pos + (Vector3.up * ac.Evaluate(timer) * curve_scale);
        }
    }

    IEnumerator Wait(float time)
    {
        Debug.Log("Start coroutine Wait");
        // yield return new WaitForSecondsRealtime(time);
        yield return new WaitForSeconds(time);
        isHopping = true;
        Debug.Log("Stop Coroutine Wait");
    }
}
