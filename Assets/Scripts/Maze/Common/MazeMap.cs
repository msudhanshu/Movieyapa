using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeMap 
{
	public int mazeX=0;
	public int mazeY=0;
	public MazeBlockType[,] mazeGrid;
	public List<MazePoint> allSinkTarget = new List<MazePoint>();
	public Dictionary<MazePathType, List<MazePoint>> allSink = new Dictionary<MazePathType,List<MazePoint>>();
	public Dictionary<MazePathType, List<MazePoint>> allSource = new Dictionary<MazePathType,List<MazePoint>>();


	public MazeMap() {
	}

	public MazeMap(int width, int height) {
		mazeX = width;
		mazeY = height;
		mazeGrid = new MazeBlockType[mazeX,mazeY];
	}

	public MazeMap(MazeBlockType[,] _MazeBlockType, int width, int height) {
		this.mazeGrid = _MazeBlockType;
		mazeX = width;
		mazeY = height;
	}

	public bool IsSinkTarget(MazePoint mazePoint) {
		if(allSinkTarget.Contains(mazePoint)) return true;
		return false;
	}

}