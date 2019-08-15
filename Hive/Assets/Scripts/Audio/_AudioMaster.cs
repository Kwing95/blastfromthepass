using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class _AudioMaster : MonoBehaviour
{
	public static _AudioMaster inst;
	// REMEMBER: order matters here
	private AudioSource aus;
	public AudioClip[] tracks;
	 
	private int[] beats = { 64, 83 }; // the non-intro arcade track has a lot of off-beats
	private float[] offsets = { 0.01f, 0.30f, 0.20f, 0.20f };
	public int bpm = 0;
	public float offset = 0.0f;

	// cheering
	public float cheerTime = 3f;
	public AudioSource cheer_source;

	// play in order
	private int music_index = 0;

	// inactive
	public bool inactive = true;


	// init vars
	void Awake()
	{
		// enforce singleton
		if (!inst) inst = this;
		else if (inst != this) Destroy(this);

		aus = GetComponent<AudioSource>();
        cheer_source.volume = SoundLevels.effects;
		// tracks = Resources.LoadAll<AudioClip>("Background_Music");
		
		//new_track();
		//debug_track();
	}

    public void ArtificialStart()
    {
        // wait a second before playing
        StartCoroutine(Wait(1.0f));
    }

	// update volume
	private void Update()
	{
		//aus.volume = Audio_Manager.inst.get_volume();
		if (inactive) return;
		if (!aus.isPlaying) in_order();
	}

	// play tracks in order
	private void in_order()
	{
		//music_index++;
		aus.clip = tracks[music_index];
		bpm = beats[music_index];
		offset = offsets[music_index];
        aus.volume = SoundLevels.music;
		aus.Play();
		// Debug.Log("Playing: " + tracks[music_index].name + " || bpm: " + bpm + " || offset: " + offset);
		music_index = ++music_index % tracks.Length;
	}

	// randomly choose a track and attach it to the audiosource
	public void new_track()
	{
		int index = Random.Range(0, tracks.Length);
		aus.clip = tracks[index];
		bpm = beats[index];
		offset = offsets[index];
		aus.Play();
		// aus.volume = Audio_Manager.inst.get_volume();
		Debug.Log("Playing: " + tracks[index].name + " || bpm: " + bpm + " || offset: " + offset);
	}

	// for debugging
	public void debug_track()
	{
		int index = 2;
		aus.clip = tracks[index];
		bpm = beats[index];
		offset = offsets[index];
		aus.Play();
		Debug.Log("Playing: " + tracks[index].name + " || bpm: " + bpm + " || offset: " + offset);
	}

	public void ScoreSound()
	{
		// scoreCheerClip.Play();
		StartCoroutine(playfortime(cheerTime, cheer_source));
	}

	IEnumerator playfortime(float time, AudioSource sound)
	{
		float soundFadeoutTime = 0.1f;
		float soundFadeoutValue = 0.1f;
		sound.Play();
		sound.volume = .4f;
		yield return new WaitForSeconds(time);
		while (sound.volume > 0)
		{
			yield return new WaitForSeconds(soundFadeoutTime);
			sound.volume -= soundFadeoutValue;
		}
		sound.Stop();
	}

	// wait to start
	IEnumerator Wait(float time)
	{
		yield return new WaitForSeconds(time);
		in_order();
		aus.Play();
		inactive = false;
	}
}
