using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Input;

public class _TutorialManager : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;
    public Camera camera3;
    public Camera camera4;
    public Vector3Int cameraOffsetFromPlayer = new Vector3Int(0, 0, -5);

    public GameObject canvas3;
    public GameObject canvas4;

    public Vector3[] spawnLocations = new[] {
        new Vector3(-32f, 13.7f, 0f),
        new Vector3(-32f, 0.7f, 0f),
        new Vector3(3f, 13.7f, 0f),
        new Vector3(3f, 0.7f, 0f)
    };

    public GameObject playerPrefab;
    public int minNumPlayers = 2;
    int numPlayers { get; set; }
    public List<GameObject> players;
    static public Color[] team_colors = new[] {
        Color.red,
        Color.green,
        Color.magenta,
        Color.cyan
    };
    public GameObject[] team_indicators = new GameObject[2];

    private int numDone;

    public static _TutorialManager Instance { get { return _instance; } }
    private static _TutorialManager _instance;

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

        CreatePlayers();
        SetNumPlayer(players.Count);
        AttachControllers();
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 2.0f;

        numDone = 0;

        //Move players to respective spaces
        int numPlayers = GetNumPlayers();
        for (int i = 0; i < numPlayers; i++)
        {
            players[i].transform.position = spawnLocations[i];
        }

        // Get rid of cameras if necessary
        if (numPlayers < 3)
        {
            // Get rid of cameras 3 and 4
            Destroy(camera3);
            Destroy(camera4);

            // Get rid of canvases 3 and 4
            Destroy(canvas3);
            Destroy(canvas4);

            // Adjust viewport rect width of cameras 1 and 2
            Rect rect1 = new Rect(0.0f, 0.515f, 1.0f, 1.0f);
            Rect rect2 = new Rect(0.0f, 0.0f, 1.0f, 0.485f);
            camera1.rect = rect1;
            camera2.rect = rect2;
        }

        // Have cameras follow players
        for (int i = 0; i < numPlayers; i++)
        {
            Camera camera;
            GameObject targetPlayer;
            Vector3Int offset = cameraOffsetFromPlayer;
            switch (i)
            {
                case 0:
                    camera = camera1;
                    targetPlayer = players[0];
                    break;
                case 1:
                    camera = camera2;
                    targetPlayer = players[1];
                    break;
                case 2:
                    camera = camera3;
                    targetPlayer = players[2];
                    break;
                case 3:
                    camera = camera4;
                    targetPlayer = players[3];
                    break;
                default:
                    // won't go here, this just gets rid of warnings
                    camera = camera1;
                    targetPlayer = players[0];
                    break;
            }

            FollowTarget followTarget = camera.gameObject.GetComponent<FollowTarget>();
            followTarget.target = targetPlayer;
            followTarget.offset = offset;
            followTarget.enabled = true;

        }
    }

    public void PlayerDone()
    {
        numDone++;
    }

    // Update is called once per frame
    void Update()
    {
        if (numDone >= GetNumPlayers())
        {
            SceneManager.LoadScene("BugbyScene");
        }
    }

    //******************************************************************************************************************

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
            GameObject temp = Instantiate(team_indicators[i % 2], players[i].transform.position + Vector3.up * 1.5f, Quaternion.identity);
            temp.transform.parent = players[i].transform;
            temp.GetComponent<SpriteRenderer>().color = team_colors[i];
            players[i].GetComponent<Movement2D_Base>().team_id = (i % 2) + 1;
            players[i].GetComponent<Movement2D_Base>().player_id = i + 1;

            // Debug.Log(vis.gameObject.GetComponent<SpriteRenderer>().color);
        }
        // Debug.Log("Attempting to print players[0]");
        // Debug.Log(players[0]);
    }

    // resets players to spawn location
    public void ResetPlayers()
    {
        for (int a = 0; a < players.Count; a++)
        {
            players[a].SetActive(true);
            players[a].transform.position = spawnLocations[a];
        }
    }

    // Removes a player for 10 seconds
    public void RemovePlayer(int index)
    {
        players[index].transform.position = spawnLocations[index];
        players[index].SetActive(false);
        StartCoroutine(Respawn(10.0f * Time.timeScale, index));
    }
    IEnumerator Respawn(float time, int index)
    {
        yield return new WaitForSeconds(time);
        players[index].SetActive(true);
    }

    // freezes/unfreezes players
    public void FreezePlayers()
    {
        //Debug.Log("FREEZING PLAYERS!");
        foreach (GameObject player in players)
        {
            player.GetComponent<Movement2D_Base>().is_dummy = true;
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
        Debug.Log("Number of Gamepads found: " + Gamepad.all.Count);
        for (int a = 0; a < Gamepad.all.Count; a++)
        {
            Debug.Log(Gamepad.all[a].id);
            Debug.Log(Gamepad.all[a].name);
        }
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
        for (int i = 0; i < players.Count; i++)
        {
            Movement2D_Base ctrl = players[i].GetComponent<Movement2D_Base>();
            if (i < Gamepad.all.Count)
            {
                ctrl.AssignController(Gamepad.all[i]);
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
        if (playerId >= players.Count)
        {
            Debug.LogError("Invalid playerID passed into PlayerManager: " + playerId);
            return null;
        }
        return players[playerId];
    }
}
