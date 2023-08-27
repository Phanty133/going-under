using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalGenerator : MonoBehaviour
{
	public GameObject startPrefab;
	public GameObject endPrefab;
	public GameObject container;
	public float centerOffset = 40;
	public float side = 50;

	public GameObject cooldownObj;
	public GameObject healthObj;
	public GameObject levelFailedObj;

	GameObject startObj;

	public Vector2 RandCornerPos(int xSide, int ySide)
	{
		float width = side - centerOffset;
		Vector2 pos = new(Random.Range(0, width) + centerOffset, Random.Range(0, width) + centerOffset);
		pos.x *= xSide;
		pos.y *= ySide;

		return pos;
	}

	public IEnumerator SpawnGoals()
	{
		// Select corner
		int xSide = MathUtils.RandomNegative();
		int ySide = MathUtils.RandomNegative();

		Vector2 pos;
		pos = RandCornerPos(xSide, ySide);

		while (Physics2D.OverlapPoint(pos, LayerMask.GetMask("Enemy", "Terrain")))
		{
			pos = RandCornerPos(xSide, ySide);
			yield return null;
		}

		startObj = Instantiate(startPrefab, pos, new Quaternion(), container.transform);

		pos = RandCornerPos(-xSide, -ySide);

		while (Physics2D.OverlapPoint(pos, LayerMask.GetMask("Enemy", "Terrain")))
		{
			pos = RandCornerPos(-xSide, -ySide);
			yield return null;
		}
		GameObject end = Instantiate(endPrefab, pos, new Quaternion(), container.transform);
		GameObject.FindGameObjectWithTag("GoalIndicator").GetComponent<GoalIndicator>().goal = end;

		SpawnPlayer();
	}

	public void SpawnPlayer()
	{
		StartGoal startGoal = startObj.GetComponent<StartGoal>();
		startGoal.InitPlayer(cooldownObj, healthObj, levelFailedObj);
	}
}
