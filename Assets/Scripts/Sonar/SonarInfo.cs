using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SonarPointType
{
	TERRAIN,
	OBJECT
}

public class SonarPoint
{
	public Collider2D col;
	public SonarPointType type;
	public Vector2 pos;
	public SonarObject obj = null; // Null for type=TERRAIN
	public GameObject prefab = null; // Null for type=TERRAIN
	public SonarPoint prevPoint = null; // Null for type=OBJECT
	public float? maxConnectDist = null; // Null for type=OBJECT
}

public class SonarInfo : MonoBehaviour
{
	public int ptWidth = 3;
	public int sonarRadius = 1000;
	public float pointFadeTime = 3f;
	public Color color = Color.blue;
	public GameObject objectContainer;
	public GameObject terrainImageObj;

	RawImage image;
	RectTransform canvasRect;
	Texture2D texture;
	Dictionary<Collider2D, List<SonarPoint>> sonarPoints = new();

	// Set as static to cache it between instances
	static Color32[] EmptyTexturePixels = null;

	private void Awake()
	{
		image = terrainImageObj.GetComponent<RawImage>();
		canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();

		texture = new Texture2D((int)canvasRect.sizeDelta.x, (int)canvasRect.sizeDelta.y);
		image.texture = texture;

		if (EmptyTexturePixels == null)
		{
			EmptyTexturePixels = new Color32[texture.width * texture.height];

			for (int i = 0; i < EmptyTexturePixels.Length; i++)
			{
				EmptyTexturePixels[i] = Color.clear;
			}
		}

		texture.SetPixels32(EmptyTexturePixels);
		UpdateTexture();
		image.color = Color.white;
	}

	private void DeletePoint(SonarPoint pt)
	{
		switch (pt.type)
		{
			case SonarPointType.TERRAIN:
				if (pt.prevPoint != null)
				{
					DrawUtils.DrawLine(texture, pt.prevPoint.pos, pt.pos, ptWidth, Color.clear);
				}
				else
				{
					DrawUtils.DrawDot(texture, pt.pos, ptWidth, Color.clear);
				}

				break;
			case SonarPointType.OBJECT:
				pt.obj.Kill();
				break;
		}
	}

	public void ClearPointsInRing(Vector2 ringCenter, float fractMin, float fractMax)
	{
		bool terrainChanged = false;
		List<SonarPoint> killedPoints = new();
		int minDist = Mathf.FloorToInt(fractMin * sonarRadius);
		int maxDist = Mathf.FloorToInt(fractMax * sonarRadius);
		List<Collider2D> emptyColliders = new();

		foreach (var colPoints in sonarPoints)
		{
			foreach (var pt in colPoints.Value)
			{
				float dist = Vector2.Distance(pt.pos, ringCenter);
				if (dist < minDist || dist > maxDist) continue;

				DeletePoint(pt);

				if (pt.type == SonarPointType.TERRAIN) terrainChanged = true;
				killedPoints.Add(pt);
			}

			sonarPoints[colPoints.Key].RemoveAll(x => killedPoints.Contains(x));
			if (sonarPoints[colPoints.Key].Count == 0) emptyColliders.Add(colPoints.Key);
		}

		foreach (var col in emptyColliders)
		{
			sonarPoints.Remove(col);
		}

		if (terrainChanged) UpdateTexture();
	}

	private void DrawSonarPoint(SonarPoint pt)
	{
		switch (pt.type)
		{
			case SonarPointType.TERRAIN:
				if (pt.prevPoint != null)
				{
					DrawUtils.DrawLine(texture, pt.prevPoint.pos, pt.pos, ptWidth, color);
				}
				else
				{
					DrawUtils.DrawDot(texture, pt.pos, ptWidth, color);
				}

				break;
			case SonarPointType.OBJECT:
				GameObject obj = Instantiate(pt.prefab, objectContainer.transform);
				obj.GetComponent<RectTransform>().anchoredPosition = pt.pos - 0.5f * canvasRect.sizeDelta;
				obj.transform.rotation = pt.col.transform.rotation;
				pt.obj = obj.GetComponent<SonarObject>();

				break;
		}
	}

	private SonarPoint GetClosestPoint(SonarPoint pt, List<SonarPoint> pts)
	{
		SonarPoint closest = null;
		float closestDist = float.MaxValue;

		foreach (SonarPoint curPt in pts)
		{
			if (curPt == pt) continue;

			float curDist = Vector2.Distance(pt.pos, curPt.pos);

			if (curDist > pt.maxConnectDist) continue;
			if (curDist < closestDist)
			{
				closest = curPt;
				closestDist = curDist;
			}
		}

		return closest;
	}

	private void StorePoint(SonarPoint pt)
	{
		if (!sonarPoints.ContainsKey(pt.col))
		{
			sonarPoints.Add(pt.col, new List<SonarPoint>());
		}

		sonarPoints[pt.col].Add(pt);
	}

	public void AddPoint(SonarPoint pt)
	{
		if (pt.type == SonarPointType.OBJECT && sonarPoints.ContainsKey(pt.col)) return;

		StorePoint(pt);

		if (pt.type == SonarPointType.TERRAIN)
		{
			if (sonarPoints[pt.col].Count > 1)
			{
				SonarPoint closest = GetClosestPoint(pt, sonarPoints[pt.col]);
				if (closest != null) pt.prevPoint = closest;
			}
		}

		DrawSonarPoint(pt);
	}

	public void UpdateTexture()
	{
		texture.Apply();
	}
}
