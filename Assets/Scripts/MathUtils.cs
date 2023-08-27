using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MathUtils
{
	public static Vector2 QuadBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
	{
		Vector2 p01 = Vector2.Lerp(p0, p1, t);
		Vector2 p12 = Vector2.Lerp(p1, p2, t);
		Vector2 p = Vector2.Lerp(p01, p12, t);

		return p;
	}

	public static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		Vector2 p01 = Vector2.Lerp(p0, p1, t);
		Vector2 p12 = Vector2.Lerp(p1, p2, t);
		Vector2 p23 = Vector2.Lerp(p2, p3, t);
		Vector2 p012 = Vector2.Lerp(p01, p12, t);
		Vector2 p123 = Vector2.Lerp(p12, p23, t);
		Vector2 p = Vector2.Lerp(p012, p123, t);

		return p;
	}

	public static int RandomNegative()
	{
		return Random.value > 0.5f ? 1 : -1;
	}

	public static Vector2 RandomInRing(float minRadius, float maxRadius)
	{
		float r = Random.Range(minRadius, maxRadius);
		float theta = Random.Range(0, 2 * Mathf.PI);
		return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
	}
}

