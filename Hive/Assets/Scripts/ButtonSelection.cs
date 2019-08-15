using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Input;
public class ButtonSelection : MonoBehaviour
{

    private Button[] buttonArray;
    private int buttonIndex = 0;
    private bool allowSelection;
    private float selectionPause = 0.4f;
    private float deadzone = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        buttonIndex = 0;
        FindButtons(gameObject);
        if(buttonArray != null)
        {
            buttonArray[buttonIndex].Select();
        }

        allowSelection = false;
    }

    public void ActivateSelection()
    {
        if (buttonArray == null)
        {
            buttonIndex = 0;
            FindButtons(gameObject);
        }
        if (buttonArray != null)
        {
            buttonArray[buttonIndex].Select();
        }

        StartCoroutine(WaitForActivation());
    }

    IEnumerator WaitForActivation()
    {

        yield return new WaitForSecondsRealtime(0.5f);
        allowSelection = true;

    }

    public void DeactivateSelection()
    {
        allowSelection = false;
    }
    void FindButtons(GameObject panel)
    {
        int numButtons = 0;
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            Button childButton = panel.transform.GetChild(i).GetComponent<Button>();
            if (childButton != null && childButton.gameObject.activeSelf)
            {
                // print(childButton.gameObject);
                numButtons++;
            }
        }
        if(numButtons == 0)
        {
            buttonArray = null;
            return;
        }
        buttonArray = new Button[numButtons];
        int arrayIndex = 0;
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            Button childButton = panel.transform.GetChild(i).GetComponent<Button>();
            if (childButton != null && childButton.gameObject.activeSelf)
            {
                buttonArray[arrayIndex] = childButton;
                arrayIndex++;
            }
        }
    }

    public int GetButtonIndex()
    {
        return buttonIndex;
    }


    IEnumerator WaitForSelection()
    {
        yield return new WaitForSecondsRealtime(selectionPause);
        // print("can selected again");
        allowSelection = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (Gamepad.current == null) return;
        if (!gameObject.activeInHierarchy || !allowSelection) return;
        bool changeSelection = false;

        if (Gamepad.current.leftStick.ReadValue().y > deadzone)
        {
            // print("move up");

            buttonIndex--;
            changeSelection = true;
        }
        else if (Gamepad.current.leftStick.ReadValue().y < -deadzone)
        {
            buttonIndex++;
            changeSelection = true;
        }

        if (changeSelection)
        {
            allowSelection = false;
            StartCoroutine(WaitForSelection());

            if (buttonIndex >= buttonArray.Length)
            {
                buttonIndex = 0;
            }
            else if(buttonIndex < 0)
            {
                buttonIndex = buttonArray.Length - 1;
            }
            buttonArray[buttonIndex].Select();

        }

        if (Gamepad.current.buttonSouth.wasPressedThisFrame && gameObject.activeInHierarchy)
        {
            buttonArray[buttonIndex].onClick.Invoke();

        }
    }

}
