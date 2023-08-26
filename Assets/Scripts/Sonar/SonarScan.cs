using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarScan : MonoBehaviour
{
	public delegate void RadarScanHit(Vector2 point, Collider2D other);
	public RadarScanHit radarScanEnemyHit;
	public RadarScanHit radarScanTerrainHit;

	public delegate void RadarScanIterDone();
	public RadarScanIterDone radarScanIterDone;

	public delegate void RadarScanDone();
	public RadarScanDone radarScanDone;

	public delegate void SonarPreparePingStep(float prevRadius_m, float curRadius_m);
	public SonarPreparePingStep sonarPreparePingStep;

	public float pingTime_s = 1f;
	public float radius_m = 0f;
	public float radiusStep_m = 0f;
	public float maxRadius_m = 5f;
	public float raycastPrecision_deg = 2f; // Degrees per raycast
	public float pingDelay_s = 3f;

	public Vector2 pingAnimBezierAdjust = new(0.5f, 0.5f);
	public Vector2 pingedPos;

	public bool IsScanning
	{
		get
		{
			return pingAnimTimer >= 0;
		}
	}

	public Vector2 WorldPos
	{
		get
		{
			return transform.position;
		}
	}

	int enemyMask;
	int terrainMask;
	int numRaycasts;

	List<RaycastHit2D> pingHits;
	float pingDelayTimer = 0;
	float pingAnimTimer = -1;

	public float GetDistBetweenRaycasts()
	{
		float dist = 2 * Mathf.PI * radius_m / numRaycasts;
		return dist;
	}

	public void StartPing()
	{
		pingHits.Clear();
		radius_m = 0f;
		pingedPos = transform.position;

		int layerMask = enemyMask | terrainMask;

		for (int i = 0; i < numRaycasts; i++)
		{
			Vector2 castDir = Quaternion.Euler(0, 0, i * raycastPrecision_deg) * Vector2.up;
			pingHits.Add(Physics2D.Raycast(pingedPos, castDir, maxRadius_m, layerMask));
		}

		pingAnimTimer = 0;
	}

	private void GetPingHits(float minDist, float maxDist)
	{
		foreach (var hit in pingHits)
		{
			if (!hit) continue;

			if (hit.distance >= minDist && hit.distance <= maxDist)
			{
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
				{
					radarScanEnemyHit?.Invoke(hit.point, hit.collider);
				}
				else
				{
					radarScanTerrainHit?.Invoke(hit.point, hit.collider);
				}
			}
		}
	}

	private void Start()
	{
		enemyMask = 1 << LayerMask.NameToLayer("Enemy");
		terrainMask = 1 << LayerMask.NameToLayer("Terrain");
		numRaycasts = Mathf.FloorToInt(360 / raycastPrecision_deg);
		pingHits = new List<RaycastHit2D>(numRaycasts);
	}

	private void UpdateRadius()
	{
		float t = pingAnimTimer / pingTime_s;
		Vector2 p = MathUtils.QuadBezier(Vector2.zero, pingAnimBezierAdjust, Vector2.one, t);
		radius_m = maxRadius_m * p.y;

		pingAnimTimer += Time.deltaTime;
	}

	private void Update()
	{
		if (pingDelayTimer > 0)
		{
			pingDelayTimer -= Time.deltaTime;
			return;
		}

		if (IsScanning)
		{
			float prevRadius_m = radius_m;
			UpdateRadius();
			sonarPreparePingStep?.Invoke(prevRadius_m, radius_m);

			// Debug.DrawRay(transform.position, Vector2.up * radius_m, Color.green, 0.1f);

			if (radius_m >= maxRadius_m)
			{
				pingDelayTimer = pingDelay_s;
				pingAnimTimer = -1;
				radarScanDone?.Invoke();
			}
			else
			{
				GetPingHits(prevRadius_m, radius_m);
			}

			radarScanIterDone?.Invoke();
		}
	}
}
