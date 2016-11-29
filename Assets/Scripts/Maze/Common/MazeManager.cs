using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SgUnity;

public enum MazeModelType {
	RECTANGULAR_MODEL,
	CIRCULAR_TEXTURE_FILL,
	RECTANGULAR_TEXTURE_FILL,
	RECTANGULAR_MODEL_FILL,
	DEFAULT
}

public enum MazeMapLoadType {
	MAZE_MAP,
	AUTO_GEN,
	ALGO_GEN,
	MAZE_SCENE
}

public class MazeManager : Manager<MazeManager>
{
	public GameObject mazeRoot;
	public Camera mazeCamera;

	[HideInInspector]
	public MazeMap mazeMap;
	private GamePlayManager _gamePlayer;
	[HideInInspector]
	public GamePlayManager gamePlayer {
		get {
			if(_gamePlayer==null)
				_gamePlayer =  GetComponent<GamePlayManager>();
			return _gamePlayer;
		}
	}

	public static int MAZE_BLOCK_LAYER = 10;
	public const string MAZEBLOCKNAMEPREFIX = "BLOCK";
	public const string DELIMITER = "-";


	public IMazeMapGeneratorAlgo mazeMapGenerator = new MazeMapGeneratorAlgo();
	
	public LevelSceneData levelSceneData;

	MazeGridManager _maze3DGrid;
	public MazeGridManager maze3DGrid {
		get {
			if(_maze3DGrid==null) {
				_maze3DGrid = gameObject.GetComponent<MazeGridManager>();
			}
			return _maze3DGrid;
		}
	}
	private MazeModelBuilder _mazeModelBuilder;
	public MazeModelBuilder mazeModelBuilder {
		get {
			if(_mazeModelBuilder==null)
				_mazeModelBuilder = GetComponent<MazeModelBuilder>();
			return _mazeModelBuilder;
		}
		set {
			
		}
	}

	override public void StartInit() {

	}


	public void LoadLevel(LevelSceneData level) {
		this.levelSceneData = level;
		AddMazeModelComponent(level.mazeModelType);
		StartCoroutine( LoadMazeMap () );
	}

	public void UnLoadLevel(LevelSceneData level) {
		//AddMazeModelComponent(level.mazeModelType);
		if(gameObject.GetComponent<MazeModelBuilder>())
			Object.Destroy(gameObject.GetComponent<MazeModelBuilder>());

		if(gameObject.GetComponent<MazeGridManager>())
			Object.Destroy(gameObject.GetComponent<MazeGridManager>());

		GameObject.Destroy(mazeRoot);
	}

	public void AddMazeModelComponent(MazeModelType tMazeType) {
		//Destroy(GetComponent<MazeModelBuilder>());
		//Destroy(GetComponent<MazeGridManager>());
		_maze3DGrid=null;
		_mazeModelBuilder= null;
		switch(tMazeType) {
			case MazeModelType.CIRCULAR_TEXTURE_FILL:
				gameObject.AddComponent<CircularTextureFillMazeBuilder>();
				gameObject.AddComponent<CircularMazeGridManager>();
				break;
			case MazeModelType.RECTANGULAR_MODEL_FILL:
				gameObject.AddComponent<RectModelFillMazeBuilder>();
				gameObject.AddComponent<RectMazeGridManager>();
				break;
			case MazeModelType.RECTANGULAR_TEXTURE_FILL:
				gameObject.AddComponent<RectTextureFillMazeBuilder>();
				gameObject.AddComponent<RectMazeGridManager>();
				break;
			case MazeModelType.RECTANGULAR_MODEL:
				gameObject.AddComponent<RectModelMazeBuilder>();
				gameObject.AddComponent<RectMazeGridManager>();
				break;
			case MazeModelType.DEFAULT:
			default:
				//dont do any thing
				break;
		}
	}

	override public void PopulateDependencies() {}

	private IEnumerator LoadMazeMap() {
			//load pregenerated map 
		switch(levelSceneData.mazeMapLoadType) {
			case MazeMapLoadType.MAZE_SCENE:
				if (mazeRoot != null) {
								mazeMap = new MazeMapLoader ().Deserialize (mazeRoot.GetComponent<MazeRootDetail> ().mazeMapAsset);
								PopulateOccupiedBlockList ();
				}
			Debug.LogError("maze scene is not present. so generating autogen map");
			GenerateMaze(levelSceneData.mazeSizeX,levelSceneData.mazeSizeY);
            break;
		case MazeMapLoadType.MAZE_MAP:
			GenerateMaze(levelSceneData.mazeMapAsset);
			break;
		case MazeMapLoadType.ALGO_GEN:
			mazeMapGenerator = new MazeMapGeneratorAlgo();
			GenerateMaze(levelSceneData.mazeSizeX,levelSceneData.mazeSizeY);
			break;
		case MazeMapLoadType.AUTO_GEN:
			FilledMazeMapGeneratorAlgo tmazeMapGenerator = new FilledMazeMapGeneratorAlgo();
			mazeMap = tmazeMapGenerator.GenerateMaze(levelSceneData.mazeSizeX, levelSceneData.mazeSizeY);
			CreateMazeGrid();
			mazeModelBuilder.Generate(mazeMap,GetMazeRootObject());
			yield return StartCoroutine( maze3DGrid.SlideShuffle() );
			break;
		}
		yield break;
	}

	public void GenerateMaze(TextAsset mazeMapAsset) {
		mazeMap = new MazeMapLoader ().Deserialize (mazeMapAsset);
		CreateMazeGrid();
		mazeModelBuilder.Generate(mazeMap,GetMazeRootObject());
	}

	public void GenerateMaze(int mazeSizeX, int mazeSizeY) {
		mazeMap = mazeMapGenerator.GenerateMaze(mazeSizeX,mazeSizeY);
		CreateMazeGrid();
		mazeModelBuilder.Generate(mazeMap,GetMazeRootObject());
	}

	private void CreateMazeGrid() {
		maze3DGrid.Init(mazeMap.mazeX,mazeMap.mazeY);
	}

	private GameObject GetMazeRootObject() {
		if (mazeRoot != null)
				DestroyImmediate (mazeRoot);
		mazeRoot = new GameObject ("MazeRootName");
		mazeRoot.AddComponent<MazeRootDetail> ();
		return mazeRoot;
	}

	private void PopulateOccupiedBlockList() {
		for (int i = 0; i <= mazeMap.mazeGrid.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= mazeMap.mazeGrid.GetUpperBound(1); j++)
			{
				if (mazeMap.mazeGrid[i, j] != MazeBlockType.NULL && mazeMap.mazeGrid[i, j] != MazeBlockType.FREE)
				{
					string blockName = GetMazeBlockName(i,j);
					MazeBlock mb = GetMazeBlock(i,j);
					if(mb==null) continue;
					MazeManager.GetInstance().maze3DGrid.mazeBlockOccupied.Add(blockName, mb);
				}	
			}
		}
	}

	public static string GetMazeBlockName(int x, int y) {
		return MAZEBLOCKNAMEPREFIX + DELIMITER + x + DELIMITER + y;
	}

	public MazeBlock GetMazeBlock(MazePoint m){
		return GetMazeBlock(m.x,m.y);
	}
	public MazeBlock GetMazeBlock(int x, int y){
		return maze3DGrid.GetObjectAtPosition(x,y) as MazeBlock;
		/*
		string blockName = GetMazeBlockName(x,y);
		Transform blockObject = (Transform) mazeRoot.transform.Find(blockName);
		if(blockObject==null) return null;
		return blockObject.GetComponent<MazeBlock>();
		*/
	}

}
