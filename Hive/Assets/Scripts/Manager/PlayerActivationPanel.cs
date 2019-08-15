using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Input;
public class PlayerActivationPanel : MonoBehaviour
{
    public PlayerPanelController[] playerPanels;
    public GameObject playerPanelPrefab;
    public Text countDownText;
    private int countDown = 3;
    public float countDownTime = 0.5f;
    public float switchTime = 1.0f;
    public GameObject playerPrefab;
	
    public Vector2[] positions;
    public Vector2[] windowSizes;
    public Vector2[] backgroundPositions;

    public Color[] team_colors;



    private int numActivatedPlayers = 0;
    private GameObject[] players;


    // Start is called before the first frame update
    void Start()
    {
        countDownText.GetComponent<Animator>().SetBool("countdown", false);

        //countDownText.gameObject.SetActive(false);
    }
    private bool countdown = false;
    // Shows the player panels and allow for player to switch class
    public void ActivatePanels(int numPlayers)
    {
        numActivatedPlayers = 0;
        //countDownText.gameObject.SetActive(false);
        countdown = false;
        DestroyPlayers();
        playerPanels = new PlayerPanelController[numPlayers];
        players = new GameObject[numPlayers];
        Vector2 windowSize = windowSizes[0];
        if (numPlayers > 2)
        {
            windowSize = windowSizes[1];
        }
        for (int i = 0; i < numPlayers; i++)
        {
            GameObject newPanel = Instantiate(playerPanelPrefab, transform);
            newPanel.SetActive(true);
            newPanel.transform.position = backgroundPositions[i];
            playerPanels[i] = newPanel.GetComponent<PlayerPanelController>();
            playerPanels[i].CanActivate(positions[i], windowSize);
        }
        Camera.main.transform.position = new Vector3(100f,0f,-10f);
    }

 

    // Activates player, the player is ready to play and have selected a class
    public void ActivatePlayer()
    {
        numActivatedPlayers++;


        StartCoroutine(CheckAllReady());
    }

    IEnumerator CheckAllReady()
    {
        yield return new WaitForSecondsRealtime(countDownTime * 10f);
        if (numActivatedPlayers == _StartScreenManager.Instance.GetNumPlayers())
        {
            _StartScreenManager.Instance.DisablePlayerControls();

            print(_StartScreenManager.Instance.GetNumPlayers());
            bool tutorial = false;
            for (int i = 0; i < playerPanels.Length; i++)
            {
                tutorial = playerPanels[i].GetOption() || tutorial;
            }


            /* start count down to start of game */
            yield return StartCoroutine(CountDown(countDown, tutorial));
        }
    }




    // Deactivates player, the player is able to switch classes again
    public void DeactivatePlayer()
    {
        numActivatedPlayers--;

    }



    // The game actually begins - countdown is over
    private void StartGame()
    {
        Debug.Log("START GAME!");
        countDownText.text = " ";

        _StartScreenManager.Instance.LevelSelection();
    }



    // Countdown - changes the text
    IEnumerator CountDown(int count, bool tutorial)
    {
        if (!countdown)
        {
            countdown = !countdown;
            countDownText.gameObject.SetActive(true);
            countDownText.GetComponent<Animator>().SetBool("countdown", true);

            for (; count >= 0; count--)
            {
                countDownText.text = count.ToString();
                yield return new WaitForSecondsRealtime(countDownTime);
                print("count");
            }
            _StateController.inst.wasTutorial = tutorial;

            if (tutorial)
            {
                _LevelManager.Instance.LoadScene("LAB_Ian2");
            }
            else
            {
                StartGame();
            }
        }
       
    }

    public void SwitchOptions(int playerId, bool moveRight)
    {
		if (playerId >= playerPanels.Length) Debug.Log("Switch Options out of range: " + playerId);
		playerPanels[playerId].SwitchOption(moveRight);
       
    }


    // Toggles the activation of the player's readiness to play the game
    public void TogglePlayerPanel(int playerId, bool activated)
    {
        if (playerId >= playerPanels.Length)
        {
            Debug.LogError("Player ID exceeds number of player panels: " + playerId);
            return;
        }
        playerPanels[playerId].ToggleActivation(playerId);

        if (activated)
        {
            if(players[playerId] == null) 
            {
                CreatePlayer(playerId);
            }

            ActivatePlayer();
        }
        else
        {
            DeactivatePlayer();
        }
    }
    public void DestroyPlayers()
    {
        if (players != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null)
                {
                    Destroy(players[i].gameObject);
                }

            }

            players = null;
        }
        if (playerPanels != null)
        {
            for (int i = 0; i < playerPanels.Length; i++)
            {
                if (playerPanels[i] != null)
                {
                    Destroy(playerPanels[i].gameObject);
                }

            }

            playerPanels = null;
        }

        //print(players);
    }


    public void CreatePlayer(int index)
    {
       
        print("numebr of players: " + _StartScreenManager.Instance.GetNumPlayers());

        Gamepad gp = _GameControls.All.GetGamePad(index);
        GameObject newPlayer = Instantiate(playerPrefab, playerPanels[index].transform.Find("Background"));

        Transform vis = newPlayer.transform.Find("Visual");
        vis.gameObject.GetComponent<SpriteRenderer>().color = team_colors[index];
        newPlayer.GetComponent<Movement2D_Base>().AssignController(gp);
        players[index] = newPlayer;
  
    }
}
