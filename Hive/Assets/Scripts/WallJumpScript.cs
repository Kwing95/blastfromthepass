using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class WallJumpScript : MonoBehaviour
{
    public Vector3 pushoff_vector = new Vector3(0f, 0f, 0f);

    // private GameObject parent;
    // private Rigidbody2D parent_rb;
    // private Gamepad gp;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // parent = transform.parent.gameObject;
        rb = GetComponent<Rigidbody2D>();
        // gp = parent.GetComponent<Movement2D_Base>().GetGamepad();
    }


    public void wall_jump(Collider2D other, Gamepad gp)
    {
        Vector3 tmp = pushoff_vector;
        Debug.Log("Wall Jump");
        bool jumping = false;
        if (gp == null) jumping = Input.GetKeyDown("space");
        else jumping = gp.leftTrigger.wasPressedThisFrame;
        if (jumping){
            if (other.transform.position.x > transform.position.x){
                tmp = new Vector3 (-tmp.x, tmp.y, tmp.z);
             }
            rb.velocity = tmp;
        }
    }
}
