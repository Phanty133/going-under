using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalIndicator : MonoBehaviour
{
	public GameObject goal;

	GameObject playerObj;

	private void Start()
	{
		playerObj = GameObject.FindGameObjectWithTag("Player");
	}

	private void PointTowardsGoal()
	{
		Vector2 goalDir = goal.transform.position - playerObj.transform.position;
		float goalAngle = Mathf.Atan2(goalDir.y, goalDir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0, 0, goalAngle);
	}

	private void Update()
	{
		PointTowardsGoal();
	}
}
