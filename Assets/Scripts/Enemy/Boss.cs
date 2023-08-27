using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	public float spawnedSpeedFactor = 0.5f;
	public float spawnFrequency_s = 15f;
	public float minSpawnRadius = 3f;
	public float maxSpawnRadius = 10f;
	public float maxSubs = 3;
	public GameObject enemySub;
	public GameObject explosionObj;
	public int defeatExplosions = 5;

	float spawnTimer_s = 0f;
	List<GameObject> subs = new();
	GameObject player;
	Enemy selfEnemy;

	private void Start()
	{
		selfEnemy = GetComponent<Enemy>();
	}

	bool TryGetPlayer()
	{
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
		}
		return player != null;
	}

	private int UpdateSubCount()
	{
		subs.RemoveAll((GameObject obj) => obj == null);
		return subs.Count;
	}

	private void SpawnSub()
	{
		// Choose random position around player
		Vector2 offset = MathUtils.RandomInRing(minSpawnRadius, maxSpawnRadius);
		Vector2 pos = (Vector2)player.transform.position + offset;

		// Spawn sub
		GameObject subObj = Instantiate(enemySub, pos, Quaternion.identity, transform.parent);
		Enemy sub = subObj.GetComponent<Enemy>();
		sub.squad = 0;
		sub.speed *= spawnedSpeedFactor;
	}

	private void Update()
	{
		if (player == null && !TryGetPlayer()) return;
		if (selfEnemy.State != EnemyStates.PURSUIT) return;

		spawnTimer_s += Time.deltaTime;

		if (spawnTimer_s >= spawnFrequency_s)
		{
			if (UpdateSubCount() > maxSubs) return;

			spawnTimer_s = 0f;
			SpawnSub();
		}
	}

	public IEnumerator TriggerDefeatSeq()
	{
		UpdateSubCount();

		foreach (GameObject sub in subs)
		{
			Destroy(sub);
		}

		for (int i = 0; i < defeatExplosions; i++)
		{
			Vector2 offset = MathUtils.RandomInRing(0f, 2f);
			Vector2 pos = (Vector2)transform.position + offset;
			Instantiate(explosionObj, pos, Quaternion.identity, transform.parent);
			yield return new WaitForSeconds(0.5f);
		}

		yield return new WaitForSeconds(5f);

		StartCoroutine(LevelManager.NextLevel(true));
		Destroy(gameObject);
	}
}