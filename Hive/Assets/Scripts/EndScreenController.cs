using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EndScreenController : MonoBehaviour
{
    //public GameObject one, two;

    Text text;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void SetWinner(int teamId)
    {

        //teamId++;
        //players

        // team 1 : 0 - 0, 2
        // team 2 : 1 - 1, 3
        float xPosition = 907f;
        float yPosition = 15f;

        for (int i = 0; i < _PlayerManager.Instance.players.Count; i++)
        {
            if(i % 2 == teamId) 
            {
                GameObject winner = _PlayerManager.Instance.players[i];
                //winner.GetComponent<Movement2D_Base>().freeze_player = true;
                winner.transform.position = new Vector3(xPosition, yPosition, 0f);
                xPosition += 10.5f;
            }

        }


    }
}
