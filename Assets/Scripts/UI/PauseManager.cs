using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
	public GameObject pauseMenuObj;
	public AudioClip clickClip;
	AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void PlayClick()
	{
		audioSource.PlayOneShot(clickClip);
	}

	public void OnPause()
	{
		if (Time.timeScale == 0)
		{
			UnpauseGame();
		}
		else
		{
			PauseGame();
		}
	}

	public void PauseGame()
	{
		Time.timeScale = 0;
		pauseMenuObj.SetActive(true);
		PlayClick();
	}

	public void UnpauseGame()
	{
		Time.timeScale = 1;
		pauseMenuObj.SetActive(false);
		PlayClick();
	}
}
