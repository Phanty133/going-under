using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public AudioClip clickClip;
	public GameObject settingsMenu;
	public GameObject mainMenu;
	AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		Time.timeScale = 1f;
		LevelManager.ResetLevels();

		settingsMenu.SetActive(false);
		mainMenu.SetActive(true);
	}

	public void PlayClick()
	{
		audioSource.PlayOneShot(clickClip, MainSettings.sfxVolume);
	}

	public static void ManagerPlayClick()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<MainMenuManager>().PlayClick();
	}

	public static void StartGame()
	{
		SceneManager.LoadScene("GameLevel");
		ManagerPlayClick();
	}

	public static void QuitGame()
	{
		Application.Quit();
		ManagerPlayClick();
	}

	public static void OpenOptions()
	{
		MainMenuManager mainMenuManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainMenuManager>();
		mainMenuManager.settingsMenu.SetActive(true);
		mainMenuManager.mainMenu.SetActive(false);
		ManagerPlayClick();
	}
}
