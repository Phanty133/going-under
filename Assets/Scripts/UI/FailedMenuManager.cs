using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FailedMenuManager : MonoBehaviour
{
	public GameObject titleObj;

	private void Start()
	{
		titleObj.GetComponent<TMP_Text>().text = string.Format("Level {0} Passed", LevelManager.Level);
	}

	public static void OnRetry()
	{
		SceneManager.LoadScene("GameLevel");
	}

	public static void OnGiveUp()
	{
		SceneManager.LoadScene("MainMenu");
		Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
	}
}
