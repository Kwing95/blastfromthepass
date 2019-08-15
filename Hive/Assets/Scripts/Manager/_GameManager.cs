using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class _GameManager : MonoBehaviour
{

    public GameObject endScreenPrefab;
    public float endScreenDelay = 3.0f;
    public static _GameManager Instance { get { return _instance; } }
    private static _GameManager _instance;

    public static int[] score = new int[] { 0, 0 };
    public int points_to_win = 5;

    private GameObject winCamera;

    private Dynamic_Camera dc;

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
    }



    // Start is called before the first frame update
    void Start()
    {
        winCamera = GameObject.FindGameObjectWithTag("WinCamera");
        winCamera.SetActive(false);

        Time.timeScale = 2.0f;
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            dc = mainCamera.GetComponent<Dynamic_Camera>();
        }
        // _PlayerManager.Instance.CreatePlayers();
    }

    // Update is called once per frame
    void Update()
    {
        // quit build of game
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        // check score for five
        if (score[0] >= points_to_win)
        {
            // some game win text or something here
            //reset_level();
            score[1] = 0;
            score[0] = 0;
            StartCoroutine(VictoryAfterDelay(0));

        }
        if (score[1] >= points_to_win)
        {
            score[1] = 0;
            score[0] = 0;
            // some game win text or something here
            Debug.Log("Points to win: " + points_to_win);
            //reset_level();
            StartCoroutine(VictoryAfterDelay(1));

        }
    }

    public bool GamePoint()
    {

        if (score[0] >= points_to_win  || score[1] >= points_to_win)
        {
            return true;
        }
        return false;
    }


    // reset scene
    private void reset_level()
	{
		Debug.Log("Reset players: Team1: " + score[0] + " || Team2: " + score[1]);
		score[0] = 0;
		score[1] = 0;
		_PlayerManager.Instance.ResetPlayers();

	}

    GameObject endScreen;
    private void LoadEndScreen(int teamId)
    {

        if(dc != null)
        {
            dc.in_control = false;
        }
        winCamera.SetActive(true);
        if (endScreen == null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            endScreen = (GameObject) Instantiate(endScreenPrefab, canvas.transform);
            endScreen.GetComponent<EndScreenController>().SetWinner(teamId);
            StartCoroutine(WaitForButtonActivation());
            print("create end screen");

        }
        print("Show end screen");
        _PlayerManager.Instance.FreezePlayers();

    }

    IEnumerator WaitForButtonActivation()
    {
        yield return new WaitForSecondsRealtime(1.2f);
        endScreen.GetComponent<ButtonSelection>().ActivateSelection();

    }


    public void ReturnToMenu()
    {
        Transitioner.instance.Transition("StartMenu");
        //SceneManager.LoadScene();
    }

    public void ReturnToLevelMenu()
    {
        if(_StateController.inst != null)
        {
            print("This shouldve been called");
            _StateController.inst.wasTutorial = true;
        }
        Transitioner.instance.Transition("StartMenu");

        //SceneManager.LoadScene("StartMenu");
    }


    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        _PlayerManager.Instance.UnFreezePlayers();
        SceneManager.LoadScene(scene.name);

        //if(_GameManager.Instance.endScreen != null)
        //{
        //    print("destroying end screen");
        //    _GameManager.Instance.endScreen.SetActive(false);
        //}
        //reset_level();

    }

    IEnumerator VictoryAfterDelay(int teamId)
    {
        yield return new WaitForSecondsRealtime(endScreenDelay);
        LoadEndScreen(teamId);
    }

}
