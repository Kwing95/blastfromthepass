using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class _TransitionManager : MonoBehaviour
{
    public Text readyText;
    public Text hereWeGoText;
    public float fadeInTime = 2.0f;

    private bool oneDone;

    // Start is called before the first frame update
    void Start()
    {
        oneDone = false;
        Ready();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Ready()
    {
        StartCoroutine(FadeIn(readyText));
    }

    private void HereWeGo()
    {
        StartCoroutine(FadeIn(hereWeGoText));
    }

    private void Done()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    IEnumerator FadeIn(Text text)
    {
        float deltaTime = fadeInTime / 100.0f;
        Debug.LogWarning("deltatime: " + deltaTime.ToString());
        while(text.color.a < 0.99f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + 0.01f);
            yield return new WaitForSecondsRealtime(deltaTime);
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
        if (!oneDone)
        {
            oneDone = true;
            HereWeGo();
        }
        else
        {
            Done();
        }

    }
}
