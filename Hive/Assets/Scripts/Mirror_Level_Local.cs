using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mirror_Level_Local : MonoBehaviour
{
	// mirrors the current level layout
	public Color alt_goal_color;

	// Start is called before the first frame update
	void Start()
	{
		// get all gameobjects that are tagged "Ground"
		GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
		GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");
		GameObject[] cages = GameObject.FindGameObjectsWithTag("Cage");

		// duplicate all objects that aren't at x
		SpriteRenderer sr_temp;
		GameObject temp;
		Vector3 pos;

		for (int a = 0; a < platforms.Length; a++)
		{
			if (platforms[a].transform.parent.parent.parent != null)
			{
				// Debug.Log("Platform.parent.parent.parent: " + platforms[a].transform.parent.parent.parent.name);
			}
			if (platforms[a].transform.parent.parent.parent != transform) continue;
			if (platforms[a].name == "Wall")
			{
				continue;
			}
			temp = Instantiate(platforms[a], platforms[a].transform.position, Quaternion.identity);
			// mirror if not at zero
			if (temp.transform.position.x != 0)
			{
				pos = temp.transform.position;
				pos.x = -temp.transform.position.x;
				temp.transform.position = pos;
				sr_temp = temp.GetComponent<SpriteRenderer>();
				if (sr_temp != null) sr_temp.flipX = true;
				// temp.GetComponent<SpriteRenderer>().flipX = true;
			}
		}
		for (int a = 0; a < goals.Length; a++)
		{
			if (goals[a].transform.parent != transform) continue;
			temp = Instantiate(goals[a], goals[a].transform.position, Quaternion.identity);
			// mirror if not at zero
			if (temp.transform.position.x != 0)
			{
				pos = temp.transform.position;
				pos.x = -temp.transform.position.x;
				temp.transform.position = pos;
				sr_temp = temp.GetComponent<SpriteRenderer>();
				if (sr_temp != null) sr_temp.flipX = true;
				temp.GetComponent<ScoreGoal>().team = 0;
				SpriteRenderer[] children = temp.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer sr in children) { sr.color = alt_goal_color; }
				// get sprite renderer or children and change color
			}
		}
		for (int a = 0; a < cages.Length; a++)
		{
			if (cages[a].transform.parent != transform) continue;
			temp = Instantiate(cages[a], cages[a].transform.position, Quaternion.identity);
			// mirror if not at zero
			if (temp.transform.position.x != 0)
			{
				pos = temp.transform.position;
				pos.x = -temp.transform.position.x;
				temp.transform.position = pos;
				sr_temp = temp.GetComponent<SpriteRenderer>();
				if (sr_temp != null) sr_temp.flipX = true;
			}
		}

		// save scene
		//string path = "Assets//Scenes//";
		//EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", true);
	}
}
