using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
	private GameObject text;
    private float timeToCollapse = 5;
    private bool destroyed = false;

    // Start is called before the first frame update
    void Awake()
    {
        Freeze();
		text = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // print("OnTriggerEnter2D" + collision.name);

        if (collision.CompareTag("Ball"))
        {
            Collapse(collision.transform.position);
            if (GetComponent<BoxCollider2D>() != null)
            {
               GetComponent<BoxCollider2D>().enabled = false;

            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        // print("OnTriggerEnter2D" + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Ball"))
        {
            Collapse(collision.transform.position);
            if (GetComponent<BoxCollider2D>() != null)
            {
                GetComponent<BoxCollider2D>().enabled = false;

            }
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Ball") && destroyed)
    //    {
    //        GoalZoom goalZoom = collision.transform.GetComponent<GoalZoom>();
    //        if (goalZoom != null)
    //        {
    //            goalZoom.ZoomOut();
    //            goalZoom.SetInContact(false);
    //        }
    //        GetComponent<BoxCollider2D>().enabled = false;
    //    }
    //}

    public void Freeze()
    {
        for (int i = 1; i < transform.childCount; ++i)
        {
            GameObject go = transform.GetChild(i).gameObject;
            go.GetComponent<PolygonCollider2D>().enabled = false;
            go.GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    public void Collapse(Vector3 other_pos)
    {
		// set up blast vars
		float blast_force = 500f;
		float blast_dist = 0f;
        if(text != null)
        {
            Debug.Log("Destroying Text: " + text.name);
            Destroy(text);
        }
        for (int i = 1; i < transform.childCount; ++i)
        {
            GameObject go = transform.GetChild(i).gameObject;
            go.GetComponent<PolygonCollider2D>().enabled = true;
            go.GetComponent<Rigidbody2D>().isKinematic = false;
			Vector2 force = go.transform.position - other_pos;
			force = Vector3.Normalize(force);
			force *= blast_force;
			blast_dist = Vector3.Distance(go.transform.position, other_pos);
			blast_dist = 1 / Mathf.Pow(blast_dist, 2);
			force *= blast_dist;
            go.GetComponent<Rigidbody2D>().AddForce(force);
            go.layer = 15;
        }
        destroyed = true;
        StartCoroutine(DisableAfterDelay(timeToCollapse));
    }

    public bool isPillarDestroyed() { return destroyed; }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Freeze();
    }

}
