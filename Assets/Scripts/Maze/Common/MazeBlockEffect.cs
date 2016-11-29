using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[Serializable]
public class MazeBlockEffect //basedao
{
	//id,type,color,prefab
	public string id;
	public MazePathType pathType;
	public MazePathBlockType pathBlockType ;
	public Color color;
	public string prefabName;

	//temp
	public MazeBlockEffect(Color color, string prefabName=null){
		this.color = color;
	}

	//	public static MazePathTypeEffect GetPathEffect(string pathTypeId) {
	//public static MazePathTypeEffect GetPathEffect(MazePathType pathType, MazePathBlockType pathBlockType ) {
	//temp
	public static MazeBlockEffect GetPathEffect(MazeBlockType blockType) {
		if(blockType == MazeBlockType.SINK1 || blockType==MazeBlockType.SOURCE1) {
			return new MazeBlockEffect(Color.red);
		} else if(blockType == MazeBlockType.SINK2 || blockType==MazeBlockType.SOURCE2) {
			return new MazeBlockEffect(Color.green);
		}
		else if(blockType == MazeBlockType.BLOCK1) {
			return new MazeBlockEffect(Color.blue);
		}
		return new MazeBlockEffect(Color.black);
	}

	public static MazeBlockEffect GetSinkTargetEffect() {
		return new MazeBlockEffect(Color.yellow);
	}

	public static MazeBlockEffect GetWrongSinkTargetClickEffect() {
		return new MazeBlockEffect(Color.black);
	}
}