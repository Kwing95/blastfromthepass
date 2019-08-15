using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeGoal : MonoBehaviour
{
    // Start is called before the first frame update
    public Color activeColor = new Color(255, 60, 0);
    public Color inactiveColor = new Color (115, 115, 115);

    private int team;
    private bool in_tutorial;
    private GameObject targetText;
    private SpriteRenderer[] srList;

    void Start()
    {
        team = GetComponent<ScoreGoal>().team;
        targetText = transform.Find("TargetText").gameObject;
        srList = GetComponentsInChildren<SpriteRenderer>();
        if (SceneManager.GetActiveScene().name == "LAB_Ian2")
		{
			in_tutorial = true;
		}
        if (Observer.Instance != null && !in_tutorial)
            Observer.Instance.TeamHasBall += EnablePillar;
		Debug.Log(team);
    }

    void EnablePillar(int teamId){
		Debug.Log(string.Format("Switching to team {0}.  Current Team {1}", teamId, team));
        bool isTeam = (teamId == team || teamId < 0);

        if (targetText != null) targetText.SetActive(isTeam);
        foreach(SpriteRenderer sp in srList) {
            if (sp != null) sp.color = (isTeam ? activeColor : inactiveColor);
            }
    }

	private void OnDestroy()
	{
		if (Observer.Instance != null) Observer.Instance.TeamHasBall -= EnablePillar;
	}
}
