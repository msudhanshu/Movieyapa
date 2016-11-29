using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//data
using KiwiCommonDatabase;
using SimpleSQL;


[Serializable]
public class LevelSceneData : BaseDbModel
{
	[PrimaryKey]
	public string id {get; set;}
	
	public string name {get; set;}

	public string mapLoadType {get; set;}
	public string mapModelType {get; set;}
	public string mapFile {get; set;}
	public int mazeSizeX {get; set;}
	public int mazeSizeY {get; set;}
	public string imageFile {get; set;}
	public string matFile {get; set;}

	public MazeMapLoadType mazeMapLoadType {
		get{
			switch(mapLoadType) {
			case "auto":
				return MazeMapLoadType.AUTO_GEN;
				break;
			case "algo":
				return MazeMapLoadType.ALGO_GEN;
				break;
			case "map":
				return MazeMapLoadType.MAZE_MAP;
					break;
			case "scene":
				return MazeMapLoadType.MAZE_SCENE;
					break;
			default:
				return MazeMapLoadType.AUTO_GEN;
			}
		}
	}

	public MazeModelType mazeModelType {
		get {
			switch(mapModelType) {
			case "rect_tex_fill":
				return MazeModelType.RECTANGULAR_TEXTURE_FILL;
				break;
			case "circ_tex_fill":
				return MazeModelType.CIRCULAR_TEXTURE_FILL;
				break;
			case "rect_mod":
				return MazeModelType.RECTANGULAR_MODEL;
				break;
			case "rect_mod_fill":
				return MazeModelType.RECTANGULAR_MODEL_FILL;
				break;
			case "default":
			default:
				return MazeModelType.DEFAULT;
			}
		}
	}
	
	private TextAsset _mazeMapAsset ;
	public TextAsset mazeMapAsset {
		get {
			if(_mazeMapAsset==null) {
				_mazeMapAsset =  (TextAsset)Resources.Load<TextAsset>(mapFile);
			}
			return _mazeMapAsset;
		}
	}

	public Texture2D texture = null;

	public static LevelSceneData GetLevelSceneData(string _id) {
		return DatabaseManager.GetInstance ().GetDbHelper ().QueryObjectById<LevelSceneData> (_id);
	}
 
}