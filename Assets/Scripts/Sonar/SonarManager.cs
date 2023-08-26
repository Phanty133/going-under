using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarManager : MonoBehaviour
{
	public GameObject pingCameraPrefab;
	public GameObject scannerObj;
	public GameObject sonarUIObj;
	public AudioClip sonarPingClip;

	SonarScan scanner;
	SonarUI ui;

	Camera pingCamera;
	RectTransform canvasRect;
	AudioSource audioSource;

	private void Start()
	{
		canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();

		scanner = scannerObj.GetComponent<SonarScan>();
		scanner.radarScanEnemyHit += OnRadarEnemyHit;
		scanner.radarScanTerrainHit += OnRadarTerrainHit;
		scanner.radarScanIterDone += OnRadarScanIterDone;
		scanner.radarScanDone += OnRadarScanDone;
		scanner.sonarPreparePingStep += OnSonarPreparePingStep;

		ui = sonarUIObj.GetComponent<SonarUI>();
		audioSource = GetComponent<AudioSource>();
	}

	private void OnSonarPreparePingStep(float prevRadius_m, float curRadius_m)
	{
		float curFract = curRadius_m / scanner.maxRadius_m;
		float prevFract = prevRadius_m / scanner.maxRadius_m;

		Vector2 ringCenter = CanvasUtils.WorldToCanvasPoint(canvasRect, scanner.pingedPos, pingCamera);
		ui.ClearOldPoints(ringCenter, prevFract, curFract);
		ui.SetPing(curFract);
	}

	void OnRadarEnemyHit(Vector2 point, Collider2D other)
	{
		Vector2 centerPos = other.GetComponent<Renderer>().bounds.center;
		Vector2 screenPos = CanvasUtils.WorldToCanvasPoint(canvasRect, centerPos, pingCamera);
		ui.DrawObject(screenPos, other);
	}

	void OnRadarTerrainHit(Vector2 point, Collider2D other)
	{
		float delta_dist_m = scanner.GetDistBetweenRaycasts();
		float delta_dist_fract = delta_dist_m / scanner.maxRadius_m;
		float delta_dist_px = delta_dist_fract * ui.sonarRadius;
		float delta_factor = 1.5f;

		Vector2 screenPos = CanvasUtils.WorldToCanvasPoint(canvasRect, point, pingCamera);
		ui.DrawTerrain(screenPos, other, delta_dist_px * delta_factor);
	}

	void OnRadarScanIterDone()
	{
		ui.UpdateTexture();
	}

	void OnRadarScanDone()
	{
		ui.SetPing(0);
		ui.KillOldOverlay();

		Destroy(pingCamera.gameObject);
		pingCamera = null;
	}

	public void SetSonarPlayer(float rotation, Vector3? worldPos = null)
	{
		Vector2? pos = null;

		if (worldPos.HasValue)
		{
			pos = CanvasUtils.WorldToCanvasPointCentered(canvasRect, worldPos.Value, Camera.main);
		}

		ui.SetPlayer(rotation, pos);
	}

	public void PingSonar()
	{
		if (scanner.IsScanning) return;

		pingCamera = Instantiate(pingCameraPrefab, null).GetComponent<Camera>();
		pingCamera.transform.position = Camera.main.transform.position;
		ui.TransitionTerrainOverlay();
		scanner.StartPing();
		audioSource.PlayOneShot(sonarPingClip);
	}

	public void CreateSonarTorpedo(GameObject worldTorpedo, bool fromEnemy = false)
	{
		Vector2 screenPos = CanvasUtils.WorldToCanvasPointCentered(canvasRect, worldTorpedo.transform.position, Camera.main);
		ui.CreateSonarTorpedo(worldTorpedo, screenPos, fromEnemy);
	}
}
