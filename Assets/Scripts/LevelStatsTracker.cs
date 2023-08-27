using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatsTracker : MonoBehaviour
{
	public static LevelPassedStats LevelStats;

	public static void ResetStats()
	{
		LevelStats = new LevelPassedStats();
	}
}
