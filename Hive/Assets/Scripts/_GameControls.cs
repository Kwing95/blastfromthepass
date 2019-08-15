using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;


public class _GameControls : MonoBehaviour
{


    public Gamepad[] controls { get; set; }
    public static _GameControls All { get { return _all; } }
    private static _GameControls _all;
    

    //private bool isStartButtonMenu = false, isActivationPanel = false;
    //private bool isSettingPanel = false, isLevelPanel = false;

    private void Awake()
    {
        if (_all != null && _all != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _all = this;
            DontDestroyOnLoad(this);
        }
        ReloadGamePad();
    }

    private void Start()
    {
		Time.timeScale = 2.0f;
    }

    public int ReloadGamePad()
    {
		Debug.Log("_GAMECONTROL SET UP: " + Gamepad.all.Count);
        controls = new Gamepad[Gamepad.all.Count];
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            controls[i] = Gamepad.all[i];
        }
        return controls.Length;
    }

    public int Count()
    {
        return controls.Length;
    }

    public Gamepad GetGamePad(int id)
    {
        return controls[id];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
