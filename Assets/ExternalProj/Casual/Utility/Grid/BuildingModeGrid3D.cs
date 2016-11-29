using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The grid used in building mode for the 3D view.
 */ 
public class BuildingModeGrid3D: BuildingModeGrid
{
	public int PlaceableSteepness = 30;
	public static int STATIC_OBSTACLE_LAYER = 14;
	/**
	 * If there is already a grid destroy self, else initialise and assign to the static reference.
	 */ 
	void Awake(){
		if (instance == null) {
			if (!initialised) Init();
			instance = (BuildingModeGrid3D) this;
		} else if (instance != this) {
			Destroy(gameObject);	
		}
	}

	/**
	 * Initialise the grid.
	 */ 
	override public void Init() {
		base.Init ();
	}
	override public void Init(int gridSizeX, int gridSizeY) {
		base.Init(gridSizeX,gridSizeY);
	}

	/**
	 * Get the instance of the grid class or create if one has not yet been created.
	 * 
	 * @returns An instance of the grid class.
	 */ 
	public static BuildingModeGrid3D Get3DInstance(){
		return instance as BuildingModeGrid3D;
	}

	override protected Vector3 CalculateGridPositionToArenaPosition(GridPosition position, List<GridPosition> shape) {
		//TODO : take shape into consideration: return center of the shape
		return new Vector3(position.x * gridWidth, 0, position.y * gridHeight);
	}
	
	override protected GridPosition CalculateArenaPositionToGridPosition(Vector3 position) {
		return new GridPosition((int)((position.x)/ gridWidth), (int)((position.z) / gridHeight));
	}

	/**
	 * Fill up the grid.
	 */ 
	protected void FillUnusableGrid() {
		// No need for unusable grid in 3D view as its already a rectangle.
		if (gridUnusuableHeight > 0 || gridUnusuableWidth > 0) Debug.LogWarning("Unusable grid is ignored in 3D mode");
	}


	override public bool CanObjectBePlacedAtPosition(IGridObject gridObject, GridPosition gridPosition) {
		return base.CanObjectBePlacedAtPosition (gridObject, gridPosition) &&
			CanShapeBePlacedAtSteepPosition(gridObject.Shape.ToArray(),gridPosition) && 
			!PositionContainsStaticObstacle(gridObject.Shape.ToArray(),gridPosition);
	}

	virtual public bool CanObjectBePlacedAtPosition(IGridObject gridObject, Vector3 position) {
		GridPosition gridPosition = WorldPositionToGridPosition(position);
		return CanObjectBePlacedAtPosition (gridObject, gridPosition);
	}

	override public bool CanObjectBePlacedAtPosition(GridPosition[]  shape, GridPosition gridPosition) {
		return base.CanObjectBePlacedAtPosition (shape, gridPosition) && 
			CanShapeBePlacedAtSteepPosition(shape,gridPosition) && 
			!PositionContainsStaticObstacle(shape,gridPosition);
	}

	private bool CanShapeBePlacedAtSteepPosition(GridPosition[]  shape, GridPosition gridPosition) {
		GridPosition newPosition;
		foreach (GridPosition g in shape) {
			newPosition = gridPosition + g;
//			Debug.Log ("grid position picked " + gridPosition.x +","+gridPosition.y);
//			Debug.Log ("grid position in world coord picked " + GridPositionToWorldPosition (newPosition));	
			Vector3 terrainLocalPos = GameManager.GetInstance().terrain.transform.InverseTransformPoint (GridPositionToWorldPosition (newPosition));
			Vector2 normalizedPos = new Vector2 (Mathf.InverseLerp (0.0f, GameManager.GetInstance().terrain.terrainData.size.x, terrainLocalPos.x),
			                                     Mathf.InverseLerp (0.0f, GameManager.GetInstance().terrain.terrainData.size.z, terrainLocalPos.z));
			float steepness = GameManager.GetInstance().terrain.terrainData.GetSteepness (normalizedPos.x, normalizedPos.y);
//			Debug.Log ("slopenormal " + steepness);
			if((int)steepness > PlaceableSteepness) {
				return false;
			}
		}
		return true;
	}

	private bool PositionContainsStaticObstacle(GridPosition[]  shape, GridPosition gridPosition)  {
		RaycastHit hit;
		int obstacleMask = 1 << STATIC_OBSTACLE_LAYER;
		GridPosition newPosition;
		foreach (GridPosition g in shape) {
			newPosition = gridPosition + g;
			if( Physics.Raycast(GridPositionToWorldPosition (newPosition)-new Vector3(0,50,0) , Vector3.up , out hit, 1000, obstacleMask) ) {
				return true;
			} 
			
		}
		return false;
	}

	 public Vector3 SnapPositionToTerrainHeight(GridPosition gridPosition) {
		return new Vector3( GridPositionToWorldPosition(gridPosition).x , 
		                   GetTerrainHeightAtPosition(gridPosition).h, 
		                   GridPositionToWorldPosition(gridPosition).z );
	}

	 public Vector3 SnapPositionToTerrainHeight(Vector3 position) {
		return new Vector3( position.x , 
		                   GetTerrainHeightAtPosition(position).h, 
		                   position.z );
	}

	//get height of the terrain (terrain collider at this point )
	override public GridHeight GetTerrainHeightAtPosition(GridPosition gridPosition) {
		float ObjectSizeHeight = GameManager.GetInstance().terrain.transform.position.y;
		return new GridHeight(GameManager.GetInstance().terrain.SampleHeight(GridPositionToWorldPosition(gridPosition)) + ObjectSizeHeight ) ;
	}

	 public GridHeight GetTerrainHeightAtPosition(Vector3 position) {
		float ObjectSizeHeight = GameManager.GetInstance().terrain.transform.position.y;
		return new GridHeight(GameManager.GetInstance().terrain.SampleHeight(position) + ObjectSizeHeight ) ;
	}
}

