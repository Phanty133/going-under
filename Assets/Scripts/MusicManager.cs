using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public float ambientFadeIn_s = 5f;
	public float ambientFadeOut_s = 0.5f;
	public float battleFadeIn_s = 1f;
	public float battleFadeOut_s = 5f;
	public float ambientVolume = 0.5f;
	public float battleVolume = 0.5f;

	public List<AudioClip> ambientMusic;
	public List<AudioClip> battleMusic;

	public bool PlayingBattle
	{
		get => _playingBattle;
		private set => _playingBattle = value;
	}
	public bool PlayingAmbient
	{
		get => _playingAmbient;
		private set => _playingAmbient = value;
	}

	public int? Track
	{
		get => track.Value;
		private set => track = value;
	}

	bool _playingBattle = false;
	bool _playingAmbient = false;
	int? track = null;
	AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.volume = 0;
	}

	private IEnumerator FadeOut(float fadeTime_s)
	{
		Debug.Log(string.Format("Stopping {0}", audioSource.clip.name));
		float startVol = audioSource.volume;

		while (audioSource.volume > 0)
		{
			audioSource.volume -= startVol * Time.deltaTime / fadeTime_s;
			yield return null;
		}

		audioSource.Stop();
		audioSource.volume = 0;
	}

	private IEnumerator FadeIn(AudioClip clip, float fadeTime_s, float targetVolume)
	{
		Debug.Log(string.Format("Playing {0}", clip.name));
		audioSource.clip = clip;
		audioSource.Play();
		audioSource.volume = 0;

		while (audioSource.volume <= targetVolume)
		{
			audioSource.volume += targetVolume * Time.deltaTime / fadeTime_s;
			yield return null;
		}

		audioSource.volume = targetVolume;
	}

	public IEnumerator PlayBattle(int? track_override = null)
	{
		if (!PlayingBattle)
		{
			if (PlayingAmbient) yield return StopAmbient();

			PlayingBattle = true;

			if (track_override == null)
			{
				Track = Random.Range(0, battleMusic.Count);
			}
			else
			{
				Track = track_override;
			}

			yield return FadeIn(battleMusic[Track.Value], battleFadeIn_s, battleVolume);
		}
	}

	public IEnumerator StopBattle()
	{
		if (PlayingBattle)
		{
			PlayingBattle = false;
			yield return FadeOut(battleFadeOut_s);
		}
	}

	public IEnumerator PlayAmbient(int? track_override = null)
	{
		if (!PlayingAmbient)
		{
			if (PlayingBattle) yield return StopBattle();

			PlayingAmbient = true;

			if (track_override == null)
			{
				Track = Random.Range(0, ambientMusic.Count);
			}
			else
			{
				Track = track_override;
			}

			yield return FadeIn(ambientMusic[Track.Value], ambientFadeIn_s, ambientVolume);
		}
	}

	public IEnumerator StopAmbient()
	{
		if (PlayingAmbient)
		{
			PlayingAmbient = false;
			yield return FadeOut(ambientFadeOut_s);
		}
	}
}
