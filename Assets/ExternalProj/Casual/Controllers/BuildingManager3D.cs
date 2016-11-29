using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KiwiCommonDatabase;

/**
 * A building manager that creates 3D buildings
 */ 
public class BuildingManager3D : Manager<BuildingManager3D>
{
	//Start Building Manager
	/**
	 * Percentage of resources reclaimed when selling a building.
	 */ 
	public static float RECLAIM_PERCENTAGE = 0.75f;
	
	/**
	 * Percentage of resources reclaimed as gold when selling a building. If set to 0
	 * you cannot sell the building for gold.
	 */ 
	public static float GOLD_SELL_PERCENTAGE = 0.01f;
	
	/**
	 * The number of seconds of speed-up each gold coin buys.
	 */ 
	public static float GOLD_TO_SECONDS_RATIO = 60;
	
	/**
	 * Prefab to use when creating buildings.
	 */ 
	public GameObject buildingPrefab;
	public GameObject cropPrefab;
	/**
	 * A list of data files (resources) containing building data
	 */ 
	public List<string> buildingDataFiles;
	
	/**
	 * How often to save data.
	 */ 
	public SaveMode saveMode;
	
	/**
	 * Building types mapped to ids.
	 */ 
	public Dictionary <string, Asset> types;
	
	/**
	 * Loader for loading the data.
	 */ 
	protected Loader<AssetData> loader;
	
	/**
	 * Individual buildings mapped to guids.
	 */ 
	protected Dictionary <string, Building> buildings;	
	
	/**
	 * List of buildings in progress
	 */ 
	protected List<Building> buildingsInProgress;
	
	/**
	 * Initialise the instance.
	 */
	/*override protected void Init() {
		types = new Dictionary<string, AssetData>();
		buildings = new Dictionary<string, Building>();
		buildingsInProgress = new List<Building>();
		
		if (buildingDataFiles != null){
			foreach(string dataFile in buildingDataFiles){	
				LoadBuildingDataFromResource(dataFile, false);
			}
		}
	}*/

	protected void LoadBuildingData() {
		types = new Dictionary<string, Asset>();
		buildings = new Dictionary<string, Building>();
		buildingsInProgress = new List<Building>();
		
		if (buildingDataFiles != null){
			foreach(string dataFile in buildingDataFiles){	
				LoadBuildingDataFromResource(dataFile, false);
			}
		}
	}
	
	override public void PopulateDependencies() {
		dependencies = new List<ManagerDependency>();
		dependencies.Add(ManagerDependency.DATA_LOADED);
	}	


	/**
	 * Get a list of each building type.
	 */ 
	virtual public List<Asset> GetAllBuildingTypes() {
		return types.Values.ToList();
	}
	
	/**
	 * Load the building type data from the given resource.
	 * 
	 * @param dataFile	Name of the resource to load data from.
	 * @param skipDuplicates	If false throw an exception if a duplicate is found.
	 */
	virtual public void LoadBuildingDataFromResource(string dataFile, bool skipDuplicates) {
		if (loader == null) loader = new Loader<AssetData>();
		List <AssetData> data = null;
		List<Asset> assets = null;
		if (ServerConfig.SERVER_ENABLED) {
			data = DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetData>().Where(b =>  (b.assetCategory==AssetCategoryEnum.HELPER)).ToList();
			assets = DatabaseManager.GetInstance().GetDbHelper().QueryForAll<Asset>();
			//data = new List<AssetData>();
			if (assets != null)
			foreach (Asset asset in assets) {
				data.Add(asset.ConvertToAssetData());
			}
			//data = DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetData>();
		} else {
			#if ENABLE_JSON
			data = loader.LoadJSON(dataFile);
			#else
			data = loader.LoadXML(dataFile);
			#endif
		}
		
		foreach (Asset type in assets) {
			try {
				types.Add(type.id, type);
			} catch (System.Exception ex) {
				if (!skipDuplicates) throw ex;
			}
		}
	}
	
	/**
	 * Return all completed buildings. Returns a copy of the list.
	 */ 
	virtual public List<Building> GetAllBuildings() {
		List<Building> result = new List<Building>();
		result.AddRange(buildings.Values);
		return result;
	}
	
	/**
	 * Return the type data for the given building id. Returns null if the building type is not found.
	 */ 
	virtual public Asset GetBuildingTypeData(string id) {
		if (types.ContainsKey(id)) {
			return types[id];
		}
		return null;
	}
	
	/**
	 * Return true if the player has at least one building of the given type.
	 */ 
	virtual public bool PlayerHasBuilding(string id) {
		if (buildings.Values.Where (b => b.asset.id == id).Count () > 0) return true;
		return false;
	}
	
	/**
	 * Return true if player can build the given building. Excludes
	 * resource costs as we can pop up an IAP purchase window here.
	 */ 
	virtual public bool CanBuildBuilding(string buildingTypeId) {
		if(!types.ContainsKey(buildingTypeId)) return false;
		if (types[buildingTypeId].level > ResourceManager.GetInstance().Level) return false;
		if (types.ContainsKey(buildingTypeId)) {
			foreach (string id in types[buildingTypeId].requireIds){
				if (!PlayerHasBuilding(id)) return false;
			}
		} else {
			Debug.LogError("Unknown building id: " + buildingTypeId);
			return false;
		}
		return true;
	}
	
	/**
	 * Return true if player can build the given building. Excludes
	 * resource costs as we can pop up an IAP purchase window here.
	 */
	virtual public bool CanBuildBuilding(Building building) {
		foreach (string id in building.asset.requireIds){
			if (!PlayerHasBuilding(id)) return false;
		}
		return true;
	}
	
	protected GameObject CreateAssetCatPrefab(string buildingTypeId) {
		string prefabName = types[buildingTypeId].assetCategory.PrefabName();
		return Util.InstantiatePrefab(prefabName,true);
	}

	/**
	 * Build a building.
	 */ 
	virtual public void CreateBuilding(string buildingTypeId, GridPosition buildingGridPosition) {
		if (CanBuildBuilding (buildingTypeId) && ResourceManager.GetInstance().CanBuild(GetBuildingTypeData(buildingTypeId))) {
			GameObject go = CreateAssetCatPrefab(buildingTypeId);
			go.transform.parent = GameManager.GetInstance().gameView.transform;
			Building building = go.GetComponent<Building>();
			
			building.Init(types[buildingTypeId], buildingGridPosition);
			ActiveBuilding = building;
			if (ActiveBuilding.asset.additionalCosts != null) {
				foreach (CustomResource cost in ActiveBuilding.asset.additionalCosts) {
					ResourceManager.GetInstance().RemoveCustomResource(cost.id, cost.amount);
				}
			}
			if ((int)saveMode < (int) SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
		} else {
			if (CanBuildBuilding (buildingTypeId)) {
				// TODO Show info message if not enough resource
			} else {
				Debug.LogError("Tried to build unbuildable building");
			}
		}
	}
	
	/**
	 * Create a building during loading process
	 */ 
	virtual public void CreateAndLoadBuilding(BuildingData data) {
		GameObject go= CreateAssetCatPrefab(data.assetId);
		go.transform.parent = GameManager.GetInstance().gameView.transform;
		Building building = go.GetComponent<Building>();
		
		//TODO : HACK : Since userasset table doesn't have height now, we are getting height from terrain.
		data.height = BuildingModeGrid3D.GetInstance ().GetTerrainHeightAtPosition (data.position);
		
		building.Init(GetBuildingTypeData(data.buildingTypeString), data);
		if (!building.State.InBuiltState()) 
			buildingsInProgress.Add (building);
		else 
			buildings.Add(building.uid, building);
		//Occupy grid if it isn't Expansion Asset
		if(!building.asset.IsExpansionAsset())
			BuildingModeGrid.GetInstance().AddObjectAtPosition(building, data.position);
		
		//Adding to State and Category Map
		BuildingData.AddToAssetStateMap (data);
		BuildingData.AddToAssetCategoryMap (data);
	}
	
	
	/**
	 *	Place active building on the grid. 
	 *
	 * Returns true if successful.
	 */
	virtual public bool PlaceBuilding() {
		if (CanBuildBuilding (ActiveBuilding)) {
			ActiveBuilding.Place();
			buildingsInProgress.Add (ActiveBuilding);
			BuildingModeGrid.GetInstance().AddObjectAtPosition(ActiveBuilding, ActiveBuilding.Position);
			//GameEventTask.notifyAction("buy_"+ActiveBuilding.BuildingData.assetId);
			if ((int)saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
			
			Dictionary<IGameResource, int> costDiff = ActiveBuilding.asset.GetCostDiff();
			
			ResourceManager.GetInstance().AddResources(costDiff);
			ServerAction.takeAction(EAction.PURCHASE, ActiveBuilding.BuildingData, 
			                        ServerSyncManager.GetInstance().serverNotifier, costDiff);
			//FIXME: Hack to send ASSET_STATE notify for first state
			UserQuestTask.notifyAction(ActivityTaskType.ASSET_STATE, ActiveBuilding.BuildingData.asset, ActiveBuilding.BuildingData.assetState);
			ActiveBuilding = null;
			return true;
		}
		return false;
	}
	
	/**
	 *	Move active building on the grid. 
	 *
	 *  Returns true if successful.
	 */
	virtual public bool MoveBuilding() {
		BuildingModeGrid.GetInstance().RemoveObject(ActiveBuilding);
		ActiveBuilding.Position = ActiveBuilding.MovePosition;
		ActiveBuilding.Height = ActiveBuilding.MoveHeight;
		BuildingModeGrid.GetInstance().AddObjectAtPosition(ActiveBuilding, ActiveBuilding.Position);
		ActiveBuilding.FinishMoving ();
		if ((int)saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
		
		ServerAction.takeAction(EAction.UPDATE, ActiveBuilding.BuildingData, ServerSyncManager.GetInstance().serverNotifier);
		return true;
	}
	
	/**
	 *	Finish active building for gold, or speed up activity if its already built.
	 *
	 *  Returns true if successful.
	 */
	virtual public bool SpeedUp() {
		if (ActiveBuilding == null) {
			Debug.LogError ("Active Building was NULL");
			return false;	
		}
		if (ActiveBuilding.CurrentTransition == null) {
			Debug.LogError ("Current activity was NULL");
			return false;	
		}
		int cost = ((int)Mathf.Max (1, (float) (ActiveBuilding.CurrentTransition.RemainingTime + 1 ) / (float)BuildingManager3D.GOLD_TO_SECONDS_RATIO));
		if (ResourceManager.GetInstance().Gold >= cost) {
			ResourceManager.GetInstance().RemoveGold(cost);
			if (ActiveBuilding.CurrentTransition.Type == StateTransitionType.CONSTRUCT) {
				ActiveBuilding.CompleteBuild();
				AcknowledgeBuilding(ActiveBuilding);
			} else {
				ActiveBuilding.SpeedUp();
			}
		} else {
			return false;
		}
		return true;
	}
	
	/**
	 * Acknowledge building changing it from READY
	 * to BUILT.
	 */ 
	virtual public bool AcknowledgeBuilding(Building building) {
		if (building.State.IsReady()) {
			buildings.Add(building.uid, building);
			buildingsInProgress.Remove(building);
			ResourceManager.GetInstance().AddXp(GetXpForBuildingBuilding(building));
			if ((int)saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
			
			return true;
		}
		return false;
	}
	
	/**
	 * Sell building. 
	 *
	 * Returns true if successful.
	 */
	virtual public bool SellBuilding(Building building, bool savedBuilding = true) {
		if (buildings.ContainsKey (building.uid)) buildings.Remove (building.uid);

		if(savedBuilding) //if (building.dragState != DragState.PLACING && building.dragState != DragState.PLACING_INVALID )
			BuildingModeGrid.GetInstance ().RemoveObject (building);

		if (ActiveBuilding == building) ActiveBuilding = null;
		CharacterManager.GetInstance().FreeAssignedHelper(building);

		
		if (savedBuilding) {
			//TODO: Old code, add resources according to req
			Dictionary<IGameResource, int> sellValue = building.BuildingData.asset.GetSellValue();
			ResourceManager.GetInstance().AddResources(sellValue);
			ServerAction.takeAction(EAction.SELL, building.BuildingData, ServerSyncManager.GetInstance().serverNotifier, sellValue);
		}

		Destroy (building.gameObject);
		if ((int)saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
		return true;
	}
	
	/**
	 * delete building. 
	 *
	 * Returns true if successful.
	 */
	virtual public bool DeleteBuilding(Building building, Dictionary<IGameResource, int> rewards) {
		if (buildings.ContainsKey (building.uid)) buildings.Remove (building.uid);
		BuildingModeGrid.GetInstance ().RemoveObject (building);
		if (ActiveBuilding == building) ActiveBuilding = null;
		CharacterManager.GetInstance().FreeAssignedHelper(building);
		
		ServerAction.takeAction(EAction.DELETE,building.BuildingData,ServerSyncManager.GetInstance().serverNotifier, rewards);
		
		if(Pool.Contains(building.gameObject)) 
			Pool.Destroy(building.gameObject);
		else
			Destroy (building.gameObject);
		
		if ((int)saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
		return true;
	}
	
	/**
	 * Sells building for gold. 
	 *
	 * Returns true if successful.
	 */
	virtual public bool SellBuildingForGold(Building building) {
		if (buildings.ContainsKey (building.uid)) buildings.Remove (building.uid);
		ResourceManager.GetInstance().AddGold ((int)Mathf.Max(1.0f, (int)(building.asset.cost * GOLD_SELL_PERCENTAGE)));
		BuildingModeGrid.GetInstance ().RemoveObject (building);
		if (ActiveBuilding == building) ActiveBuilding = null;
		CharacterManager.GetInstance().FreeAssignedHelper(building);
		
		ServerAction.takeAction(EAction.SELL,building.BuildingData,ServerSyncManager.GetInstance().serverNotifier);
		Destroy (building.gameObject);
		if ((int)saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
		return true;
	}
	
	/**
	 * Clear an obstacle.
	 *
	 * Returns true if successful.
	 */
	virtual public bool ClearObstacle(Building building) {
		if (buildings.ContainsKey (building.uid)) buildings.Remove (building.uid);
		switch (building.asset.generationType) {
			// TODO Special cases
		case RewardType.RESOURCE : 
			ResourceManager.GetInstance().AddSilver (building.asset.generationAmount);
			break;
		case RewardType.GOLD : 
			ResourceManager.GetInstance().AddGold (building.asset.generationAmount);
			break;
		}
		BuildingModeGrid.GetInstance ().RemoveObject (building);
		if (ActiveBuilding == building) ActiveBuilding = null;
		Destroy (building.gameObject);
		if ((int)saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
		return true;
	}
	
	/**
	 * Create an Obstacle in the scene (generally used on first load).
	 */ 
	virtual protected void CreateObstacle (string buildingTypeId, GridPosition pos) {
		if (!types.ContainsKey(buildingTypeId)) {
			Debug.LogWarning("Tried to create an obstacle with an invalid ID");	
			return;
		}
		Asset type = types[buildingTypeId];
		if (!type.isObstacle) {
			Debug.LogError("Tried to create an obstacle with non-obstacle building data");	
			return;
		}
		GameObject go = (GameObject) Instantiate(buildingPrefab);
		go.transform.parent = GameManager.GetInstance().gameView.transform;
		Building obstacle = go.GetComponent<Building>();
		obstacle.Init(type, pos);
		BuildingModeGrid.GetInstance().AddObjectAtPosition(obstacle, pos);
		buildings.Add(obstacle.uid, obstacle);
		if ((int)saveMode < (int) SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
	}
	
	/**
	 * Add a building to the list (for example for paths or if you have other custom builders).
	 */ 
	virtual public void AddBuilding(Building building)
	{
		buildings.Add(building.uid, building);
	}
	
	/**
	 * Remove a building from the list (for example for paths or if you have other custom builders).
	 */ 
	virtual public void RemoveBuilding(Building building)
	{
		buildings.Remove(building.uid);
	}
	
	
	/**
	 * Building currently being placed/moved/interacted with.
	 */ 
	protected static Building _activeBuilding;
	public static Building ActiveBuilding {
		get { return _activeBuilding; }
		set {
			// Make sure we cancel any moving if we click another bulding
			if (_activeBuilding != null)
				_activeBuilding.ResetView();
			_activeBuilding = value;
		}
	}
	
	/**
	 * Gets built and in progress building state for use by the save game system.
	 */ 
	virtual public List<BuildingData> GetSaveData() {
		List<BuildingData> dataToSave = new List<BuildingData>();
		dataToSave.AddRange(buildingsInProgress.Select(b=>b.BuildingData).ToList());
		dataToSave.AddRange(buildings.Values.Select(b=>b.BuildingData).ToList());
		return dataToSave;
	}
	
	/// <summary>
	/// Gets the xp for building the provided building.
	/// </summary>
	/// <returns>The xp for building building.</returns>
	/// <param name="building">Building.</param>
	virtual public int GetXpForBuildingBuilding(Building building) {
		return (building.asset.level + 1) * building.asset.cost;
	}

	///////////////End building Manager, Start Building Manager 3d///////////
	/**
	 * Layer used for terrain collider.
	 */ 
	public const int TERRAIN_LAYER = 10;

	/**
	 * Layer used for building colliders.
	 */ 
	public const int BUILDING_LAYER = 11;

	public const int SELECTION_LAYER = 12;

	public static Dictionary<string,int> userQuestTaskMap;

	override public void StartInit () {
		LoadBuildingData(); //marketData
		CharacterManager.GetInstance().LoadUserCharacters(); //userAnimalHelpers
		LoadPlayerGame(); //userAssets : Buildings + RPGCharacter, userQuests
	}

	protected void LoadPlayerGame() {

		ServerConfig.LOADING_SERVER_DATA = true;

		if (PersistenceManager.GetInstance() != null) {
			List<BuildingData> data = null;
			if(ServerConfig.SERVER_ENABLED){
				//data = DataHandler.wrapper.buildingData;
				//data = DataHandler.wrapper.userBuildings;
				data = DataHandler.wrapper.userAssets;
				foreach(BuildingData building in data){
					if(types[building.buildingTypeString].assetCategoryEnum == AssetCategoryEnum.HELPER){
						//						CharacterManager.GetInstance().CreateAndLoadCharacter(building);
					} else if(types[building.buildingTypeString].IsRpgCharacter()) {
						CharacterManager.GetInstance().CreateAndLoadCharacter(building);
					}else
						CreateAndLoadBuilding(building);
				}
				foreach(BuildingData building in data) {
					if(types[building.buildingTypeString].assetCategoryEnum == AssetCategoryEnum.HELPER){
						//CharacterManager.GetInstance().CreateAndLoadCharacter(building);
					}
				}
				ResourceManager.GetInstance().Load();
				ResourceManager.GetInstance().InitializeUserLevel();
				ResourceManager.GetInstance().InitializeUserCollectable();

				//FIXME: temp location to starting quest system
				startQuestSystem(DataHandler.wrapper);
			}
			if (PersistenceManager.GetInstance().SaveGameExists() && !ServerConfig.SERVER_ENABLED) {
				SaveGameData savedGame = PersistenceManager.GetInstance().Load();
				if(!ServerConfig.SERVER_ENABLED){
					data = savedGame.buildings;
					//FIXME : load building fIRST then the characters
					foreach(BuildingData building in data){
						if(types[building.buildingTypeString].assetCategoryEnum == AssetCategoryEnum.HELPER){
							//						CharacterManager.GetInstance().CreateAndLoadCharacter(building);
						} else if(types[building.buildingTypeString].IsRpgCharacter()) {
							CharacterManager.GetInstance().CreateAndLoadCharacter(building);
						}else
							CreateAndLoadBuilding(building);
					}
					foreach(BuildingData building in data) {
						if(types[building.buildingTypeString].assetCategoryEnum == AssetCategoryEnum.HELPER){
							//CharacterManager.GetInstance().CreateAndLoadCharacter(building);
						}
					}
					ResourceManager.GetInstance().Load(savedGame);
				}
				ActivityManager.GetInstance().Load(savedGame);
			}
		}
		ServerConfig.LOADING_SERVER_DATA = false;
//		CreateNewScene ();
	}

	public static void startQuestSystem(UserDataWrapper userDataWrapper){
		userQuestTaskMap = new Dictionary<string, int>();

		if(userDataWrapper.userQuests == null)
			return;
		
		foreach(UserQuestTask userQuestTask in userDataWrapper.userQuestTasks){
			userQuestTaskMap.Add(userQuestTask.questTaskId, userQuestTask.currentCount);
		}
		
		
		if(!UserQuest.isQuestSystemActivated()){
			foreach(UserQuest quest in userDataWrapper.userQuests)
				quest.populate();
			
		}
		//Dispose the hash-map
		userQuestTaskMap.Clear();
	}

	/**
	 * Build a building. override this as we want to determine building position differently.
	 */ 
	public void CreateBuilding(string buildingTypeId) {
		if (CanBuildBuilding (buildingTypeId) && ResourceManager.GetInstance().CanBuild(GetBuildingTypeData(buildingTypeId))) {
			GameObject go = CreateAssetCatPrefab(buildingTypeId);
			go.transform.parent = GameManager.GetInstance().gameView.transform;
			Building building = go.GetComponent<Building>();
			Ray ray = GameManager.GetInstance().gameCamera.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
			RaycastHit hit;
			if (Physics.Raycast(ray,out hit, 1 << TERRAIN_LAYER)) {
				GridPosition buildingGridPosition  =   BuildingModeGrid3D.GetInstance().getRandomFreeGridPosition(BuildingManager3D.GetInstance().types[buildingTypeId].shape.ToArray());
				building.Init(types[buildingTypeId], buildingGridPosition, 
					new GridHeight(hit.point.y - GameManager.GetInstance().gameView.transform.position.y) );
				ActiveBuilding = building;
				ResourceManager.GetInstance().RemoveSilver(ActiveBuilding.asset.cost);
				if ((int)saveMode < (int) SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
			} else {
				Debug.LogWarning ("Couldn't find terrain, not able to place building.");
			}
			
		} else {
			if (CanBuildBuilding (buildingTypeId)) {
				Debug.LogWarning("This is where you bring up your in app purchase screen");
			} else {
				Debug.LogError("Tried to build unbuildloadable building");
			}
		}
	}

	/**
	 * Override as no obstacles in the 3D view.
	 */ 
	protected void CreateNewScene() {

	}

	public Building GetBuildingById(long uid){
		if (buildings.ContainsKey (uid.ToString()))
			return buildings [uid.ToString()];
		return null;
	}

/*	public static BuildingManager3D Get3DInstance(){
		return BuildingManager3D.GetInstance() as BuildingManager3D;
	}
	*/
}

