using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _SoundManager : MonoBehaviour
{
    private AudioSource music1;
    private AudioSource music2;
    private AudioSource music3;
    private AudioSource music4;
    private AudioSource music5;
    private AudioSource scoreCheerClip;
    public float cheerTime = 3f;

    private int background;
    private AudioSource current;

    public static _SoundManager Instance { get { return _instance; } }
    private static _SoundManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] allAudio = GetComponents<AudioSource>();
        music1 = allAudio[0];
        music2 = allAudio[1];
        music3 = allAudio[2];
        music4 = allAudio[3];
        music5 = allAudio[4];
        scoreCheerClip = allAudio[5];
        music1.Play();

        background = 1;
        current = music1;
    }

    // Update is called once per frame
    void Update()
    {
        // For testing //////////////////
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    ToggleBackground();
            
        //}
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    ScoreSound();
        //}
        /////////////////////////////////

        if (!current.isPlaying)
        {
            ToggleBackground();
        }
    }

    private void ToggleBackground()
    {
        background++;
        if (background > 5)
        {
            background = 1;
        }

        switch (background)
        {
            case 1:
                music5.Stop();
                music1.Play();
                current = music1;
                break;
            case 2:
                music1.Stop();
                music2.Play();
                current = music2;
                break;
            case 3:
                music2.Stop();
                music3.Play();
                current = music3;
                break;
            case 4:
                music3.Stop();
                music4.Play();
                current = music4;
                break;
            case 5:
                music4.Stop();
                music5.Play();
                current = music5;
                break;

        }
    }

    public void ScoreSound()
    {
        // scoreCheerClip.Play();
        StartCoroutine(playfortime(cheerTime, scoreCheerClip));
    }

    IEnumerator playfortime(float time, AudioSource sound)
    {
        float soundFadeoutTime = 0.1f;
        float soundFadeoutValue = 0.1f;
        sound.Play();
        sound.volume = 1;
        yield return new WaitForSeconds(time);
        while (sound.volume > 0)
        {
            yield return new WaitForSeconds(soundFadeoutTime);
            sound.volume -= soundFadeoutValue;
        }
        sound.Stop();
    }
}
