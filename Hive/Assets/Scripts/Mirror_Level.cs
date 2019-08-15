using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mirror_Level : MonoBehaviour
{
	// mirrors the current level layout
	public Color alt_goal_color;
	public Color alt_outline_color;

    // Start is called before the first frame update
    void Start()
    {
		// get all gameobjects that are tagged "Ground"
		GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
		GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");
        GameObject[] cages = GameObject.FindGameObjectsWithTag("Cage");
        GameObject[] filter = GameObject.FindGameObjectsWithTag("Filter");

        // duplicate all objects that aren't at x
        SpriteRenderer sr_temp;
		GameObject temp;
		Vector3 pos;

		for (int a = 0; a < platforms.Length; a++)
		{
            if(platforms[a].name == "Wall")
            {
                continue;
            }
            temp = Instantiate(platforms[a], platforms[a].transform.position, Quaternion.identity);
			temp.transform.localEulerAngles = platforms[a].transform.localEulerAngles;
			temp.transform.localScale = platforms[a].transform.localScale;
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
				if (temp.GetComponent<ChangeGoal>() != null){
					temp.GetComponent<ChangeGoal>().activeColor = alt_goal_color;
					temp.GetComponent<ChangeGoal>().inactiveColor = new Color(115f, 115f, 115f);
				}
				SpriteRenderer[] children = temp.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer sr in children) { sr.color = alt_goal_color; }
				TextMesh[] texts = temp.GetComponentsInChildren<TextMesh>();
				foreach (TextMesh item in texts)
				{
					item.color = alt_goal_color;
					// reduce color saturation on outline
					if (item.gameObject.name == "Text_Outline")
					{
						item.color = alt_outline_color;
					}
				}
				// get sprite renderer or children and change color
			}
		}
        for (int a = 0; a < cages.Length; a++)
        {
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

        for (int a = 0; a < filter.Length; a++)
        {
            temp = Instantiate(filter[a], filter[a].transform.position, Quaternion.identity);
            // mirror if not at zero
            if (temp.transform.position.x != 0)
            {
                pos = temp.transform.position;
                pos.x = -temp.transform.position.x;
                temp.transform.position = pos;
                sr_temp = temp.GetComponent<SpriteRenderer>();
                if (sr_temp != null) sr_temp.flipX = true;
                SpriteRenderer[] children = temp.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sr in children) { sr.color = new Color(alt_goal_color.r, alt_goal_color.g, alt_goal_color.b, .13f); }

                // get sprite renderer or children and change color
            }
        }
        // save scene
        //string path = "Assets//Scenes//";
        //EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", true);
    }
}
