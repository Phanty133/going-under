using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalIndicator : MonoBehaviour
{
	public GameObject goal;

	GameObject player;

	bool TryFindPlayer()
	{
		player = GameObject.FindGameObjectWithTag("Player");

		return player != null;
	}

	private void Start()
	{
		TryFindPlayer();
	}

	private void PointTowardsGoal()
	{
		Vector2 goalDir = goal.transform.position - player.transform.position;
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
		PointTowardsGoal();
	}
}
