using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transitioner : MonoBehaviour
{

    public static Transitioner instance;
    public float refreshRate = 0.025f;
    public float resizeSpeed = 0.05f;
    public GameObject wipe;

    private float sizeNeeded;
    private int expanding = 0;
    private string targetScene;
    private float counter = 0;

    // Start is called before the first frame update
    void Start()
    {

        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        float width = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float height = GetComponent<SpriteRenderer>().sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        sizeNeeded = Mathf.Max(worldScreenWidth / width, worldScreenHeight / height);
        sizeNeeded = 1;

        wipe.transform.localScale = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {

        //Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        // Debug.Log(cameras.Length);
        //transform.position = Camera.main.transform.position + new Vector3(0, 0, 10);

        //transform.position = Camera.main.transform.position + new Vector3(0, 0, 10);

        counter += Time.deltaTime;

        if (counter > refreshRate)
        {
            counter = 0;

            if (expanding == 1)
            {
                if (wipe.transform.localScale.x < sizeNeeded)
                {
                    //Debug.Break();
                    // Debug.Log("expanding to " + ((wipe.transform.localScale + Vector3.one) * (1 + resizeSpeed)));
                    wipe.transform.localScale = wipe.transform.localScale + (Vector3.one * resizeSpeed);
                    // Debug.Log("localScale = " + wipe.transform.localScale);
                }
                else
                {
					//_GameManager.Instance.ReturnToLevelMenu();
                    SceneManager.LoadScene(targetScene);
                    expanding = -1;
                }
            }
            else if (expanding == -1)
            {
                if (wipe.transform.localScale.x > 0)
                    wipe.transform.localScale = wipe.transform.localScale - (Vector3.one * resizeSpeed);
                else
                {
                    wipe.transform.localScale = Vector3.zero;
                    expanding = 0;
                }

            }

        }

        //wipeB.wipe.transform.localScale = wipeA.wipe.transform.localScale;

    }

    public void Transition(string scene)
    {
        targetScene = scene;
        expanding = 1;
    }

}
