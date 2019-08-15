using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    //public Health_Characters health;
    public SpriteRenderer healthBar;

    private int maxHealth;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        currentHealth = 100;
    }

    // Update is called once per frame
    void Update()
    {
        //***** TESTING PURPOSES ONLY *****
        if (Input.GetKeyDown(KeyCode.U))
        {
            // Increase health by 10
            ChangeHealth(10);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            // Decrease health by 20
            ChangeHealth(-20);
        }
        //*********************************
    }

    private void ChangeHealth(int update)
    {
        currentHealth += update;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        else if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthBar.transform.localScale = new Vector3(((float)currentHealth * 0.01f), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        //healthBar.transform.localPosition = new Vector3((float)(currentHealth - 100) / 2.0f * 0.01f, healthBar.transform.localPosition.y, healthBar.transform.localPosition.z);
    }
}
