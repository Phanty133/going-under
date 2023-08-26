using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
	public GameObject pauseManagerObj;
	public AudioClip clickClip;

	AudioSource audioSource;
	static PauseManager pauseManager;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		pauseManager = pauseManagerObj.GetComponent<PauseManager>();
	}

	public static void Continue()
	{
		pauseManager.UnpauseGame();
		// No need to play click because the manager already plays one when Pausing/Unpausing
	}

	public static void ExitToMain()
	{
		SceneManager.LoadScene("MainMenu");
		pauseManager.PlayClick();
	}

	public static void ExitGame()
	{
		Application.Quit();
		pauseManager.PlayClick();
	}
}
