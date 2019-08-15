using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTextID : MonoBehaviour
{
	// list of sprite renderers on children
	private SpriteRenderer[] sr_list;
	private MeshRenderer[] mr_list;
	private int team;

    // Start is called before the first frame update
    void Start()
    {
		team = transform.parent.GetComponent<ScoreGoal>().team;
		sr_list = GetComponentsInChildren<SpriteRenderer>();
		mr_list = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer mr in mr_list) {
			mr.sortingLayerName = "HUD";
			mr.sortingOrder = 1;
		}
		// GetComponent<MeshRenderer>().sortingLayerName = "HUD";
		if (Observer.Instance != null) Observer.Instance.TeamHasBall += Text_Enable;
    }

	// disables all sprite renderers on children
	private void Text_Enable(int team_id)
	{
		foreach (SpriteRenderer sr in sr_list) { sr.enabled = (team_id == team); }
		foreach (MeshRenderer mr in mr_list) { mr.enabled = (team_id == team); }
	}
	private void OnDestroy()
	{
		if (Observer.Instance != null) Observer.Instance.TeamHasBall -= Text_Enable;
	}
}
