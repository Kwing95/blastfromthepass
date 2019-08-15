using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraZoom : MonoBehaviour
{
    // camera delay in returning back control
    public float delay = 0.2f;
    private float originalTimeScale = 1.0f;
    
    // target locations
    private Dynamic_Camera dc;

	// change behavior for tutorial scene
	private bool in_tutorial = false;
	private Tutorial_Camera tc;

    private bool zoomIn = false;
    private bool zoomingOut = false;

	private void Awake()
	{
		originalTimeScale = 2.0f;
		Debug.Log("Original Timescale: " + originalTimeScale);
	}

	void Start()
    {
		in_tutorial = (SceneManager.GetActiveScene().name == "LAB_Ian2" || SceneManager.GetActiveScene().name == "LAB_Ian3");
        dc = GetComponent<Dynamic_Camera>();
		tc = GetComponent<Tutorial_Camera>();
        zoomIn = false;
        zoomingOut = false;
    }

	// return original timescale
	public float get_orig_timescale() { return originalTimeScale; }

    // zoom on goal
    public void ZoomIn(GameObject followObject)
    {
        ZoomingIn(followObject, 0.2f);
        StartCoroutine(WaitToZoomOut(1.5f));
        return;
    }

    public void ZoomIn(GameObject followObject, float time, float slowDownRate)
    {
        ZoomingIn(followObject, slowDownRate);
        StartCoroutine(WaitToZoomOut(time));
        return;
    }

    private void ZoomingIn(GameObject followObject, float slowDownRate)
    {
        if (zoomIn) return;
        zoomIn = true;
        Debug.Log(followObject.name + ": Zooming In!!!");
        // print(followObject);
        Time.timeScale = Time.timeScale * slowDownRate;

        if (in_tutorial)
        {
            tc.in_control = false;
            tc.target_fov = 45;
            tc.target_pos = followObject.transform.position;
        }
        else
        {
            dc.in_control = false;
            dc.target_fov = 45;
            dc.target_pos = followObject.transform.position;
        }

    }


    IEnumerator WaitToZoomOut(float time)
    {
        yield return new WaitForSeconds(time);
        ZoomOut();

    }

    // waits for ball to reset and then let the camera reset itself
    IEnumerator WaitForBall()
    {
        yield return new WaitForSecondsRealtime(delay);
        //Debug.LogWarning("giving control back");
		if (in_tutorial)
		{
			tc.in_control = true;
			tc.reset_lerp_spd();
		}
		else
		{
			dc.in_control = true;
			dc.reset_lerp_spd();
		}
        zoomIn = false;
        zoomingOut = false;
    }

    // camera returns controls
    public void ZoomOut()
    {
        if (!zoomIn) return;
        if (zoomingOut) return;
        zoomingOut = true;
        // print("Camera is zooming out");
        Time.timeScale = originalTimeScale;
        StartCoroutine(WaitForBall());
    }
}
