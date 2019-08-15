using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class _PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector3[] spawnLocations = new [] {
        new Vector3(-3f, 2f, 0f),
        new Vector3(3f, 2f, 0f),
        new Vector3(-6f, 2f, 0f),
        new Vector3(6f, 2f, 0f)
    };

    public int minNumPlayers = 2;

    int numPlayers { get; set; }
    public List<GameObject> players;
	public Color[] team_colors;
	public GameObject[] team_indicators = new GameObject[2];

    public GameObject respawnCanvasPrefab;

    public static _PlayerManager Instance { get { return _instance; } }
    private static _PlayerManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this);

        }
		PrintAllControllers(); // DEBUG
        // attach controllers to players
        SetNumPlayer(players.Count);
        CreatePlayers();
    }


    // Start is called before the first frame update
    void Start()
    {
        AttachControllers();
    }

    // Update is called once per frame
    void Update()
    {

    }
    // Create players based on number of Gamepad
    // If no gamepads are detected,
    // create players based on minNumPlayers
    public void CreatePlayers()
    {

        int playerCount = (Gamepad.all.Count > minNumPlayers) ?
            Gamepad.all.Count :
            minNumPlayers;
        for (int i = 0; i < playerCount; ++i)
        {
            players.Add(Instantiate(playerPrefab,
                spawnLocations[i],
                Quaternion.identity,
                transform.parent)
            );

            Transform vis = players[i].transform.Find("Visual");
            vis.gameObject.GetComponent<SpriteRenderer>().color = team_colors[i];
			// attach a team indicator to the player
			//GameObject temp = Instantiate(team_indicators[i % 2], players[i].transform.position + Vector3.up * 1.5f, Quaternion.identity);
			//temp.transform.parent = players[i].transform;
			//temp.GetComponent<SpriteRenderer>().color = team_colors[i];
			players[i].GetComponent<Movement2D_Base>().team_id = (i % 2);
			players[i].GetComponent<Movement2D_Base>().player_id = i + 1;
			// Debug.Log("Players[i].id: " + players[i].GetComponent<Movement2D_Base>().player_id);
			// Debug.Log("From PM: " + players[i].GetComponent<Movement2D_Base>().is_dummy);
            // Debug.Log(vis.gameObject.GetComponent<SpriteRenderer>().color);
        }
        // Debug.Log("Attempting to print players[0]");
        // Debug.Log(players[0]);
    }

	// resets players to spawn location
	public void ResetPlayers()
	{
		for (int a=0; a<players.Count; a++)
		{
			players[a].SetActive(true);
			players[a].transform.position = spawnLocations[a];
		}
	}

	// Removes a player for 10 seconds
	public void RemovePlayer(int index)
	{
		// Debug.Log("Index: " + index + " || Player_Count: " + players.Count);
		players[index].transform.position = spawnLocations[index];
		players[index].GetComponent<Movement2D_Base>().RumbleOff();
		players[index].SetActive(false);
		Respawn(10.0f, index);
	}
	private void Respawn(float time, int index)
	{
        //yield return new WaitForSeconds(time);
        //players[index].SetActive(true);

        // Instantiate RespawnCanvas prefab
        GameObject respawnCanvas = Instantiate(respawnCanvasPrefab);
        RespawnCountdown respawnCountdown = respawnCanvas.GetComponent<RespawnCountdown>();

        respawnCountdown.StartCountdown(time, index, team_colors[index]);

	}

	// freezes/unfreezes players
	public void FreezePlayers()
	{
		//Debug.Log("FREEZING PLAYERS!");
		foreach(GameObject player in players)
		{
			player.GetComponent<Movement2D_Base>().is_dummy = true;
			player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}
	}


    public void UnFreezePlayers()
	{
		//Debug.Log("LETTING GO!");
		foreach (GameObject player in players)
		{
			player.GetComponent<Movement2D_Base>().is_dummy = false;
		}
	}

	// rumbles controller
	public void RumblePlayer(int index)
	{
		players[index].GetComponent<Movement2D_Base>().rumble(.7f);
	}



	public void SetNumPlayer(int num)
    {
        //players = new GameObject[num];
        Debug.Log("Number of players: " + num);
        // checks that there are enough controllers to match
        // the selected number of players
        if (Gamepad.all.Count < num)
        {
            Debug.Log("Not Enough Controllers");
            PrintAllControllers();
            return;
        }
        numPlayers = num;
    }

    /* void CreatePlayersControllers()
    {
        // creates selected amount of prefabs
        for (int i = 0; i < numPlayers; i++)
        {
            players[i] = Instantiate(playerPrefab, transform.parent);
            // int bug_index = i % 2;
            //players[i].SetClass(bug_index);

        }
        AttachControllers();
        //_StartScreenManager.Instance.ActivatePanels();
    }*/

    void PrintAllControllers()
    {
        // check for gamepads
		/*
        Debug.Log("Number of Gamepads found: " + Gamepad.all.Count);
        for (int a = 0; a < Gamepad.all.Count; a++)
        {
            Debug.Log(Gamepad.all[a].id);
            Debug.Log(Gamepad.all[a].name);
        }
		*/
    }

    //attempt to attach controllers to players
    public void AttachControllers()
    {

        // if count, assign controllers

        /* if (Gamepad.all.Count >= players.Count)
        {
            for (int a = 0; a < players.Count; a++)
            {
                Debug.Log("players: " + players[a].name);
                players[a].GetComponent<Movement2D_Base>().AssignController(Gamepad.all[a]);
                //players[a].ActivatePlayer(Gamepad.all[a], a);
            }
        }
        else */
        for (int i = 0; i < players.Count; i++){
            Movement2D_Base ctrl = players[i].GetComponent<Movement2D_Base>();
            if (i < Gamepad.all.Count)
            {
				Debug.Log("Ctrl: " + ctrl.name);
				Debug.Log("GameControls: " + _GameControls.All.name);
				ctrl.AssignController(_GameControls.All.GetGamePad(i));
            }
            else
            {
                ctrl.is_keyboard = true;
                ctrl.keyboard_id = i + 1;
            }
        }
    }

    public int GetNumPlayers()
    {
        return players.Count;
    }

    public GameObject GetPlayer(int playerId)
    {
        if(playerId >= players.Count)
        {
            Debug.LogError("Invalid playerID passed into PlayerManager: " + playerId);
            return null;
        }
        return players[playerId];
    }

}
