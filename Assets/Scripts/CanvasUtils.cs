using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanvasUtils
{
	public static Vector2 WorldToCanvasPoint(RectTransform canvasRect, Vector2 worldPoint, Camera cam = null)
	{
		if (cam == null) cam = Camera.main;

		Vector2 viewportPt = cam.WorldToViewportPoint(worldPoint);
		float w = canvasRect.sizeDelta.x;
		float h = canvasRect.sizeDelta.y;
		Vector2 canvasPt = new(viewportPt.x * w, viewportPt.y * h);

		return canvasPt;
	}

	public static Vector2 WorldToCanvasPointCentered(RectTransform canvasRect, Vector2 worldPoint, Camera cam = null)
	{
		if (cam == null) cam = Camera.main;

		Vector2 viewportPt = cam.WorldToViewportPoint(worldPoint);
		float w = canvasRect.sizeDelta.x;
		float h = canvasRect.sizeDelta.y;
		Vector2 canvasPt = new((viewportPt.x - 0.5f) * w, (viewportPt.y - 0.5f) * h);

		return canvasPt;
	}

}
