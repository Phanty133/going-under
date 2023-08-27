using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGoal : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag != "Player") return;
		if (LevelManager.BossLevel) return;

		StartCoroutine(LevelManager.NextLevel());
	}
}
