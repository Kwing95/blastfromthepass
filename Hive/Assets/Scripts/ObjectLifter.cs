using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLifter : MonoBehaviour
{

    public Rigidbody rb;
    public float reach = 0.8f;

    private GameObject carriedObject = null;
    private float rotation;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // currently left control on keyboard
        {
            rotation = (rb.rotation.eulerAngles.z + 90) * Mathf.PI / 180;
            direction = new Vector3(Mathf.Cos(rotation), Mathf.Sin(rotation), 0);

            if (carriedObject != null)
                Drop();
            else
                Grab();
        }
    }

    private void Grab()
    {
        RaycastHit hit;
        Physics.Raycast(rb.position, direction, out hit, reach);

        if (hit.collider != null && hit.collider.CompareTag("Grabbable"))
        {
            GameObject go = hit.collider.gameObject;
            go.GetComponent<BoxCollider>().enabled = false;
            Follower f = go.AddComponent<Follower>();
            go.GetComponent<Rigidbody>().useGravity = false;
            f.subject = rb.gameObject;
            f.offset = Vector3.up * 0.5f;
            go.transform.localScale = Vector3.one * 0.5f;
            carriedObject = go;
        }
    }

    private void Drop()
    {
        carriedObject.transform.localScale = Vector3.one;
        Destroy(carriedObject.GetComponent<Follower>());
        Destroy(carriedObject.GetComponent<Rigidbody>());
        carriedObject.transform.position = transform.position + direction;
        carriedObject.GetComponent<BoxCollider>().enabled = true;
        carriedObject = null;
    }

}
