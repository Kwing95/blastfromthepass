using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject team1;
    public GameObject team2;
    public string team1_message;
    public string team2_message;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        team1.GetComponent<Text>().text = team1_message + _GameManager.score[0];
        team2.GetComponent<Text>().text = team2_message + _GameManager.score[1]; 
    }
}
