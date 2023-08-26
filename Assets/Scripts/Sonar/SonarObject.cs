using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SonarObject : MonoBehaviour
{
	public float fadetime_s = 1f;
	private float fadeTimer_s = 0f;

	bool fadeOut = false;
	bool fadeIn = false;
	RawImage rend;
	float alpha;

	public void Kill()
	{
		fadeOut = true;
	}

	private void Start()
	{
		rend = GetComponent<RawImage>();
		fadeIn = true;
	}

	private void Update()
	{
		if (fadeIn)
		{
			alpha = fadeTimer_s / fadetime_s;

			if (alpha >= 1f)
			{
				fadeIn = false;
				return;
			}
		}
		else if (fadeOut)
		{
			alpha = 1f - fadeTimer_s / fadetime_s;

			if (alpha <= 0f)
			{
				Destroy(gameObject);
				return;
			}
		}

		if (fadeIn || fadeOut)
		{
			rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
			fadeTimer_s += Time.deltaTime;
		}
	}
}
