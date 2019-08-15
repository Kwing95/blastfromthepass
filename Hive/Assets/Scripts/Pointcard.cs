using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointcard : MonoBehaviour
{
	// ref to children
	// public GameObject explosion_prefab;
	private Transform[] children;
	private int lives;
	private int target_lives;
	public int team_id = -1;

    // Start is called before the first frame update
    void Start()
    {
		children = transform.GetComponentsInChildren<Transform>();
		lives = transform.childCount;
		target_lives = lives;
    }

    // Update is called once per frame
    void Update()
    {
		// update lives counter
        if (_GameManager.score[team_id-1] != (transform.childCount - lives))
		{
			target_lives = transform.childCount - _GameManager.score[team_id-1];
		}
		// update visual
		if (target_lives < lives)
		{
			//Instantiate(explosion_prefab, children[lives].position, Quaternion.identity);
			//children[lives].gameObject.SetActive(false);
			children[lives].GetComponent<Image>().color = Color.red;
			lives--;
		}
		if (target_lives > lives)
		{
			lives = target_lives;
			for (int a=1; a<lives+1; a++)
			{
				//children[a].gameObject.SetActive(true);
				children[a].GetComponent<Image>().color = Color.green;
			}
		}
	}
}
