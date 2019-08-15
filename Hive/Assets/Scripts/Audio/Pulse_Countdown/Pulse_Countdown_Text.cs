using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pulse_Countdown_Text : Pulse_Countdown
{
	// attach to text object
	private Text txt;

	protected override void Start()
	{
		base.Start();
		if (!use) return;
		txt = GetComponent<Text>();
		txt.enabled = false;
		//_PlayerManager.Instance.FreezePlayers();
	}

	protected override void pulse()
	{
		if (!use) return;
		// sorry, it just looks so... orderly
		base.pulse();
		if (countdown == 5) scale_bpm(2);
		if (countdown == 3) txt.color = Color.red;
        if (countdown == 0)
        {
            txt.text = "GO!";
            // Destroy the player cages
            GameObject[] cages = GameObject.FindGameObjectsWithTag("Cage");
            for (int a = 0; a < cages.Length; a++)
            {
				cages[a].GetComponent<CageScript>().FadeOut();
            }
        }
        else txt.text = countdown.ToString();
        //if (countdown <= 0) _PlayerManager.Instance.UnFreezePlayers();
        if (countdown < 0) gameObject.SetActive(false);
	}

	public void show_text()
	{
		txt.enabled = true;
	}
}
