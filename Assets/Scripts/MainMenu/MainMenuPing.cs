using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPing : MonoBehaviour
{
	public float pingTime_s = 1f;
	public float pingDelay_s = 0.5f;
	public float radius_m = 0f;
	public float maxRadius_m = 5f;
	public Vector2 pingAnimBezierAdjust = new(0.25f, 0.5f);
	public int sonarRadius = 1000;
	public int sonarFadeRadius = 800; // Radius from which the sonar ping linearly fades to the max radius
	public GameObject pingObj;
	public GameObject pingedObj;
	public AudioClip pingAudio;

	public float pingedRadiusAppear_m = 0.5f;
	public float pingedFadeInTime_s = 0.5f;
	public float pingedWaitTime_s = 0.5f;
	public float pingedFadeOutTime_s = 0.5f;
	float pingedAlpha = 0f;
	bool pingedFadeIn = false;
	bool pingedWait = false;
	bool pingedFadeOut = false;

	float pingAnimTimer = 0;
	float delayTimer = -1;
	RectTransform pingRect;
	RawImage pingImage;
	Image pingedImage;
	float pingedTimer = 0;

	AudioSource audioSource;

	private void UpdateRadius()
	{
		float t = pingAnimTimer / pingTime_s;
		Vector2 p = MathUtils.QuadBezier(Vector2.zero, pingAnimBezierAdjust, Vector2.one, t);
		radius_m = maxRadius_m * p.y;

		pingAnimTimer += Time.deltaTime;
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

	public void UpdatePingedObject()
	{
		if (radius_m >= pingedRadiusAppear_m && !pingedFadeIn)
		{
			pingedFadeIn = true;
			pingedTimer = 0;
		}

		if (pingedFadeIn)
		{
			pingedAlpha = pingedTimer / pingedFadeInTime_s;

			if (pingedAlpha >= 1)
			{
				pingedAlpha = 1;
				pingedFadeIn = false;
				pingedWait = true;
			}
		}
		else if (pingedWait && pingedTimer >= pingedWaitTime_s)
		{
			pingedWait = false;
			pingedFadeOut = true;
			pingedTimer = 0;
		}
		else if (pingedFadeOut)
		{
			pingedAlpha = 1 - (pingedTimer / pingedFadeOutTime_s);

			if (pingedAlpha <= 0)
			{
				pingedAlpha = 0;
				pingedFadeOut = false;
			}
		}

		pingedImage.color = new Color(pingedImage.color.r, pingedImage.color.g, pingedImage.color.b, pingedAlpha);
		pingedTimer += Time.deltaTime;
	}

	private void Start()
	{
		pingRect = pingObj.GetComponent<RectTransform>();
		pingImage = pingObj.GetComponent<RawImage>();
		pingedImage = pingedObj.GetComponent<Image>();
		audioSource = GameObject.FindGameObjectWithTag("GameController").GetComponent<AudioSource>();

		SetPing(0f);
		delayTimer = pingDelay_s;
	}

	private void Update()
	{
		UpdatePingedObject();

		if (delayTimer > 0)
		{
			delayTimer -= Time.deltaTime;
			return;
		}
		else if (delayTimer <= 0 && delayTimer != -1)
		{
			delayTimer = -1;
			audioSource.PlayOneShot(pingAudio);
		}

		if (pingAnimTimer > pingTime_s)
		{
			pingAnimTimer = 0;
			delayTimer = pingDelay_s;
		}

		UpdateRadius();
		SetPing(radius_m / maxRadius_m);

		pingAnimTimer += Time.deltaTime;
	}
}
