using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZoom : MonoBehaviour
{
    public Rigidbody2D rb;
	public GameObject backup_cam;
    public float speedThreshold = 5;

    private CameraZoom cameraZoom;
    private CircleCollider2D circleCollider;
    //private bool inContact = false;


    private void Start()
    {
        GameObject cameraZoomGO = GameObject.FindGameObjectWithTag("MainCamera");
        if(cameraZoomGO == null) 
        {
			cameraZoomGO = backup_cam;
			speedThreshold = 1.0f;
            //Debug.LogError(gameObject.name + ": No main camera detected in scene");
            // return;
        }
        cameraZoom = cameraZoomGO.GetComponent<CameraZoom>();

        circleCollider = GetComponent<CircleCollider2D>();
        //inContact = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cameraZoom == null)
        {
            Debug.LogError("No camera zoom");

        }
        if (collision.transform.CompareTag("Goal"))
        {
            float speed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.y * rb.velocity.y);
			if (speed < speedThreshold)
			{
				Debug.Log("BOMB SPEED IS BELOW THRESHOLD");
				return;
			}
            //if (inContact) return;
            //inContact = true;
            //print("TRIGGER");
            //print(collision.name);
            //print("zoom zoom");
            cameraZoom.ZoomIn(gameObject);
        }
        // print("finsihed trigger");
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    inContact = true;
        
    //}
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (cameraZoom == null)
        {
            Debug.LogError("No camera zoom");

        }
        if (collision.transform.CompareTag("Goal"))
        {
			//if (collision.transform.GetComponent<Pillar>().isPillarDestroyed())
			//{
			// zoom out after .5f seconds
			StartCoroutine(ZoomOutAfter(.1f));
            //ZoomOut();
            //}

            //StartCoroutine(destroyCollider(collision));
        }

    }
	IEnumerator ZoomOutAfter(float time)
	{
		yield return new WaitForSecondsRealtime(time);
		ZoomOut();
	}


    //public void SetInContact(bool value)
    //{
    //    inContact = value;
    //}

    public void ZoomOut()
    {
        if(cameraZoom)
            cameraZoom.ZoomOut();

    }

    public void Shrink()
    {
        //if (inContact) return;
        if(circleCollider != null)
            circleCollider.radius = 0.32f;
    }
    public void Regular()
    {
        //if (inContact) return;
        if (circleCollider != null)
            circleCollider.radius = 2.0f;
    }
}






