using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UISoftMask;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SonarEnemyPrefab
{
	public EnemyType type;
	public GameObject prefab;
}

public class SonarUI : MonoBehaviour
{
	public int sonarRadius = 1000;
	public int sonarFadeRadius = 800; // Radius from which the sonar ping linearly fades to the max radius

	public GameObject sonarInfoPrefab;
	public GameObject pingObj;
	public GameObject playerObj;
	public GameObject sonarPlayerTorpedoPrefab;
	public GameObject sonarEnemyTorpedoPrefab;
	public SonarEnemyPrefab[] enemyPrefabs;

	RectTransform pingRect;
	RawImage pingImage;
	RectTransform playerRect;

	GameObject sonarInfoObj;
	SonarInfo sonarInfo;

	GameObject oldSonarInfoObj;
	SonarInfo oldSonarInfo;

	private GameObject CreateSonarInfo()
	{
		GameObject info = Instantiate(sonarInfoPrefab, transform);
		info.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
		info.AddComponent<SoftMaskable>(); // Have to add it here because you can't add it to a prefab

		return info;
	}

	private void Start()
	{
		pingRect = pingObj.GetComponent<RectTransform>();
		pingImage = pingObj.GetComponent<RawImage>();
		playerRect = playerObj.GetComponent<RectTransform>();

		sonarInfoObj = CreateSonarInfo();
		sonarInfo = sonarInfoObj.GetComponent<SonarInfo>();
	}

	public void UpdateTexture()
	{
		sonarInfo.UpdateTexture();
	}

	public void DrawTerrain(Vector2 screenPos, Collider2D col, float maxConnectDist = Mathf.Infinity)
	{
		SonarPoint pt = new()
		{
			type = SonarPointType.TERRAIN,
			pos = screenPos,
			col = col,
			maxConnectDist = maxConnectDist
		};

		sonarInfo.AddPoint(pt);
	}

	public void DrawObject(Vector2 screenPos, Collider2D col)
	{
		EnemyType type = col.GetComponent<Enemy>().type;

		SonarPoint pt = new()
		{
			type = SonarPointType.OBJECT,
			pos = screenPos,
			col = col,
			prefab = Array.Find(enemyPrefabs, e => e.type == type).prefab
		};

		sonarInfo.AddPoint(pt);
	}

	public void SetPing(float fract)
	{
		int radius_px = Mathf.FloorToInt(fract * sonarRadius);
		pingRect.sizeDelta = new Vector2(radius_px, radius_px);

		if (radius_px >= sonarFadeRadius)
		{
			float fadeFract = (radius_px - sonarFadeRadius) / (float)(sonarRadius - sonarFadeRadius);
			pingImage.color = new Color(pingImage.color.r, pingImage.color.g, pingImage.color.b, 1 - fadeFract);
		}
		else if (pingImage.color.a < 1)
		{
			pingImage.color = new Color(pingImage.color.r, pingImage.color.g, pingImage.color.b, 1);
		}
	}

	public void SetPlayer(float rotation, Vector2? pos)
	{
		playerRect.rotation = Quaternion.Euler(0, 0, rotation);

		if (pos.HasValue)
		{
			playerRect.anchoredPosition = pos.Value;
		}
	}

	public void TransitionTerrainOverlay()
	{
		oldSonarInfoObj = sonarInfoObj;
		oldSonarInfo = sonarInfo;

		sonarInfoObj = CreateSonarInfo();
		sonarInfo = sonarInfoObj.GetComponent<SonarInfo>();
	}

	public void ClearOldPoints(Vector2 center, float prevFract, float curFract)
	{
		if (oldSonarInfo != null)
		{
			oldSonarInfo.ClearPointsInRing(center, prevFract, curFract);
		}
	}

	public void KillOldOverlay()
	{
		if (oldSonarInfoObj != null)
		{
			Destroy(oldSonarInfoObj);
			oldSonarInfoObj = null;
			oldSonarInfo = null;
		}
	}

	public void CreateSonarTorpedo(GameObject srcTorpedo, Vector2 screenPos, bool fromEnemy = false)
	{
		GameObject prefab = fromEnemy ? sonarEnemyTorpedoPrefab : sonarPlayerTorpedoPrefab;
		GameObject sonarTorpedo = Instantiate(prefab, transform);
		sonarTorpedo.GetComponent<RectTransform>().anchoredPosition = screenPos;
		sonarTorpedo.transform.rotation = srcTorpedo.transform.rotation;

		srcTorpedo.GetComponent<Torpedo>().AddSonarTorpedo(sonarTorpedo);
	}
}
