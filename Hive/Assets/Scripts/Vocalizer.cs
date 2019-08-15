using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class Vocalizer : MonoBehaviour
{

    public List<AudioClip> pass;
    public List<AudioClip> gimme;
    public List<AudioClip> wait;
    public List<AudioClip> taunt;

    private AudioSource source;
    bool canSpeak = true;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayVoice(int type)
    {
        if (canSpeak)
        {
            source.pitch = 0.8f + (0.1f * GetComponent<Movement2D_Base>().player_id);
            switch (type)
            {
                case 0:
                    source.PlayOneShot(pass[Random.Range(0, pass.Count)], SoundLevels.taunts);
                    break;
                case 1:
                    source.PlayOneShot(gimme[Random.Range(0, gimme.Count)], SoundLevels.taunts);
                    break;
                case 2:
                    source.PlayOneShot(wait[Random.Range(0, wait.Count)], SoundLevels.taunts);
                    break;
                case 3:
                    source.PlayOneShot(taunt[Random.Range(0, taunt.Count)], SoundLevels.taunts);
                    break;
            }
            canSpeak = false;
            StartCoroutine(ResetSpeak(3));
        }
        
    }

    IEnumerator ResetSpeak(float time)
    {
        yield return new WaitForSeconds(time);
        canSpeak = true;
    }

}
