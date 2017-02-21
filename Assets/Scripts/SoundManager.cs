using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ListSampling;

public class SoundManager : MonoBehaviour
{

	public AudioSource efxSource;
	public AudioSource musicSource;
	public static SoundManager instance = null;

	private float lowPitchRange = 0.95f;
	private float highPitchRange = 1.05f;

	void Awake ()
	{

		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
		
	}

	public void PlaySingle (AudioClip clip)
	{
		efxSource.clip = clip;
		efxSource.Play ();
	}

	public void RandomSfx(params AudioClip[] clips)
	{
		AudioClip randomClip = clips.SampleFrom();
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);

		efxSource.pitch = randomPitch;
		efxSource.clip = randomClip;
		efxSource.Play ();
	}	
}
