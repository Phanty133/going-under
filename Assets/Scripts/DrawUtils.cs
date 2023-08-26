using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class DrawUtils
{
	public static void DrawLine(Texture2D texture2D, Vector2 start, Vector2 end, int width, Color color)
	{
		// Draw line using Bresenham's line algorithm
		int x0 = (int)start.x;
		int y0 = (int)start.y;
		int x1 = (int)end.x;
		int y1 = (int)end.y;

		int dx = Mathf.Abs(x1 - x0);
		int dy = Mathf.Abs(y1 - y0);

		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;

		int err = dx - dy;

		while (true)
		{
			DrawDot(texture2D, new Vector2(x0, y0), width, color);

			if (x0 == x1 && y0 == y1)
			{
				break;
			}

			int e2 = 2 * err;

			if (e2 > -dy)
			{
				err -= dy;
				x0 += sx;
			}

			if (e2 < dx)
			{
				err += dx;
				y0 += sy;
			}
		}
	}

	public static void DrawDot(Texture2D texture, Vector2 point, int width, Color color)
	{
		int halfWidth = width / 2;

		for (int x = -halfWidth; x < halfWidth; x++)
		{
			for (int y = -halfWidth; y < halfWidth; y++)
			{
				texture.SetPixel((int)point.x + x, (int)point.y + y, color);
			}
		}
	}
}

