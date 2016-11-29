using UnityEngine;

#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityExtensions;

public class StaticObstacleBake : MonoBehaviour {
	private bool _editMap = false;
	public bool editMap {
				get { return _editMap;}
				set {
			_editMap = value;
			transform.GetChild(0).gameObject.SetActive(_editMap);
		}
		}
	private bool _autoBakeMap = false;
	public bool autoBakeMap {
		get { return _autoBakeMap;} 
		set {_autoBakeMap=value;
			if(_autoBakeMap)
				StartCoroutine (AutoBakeMap ());
		}
	}

	private bool _bakeMap = false;
	public bool bakeMap {
		get { return _bakeMap;} 
		set {_bakeMap=value;
			if(_bakeMap)
				StartCoroutine (BakeGridMap ());
		}
	}

	private bool _clearMap = false;
	public bool clearMap {
		get { return _clearMap;} 
		set {_clearMap=value;
			if(_clearMap)
				StartCoroutine (ClearMap ());
		}
	}

	public Transform ArenaGameView;
	
	/**
	 * Layer used for gridtile.
	 */ 
	public const int GRID_TILE_LAYER = 13;

	public Camera tileCamera;

	public string StaticObstacleFilePath = "Assets/Kiwi/Content/Data/Resources/";
	//public string StaticObstacleFile = "StaticObstacleData";

	public OBSTACLE_EDIT_MODE obstacleEditMode = OBSTACLE_EDIT_MODE.NONE;
	public ObstacleType obstacleType = ObstacleType.NONE;

	private Dictionary<string, GridTile> obstacleGridTiles = new Dictionary<string, GridTile> ();

	void Start() {
		if(ArenaGameView != null)
			transform.parent = ArenaGameView;
		StartCoroutine(GenerateGridView());
	}

	
	#region GridView
	public float TileGapFactor = 0.1f;
	public const string TILENAMEPREFIX = "TILE";
	public const string DELIMITER = "-";
	public GameObject gridTilePrefab;
	private Dictionary<string, GridTile> allGridTiles = new Dictionary<string, GridTile> ();
	
	virtual protected IEnumerator GenerateGridView () {
		yield return true;
		GenerateEmptyGrid ();
		LoadStaticObstacleData ();
	}
	
	private void GenerateEmptyGrid() {
		int sizeX = BuildingModeGrid.GetInstance().gridSizeX;
		int sizeY = BuildingModeGrid.GetInstance().gridSizeY;
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				IGridObject gridObject = BuildingModeGrid3D.GetInstance().GetObjectAtPosition(new GridPosition(x, y));
				if (!(gridObject is UnusableGrid))
				{
					GridTile gt = CreateGridTile(x, y);
					//gt.SetObstacleType(OBSTACLE_EDIT_MODE.NONE, ObstacleType.NONE);
					allGridTiles.Add(gt.TileName,gt);
				}
			}
		}
	}
	
	private void LoadStaticObstacleData() {
		Loader<StaticObstacleData> loader = new Loader<StaticObstacleData>();
		List<StaticObstacleData> staticObstacleData =  loader.Load (BuildingModeGrid3D.GetInstance().staticObstacleFileName);
		if (staticObstacleData == null) return;
		foreach (StaticObstacleData griddata in staticObstacleData) {
				AddObstacleToGrid( new StaticObstacleGrid(griddata) );
		}
	}
	
	private void AddObstacleToGrid(StaticObstacleGrid gridObject) {
		GridPosition newPosition;
		foreach (GridPosition g in gridObject.Shape) {
			newPosition = gridObject.Position + g;
			GridTile gt = allGridTiles[GetTileName(newPosition.x,newPosition.y)];
			gt.SetObstacleType(OBSTACLE_EDIT_MODE.ADD_OBSTACLE, gridObject.data.obstacleType);
			obstacleGridTiles.Add (gt.TileName,gt);
		}
	}
	
	private GridTile CreateGridTile(int x, int y)
	{
		Vector3 position = BuildingModeGrid3D.GetInstance().GridPositionToArenaPosition(new GridPosition(x,y));
		GridPosition g = BuildingModeGrid3D.GetInstance ().ArenaPositionToGridPosition (position+new Vector3(0.2f,0.3f,0.4f));
		//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); 	//Destroy(cube.GetComponent(typeof(BoxCollider)));
		GameObject cube = (GameObject)Instantiate (gridTilePrefab);
		cube.transform.ResetTransformation ();
		string tileName = GetTileName (x, y);
		cube.name = tileName;
		
		GridTile gt = cube.GetComponent<GridTile> ();
		if (gt != null) {
			gt.TileName = tileName;
			gt.x = x;
			gt.y = y;
		}
		
		cube.transform.parent = transform;
		cube.transform.localPosition = position;// - new Vector3(BuildingModeGrid3D.GetInstance().gridWidth/2,0,BuildingModeGrid3D.GetInstance().gridHeight/2);
		cube.layer = GRID_TILE_LAYER;
		cube.transform.localScale = new Vector3(BuildingModeGrid3D.GetInstance().gridWidth*(1-TileGapFactor),0.3f,BuildingModeGrid3D.GetInstance().gridHeight*(1-TileGapFactor));
		return gt;
	}

	private string GetTileName(int x, int y) {
		return TILENAMEPREFIX + DELIMITER + x + DELIMITER + y;
	}
	
	#endregion

	void Update() {

		if (editMap && !bakeMap)
						EditGridMap ();
	//	if (!editMap) return;
	//	if (!bakeMap) 
	//		EditGridMap ();
		//else 
		//	StartCoroutine (BakeGridMap ());
	}

	//do a ray cast with tile as filter.. and toggle its look and save it detail
	private void EditGridMap() {
		
		if (obstacleEditMode == OBSTACLE_EDIT_MODE.NONE) return;
		
		if (Input.GetMouseButton(0)) {
			Ray ray = tileCamera.ScreenPointToRay (UICamera.lastTouchPosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 10000, 1 << GRID_TILE_LAYER)) {
				GameObject tile = hit.collider.gameObject;
				GridTile gt = tile.GetComponent<GridTile>();
				
				switch(obstacleEditMode) {
				case OBSTACLE_EDIT_MODE.NONE:
					break;
				case OBSTACLE_EDIT_MODE.REMOVE_OBSTACLE:
					gt.SetObstacleType(obstacleEditMode , obstacleType);	
					obstacleGridTiles.Remove (gt.TileName);
					break;
				default:
					if( !obstacleGridTiles.ContainsKey(gt.TileName) ) {
					gt.SetObstacleType(obstacleEditMode, obstacleType);	
					obstacleGridTiles.Add (gt.TileName,gt);
					}
					break;
				}
			}
		}
	}

	private IEnumerator AutoBakeMap() {
		foreach(GridTile gt in allGridTiles.Values) {
			if( !obstacleGridTiles.ContainsKey(gt.TileName) ) {

				Vector3 cen = gt.transform.position;
				float gridHalfW = BuildingModeGrid3D.GetInstance().gridWidth/2.0f;
				float gridHalfH = BuildingModeGrid3D.GetInstance().gridHeight/2.0f;
				//right bottom
				Vector3 rb = gt.transform.position + new Vector3(gridHalfW,0,-gridHalfH);
				Vector3 rt = gt.transform.position + new Vector3(gridHalfW,0,gridHalfH);
				Vector3 lb = gt.transform.position + new Vector3(-gridHalfW,0,-gridHalfH);
				Vector3 lt = gt.transform.position + new Vector3(-gridHalfW,0,gridHalfH);
		  	    if(IsObstacle(cen) || IsObstacle(rb) || IsObstacle(rt) || IsObstacle(lb) || IsObstacle(lt) )
				{
					gt.SetObstacleType(OBSTACLE_EDIT_MODE.ADD_OBSTACLE, ObstacleType.DEFAULT);	
					obstacleGridTiles.Add (gt.TileName,gt);
				}
			}
		}
		autoBakeMap = false;
		yield return 0;
	}

	private bool IsObstacle(Vector3 tileCornerPos) {
		RaycastHit hit;	
		//TODO : add all type of obstacle layer in mask .. from hit get the layer type and bake the map accordingly
		int obstacleMask = 1 << BuildingModeGrid3D.STATIC_OBSTACLE_LAYER;
		if( Physics.Raycast(tileCornerPos -new Vector3(0,50,0), Vector3.up , out hit, 100, obstacleMask) ) {
			return true;
		}
		return false;
	}


	private IEnumerator ClearMap() {
		foreach(GridTile gt in obstacleGridTiles.Values)
		{
			gt.SetObstacleType(OBSTACLE_EDIT_MODE.REMOVE_OBSTACLE, ObstacleType.DEFAULT);	
		}
		obstacleGridTiles.Clear ();
		clearMap = false;
		yield return 0;
	}

	private IEnumerator BakeGridMap() {
		obstacleEditMode = OBSTACLE_EDIT_MODE.NONE;
		List<StaticObstacleData> dataToSave = new List<StaticObstacleData> ();
		foreach(GridTile gt in obstacleGridTiles.Values)
		{
			gt.BakeTile();
			StaticObstacleData sod = new StaticObstacleData();
			sod.position = new GridPosition(gt.x,gt.y);
			sod.obstacleType = gt.obstacleType;//ConvertObstacleEditModeToType(gt.obstacleMode);// ObstacleType.DEFAULT;
			dataToSave.Add(sod);
		}
		Loader<StaticObstacleData> loader = new Loader<StaticObstacleData>();
		loader.Save (dataToSave, GetStaticObstacleFileName ());
		Debug.Log("Static obstacle data backed in file " + GetStaticObstacleFileName());
		UnityEditor.AssetDatabase.Refresh ();
		bakeMap = false;
		//editMap = false;
		yield return 0;
	}

	private string GetStaticObstacleFileName() {
		#if ENABLE_JSON
		return StaticObstacleFilePath + BuildingModeGrid3D.GetInstance().staticObstacleFileName +".json";
		#else
			return StaticObstacleFilePath + BuildingModeGrid3D.GetInstance().staticObstacleFileName +".xml";
		#endif
	}

	public void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		//	Gizmos.DrawWireCube (ArenaTransform.position + new Vector3(gridSize * gridWidth, 0, gridSize * gridHeight)/2.0f, new Vector3(gridSize * gridWidth, 1, gridSize * gridHeight) );
	}

}



public enum OBSTACLE_EDIT_MODE {
	NONE,
	ADD_OBSTACLE,
//	ADD_BUILDING_OBSTACLE,
//	ADD_CHARACTER_OBSTACLE,
	REMOVE_OBSTACLE
};

#endif