using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Input;
public class PlayerPanelController : MonoBehaviour
{


    private float buttonYPosition = 3.44f;
    public float groundYPosition = -4f;
    private Camera playerCamera;
    private bool tutorial = false;
    private bool playerActivated;
    private Image image;
    private float movementScale = 0.2f;

    private OptionSelection optionSelection;

    private Vector3 cameraPosition = new Vector3(0f, -4f, -10f);

    // Start is called before the first frame update
    void Start()
    {
    }

    public void CanActivate(Vector2 position, Vector2 windowSize)
    {
        print("can activate panel");
        playerCamera = transform.Find("Background").Find("Camera").GetComponent<Camera>();
        playerCamera.rect = new Rect(position, windowSize);
        playerCamera.transform.localPosition = cameraPosition;

        optionSelection = transform.Find("Background").Find("Options").GetComponent<OptionSelection>();

        //Transform imageChild = transform.Find("image");
        //if (imageChild != null)
        //{
        //    image = imageChild.GetComponent<Image>();
        //    image.enabled = true;
        //    image.color = Color.white;
        //    image.sprite = unactivedSprite;
        //    playerCamera.gameObject.SetActive(false);
        DeactivatePanel();
        playerActivated = false;
    }


    public void ToggleActivation(int id)
    {
        Debug.Log("Toggle player panel");
        playerActivated = !playerActivated;

        if (playerActivated)
        {
            //image.sprite = null;
            ////image.sprite = activedSprite;
            //image.color = Color.clear;
            //playerCamera.gameObject.SetActive(true);
            ////image.color = _PlayerManager.team_colors[id];

            ActivatePanel();
            tutorial = (optionSelection.GetOption() == "yes") ? true : false;

        }
        else
        {

            DeactivatePanel();
            tutorial = false;
            //image.sprite = unactivedSprite;
            //image.color = Color.white;
            //playerCamera.gameObject.SetActive(false);

        }
    }

    public void SwitchOption(bool moveRight)
    {
      
        optionSelection.SwitchOption(moveRight);
    }

    public void DeactivatePanel() 
    {
        float xPosition = playerCamera.transform.localPosition.x;
        float zPosition = playerCamera.transform.localPosition.z;
        //StartCoroutine(MoveCamera(buttonYPosition));
        StartCoroutine(MoveCamera(buttonYPosition));
        //playerCamera.transform.localPosition = new Vector3(xPosition, buttonYPosition, zPosition);
        print("player deactivated");
        //playerCamera.gameObject.SetActive(true);
    }

    public bool GetOption()
    {
        return tutorial;

    }
    public void ActivatePanel()
    {
        float xPosition = playerCamera.transform.localPosition.x;
        float zPosition = playerCamera.transform.localPosition.z;




        StartCoroutine(MoveCamera(groundYPosition));
        //playerCamera.transform.localPosition = new Vector3(xPosition, groundYPosition, zPosition);
        //playerCamera.gameObject.SetActive(true);
    }


    private IEnumerator MoveCamera(float newPosition)
    {
        print("moving");
        float yPosition = playerCamera.transform.localPosition.y;
        float xPosition = playerCamera.transform.localPosition.x;
        float zPosition = playerCamera.transform.localPosition.z;

        float direction = -Mathf.Sign(yPosition - newPosition);
                          

        while (Mathf.Sign(yPosition - newPosition) * direction < 0)
        {
            yPosition += direction * movementScale;
            playerCamera.transform.localPosition = new Vector3(xPosition, yPosition, zPosition);
            //yPosition = playerCamera.transform.localPosition.y;

            yield return new WaitForSecondsRealtime(0.01f);
        }


    }


}
