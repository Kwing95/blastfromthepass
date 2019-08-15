using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]

public class UI_EggCount : MonoBehaviour
{
    private Text eggCountText;

    // Start is called before the first frame update
    void Start()
    {
        eggCountText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //eggCountText.text = _GameManager.Instance.GetEggCount().ToString();
    }
}
