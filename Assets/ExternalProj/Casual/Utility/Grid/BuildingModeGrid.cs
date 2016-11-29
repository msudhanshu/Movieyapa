using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The grid used in building mode.
 */
using System;

 
public class BuildingModeGrid: AbstractGrid
{
	public float gridWidth=5;						// Width of the grid in world units (not grid units)
	public float gridHeight=5;					// Height of the grid in world units (not grid units)
	public float gridUnusuableHeight;			// Height (from bottom AND top) that cannot be used (used to make the grid a square instead of a diamond).
	public float gridUnusuableWidth;			// Width from (left AND right) that cannot be used (used to make the grid a square instead of a diamond).
	
	protected static BuildingModeGrid instance;	// Static reference to this grid.
	
	protected bool initialised = false;			// Has this grid been initialised.

	private string _staticObstacleFile=null;
	public static string staticObstacleFileSuffix = "_obstacleJson";
	public string staticObstacleFileName {
		get {
			if(_staticObstacleFile==null) 
				_staticObstacleFile = Application.loadedLevelName + staticObstacleFileSuffix;
			Debug.Log("Static obstacle data json :"+_staticObstacleFile);
			return _staticObstacleFile;
		}
	}

	/**
	 * Get the instance of the grid class or create if one has not yet been created.
	 * 
	 * @returns An instance of the grid class.
	 */ 
	public static BuildingModeGrid GetInstance(){
		return instance;
	}
	
	/**
	 * If there is already a grid destroy self, else initialise and assign to the static reference.
	 */ 
	void Awake(){

		if (instance == null) {
			if (!initialised) Init();
			instance = (BuildingModeGrid) this;
		} else if (instance != this) {
			Destroy(gameObject);	
		}

	}

	public void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		//Gizmos.DrawWireCube (GameManager.GetInstance().gameView.transform.position + new Vector3(gridSizeX * gridWidth, 0, gridSizeY * gridHeight)/2.0f,
		 //                    new Vector3(gridSizeX * gridWidth, 1, gridSizeY * gridHeight) );
	}


	/**
	 * Initialise the grid.
	 */ 
	virtual public void Init() {
		initialised = true;
		grid = new IGridObject[gridSizeX, gridSizeY];
		//FillUnusableGrid ();
		//FillStaticObstacleGrid ();
		gridObjects = new Dictionary<IGridObject, List<GridPosition>> ();
	}
	
	/**
	 * Initialise the grid.
	 */ 
	virtual public void Init(int gridSizeX, int gridSizeY) {
		initialised = true;
		this.gridSizeX = gridSizeX;
		this.gridSizeY = gridSizeY;
		grid = new IGridObject[gridSizeX, gridSizeY];
		//FillUnusableGrid ();
		//FillStaticObstacleGrid ();
		gridObjects = new Dictionary<IGridObject, List<GridPosition>> ();
	}

	override public Vector3 GridPositionToArenaPosition(GridPosition position){
		return GridPositionToArenaPosition(position, new List<GridPosition> (GridPosition.DefaultShape));
	}

	override public Vector3 GridPositionToArenaPosition(GridPosition position, List<GridPosition> shape ) {
		return CalculateGridPositionToArenaPosition(position,shape);

	}

	override public Vector3 GridPositionToWorldPosition(GridPosition position) {
		return GridPositionToWorldPosition(position, new List<GridPosition> (GridPosition.DefaultShape));
	}

	//converts to Unity World
	override public Vector3 GridPositionToWorldPosition(GridPosition position, List<GridPosition> shape ) {
		return CalculateGridPositionToArenaPosition(position,shape) + GameManager.GetInstance().gameView.transform.position;
	}

	override protected Vector3 CalculateGridPositionToArenaPosition(GridPosition position, List<GridPosition> shape) {
		float x = (gridWidth / 2) * (position.x - position.y);
		float y = (gridHeight / 2)*  (position.x + position.y);
		float sz = 9999.0f;
		float lz= -9999.0f;
		float tsz;
		
		// TODO Clean up and fix to cater for even odder shapes
		foreach (GridPosition pos in shape) {
			tsz = ((position.y + pos.y) * (gridHeight / 2)) + ((position.x + pos.x)* (gridHeight / 2)) - 2;
			if (sz >= tsz) sz = tsz;
			if (lz <= tsz) lz = tsz;
		}	
		return new Vector3(x, y, ((lz + sz) / 2.0f) );	
	}

	override public GridPosition ArenaPositionToGridPosition(Vector3 position) {
		return CalculateArenaPositionToGridPosition (position);	
	}

	override public GridPosition WorldPositionToGridPosition(Vector3 position) {
		//Debug.Log ("Arenatrasforpos :" + GameManager.GetInstance().gameView.transform.position);
		return CalculateArenaPositionToGridPosition (position - GameManager.GetInstance().gameView.transform.position);	
	}

	override protected GridPosition CalculateArenaPositionToGridPosition(Vector3 position) {
		int tx = Mathf.RoundToInt(((position.x / (gridWidth / 2)) + (position.y / (gridHeight / 2))) / 2);
		int ty = Mathf.RoundToInt(((position.y / (gridHeight / 2)) - (position.x / (gridWidth / 2))) / 2);
		return new GridPosition(tx, ty);	
	}

	virtual public GridPosition getRandomFreeGridPosition(GridPosition[] shape){ //FIXME: Can get into infinte loop

		GridPosition centerPos;
		Ray ray = GameManager.GetInstance().gameCamera.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 1 << GameManager.TERRAIN_LAYER)) {
			centerPos =  WorldPositionToGridPosition(hit.point);
		}else
			centerPos = new GridPosition ((int) (gridSizeX / 2.0f),(int) (gridSizeY / 2.0f) );

		try {
			return ScanGridInSpiral(centerPos,shape);
		} catch (GridUnavailableException e) {
			throw new GridUnavailableException ();
		}
		/*
		while(true){
		int x = (int) Random.Range (0,gridSize);
		int y = (int) Random.Range (0,gridSize);
		GridPosition pos = new GridPosition(x,y);
		if(CanObjectBePlacedAtPosition(shape,pos))
			return pos;
		} */
	}

	public GridPosition ScanGridInSpiral(GridPosition centerPos, GridPosition[] shape) {
		//SCAN THE GRID IN SPIRAL FORM
		for (int radius=0; radius < MaxSpiralScanRadius(centerPos) ;radius++) {
			for (int i=-radius; i<radius; i++) {
				if(CanCharacterBePlacedAtPosition(shape,centerPos+new GridPosition(i,-radius)))
					return centerPos+new GridPosition(i,-radius);
				if(CanCharacterBePlacedAtPosition(shape,centerPos+new GridPosition(i,radius)))
					return centerPos+new GridPosition(i,radius);
			}
			//scan along Y direction on this squared-Ring
			for (int j=-radius+1; j<radius-1; j++) {
				if(CanCharacterBePlacedAtPosition(shape,centerPos+new GridPosition(-radius,j)))
					return centerPos+new GridPosition(-radius,j);
				if(CanCharacterBePlacedAtPosition(shape,centerPos+new GridPosition(radius,j)))
					return centerPos+new GridPosition(radius,j);
			}
		}
		throw new GridUnavailableException ();
	}

	virtual public bool CanCharacterBePlacedAtPosition(GridPosition[] shape, GridPosition position) {
		return CanObjectBePlacedAtPosition (shape, position);// && !CharacterManager.GetInstance ().GridPositionHasCharacter (position);
	}

	private int MaxSpiralScanRadius(GridPosition pos) {
		return Mathf.Max(pos.x, pos.y , gridSizeX-pos.x, gridSizeY - pos.y);
	}
	
	virtual public GridHeight GetTerrainHeightAtPosition(GridPosition gridPosition) {
		return new GridHeight (0);
	}

	/**
	 * Get the building at the given position. If the space is empty or the object
	 * at the position is not a building return null.
	 */ 
	public IGridObject GetGridObjectAtPosition(GridPosition position) {
		IGridObject result;
		result = GetObjectAtPosition(position);
		return result;
	}

	/**
	 * Fill up the grid so the diamon becomes a rectangle.
	 */ 
	private void FillUnusableGrid() {
		
		for (int y = 0; y < gridSizeX; y++) {
			for (int x = 0; x < gridSizeY; x++) {
				// Fill Bottom
				if (x+y < gridUnusuableHeight) {
					grid[x,y] = new UnusableGrid();
				}
				// Fill Top
				if (x + y > (gridSizeY * 2) - gridUnusuableHeight) {
					grid[x,y] = new UnusableGrid();
				}
				// Fill Left
				if (x - y > gridSizeX - gridUnusuableWidth) {
					grid[x,y] = new UnusableGrid();
				}
				// Fill Right
				if (y - x > gridSizeX - gridUnusuableWidth) {
					grid[x,y] = new UnusableGrid();
				}
			}
		}
		
		/*for (int y = 0; y < gridSize; y++) {
			for (int x = 0; x < gridSize; x++) {
				
			}
		}*/
		// Fill Left
		for (int y = 0; y < gridSizeY; y++) {
			for (int x = 0; x < gridSizeX; x++) {
				if (x + y > (gridSizeY * 2) - gridUnusuableHeight) {
					grid[x,y] = new UnusableGrid();
				}
			}
		}
		// Fill Right
		for (int y = 0; y < gridSizeY; y++) {
			for (int x = 0; x < gridSizeX; x++) {
				if (x + y > (gridSizeY * 2) - gridUnusuableHeight) {
					grid[x,y] = new UnusableGrid();
				}
			}
		}
	}

	/**
	 * Iterate over static obstacle map and Fill up the grid with static obstacle like water, plant..
	 */ 
	private void FillStaticObstacleGrid() {
		//get the map from file (file from assets or download it from server)
		List<StaticObstacleData> staticObstacleData = LoadStaticObstacleData (staticObstacleFileName);
		if (staticObstacleData == null) return;
		Debug.Log("loading static obstacle data :"+staticObstacleFileName);
		GameObject bakedObstacleForCharacter = new GameObject ("bakedObstacleForCharacter");
		foreach (StaticObstacleData griddata in staticObstacleData) {
			switch(griddata.obstacleType)  {
					case ObstacleType.NONE: 
					 	break;
					case ObstacleType.DEFAULT:
					//	MarkGridUnWalkableForCharacter(bakedObstacleForCharacter,new StaticObstacleGrid(griddata) );
						AddObstacleToGrid( new StaticObstacleGrid(griddata) );
					    break;
					case ObstacleType.FORBUILDING:
						AddObstacleToGrid( new StaticObstacleGrid(griddata) );
						break;

					case ObstacleType.FORCHARACTER:
					//	MarkGridUnWalkableForCharacter(bakedObstacleForCharacter, new StaticObstacleGrid(griddata) );
						break;
				}
		} 
	}

	//TEMP
	private void MarkGridUnWalkableForCharacter(GameObject parentObject, StaticObstacleGrid gridObject) {
		float colliderHeight = 8;
		GameObject dummy = new GameObject ("ObstTile-" + gridObject.Position.x + "-" + gridObject.Position.y);
		dummy.transform.parent = parentObject.transform;
		//dummy.AddComponent<DynamicGridObstacle> ();
		BoxCollider dummyCollider = dummy.AddComponent<BoxCollider> ();

		dummy.transform.position = GridPositionToWorldPosition (gridObject.Position);
		dummyCollider.center = new Vector3 (0, 0.1f, 0);
		dummyCollider.size = new Vector3(gridWidth,colliderHeight,gridHeight);
	}

	private void AddObstacleToGrid(StaticObstacleGrid gridObject) {
		GridPosition newPosition;
		foreach (GridPosition g in gridObject.Shape) {
			newPosition = gridObject.Position + g;
			grid[newPosition.x, newPosition.y] = gridObject;
		}
	}

	private List<StaticObstacleData> LoadStaticObstacleData( string dataFile) {
		Loader<StaticObstacleData> loader = new Loader<StaticObstacleData>();
		try {
			List <StaticObstacleData> data = loader.Load(dataFile);
		return data;
		}catch(System.Exception e) {
			return null;
		}
	}

}

public class GridUnavailableException : Exception {
	public override string Message {
		get {
			return base.Message;
		}
	}
}

