using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
	public GameObject innerBar;
	public float minValue = 0;
	public float maxValue = 100;

	public float Value
	{
		get { return _value; }
		private set
		{
			_value = Mathf.Clamp(value, minValue, maxValue);
		}
	}

	private float _value = 0;

	RectTransform innerRect;
	float height;

	private void Start()
	{
		innerRect = innerBar.GetComponent<RectTransform>();
		height = GetComponent<RectTransform>().rect.height;
		Value = 100;
	}

	public void SetBarValue(float newVal)
	{
		Value = newVal;

		float fract = (Value - minValue) / (maxValue - minValue);
		float innerHeight = height * fract;
		innerRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, height - innerHeight, innerHeight);
	}
}
