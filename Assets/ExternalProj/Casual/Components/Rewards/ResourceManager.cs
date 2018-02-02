using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expansion;

/**
 * Handles resources, gold, etc.
 */ 
public class ResourceManager : Manager<ResourceManager>, IExpansionResource {
	
	/**
	 * View of resources.
	 */
	public GameObject view;

	//TODO : manjeet : once unity is upgraded
//	public UnityEvent;
	public delegate void ResourceUpdateAction();
	public static event ResourceUpdateAction OnResourceUpdated;

	/**
	 * Default for resources when new game is started.
	 */ 
	public int defaultSilver = 400;
	
	/**
	 * Default for gold when new game is started.
	 */ 
	public int defaultGold = 20;

	/**
	 * A list of data files (resources) containing custom resource data
	 */ 
	public List<string> resourceDataFiles;

	private Dictionary<IGameResource, int> resources;

	private Dictionary<IGameResource, Level> currentLevelMap;
	private Dictionary<IGameResource, Level> nextLevelMap;

	private int GetResourceValue(string resType) {
			return GetResourceValue (DatabaseManager.GetDbResource (resType));
	}

	private int GetResourceValue(IGameResource res) {
		if (!resources.ContainsKey (res))
			resources.Add (res, 0);
		return resources[res];
	}

	private void SetResourceValue(string resType, int quantity) {
			DbResource resource = DatabaseManager.GetDbResource (resType);

			if (!resources.ContainsKey (resource))
				resources.Add (resource, 0);
			resources [resource] = quantity;
	}

	public int GetCollectableValue(Collectable collectable){
		if (resources.ContainsKey (collectable))
			return resources [collectable];
		return 0;
	}

	public void SetCollectableValueDiff(Collectable collectable, int diffCount){
		int currentCount = GetCollectableValue (collectable);
		int newCount = currentCount + diffCount;
		resources [collectable] = newCount;

		//Event to Quest System
		if (diffCount > 0) {
			UserQuestTask.notifyAction (ActivityTaskType.COLLECTABLE_ACTIVITY, collectable, GameResourceActivityTaskType.EARN, diffCount);
			UserQuestTask.notifyAction (ActivityTaskType.COLLECTABLE_ACTIVITY, collectable, GameResourceActivityTaskType.POSSESS, newCount);
		}
		else 
			UserQuestTask.notifyAction (ActivityTaskType.COLLECTABLE_ACTIVITY, collectable, GameResourceActivityTaskType.SPEND, -diffCount);

		ServerAction.SendCollectableUpdate (collectable, diffCount, newCount);
	}
	
	/**
	 * The resource used for actually building buildings. In some games this is called coins or gp.
	 */ 
	virtual public int Silver {
		get {
			return GetResourceValue(DbResource.RESOURCE_SILVER);
		}
		protected set {
			SetResourceValue(DbResource.RESOURCE_SILVER, value);
		}
	}

	virtual public int Axe {
		get {
			return GetResourceValue(DbResource.RESOURCE_AXE);
		}
		protected set {
			SetResourceValue(DbResource.RESOURCE_AXE, value);
		}
	}

	virtual public int Cheer {
		get {
			return GetResourceValue(DbResource.RESOURCE_CHEER);
		}
		protected set {
			SetResourceValue(DbResource.RESOURCE_CHEER, value);
		}
	}

	/**
	 * The resource used to speed things up. Some games might call this gems or cash.
	 */ 
	virtual public int Gold {
		get {
			return GetResourceValue(DbResource.RESOURCE_GOLD);
		}
		protected set {
			SetResourceValue(DbResource.RESOURCE_GOLD, value);
		}
	}

	virtual public int Happiness {
		get {
			return GetResourceValue(DbResource.RESOURCE_HAPPINESS);
		}
		protected set {
			SetResourceValue(DbResource.RESOURCE_HAPPINESS, value);
		}
	}

	/**
	 * The experiecne of the current player.
	 */ 
	virtual public int Xp {
		get {
			return GetResourceValue(DbResource.RESOURCE_XP);
		}
		protected set {
			SetResourceValue(DbResource.RESOURCE_XP, value);
		}
	}

	private int GetLevel(DbResource resource) {
		if(!currentLevelMap.ContainsKey(resource)){
			InitializeUserLevel();
		}

		Level level = currentLevelMap[resource];
		if (level != null)
			return level.level;
		else
			return 1;
	}
	
	/**
	 * Level of the player calculated from experience.
	 */ 
	virtual public int Level {
		get
		{
			return GetLevel(DatabaseManager.GetDbResource(DbResource.RESOURCE_XP));
		}
	}

	/**
	 * Get amount of a custom resource. Returns -1 if type not found.
	 * 
	 */ 
	virtual public int GetCustomResource(string type){
		if (customResourceData.ContainsKey(type)) return customResourceData[type];
		return -1;
	}

	public void AddResource(IGameResource resource, int quantity){
		if (quantity == 0)
			return;
		if (!resources.ContainsKey(resource))
			resources[resource] = 0;
		resources[resource] += quantity;

		Debug.Log("Adding quantity " + quantity + " to resource " + resource.getId());
		view.SendMessage ("UpdateGenericResourceInstant", resource, SendMessageOptions.DontRequireReceiver);
		//view.SendMessage ("UpdateGenericResource", resource, SendMessageOptions.DontRequireReceiver);

		//Level up, Quest Task notify etc
		if(resource is DbResource){
			CheckLevelUp ((DbResource)resource);
		}
		Debug.Log(resource.getId() + " Resource Count = " + GetResourceValue(resource));

	}

	public void AddResource(string resType, int quantity){
		AddResource(DatabaseManager.GetDbResource(resType), quantity);
	}
	
	public void SubtractResource(string resType, int quantity) {
		AddResource(resType, -quantity);
	}

	public void SubtractResource(IGameResource resource, int quantity) {
		AddResource(resource, -quantity);
	}


	
	override public void StartInit() {
		resources = new Dictionary<IGameResource, int>();
		
		Silver = defaultSilver;
		Gold = defaultGold;
		Xp = 0;
		customResourceTypes = new List<CustomResourceType>();
		customResourceData = new Dictionary<string, int>();

		currentLevelMap = new Dictionary<IGameResource, Level>();
		nextLevelMap = new Dictionary<IGameResource, Level>();
	}
	
	override public void PopulateDependencies() {
		dependencies = new List<ManagerDependency>();
		dependencies.Add(ManagerDependency.DATABASE_INITIALIZED);
	}

	#region old_implementation
	/**
	 * Add an amount of a custom resource. Returns resulting amount or -1 if type does
	 * not exist.
	 */ 
	virtual public int AddCustomResource(string type, int amount) {
		if (customResourceData.ContainsKey(type)) 
		{
			int original = customResourceData[type] ;
			customResourceData.Remove(type);
			customResourceData.Add(type, original + amount);
			// We could make this cleaner by only sending the update we need, but this will do for illustration purposes
			if (customResourceTypes.Count > 0) view.SendMessage ("UpdateCustomResource1", false, SendMessageOptions.DontRequireReceiver);
			if (customResourceTypes.Count > 1) view.SendMessage ("UpdateCustomResource2", false, SendMessageOptions.DontRequireReceiver);
			return customResourceData[type];
		}
		else
		{
			if (customResourceTypes.Where (t=>t.id  == type).Count() > 0)
			{
				customResourceData.Add(type, amount);
				// We could make this cleaner by only sending the update we need, but this will do for illustration purposes
				if (customResourceTypes.Count > 0) view.SendMessage ("UpdateCustomResource1", false, SendMessageOptions.DontRequireReceiver);
				if (customResourceTypes.Count > 1) view.SendMessage ("UpdateCustomResource2", false, SendMessageOptions.DontRequireReceiver);
				return customResourceData[type];
			}
		}
		Debug.LogWarning ("The resource type " + type +  " is not defined in the resource configuration.");
		return -1;
	}

	/**
	 * Remove an amount of a custom resource, returns resulting amount or -1 if resource type not 
	 * found (or not enough resource to remvoe amount).
	 */ 
	virtual public int RemoveCustomResource(string type, int amount) {
		if (customResourceData.ContainsKey(type)) 
		{
			int original = customResourceData[type];
			if (original - amount < 0)  {
				Debug.LogWarning("Tried to buy something without enough " + type);
				return -1;
			}
			customResourceData.Remove(type);
			customResourceData.Add(type, original - amount);
			// We could make this cleaner by only sending the update we need, but this will do for illustration purposes
			if (customResourceTypes.Count > 0) view.SendMessage ("UpdateCustomResource1", false, SendMessageOptions.DontRequireReceiver);
			if (customResourceTypes.Count > 1) view.SendMessage ("UpdateCustomResource2", false, SendMessageOptions.DontRequireReceiver);
			return customResourceData[type];
		}
		Debug.LogWarning ("The resource type " + type +  " is not defined in the resource configuration.");
		return -1;
	}

	/**
	 * Get a list of current custom resource data
	 */ 
	virtual public List<CustomResource> OtherResources {
		get {
			List<CustomResource> result = new List<CustomResource>();
			foreach (string key in customResourceData.Keys) {
				result.Add(new CustomResource(key, customResourceData[key]));
			}
			return result;
		}
	}

	/**
	 * Get all resource types.
	 */ 
	public List<CustomResourceType> GetCustomResourceTypes() {
		return customResourceTypes.ToList();
	}

	/**
	 * Get resource data for given type or null if not found.
	 */ 
	public CustomResourceType GetCustomResourceType(string type) {
		return customResourceTypes.Where (t=>t.id == type).FirstOrDefault();
	}

	/**
	 * List of resource type data.
	 */ 
	protected List<CustomResourceType> customResourceTypes;

	/**
	 * Dictionary of resource id to current amount.
	 */
	protected Dictionary<string, int> customResourceData;

	/**
	 * Loader for loading the data.
	 */ 
	protected Loader<CustomResourceType> loader;
	#endregion
	
	/**
	 * Load resources from save game data.
	 */ 
	virtual public void Load(SaveGameData data) {
		Silver = data.silver;
		Gold = data.gold;
		Xp = data.xp;

		foreach (CustomResource c in data.otherResources) {
			// Only add data for resources that we have a type for
			if (customResourceData.ContainsKey(c.id)) {
				customResourceData.Remove (c.id);
				customResourceData.Add(c.id, c.amount);
			}
		}
		view.SendMessage ("UpdateResource", true, SendMessageOptions.DontRequireReceiver);
		view.SendMessage ("UpdateGold", true, SendMessageOptions.DontRequireReceiver);
		if (customResourceTypes.Count > 0) view.SendMessage ("UpdateCustomResource1", true, SendMessageOptions.DontRequireReceiver);
		if (customResourceTypes.Count > 1) view.SendMessage ("UpdateCustomResource2", true, SendMessageOptions.DontRequireReceiver);
		view.SendMessage("UpdateLevel", false, SendMessageOptions.DontRequireReceiver);
	}

	virtual public void Load() {

		List<UserResource> data = DataHandler.wrapper.userResources;
		foreach(UserResource userResource in data ){
			switch(userResource.id){
			case "resource_silver":
				Silver = userResource.quantity;
				break;
			case "resource_gold":
				Gold = userResource.quantity;
				break;
			case "resource_xp":
				Xp = userResource.quantity;
				break;
			case "resource_axe":
				Axe = userResource.quantity;
				break;
			case "resource_cheer":
				Cheer = userResource.quantity;
				break;
			case "resource_happiness":
				Happiness = userResource.quantity;
				break;
			}
			view.SendMessage("UpdateGenericResourceInstant", userResource.resource, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void InitializeUserLevel(){
		string[] userLevels = DataHandler.wrapper.userLevels.Split (',');

		Level hpLevel, nextHpLevel = null;
		try {
			hpLevel = DatabaseManager.GetLevelObject(Convert.ToInt32(userLevels[0]), DatabaseManager.GetDbResource(DbResource.RESOURCE_HAPPINESS));
			currentLevelMap.Add (DatabaseManager.GetDbResource(DbResource.RESOURCE_HAPPINESS), hpLevel);
		}
		catch(Exception ex) {
			throw new Exception("levels : "+ DataHandler.wrapper.userLevels,ex);
		}
		
		nextHpLevel = DatabaseManager.GetLevelObject(hpLevel.level +1, DatabaseManager.GetDbResource(DbResource.RESOURCE_HAPPINESS));
		nextLevelMap.Add (DatabaseManager.GetDbResource(DbResource.RESOURCE_HAPPINESS), nextHpLevel);

		// XP
		Level xpLevel, nextLevel = null;
		try {
			xpLevel = DatabaseManager.GetLevelObject(Convert.ToInt32(userLevels[1]), DatabaseManager.GetDbResource(DbResource.RESOURCE_XP));
			currentLevelMap.Add (DatabaseManager.GetDbResource(DbResource.RESOURCE_XP), xpLevel);
		}
		catch(Exception ex) {
			throw new Exception("levels : "+ DataHandler.wrapper.userLevels,ex);
		}

		nextLevel = DatabaseManager.GetLevelObject(xpLevel.level +1, DatabaseManager.GetDbResource(DbResource.RESOURCE_XP));
		nextLevelMap.Add (DatabaseManager.GetDbResource(DbResource.RESOURCE_XP), nextLevel);

		view.SendMessage("UpdateLevel", false, SendMessageOptions.DontRequireReceiver);
	}

	public void InitializeUserCollectable(){
		List<UserCollectable> userCollectables = DataHandler.wrapper.userCollectables;

//		if (userCollectables == null || userCollectables.Count == 0) {
//			userCollectables = new List<UserCollectable>();
//			Collectable collectable = DatabaseManager.GetInstance().GetDbHelper().QueryObjectById<Collectable>("sw_wood_collectable");
//			this.SetCollectableValueDiff(collectable, 10);
//		}

		foreach(UserCollectable userCollectable in userCollectables){
			resources.Add(userCollectable.collectable, userCollectable.count);
		}
	}

	public bool CanSkip(QuestTask task){
		if (Gold < task.skipCost)
			return false;
		return true;
	}

	public bool CanBuild(Asset building) {
		bool haveResource = CanDeductResources(GetDiffResources(
			building.GetAssetCosts().ConvertAll(x=> (IResourceUpdate)x) )
			);
		return haveResource;

		if (haveResource) 
		{
			if (building.additionalCosts != null) {
				foreach (CustomResource c in building.additionalCosts) {
					if (GetCustomResource(c.id) < c.amount) {
						Debug.Log ("Not enough " + c.id);
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}


	
	#region old_implementation
	/**
	 * Return true if there are enough resources to build the given building.
	 */ 
	public bool CanBuild(AssetData building) {
		if (Silver >= building.cost) 
		{
			if (building.additionalCosts != null) {
				foreach (CustomResource c in building.additionalCosts) {
					if (GetCustomResource(c.id) < c.amount) {
						Debug.Log ("Not enough " + c.id);
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}

	/**
	 * Subtract the given number of resources.
	 */ 
	public void RemoveSilver(int resourceCost) {
		if (Silver >= resourceCost) {
			Silver -= resourceCost;
			view.SendMessage ("UpdateResource", false, SendMessageOptions.DontRequireReceiver);
		} else {
			Debug.LogWarning("Tried to buy something without enough resource");
		}
	}

	/**
	 * Adds the given number of resources.
	 */ 
	public void AddSilver(int quantity) {
		Silver += quantity;
		view.SendMessage("UpdateResource", false, SendMessageOptions.DontRequireReceiver);
	}

	/**
	 * Subtract the given number of gold.
	 */ 
	public void RemoveGold(int goldCost) {
		if (Gold >= goldCost) {
			Gold -= goldCost;
			view.SendMessage("UpdateGold", false, SendMessageOptions.DontRequireReceiver);
		} else {
			Debug.LogWarning("Tried to buy something without enough gold");
		}
	}
	
	/**
	 * Adds the given number of gold.
	 */ 
	public void AddGold(int gold) {
		Gold += gold;
		view.SendMessage ("UpdateGold", false, SendMessageOptions.DontRequireReceiver);
	}

	/**
	 * Adds the given number of xp.
	 */ 
	public void AddXp(int xp) {
		int oldLevel = Level;
		Xp += xp;
		if (oldLevel < Level)
		{
			view.SendMessage("UpdateLevel", true, SendMessageOptions.DontRequireReceiver);
		} else {
			view.SendMessage("UpdateLevel", false, SendMessageOptions.DontRequireReceiver);
		}
	}
	#endregion

	/// <summary>
	/// Gets the players level based on their experience.
	/// </summary>
	/// <returns>The level from experience.</returns>
	/// <param name="xp">Experience.</param>
//	virtual public int GetLevelForExperience(int xp)
//	{
//		// You can override with a different equation... this is like the D&D one.
//		return Mathf.FloorToInt((1.0f + Mathf.Sqrt( xp / 125.0f + 1.0f)) / 2.0f);
//	}

	/// <summary>
	/// Xp required for next level.
	/// </summary>
	/// <returns>The required for next level.</returns>
	virtual public int XpRequiredForNextLevel() {
		// Must match GetLevelForExperience()
		Level nextLevel = nextLevelMap [DatabaseManager.GetDbResource (DbResource.RESOURCE_XP)];
		if(nextLevel != null){
			return nextLevel.minQuantity;
		}
		return Xp;	//If on max level, return current xp value
	}

	/// <summary>
	/// Xp required for current level.
	/// </summary>
	/// <returns>The xp required for current level.</returns>
	virtual public int XpRequiredForCurrentLevel() {
		// Must match GetLevelForExperience()
		if (Level <= 1) return 0;
		return 1000 * ((Level-1 ) + Combinations((Level-1 ), 2));
	}


	virtual protected int Combinations(int n, int m)
	{
		float result = 1.0f;
		for (int i = 0; i < m; i++) result *= (float)(n - i) / (i + 1.0f);
		return (int) result;
	}

	/**
	 * Load the custom resource type data from the given (file) resource.
	 * 
	 * @param dataFile	Name of the resource to load data from.
	 * @param skipDuplicates	If false throw an exception if a duplicate is found.
	 */
	virtual public void LoadResourceDataFromResource(string dataFile, bool skipDuplicates) {
		if (loader == null) loader = new Loader<CustomResourceType>();
		List <CustomResourceType> data = loader.LoadXML(dataFile);
		foreach (CustomResourceType type in data) {
			try {
				customResourceTypes.Add(type);
				customResourceData.Add(type.id, type.defaultAmount);
			} catch (System.Exception ex) {
				if (!skipDuplicates) throw ex;
			}
		}
	}

	virtual public void AddResources(Dictionary<IGameResource, int> rewards) {
		if (rewards == null)
			return;
		foreach(IGameResource resource in rewards.Keys) {
			AddResource(resource, rewards[resource]);
		}
	}

	virtual public void AddRewards(List<IResourceUpdate>  rewards) {
		if (rewards == null)
			return;
		foreach(IResourceUpdate reward in rewards) {
			AddResource(reward.GetResource(), reward.GetQuantity());
		}
	}

	virtual public bool CanDeductResources(Dictionary<IGameResource, int> costs) {
		if (costs == null)
			return true;
		
		foreach(IGameResource resource in costs.Keys) {
			if (GetResourceValue(resource.getId()) < costs[resource]) {
				return false;
			}
		}

		return true;
	}


	//Return false if not enough resources to deduct cost
	virtual public void DeductResources(Dictionary<IGameResource, int> costs) {
		if (costs == null)
			return;
		
		foreach(IGameResource resource in costs.Keys) {
			SubtractResource(resource, costs[resource]);
		}
	}

	virtual public void UpdateResource(string resourceId, int quantity) {
		AddResource(resourceId, quantity);
	}

	virtual public int GetQuantity(string resourceId) {
		return GetResourceValue(resourceId);
	}

	public string GetResourceString(Dictionary<IGameResource, int> diffResources) {
		string resourceString = "";

		if (diffResources == null)
			return resourceString;

		foreach( IGameResource gameResource in diffResources.Keys) {
			resourceString += "&" + gameResource.getId() + "=" + diffResources[gameResource];
		}
		return resourceString;
	}

	public Vector3 GetResourceIconScreenPos(DbResource res) {
		Transform resourceIconTransform= null;
		if(res.getId() == "gold") {
				resourceIconTransform = Util.SearchHierarchyForBone(view.transform,"Gold");
				if(resourceIconTransform!=null)
					return  resourceIconTransform.position;//view.transform.FindChild("Gold").position; //GoldIcon.position;
		}else if(res.getId() == "axe") {
			resourceIconTransform = Util.SearchHierarchyForBone(view.transform,"Axe");
			if(resourceIconTransform!=null)
				return  resourceIconTransform.position;
        }else if(res.getId() == "silver") {
			resourceIconTransform = Util.SearchHierarchyForBone(view.transform,"Silver");
			if(resourceIconTransform!=null)
				return  resourceIconTransform.position;
		}else if(res.getId() == "cheer") {
			resourceIconTransform = Util.SearchHierarchyForBone(view.transform,"Cheer");
			if(resourceIconTransform!=null)
				return  resourceIconTransform.position;
		}
			return new Vector3(-Screen.width,-Screen.height,0);
	}

	public Dictionary<IGameResource, int> GetDiffResources(List<IResourceUpdate> resUpdates) {
		Dictionary<IGameResource, int> diffResources = new Dictionary<IGameResource, int> ();
		foreach (IResourceUpdate resourceUpdate in resUpdates) {
			diffResources[resourceUpdate.GetResource()] = resourceUpdate.GetQuantity();
		}
		return diffResources;
	}
	
	private void CheckLevelUp (DbResource resource){
		int newResourceCount = GetResourceValue(resource);
		
		if (currentLevelMap.ContainsKey(resource)) {
			int levelIncrementIndex = 0;
			Level newLevel = null;
			Level nextLevel = nextLevelMap[resource];
			
			// Maximum level is reached.
			if (nextLevel == null) {
				view.SendMessage("UpdateLevel", true, SendMessageOptions.DontRequireReceiver);
				return;
			}
			
			while (nextLevel != null
			       && newResourceCount >= nextLevel.minQuantity
			       && levelIncrementIndex < Config.MAX_LEVEL_INCREMENT) {
				newLevel = nextLevel;
				nextLevel = nextLevel.getNextLevel();
				// Maximum level is reached.
				
				if (nextLevel != null) {
					levelIncrementIndex++;
				} else {
					break;
				}
			}
			
			if (newLevel != null)
				LevelUp(resource, newLevel, nextLevel);
			
		} else {
			// TODO : Log for the error
		}

	}


	public void LevelUp(DbResource resource, Level newLevel, Level nextLevel) {
		if(!currentLevelMap.ContainsKey(resource)){
			currentLevelMap.Add(resource, null);
		}

		if(!nextLevelMap.ContainsKey(resource)){
			nextLevelMap.Add(resource, null);
		}

		Level prevLevel = currentLevelMap[resource];
		int prevLevelId = prevLevel.level;
		currentLevelMap[resource] =  newLevel;
		nextLevelMap[resource] = nextLevel;
		
		// Reinitializes the XP Progress bar for the updated level
		if (resource.Equals(DatabaseManager.GetDbResource(DbResource.RESOURCE_XP))) {
//			view.SendMessage("UpdateLevel", true, SendMessageOptions.DontRequireReceiver);
//			KiwiGame.uiStage.activeModeHud.namePlate
//				.reInitializeXPProgressBar();
//			KiwiGame.uiStage.activeModeHud.onLevelUp();
			//			LaunchXpromoPopUp.processXPromoCampaign(newLevel);
//			GameActions.LevelUp(newLevel.level);
		}
		// register level up
//		LevelUpTask.notifyAction(resource, newLevel.level - prevLevel.level);

		//TODO: TEMP: To move this inside level up popup.
		if(resource.Equals(DatabaseManager.GetDbResource(DbResource.RESOURCE_XP)) || resource.Equals(DatabaseManager.GetDbResource(DbResource.RESOURCE_HAPPINESS))){
			string levelString = currentLevelMap[DatabaseManager.GetDbResource(DbResource.RESOURCE_HAPPINESS)].level
				+ "," + currentLevelMap[DatabaseManager.GetDbResource(DbResource.RESOURCE_XP)].level;
			view.SendMessage("UpdateLevel", true, SendMessageOptions.DontRequireReceiver);

			//XP Level up popup
			((UILevelUpPanel)PopupManager.GetInstance ().getPanel (PanelType.LEVEL_UP)).InitialiseWithData (newLevel);
			PopupManager.GetInstance().SchedulePopup(PanelType.LEVEL_UP);
			Debug.Log("Show Level Up Popup -- " + resource.getId() + ": Level " + newLevel.level);

			Dictionary<IGameResource, int> currentDiffResources = GetInstance().GetDiffResources(newLevel.getRewards().ConvertAll(x => (IResourceUpdate)x));
			// Give resources if the quest is not expired
			GetInstance().AddResources(currentDiffResources);
			ServerAction.takeAction(EAction.LEVEL_UPDATE, levelString,
			                        resource.getAbsoluteName(), newLevel.level, currentDiffResources, true);
		}

		AfterLevelUp(resource, prevLevelId);
	}

	private void AfterLevelUp(DbResource resource, int prevLevel) {



	}

	
	public bool ChargeExpansionCost(){
		Dictionary<IGameResource, int> diffResources =  ResourceManager.GetInstance ().GetDiffResources(Asset.GetExpansionAsset ().GetAssetCosts ().ConvertAll (x => (IResourceUpdate)x));
		if (CanDeductResources (diffResources)) {
			//Charge Cost
			this.DeductResources(diffResources);
			return true;
		}
		else 
			return false;
	}

}
