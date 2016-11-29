using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FilledMazeMapGeneratorAlgo : IMazeMapGeneratorAlgo {
	private MazeMap mazeMap ;

	public virtual MazeMap GenerateMaze(int width, int height)
	{
		mazeMap = new MazeMap (width, height);
		FillMazeBlock(MazeBlockType.BLOCK1);
		return mazeMap;
	}

	
	private void FillMazeBlock(MazeBlockType type) {
		for (int x = 0; x < mazeMap.mazeX; x++)
		{
			for (int y = 0; y < mazeMap.mazeY; y++)
			{
				mazeMap.mazeGrid[x, y] = type;
			}
		}
		mazeMap.mazeGrid[0,0] = MazeBlockType.FREE;
	}


}
