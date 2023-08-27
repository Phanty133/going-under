using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public struct LevelPassedStats
{
	public float timeTaken_s;
	public int torpedosFired;
	public int hitsTaken;
	public int sonarsPinged;
	public int subsKilled;
	public int patrolsKilled;
	public int destroyersKilled;
}

public class PassedMenuManager : MonoBehaviour
{
	public GameObject titleObj;
	public GameObject timeTakenObj;
	public GameObject torpedosObj;
	public GameObject hitsObj;
	public GameObject sonarsObj;
	public GameObject subsObj;
	public GameObject patrolsObj;
	public GameObject destroyersObj;
	public GameObject btnObj;

	private void Start()
	{
		if (LevelManager.BossLevel)
		{
			titleObj.GetComponent<TMP_Text>().text = "You have destroyed the enemy carrier!\nThanks for playing.";
			btnObj.GetComponent<TMP_Text>().text = "Back to Main Menu";
		}
		else
		{
			titleObj.GetComponent<TMP_Text>().text = string.Format("Level {0} Passed", LevelManager.Level);
		}
	}

	public void SetStats(LevelPassedStats stats)
	{
		timeTakenObj.GetComponent<TMP_Text>().text = string.Format("{0:0.00} s", stats.timeTaken_s);
		torpedosObj.GetComponent<TMP_Text>().text = stats.torpedosFired.ToString();
		hitsObj.GetComponent<TMP_Text>().text = stats.hitsTaken.ToString();
		sonarsObj.GetComponent<TMP_Text>().text = stats.sonarsPinged.ToString();
		subsObj.GetComponent<TMP_Text>().text = stats.subsKilled.ToString();
		patrolsObj.GetComponent<TMP_Text>().text = stats.patrolsKilled.ToString();
		destroyersObj.GetComponent<TMP_Text>().text = stats.destroyersKilled.ToString();
	}

	public static void OnContinue()
	{
		if (LevelManager.BossLevel)
		{
			SceneManager.LoadScene("MainMenu");
		}
		else
		{
			SceneManager.LoadScene("GameLevel");
		}
	}
}
