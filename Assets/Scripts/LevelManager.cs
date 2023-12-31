using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public static bool BossLevel = false;

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
		LevelStatsTracker.ResetStats();
	}

	private void Update()
	{
		levelTimer += Time.deltaTime;
	}

	public static IEnumerator NextLevel(bool skipOffScreen = false)
	{
		// Overrides player controls, makes the player go off-screen, and changes to the next level
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		PlayerControls playerControls = player.GetComponent<PlayerControls>();
		playerControls.inputControl = false;

		if (!skipOffScreen)
		{
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
		}

		Time.timeScale = 0;
		Level++;
		var trackedStats = LevelStatsTracker.LevelStats;
		trackedStats.timeTaken_s = levelTimer;

		passedMenuManager.SetStats(trackedStats);
		passedMenuManager.gameObject.SetActive(true);
	}
}