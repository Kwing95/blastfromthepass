using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _MenuManager : MonoBehaviour
{
    // singleton ref
    public static _MenuManager inst;

    // get menu count
    private int num_menus;


    // Start is called before the first frame update
    void Awake()
    {
        // enforce singleton
        if (!inst)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(this);
        }
        num_menus = transform.childCount;
    }

    // switch menus
    public void ChangeMenu(string active_name)
    {
        // the first child is the actual object
        for (int a = 0; a < num_menus; a++)
        {
            if (transform.GetChild(a).gameObject.CompareTag("Transition") && active_name != "") continue;
            transform.GetChild(a).gameObject.SetActive(false);
            //Debug.Log("Turning Off: " + transform.GetChild(a).gameObject.name);
        }
        if (transform.Find(active_name))
        {
            transform.Find(active_name).gameObject.SetActive(true);
            //Debug.Log("Turning On: " + transform.Find(active_name).gameObject.name);
        }
    }

}
