using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public GameObject levelPassedObj;

	public static int Level
	{
		get => _level;
		private set => _level = value;
	}

	static int _level = 0;
	static PassedMenuManager passedMenuManager;
	static float levelTimer = 0f;

	public static void ResetLevels()
	{
		Level = 0;
	}

	private void Start()
	{
		Debug.Log(string.Format("Level {0}", Level));
		passedMenuManager = levelPassedObj.GetComponent<PassedMenuManager>();

		Time.timeScale = 1f;
		levelTimer = 0;
	}

	private void Update()
	{
		levelTimer += Time.deltaTime;
	}

	public static IEnumerator NextLevel()
	{
		// Overrides player controls, makes the player go off-screen, and changes to the next level
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		PlayerControls playerControls = player.GetComponent<PlayerControls>();
		playerControls.inputControl = false;

		// Move the player off-screen
		bool playerOnScreen = true;
		float thresh = 0.1f;
		float TIMEOUT = 7f; // # of seconds to wait until forcing the next level dialog to appear
		float timer = 0f;

		while (playerOnScreen)
		{
			if (timer > TIMEOUT)
			{
				Debug.LogWarning("Player off-screen timeout");
				break;
			}

			Vector2 plyrScreenPos = Camera.main.WorldToViewportPoint(player.transform.position);
			if (
				plyrScreenPos.x < -thresh
				|| plyrScreenPos.x > 1 + thresh
				|| plyrScreenPos.y < -thresh
				|| plyrScreenPos.y > 1 + thresh
			)
			{
				playerOnScreen = false;
			}
			else
			{
				playerControls.SetPos(player.transform.position + 2.5f * Time.deltaTime * player.transform.up);
			}

			timer += Time.deltaTime;
			yield return null;
		}

		Time.timeScale = 0;
		Level++;
		passedMenuManager.SetStats(new LevelPassedStats()
		{
			timeTaken_s = levelTimer,
			torpedosFired = 5,
			hitsTaken = 1,
			sonarsPinged = 420,
			subsKilled = 2,
			patrolsKilled = 3,
			destroyersKilled = 4
		});
		passedMenuManager.gameObject.SetActive(true);
	}
}
