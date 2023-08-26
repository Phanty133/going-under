using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static int Level
	{
		get => _level;
		private set => _level = value;
	}

	static int _level = 0;
	public float levelTimer = 0f;

	private void Start()
	{
		Debug.Log(string.Format("Level {0}", Level));

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

		while (playerOnScreen)
		{
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

			yield return null;
		}

		Level++;
		SceneManager.LoadScene("GameLevel");
	}
}
