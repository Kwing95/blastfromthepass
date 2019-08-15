using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]

public class UI_NextWave : MonoBehaviour
{
    private Text nextWaveText;

    // Start is called before the first frame update
    void Start()
    {
        nextWaveText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        /* Commented out for compiling -> also singleton functions are obsolete
        int minutes;
        float seconds;


        float nextWaveTime = _LevelManager.Instance.GetNextWaveTime();
        float total_seconds = nextWaveTime - Time.time;
        minutes = Mathf.FloorToInt(total_seconds / 60);
        seconds = total_seconds - minutes;
        nextWaveText.text = minutes.ToString() + ":" + seconds.ToString("F2");
        */
    }
}
