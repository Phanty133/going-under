using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public AudioClip clickClip;
	AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		Time.timeScale = 1f;
		LevelManager.ResetLevels();
	}

	public void PlayClick()
	{
		audioSource.PlayOneShot(clickClip);
	}

	public static void ManagerPlayClick()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<MainMenuManager>().PlayClick();
	}

	public static void StartGame()
	{
		SceneManager.LoadScene("TestTestScene");
		ManagerPlayClick();
	}

	public static void QuitGame()
	{
		Application.Quit();
		ManagerPlayClick();
	}

	public static void OpenOptions()
	{
		Debug.Log("choice is an illusion");
		ManagerPlayClick();
	}
}
