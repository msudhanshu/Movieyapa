using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expansion;

/**
 * Handles resources, gold, etc.
 */ 
public class SgResourceManager : Manager<SgResourceManager> {

	public static string RESOURCE_GOLD = "gold";
	public static string RESOURCE_SILVER = "silver";
	public static string RESOURCE_HINT = "hint";
	public static string RESOURCE_TICKET = "ticket";
	public static string RESOURCE_XP = "xp";
	public static string RESOURCE_LEVEL = "level";

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

	private Dictionary<string, int> resourcemodelmap =  new Dictionary<string, int>();



	override public void StartInit() {
		resources = new Dictionary<IGameResource, int>();
		//resourcemodelmap = new Dictionary<string, int>();
		currentLevelMap = new Dictionary<IGameResource, Level>();
		nextLevelMap = new Dictionary<IGameResource, Level>();
		Gold = SgFirebase.GetInstance ().dataInitialiser.userGoldResource;
//		Load (SgFirebase.GetInstance ().dataInitialiser.userResources);
	}

	override public void PopulateDependencies() {
		dependencies = new List<ManagerDependency>();
		dependencies.Add(ManagerDependency.FIREBASE);
	}


	private int GetResourceValue(string resType) {
		if (SgConfigValue.FirebaseEnabled) {
			//return SgFirebase.GetInstance ().database.GetResourceValue (resType);
			if (!resourcemodelmap.ContainsKey (resType))
				resourcemodelmap.Add (resType, 0);
			return resourcemodelmap[resType];
		} else {
			return GetResourceValue (DatabaseManager.GetDbResource (resType));
		}
	}

	private int GetResourceValue(IGameResource res) {
		if (!resources.ContainsKey (res))
			resources.Add (res, 0);
		return resources[res];
	}

	private void SetResourceValue(string resType, int quantity) {
		if (SgConfigValue.FirebaseEnabled) {
			//SgFirebase.GetInstance ().database.SetResourceValue (resType,quantity);
			if (!resourcemodelmap.ContainsKey (resType))
				resourcemodelmap.Add (resType, 0);
			resourcemodelmap [resType] = quantity;

		} else {
			DbResource resource = DatabaseManager.GetDbResource (resType);

			if (!resources.ContainsKey (resource))
				resources.Add (resource, 0);
			resources [resource] = quantity;
		}
	}
	
	/**
	 * The resource used for actually building buildings. In some games this is called coins or gp.
	 */ 
	virtual public int Silver {
		get {
			return GetResourceValue(RESOURCE_SILVER);
		}
		protected set {
			SetResourceValue(RESOURCE_SILVER, value);
		}
	}

	virtual public int Hint {
		get {
			return GetResourceValue(RESOURCE_HINT);
		}
		protected set {
			SetResourceValue(RESOURCE_HINT, value);
		}
	}

	virtual public int Ticket {
		get {
			return GetResourceValue(RESOURCE_TICKET);
		}
		protected set {
			SetResourceValue(RESOURCE_TICKET, value);
		}
	}

	/**
	 * The resource used to speed things up. Some games might call this gems or cash.
	 */ 
	virtual public int Gold {
		get {
			return GetResourceValue(RESOURCE_GOLD);
		}
		protected set {
			SetResourceValue(RESOURCE_GOLD, value);
		}
	}

	/**
	 * The experiecne of the current player.
	 */ 
	virtual public int Xp {
		get {
			return GetResourceValue(RESOURCE_XP);
		}
		protected set {
			SetResourceValue(RESOURCE_XP, value);
		}
	}

	/**
	 * Level of the player calculated from experience.
	 */ 
	virtual public int Level {
		get {
			return GetResourceValue(RESOURCE_XP);
		}
		protected set {
			SetResourceValue(RESOURCE_XP, value);
		}
//		get
//		{
//			return 0;//GetLevel(DatabaseManager.GetDbResource(DbResource.RESOURCE_XP));
//		}
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
//		if(resource is DbResource){
//			CheckLevelUp ((DbResource)resource);
//		}
		Debug.Log(resource.getId() + " Resource Count = " + GetResourceValue(resource));

	}

	public void AddResource(string resType, int quantity){
		if (SgConfigValue.FirebaseEnabled) { 
			int v = GetResourceValue(resType);
			v += quantity;
			SetResourceValue(resType, v);
			SgFirebase.GetInstance ().database.SetResourceValue (resType,v);
		} else {
			AddResource(DatabaseManager.GetDbResource(resType), quantity);
		}
	}
	
	public void SubtractResource(string resType, int quantity) {
		AddResource(resType, -quantity);
	}

	public void SubtractResource(IGameResource resource, int quantity) {
		AddResource(resource, -quantity);
	}

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
		
	public void Load(ResourceModel res) {
		Silver = res.silver;
		Hint = res.hint;
		Ticket = res.ticket;
		Level = res.ticket;
	}
}
