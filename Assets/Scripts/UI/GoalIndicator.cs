using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalIndicator : MonoBehaviour
{
	public GameObject goal;

	GameObject player;
	GameObject boss;
	bool bossColorSet = false;

	bool TryFindPlayer()
	{
		player = GameObject.FindGameObjectWithTag("Player");

		return player != null;
	}

	bool TryFindBoss()
	{
		boss = GameObject.FindGameObjectWithTag("Boss");

		return boss != null;
	}


	private void Start()
	{
		TryFindPlayer();
		TryFindBoss();
	}

	private void PointTowardsGoal()
	{
		Vector2 target = LevelManager.BossLevel ? boss.transform.position : goal.transform.position;
		Vector2 goalDir = target - (Vector2)player.transform.position;
		float goalAngle = Mathf.Atan2(goalDir.y, goalDir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0, 0, goalAngle);
	}

	private void Update()
	{
		if (player == null)
		{
			bool plyrFound = TryFindPlayer();
			if (!plyrFound) return;
		}

		if (LevelManager.BossLevel)
		{
			if (!bossColorSet)
			{
				GetComponentInChildren<RawImage>().color = Color.red;
				bossColorSet = true;
			}

			if (boss == null)
			{
				bool bossFound = TryFindBoss();
				if (!bossFound) return;
			}
		}

		PointTowardsGoal();
	}
}
