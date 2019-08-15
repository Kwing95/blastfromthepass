using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnCountdown : MonoBehaviour
{
    public GameObject panel;
    public Text playerIndicator;
    public Text respawnCountdown;

    private Vector3[] offsets = new[] {
        new Vector3(-1200.0f, -750.0f, 0f),
        new Vector3(-833.2f, -750.0f, 0f),
        new Vector3(-466.6f, -750.0f, 0f),
        new Vector3(-100.0f, -750f, 0f)
    };
    private float currentCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCountdown(float numSeconds, int playerID, Color color)
    {
        // Move the panel
        panel.transform.Translate(offsets[playerID]);

        // Update playerIndicator
        playerIndicator.text = "Player " + (playerID + 1).ToString() + "\nRespawn In:";

        // Update colors
        playerIndicator.color = color;
        respawnCountdown.color = color;

        // Initialize countdown
        respawnCountdown.text = numSeconds.ToString();
        currentCount = numSeconds;
        StartCoroutine(Countdown(playerID));
    }

    IEnumerator Countdown(int playerID)
    {
        while (currentCount > 0.0f)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            currentCount -= 1.0f;
            // Update text
            respawnCountdown.text = currentCount.ToString();
        }
        Commentator c = _AudioMaster.inst.gameObject.transform.GetChild(1).GetComponent<Commentator>();
        c.Comment(Commentator.Trigger.Spawn, true);
        _PlayerManager.Instance.players[playerID].SetActive(true);
        Destroy(gameObject);
    }
}
