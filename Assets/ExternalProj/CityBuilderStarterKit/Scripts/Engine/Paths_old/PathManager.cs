using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Provides convenience functions for working with paths
 */ 
public class PathManager : Manager <PathManager> {

	/**
	 * Prefab to use when creating paths.
	 */ 
	public GameObject pathPrefab;

	
	/**
	 * How often to save data.
	 */ 
	public SaveMode saveMode;

	/**
	 * What separates sprite name from the direction flags
	 */ 
	protected const string SUFFIX = "-";

	/**
	 * Reference to grid
	 */ 
	protected BuildingModeGrid grid;

	override public void StartInit() {
		grid = BuildingModeGrid.GetInstance();
	}

	override public void PopulateDependencies() {
		dependencies = new List<ManagerDependency>();
	}

	/**
	 *	Place path on the grid. 
	 */
	virtual public void SwitchPath(string buildingTypeId, Vector3 worldPosition) {
		Building building = null;

		//HACK : MANJEET : WE changed assetdata to asset
	//	AssetData data = BuildingManager3D.GetInstance().GetBuildingTypeData(buildingTypeId);
		Asset data = BuildingManager3D.GetInstance().GetBuildingTypeData(buildingTypeId);

		GridPosition pos = grid.ArenaPositionToGridPosition(worldPosition);

		// If path exists remove and give resources
		IGridObject gridObject = grid.GetObjectAtPosition(pos);
		if (gridObject is Building && ((Building)gridObject).asset.isPath) {
			ResourceManager.GetInstance().AddSilver (((Building)gridObject).asset.cost);
			if (((Building)gridObject).asset.additionalCosts != null) {
				foreach (CustomResource cost in ((Building)gridObject).asset.additionalCosts) {
					ResourceManager.GetInstance().AddCustomResource (cost.id,  cost.amount);
				}
			}
			grid.RemoveObject(gridObject);
			BuildingManager3D.GetInstance().RemoveBuilding((Building)gridObject);
			Destroy (((Building)gridObject).gameObject);
		}

		// Add new path unless we just deleted a path of same type
		if (	(gridObject == null || !(gridObject is Building) || ((Building)gridObject).asset.id != buildingTypeId) &&
		    	data.isPath && BuildingManager3D.GetInstance().CanBuildBuilding (buildingTypeId) && 
		    grid.CanObjectBePlacedAtPosition(data.shape.ToArray(), pos) && 
		    	ResourceManager.GetInstance().CanBuild(data)) {
			GameObject go;
			go = (GameObject) Instantiate(pathPrefab);
			go.transform.parent = GameManager.GetInstance().gameView.transform;
			building = go.GetComponent<Building>();
			building.Init(data, pos);
			building.Acknowledge();
			ResourceManager.GetInstance().RemoveSilver(data.cost);
			if (data.additionalCosts != null) {
				foreach (CustomResource cost in data.additionalCosts) {
					ResourceManager.GetInstance().RemoveCustomResource(cost.id, cost.amount);
				}
			}
			grid.AddObjectAtPosition(building, pos);
			BuildingManager3D.GetInstance().AddBuilding(building);
			if ((int)saveMode == (int) SaveMode.SAVE_ALWAYS) PersistenceManager.GetInstance().Save();
		} else {
			if (BuildingManager3D.GetInstance ().CanBuildBuilding (buildingTypeId)) {
				// TODO Show info message if not enough resource
			} else {
				Debug.LogError("Tried to build unbuildable path");
			}
		}

		UpdatePosition(pos, building);

		if (GridView.GetInstance() != null) GridView.GetInstance().PathMode();
	}

	/**
	 * Started path bulding
	 */ 
	virtual public void EnterPathMode()
	{
		
	}

	/**
	 * Finished path bulding, make sure to save.
	 */ 
	virtual public void ExitPathMode()
	{
		if ((int)saveMode <(int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
	}

	/**
	 * Create a path during loading process
	 */ 
	virtual public void CreateAndLoadPath(BuildingData data) {
		GameObject go;
		go = (GameObject) Instantiate(pathPrefab);
		go.transform.parent = GameManager.GetInstance().gameView.transform;
		Building building = go.GetComponent<Building>();
		building.Init(BuildingManager3D.GetInstance().GetBuildingTypeData(data.buildingTypeString), data);
		grid.AddObjectAtPosition(building, data.position);
		BuildingManager3D.GetInstance().AddBuilding(building);
		building.Acknowledge();
		UpdatePosition(data.position, building);
	}

	/**
	 * Switch a grid position to be path (or not path).
	 */ 
	virtual public void UpdatePosition (GridPosition pos, Building building) {

		List<GridPosition> positions = new List<GridPosition>();
		positions.Add (pos);
		positions.Add (pos +  new GridPosition(1, 0));
		positions.Add (pos +  new GridPosition(-1, 0));
		positions.Add (pos +  new GridPosition(0, 1));
		positions.Add (pos +  new GridPosition(0, -1));


		if (building != null && pos != building.Position)
		{
			grid.RemoveObjectAtPosition(building.Position);
			positions.Add (building.Position);
			positions.Add (building.Position +  new GridPosition(1, 0));
			positions.Add (building.Position +  new GridPosition(-1, 0));
			positions.Add (building.Position +  new GridPosition(0, 1));
			positions.Add (building.Position +  new GridPosition(0, -1));
			building.Position = pos;
		}

		UpdatePositions(positions.Distinct().ToList());

	}

	/**
	 * Update all the views for the given UI position. 
	 */
	virtual public void UpdatePositions(List<GridPosition> positions) {
		// Not super efficient but it does the job
		foreach (GridPosition pos in positions) {
			IGridObject gridObject = grid.GetObjectAtPosition(pos);
			if (gridObject is Building && ((Building)gridObject).asset.isPath) {
				((Building)gridObject).gameObject.SendMessage ("UI_UpdateState", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	/// <summary>
	/// Gets the sprite suffix to use for a path at the given grid position.
	/// </summary>
	/// <returns>The sprite suffix.</returns>
	/// <param name="position">Position.</param>
	virtual public string GetSpriteSuffix(Building building)
	{
		string suffix = SUFFIX;

		if (grid == null) grid = BuildingModeGrid.GetInstance();

		IGridObject gridObject = grid.GetObjectAtPosition(building.Position + new GridPosition(1, 0));
		if (gridObject != building && gridObject != null && gridObject is Building && ((Building)gridObject).asset == building.asset) suffix += "N";
		
		gridObject = grid.GetObjectAtPosition(building.Position + new GridPosition(0, -1));
		if (gridObject != building && gridObject != null &&  gridObject is Building && ((Building)gridObject).asset == building.asset) suffix += "E";
		
		gridObject = grid.GetObjectAtPosition(building.Position + new GridPosition(-1, 0));
		if (gridObject != building && gridObject != null &&  gridObject is Building && ((Building)gridObject).asset == building.asset) suffix += "S";
		
		gridObject = grid.GetObjectAtPosition(building.Position + new GridPosition(0, 1));
		if (gridObject != building && gridObject != null &&  gridObject is Building && ((Building)gridObject).asset == building.asset) suffix += "W";
		
		// Default to the open path
		if (suffix == SUFFIX) suffix = "-NESW";
		
		return suffix;
	}

}
