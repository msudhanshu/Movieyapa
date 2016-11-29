using UnityEngine;
using System.Collections;
using System;

public class MazeMapLoader
{
	MazeMap mazeMap;
	public MazeMap Deserialize(TextAsset _mazeMapAsset) {
		mazeMap = new MazeMap ();

		string[] mapfile = _mazeMapAsset.text.Split ("#"[0]);
		DeserializeMap(mapfile[0]);
		DeserializeSinkTarget(mapfile[1]);
		return mazeMap;
	}

	public void DeserializeMap(string mapString){
		string[] map = mapString.Split("\n"[0]);//_mazeMapAsset.text.Split ("\n"[0]);
		mazeMap.mazeY = map.Length-1; //note : -1 to avoid last \n 
		mazeMap.mazeX=0;
		for (var j = 0; j < mazeMap.mazeY; j ++) {
			if(mazeMap.mazeX < map[j].Length)
				mazeMap.mazeX = map[j].Length;
		}
		
		mazeMap.mazeGrid = new MazeBlockType[mazeMap.mazeX,mazeMap.mazeY];
		
		for (var i = 0; i < mazeMap.mazeX; i ++) {
			for (var j = 0; j < mazeMap.mazeY; j ++) {
				mazeMap.mazeGrid[i,j] = MazeBlockType.NULL;
			}}
		
		for (var j = 0; j < map.Length; j ++) {
			for (var i = 0; i < map[j].Length; i ++) {
				mazeMap.mazeGrid[i,j] = MazeBlock.StringToBlockType(map[j][i].ToString());
				if(MazeBlock.IsSinkBlock(mazeMap.mazeGrid[i,j]) ) {
					mazeMap.allSinkTarget.Add(new MazePoint(i,j));
				}
			}
		}
	}

	public void DeserializeSinkTarget(string sinkTargets) {
		sinkTargets = sinkTargets.Trim("\n"[0]);
		string[] points = sinkTargets.Split(";"[0]);
		for (var j = 0; j < points.Length; j ++) {
			string[] point = points[j].Split(","[0]);
			int x  = Convert.ToInt32(point[0]);
			int y = Convert.ToInt32(point[1]);
			mazeMap.allSinkTarget.Add(new MazePoint(x,y));
		}
	}
	
	public TextAsset Serialize(MazeMap mazeMap, string fileName) {
		TextAsset _mazeMapAsset = new TextAsset();
		string mapData="";
		for (var j = 0; j < mazeMap.mazeY; j ++) {
			for (var i = 0; i < mazeMap.mazeX; i ++) {
				mapData+= SEnum.GetStringValue( mazeMap.mazeGrid[i,j] ) ;
			}
			mapData+='\n';
		}
		Debug.Log (mapData);
		//SAVE IT AT PROPER LOCATION WITH fileName
		return _mazeMapAsset;
	}



}