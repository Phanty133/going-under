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

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		playerControls = player.GetComponent<PlayerControls>();
		playerControls.onPlayerMoved += OnPlayerMove;

		rectTransform = GetComponent<RectTransform>();
		canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
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
