using UnityEngine;
using System.Collections;
using System;

public struct MazePoint
{
	public int x;
	public int y;
	public MazePoint(int x, int y) {
		this.x = x;
		this.y = y;
	}
}