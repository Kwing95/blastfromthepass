using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class _LevelManager : MonoBehaviour
{
    public UnityEngine.EventSystems.EventSystem eventSystem;
    private int numLevels;
    private Transform levelPanelTrans;
    public static _LevelManager Instance { get { return _instance; } }
    private static _LevelManager _instance;

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
        levelPanelTrans = transform.Find("LevelPanel");
        if(levelPanelTrans == null)
        {
            Debug.LogError("Can't Find Level Panel");
            return;
        }

        numLevels = levelPanelTrans.childCount;
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("Loading new scene " + sceneName);
        _MenuManager.inst.ChangeMenu("");
        Transitioner.instance.Transition(sceneName);
        //SceneManager.LoadScene(sceneName);
        Destroy(this.gameObject);
    }


}