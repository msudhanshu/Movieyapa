using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityExtensions;

public class GridTile :  MonoBehaviour { 
	private string _TileName = null;
//	public OBSTACLE_EDIT_MODE obstacleMode = OBSTACLE_EDIT_MODE.NONE;
	public ObstacleType obstacleType = ObstacleType.NONE;

	public Material NormalTileMaterial;
	public Color normalTile;
	public Color editDefault;
	public Color bakeDefault;
	public Color editBuilding;
	public Color bakeBuilding;
	public Color editCharacter;
	public Color bakeCharacter;

	public string TileName {
		get {
			if(_TileName == null) 
				_TileName = gameObject.name;
			return _TileName;
		} set { _TileName = value; }
	}

	private int _x = -1;
	public int x {
		get {
			if(_x == -1) 
				setXY();
			return _x;
		} set {_x = value; }
	}

	private int _y = -1;
	public int y {
		get {
			if(_y == -1) 
				setXY();
			return _y;
		} set {_y = value ;}
	}

	//parse the name and get x and y
	private void setXY() {}

	#if (UNITY_EDITOR)
	public void SetObstacleType(OBSTACLE_EDIT_MODE tObstacleMode, ObstacleType tObstacleType) {
		///gt.renderer.material = BakedObstacleTileMaterial;
		//obstacleMode = tObstacleMode;
		obstacleType = tObstacleType;

		switch (tObstacleMode) {
		case OBSTACLE_EDIT_MODE.NONE:
		case OBSTACLE_EDIT_MODE.REMOVE_OBSTACLE:
			GetComponent<Renderer>().material = NormalTileMaterial;
			GetComponent<Renderer>().material.color = normalTile;
			break;
		case OBSTACLE_EDIT_MODE.ADD_OBSTACLE:
			switch(obstacleType) {
			case ObstacleType.DEFAULT:
				GetComponent<Renderer>().material.color = editDefault;
				break;
			case ObstacleType.FORBUILDING:
				GetComponent<Renderer>().material.color = editBuilding;
				break;
			case ObstacleType.FORCHARACTER:
				GetComponent<Renderer>().material.color = editCharacter;
				break;
			}
			break;
		}
	}

	public void BakeTile() {
		switch(obstacleType) {
		case ObstacleType.DEFAULT:
			GetComponent<Renderer>().material.color = bakeDefault;
			break;
		case ObstacleType.FORBUILDING:
			GetComponent<Renderer>().material.color = bakeBuilding;
			break;
		case ObstacleType.FORCHARACTER:
			GetComponent<Renderer>().material.color = bakeCharacter;
			break;
		}
	}
	
	#endif

}
