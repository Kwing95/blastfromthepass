using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Input;

public class _StartScreenManager : MonoBehaviour
{
    public Canvas canvas;
    public PlayerController playerPrefab;
    public UnityEngine.EventSystems.EventSystem eventSystem;

    private float startScreenTransitionTime = 3.3f;
    private PlayerController[] players;
    private int num_menus;
    private GameObject activationPanel;
    private GameObject startPanel;
    private GameObject creditsPanel;
    private GameObject levelPanel;
    private GameObject transitionPanel;



    public static _StartScreenManager Instance { get { return _instance; } }
    private static _StartScreenManager _instance;

    //private bool isStartButtonMenu = false, isActivationPanel = false;
    //private bool isSettingPanel = false, isLevelPanel = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public PlayerController GetPlayerController(int index)
    {
        return players[index];
    }

    GameObject GetPanel(string panelName)
    {
        Transform trans = canvas.transform.Find(panelName);
        if (trans == null)
        {
            Debug.Log("Can't Find " + panelName);
            return null;
        }
        return trans.gameObject;
    }
    // Show the Start Button Panel in the beginning of the scene
    void Start()
    {

        activationPanel = GetPanel("ActivationPanel");

        startPanel = GetPanel("StartPanel");

        levelPanel = GetPanel("LevelPanel");

        creditsPanel = GetPanel("CreditsPanel");

        transitionPanel = GetPanel("TransitionPanel");
        if(transitionPanel != null)
        {
            transitionPanel.SetActive(false);
        }

        num_menus = transform.childCount;

        ResetCamera();
		Debug.Log("_StateController Tutorial: " + _StateController.inst.wasTutorial);
        if (_StateController.inst.wasTutorial)
        {
            _StateController.inst.wasTutorial = false;

            _MenuManager.inst.ChangeMenu("LevelPanel");
            ActivateSelection(levelPanel);
        }
        else 
        {
            ShowStartPanel();
        }

        Debug.Log("Starts");

    }


    void ResetCamera()
    {

        float zPosition = Camera.main.transform.position.z;

        Camera.main.transform.position = new Vector3(0f, 0f, zPosition);

    }

    // Update is called once per frame
    void Update()
    {
        // quit build of game
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (Gamepad.current == null)
        {
            return;
        }
       
        // Will transition to the player activation screen when someone
        // presses the start button. 
            if (startPanel.activeInHierarchy && Gamepad.current.startButton.wasPressedThisFrame)
        {
            // Only allow player to start the game if there are more than one 
            // controller activated
            if (Gamepad.all.Count > 1)
            {
                OnStartButton();
            }
        }
    }


  

    // Once someone presses start, we will create create players based off
    // of the number of controllers that are activated
    public void OnStartButton()
    {
        ShowActivationPanel();
    }
    public void ShowStartPanel()
    {

        Camera.main.transform.position = new Vector3(0f,0f, Camera.main.transform.position.z);
        CreatePlayersControllers();
        //

        //StartCoroutine(ActivateStartPanel());
        _MenuManager.inst.ChangeMenu("StartPanel");
        ActivateSelection(startPanel);



        //eventSystem.SetSelectedGameObject(startPanel.transform.Find("StartButton").gameObject);

    }
    IEnumerator ActivateStartPanel()
    {
        yield return StartCoroutine(Transition("StartPanel"));
        ActivateSelection(startPanel);
    }


    void DeactivateSelection()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform panel = transform.GetChild(i);
            ButtonSelection selectionControls = panel.GetComponent<ButtonSelection>();
            if (selectionControls != null)
            {
                selectionControls.DeactivateSelection();
            }
        }
    }

    void ActivateSelection(GameObject panel)
    {
        DeactivateSelection();
        ButtonSelection selectionControls = panel.GetComponent<ButtonSelection>();
        if (selectionControls != null)
        {
            selectionControls.ActivateSelection();
        }
    }

    // Wait for players to all confirm 
    void ShowActivationPanel()
    {
        // TODO : COMMENT THIS CHECK BACK IN WHEN SUBMITTING
        //if (Gamepad.all.Count < 4)
        //{
        //    Debug.LogError("Not enough controllers");
        //    return;
        //}

        Camera.main.transform.position = new Vector3(500f, 500f, Camera.main.transform.position.z);

        StartCoroutine(EnablePlayerControls());
        StartCoroutine(ActivateActivationPanel());

    }

    IEnumerator ActivateActivationPanel()
    {
        print("Doing the transition");
        if(transitionPanel != null)
        {
            transitionPanel.SetActive(true);
            TransitionController transitionController = transitionPanel.GetComponent<TransitionController>();
            transitionController.playTransition(startScreenTransitionTime);
            yield return new WaitForSecondsRealtime(2f);
        }
       
        _MenuManager.inst.ChangeMenu("ActivationPanel");
        ActivatePanels();


        if(transitionPanel != null)
        {
            yield return new WaitForSecondsRealtime(2.5f);
            transitionPanel.SetActive(false);
        }
    }


    IEnumerator Transition(string panel)
    {
        if(transitionPanel != null)
        {
            print("Doing the transition");
            transitionPanel.SetActive(true);
            TransitionController transitionController = transitionPanel.GetComponent<TransitionController>();
            transitionController.playTransition(startScreenTransitionTime);
            yield return new WaitForSecondsRealtime(1f);
        }

        _MenuManager.inst.ChangeMenu(panel);

        if(transitionPanel != null)
        {
            yield return new WaitForSecondsRealtime(.5f);
            transitionPanel.SetActive(false);
        }
    }

    public void ActivatePlayerPanel(int id)
    {
        activationPanel.GetComponent<PlayerActivationPanel>().TogglePlayerPanel(id, true);
    }
    public void DeactivatePlayerPanel(int id)
    {
        activationPanel.GetComponent<PlayerActivationPanel>().TogglePlayerPanel(id, false);
    }
    public void ActivatePanels() 
    {
        print("Activiating four panels from start screen manager");
        activationPanel.GetComponent<PlayerActivationPanel>().ActivatePanels(4);
    }

    public void SwitchOptions(int playerId, bool moveRight)
    {
        activationPanel.GetComponent<PlayerActivationPanel>().SwitchOptions(playerId, moveRight);

    }


    void CreatePlayersControllers()
    {
        if (players != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                Destroy(players[i].gameObject);
            }
        }
       

        players = new PlayerController[_GameControls.All.Count()];
        // creates selected amount of prefabs
        for (int i = 0; i < _GameControls.All.Count(); i++)
        {
            players[i] = Instantiate(playerPrefab, transform.parent);
        }
        AttachControllers();
    }

    void AttachControllers()
    {
        activationPanel.GetComponent<PlayerActivationPanel>().DestroyPlayers();

        for (int a = 0; a < players.Length; a++)
        {
            Debug.Log("players: " + players[a].name);
            players[a].CreatePlayer(Gamepad.all[a], a);
        }
       
    }
    public int GetNumPlayers()
    {
        return players.Length;
    }


    public void DisablePlayerControls()
    {
        if (players == null) return;
        print("Disable player controls");
        for (int i = 0; i < players.Length; i++)
        {
            players[i].DisableControls();
        }
    }


    public IEnumerator EnablePlayerControls()
    {
        print("Enabling player contorls");
        yield return new WaitForSecondsRealtime(.5f);
        EnableControls();

    }
    public void EnableControls()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].EnableControls();
        }
    }

    public void LevelSelection()
    {
        Camera.main.transform.position = new Vector3(0f, 0f, Camera.main.transform.position.z);

        DisablePlayerControls();

        StartCoroutine(ActivateLevelPanel());
    }

    IEnumerator ActivateLevelPanel()
    {
        yield return StartCoroutine(Transition("LevelPanel"));
        ActivateSelection(levelPanel);
    }

    public void ShowCreditsPanel()
    {
        StartCoroutine(ActivateCreditPanel());
        //eventSystem.SetSelectedGameObject(creditsPanel.transform.Find("BackButton").gameObject);
    }
    IEnumerator ActivateCreditPanel()
    {
        yield return StartCoroutine(Transition("CreditsPanel"));

        Transform creditsTrans = creditsPanel.transform.Find("Credits");
        if (creditsTrans != null)
        {
            creditsTrans.GetComponent<AutomaticScroll>().StartScroll();
        }
        ActivateSelection(creditsPanel);
    }

}

