using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainFollow : MonoBehaviour
{
	GameObject player;
	PlayerControls playerControls;
	RectTransform rectTransform;
	RectTransform canvasRect;

	bool TryFindPlayer()
	{
		player = GameObject.FindGameObjectWithTag("Player");

		if (player != null) InitPlayer();
		return player != null;
	}

	void InitPlayer()
	{
		playerControls = player.GetComponent<PlayerControls>();
		playerControls.onPlayerMoved += OnPlayerMove;
	}

	void Awake()
	{
		TryFindPlayer();

		rectTransform = GetComponent<RectTransform>();
		canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (player == null)
		{
			bool plyrFound = TryFindPlayer();
			if (!plyrFound) return;
		}
	}

	void OnPlayerMove(Vector2 deltaWorldPos)
	{
		Vector2 camPos = (Vector3)deltaWorldPos + Camera.main.transform.position;
		Vector2 screenPos = CanvasUtils.WorldToCanvasPointCentered(canvasRect, camPos);
		Vector2 camScreenPos = CanvasUtils.WorldToCanvasPointCentered(canvasRect, Camera.main.transform.position);
		Vector2 deltaPos = screenPos - camScreenPos;

		rectTransform.anchoredPosition -= deltaPos;
	}

	private void OnDestroy()
	{
		playerControls.onPlayerMoved -= OnPlayerMove;
	}
}
