using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class PlayerController : MonoBehaviour
{

    Gamepad gamePad;
    int id;

    private bool isPlayerCreated = false;
    private bool isPlayerActivated = false;
    private bool isControls = false;
    private float deadzone = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayerCreated || !isControls)
        {
            return;
        }
		// used for start menu
		if (gamePad.buttonSouth.wasPressedThisFrame && !isPlayerActivated)
		{
		    isPlayerActivated = !isPlayerActivated;
		    _StartScreenManager.Instance.ActivatePlayerPanel(id);
		}
        else if (gamePad.buttonEast.wasPressedThisFrame && isPlayerActivated)
        {
            isPlayerActivated = !isPlayerActivated;
            _StartScreenManager.Instance.DeactivatePlayerPanel(id);
        }
        else if (gamePad.leftStick.ReadValue().x > deadzone)
        {
            _StartScreenManager.Instance.SwitchOptions(id, false);

        }
        else if (gamePad.leftStick.ReadValue().x < -deadzone)
        {
            _StartScreenManager.Instance.SwitchOptions(id, true);

        }
    }

    public Gamepad GetGamepad()
    {
        return gamePad;
    }


    public void CreatePlayer(Gamepad gp, int playerID)
    {
        gamePad = gp;
        id = playerID;
        isPlayerCreated = true;
        isControls = false;
        // Debug.Log("Activated Player: " + id);
    }

    public void DisableControls()
    {
        isControls = false;
    }
    public void EnableControls() 
    {
        isPlayerActivated = false;
        isControls = true;

    }

}
