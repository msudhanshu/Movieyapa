using UnityEngine;
using System.Collections ;
using KiwiCommonDatabase ;
using SimpleSQL;
using System.Collections.Generic;
using System;

[System.Serializable]
public class AssetState : BaseDbModel {

	[PrimaryKey]
	public int id {get; set;}

	public string state {get; set;}

	public string name {get; set;}

	public string assetId {get; set;}

	private Asset _asset;

	[Ignore]
	public Asset asset {
		get {
			if (_asset == null)
				_asset = DatabaseManager.GetAsset(assetId);
			return _asset;
		}
		set {
			assetId = value.id;
		}
	}

	public int nextId {get; set;}

	private AssetState _next;
	[Ignore]
	[System.Xml.Serialization.XmlIgnore]
	public AssetState next {
		get {
			if (_next == null)
				_next = DatabaseManager.GetAssetState(nextId);
			return _next;
		}
		set {
			nextId = value.id;
		}
	}
	
	public string activityId {get; set;}
	private Activity _activity;
	[Ignore]
	public Activity activity {
		get {
			if (_activity == null)
				_activity = DatabaseManager.GetActivity(activityId);

			if(_activity != null && _activity.id.Equals("NULL",System.StringComparison.CurrentCultureIgnoreCase))
				return null;
			else
				return _activity;
		}
		set {
			activityId = value.id;
		}
	}

	public string nextStateName;

	public int activityDuration {get; set;}

	public bool HasAutoActivity() {
		return Utility.StringEquals(activityId, "auto");
	}

	private int width {get; set;}

	private int height {get; set;}


	private Dictionary<int, List<AssetStateCost>> costs = new Dictionary<int, List<AssetStateCost>> ();

	private Dictionary<int, List<AssetStateReward>> rewards = new Dictionary<int, List<AssetStateReward>> ();

	private Dictionary<int, List<AssetStateCollectable>> _collectables = null;
	private Dictionary<int, List<AssetStateCollectable>> collectables {
		get {
			if(_collectables == null)
				_collectables  = new Dictionary<int, List<AssetStateCollectable>> ();
			return _collectables;
		}
	}

	private Dictionary<int, List<AssetStateRewardCollectable>> _rewardCollectables = null;
	private Dictionary<int, List<AssetStateRewardCollectable>> rewardCollectables {
		get {
			if(_rewardCollectables == null) 
				_rewardCollectables = new Dictionary<int, List<AssetStateRewardCollectable>> ();
			return _rewardCollectables;
		}
	}

	private static AssetState _NullState;
	public static AssetState NullState{
		get {
			if (_NullState == null) {
				_NullState = new AssetState();
				_NullState.id = -1;
				_NullState.assetId = "null";
				_NullState.state = "null";
				_NullState.name = "null";
				_NullState.nextId = -1;
				_NullState.activityId = "null";
			}
			return _NullState;
		}

	}

	public bool InBuiltState() {
		return IsReady() || IsRegenerate() || this.asset.IsExpansionAsset();
	}

	public bool IsRegenerate() {
		return Utility.StringEquals(this.state, BuildingState.REGENERATE.ToString());
	}

	public bool IsReady() {
		return Utility.StringEquals(this.state, BuildingState.READY.ToString());
	}

	public bool IsConstruction() {
		return Utility.StringEquals(this.state, BuildingState.CONSTRUCT.ToString());
	}

	public bool Equals(BuildingState state) {
		return Utility.StringEquals(this.state, state.ToString());
	}

	public bool Equals(string state) {
		return Utility.StringEquals(this.state, state);
	}

	public List<AssetStateReward> GetRewards(int level=1) {
		KDbQuery<AssetStateReward> dbquery = new KDbQuery<AssetStateReward>(new BaseDbOp[]{new DbOpEq("assetStateId", id), new DbOpEq("level", level)});
		return DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetStateReward> (dbquery);
	}

	public List<AssetStateCollectable> GetAllCollectables(int level=1) {
		if(! collectables.ContainsKey(level)) {
		KDbQuery<AssetStateCollectable> dbquery = new KDbQuery<AssetStateCollectable>(new BaseDbOp[]{new DbOpEq("assetStateId", id), new DbOpEq("level", level)});
			collectables[level] =  DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetStateCollectable> (dbquery);
		}
		return collectables[level];
	}

	public List<AssetStateRewardCollectable> GetRewardCollectables(int level=1) {
		if(!rewardCollectables.ContainsKey(level)) {
			KDbQuery<AssetStateRewardCollectable> dbquery = new KDbQuery<AssetStateRewardCollectable>(new BaseDbOp[]{new DbOpEq("assetStateId", id), new DbOpEq("level", level)});
			rewardCollectables[level] = DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetStateRewardCollectable> (dbquery);
		}
		return rewardCollectables[level];
	}

	public List<AssetStateCost> GetCosts() {
		return GetCosts(1);
	}

	public List<AssetStateCost> GetCosts(int level) {
		KDbQuery<AssetStateCost> dbquery = new KDbQuery<AssetStateCost>(new BaseDbOp[]{new DbOpEq("assetStateId", id), new DbOpEq("level", level)});
		return DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetStateCost> (dbquery);
	}

	/*
	public string GetProperty(string name) {
		return GetProperty(name, 1);
	}

	public string GetProperty(string name, int level) {
		KDbQuery<AssetStateProperty> dbquery = new KDbQuery<AssetStateProperty>(
			new BaseDbOp[]{new DbOpEq("assetStateId", id), new DbOpEq("name", name), new DbOpEq("level", level)});
		return DatabaseManager.GetInstance().GetDbHelper().QueryForFirst<AssetStateProperty> (dbquery).value;
	}
*/
	
	/**
	 * Property should be non empty
	 * @param propName
	 * @return
	 */

	public string GetProperty(string propName, int level){
		Dictionary<String, String> props = GetProperties(level);
		return (this.HasProperty(propName, level))? props[propName] : null;
	}
	
	/**
	 * Property should be non empty
	 * @param propName
	 * @return
	 */
	public bool HasProperty(string propName, int level) {
		Dictionary<String, String> props = GetProperties(level);
		return props.ContainsKey(propName) && ! Utility.StringEquals( props[propName].Trim(), "");
	}

	public int GetIntProperty(string propName, int defaultValue, int level){
		return Convert.ToInt32(GetProperty(propName, level));
	}
	
	public long GetLongProperty(string propName, int level){
		return GetLongProperty(propName, 0, level);
	}
	
	public long GetLongProperty(string propName, long defaultValue, int level){
		bool hasProperty = this.HasProperty(propName, level);
		if(hasProperty)
			return Convert.ToInt64 (GetProperty(propName, level));
		else
			return defaultValue;
	}
	
	public bool GetBooleanProperty(string propName, bool defaultValue, int level){
		bool hasProperty = this.HasProperty(propName, level);
		if(hasProperty)
			return Convert.ToBoolean(GetProperty(propName, level));
		else
			return defaultValue;
	}
	
	public bool GetBooleanProperty(string key, int level){
		return Convert.ToBoolean(GetProperty(key, level));
	}
	
	public float GetFloatProperty(string key, float defaultValue, int level){
		return this.HasProperty(key, level) ? Convert.ToSingle(GetProperty(key, level)) : defaultValue;
	}

	private Dictionary<int, Dictionary<string,string>> properties;
	//TODO : FIXME : case if isUpgradableState
	private Dictionary<String, String> GetProperties(int level) {
		if(properties == null) {
			properties = new Dictionary<int, Dictionary<string,string>>();
		}
		
		Dictionary<String, String> levelProps = properties[level];
		if(levelProps == null){
			levelProps = new Dictionary<String, String>();
			properties.Add(level, levelProps);
			List<AssetStateProperty> assetStateProps = new List<AssetStateProperty>();
			/*if(this.isUpgradeState()) {
				List <AssetStateProperty> stateProperties = AssetStateProperty.GetStateProperties(this.nextId,level);
				if(stateProperties!=null)assetStateProps.AddRange(stateProperties);
			}*/
			List <AssetStateProperty> stateProperties = AssetStateProperty.GetStateProperties(this.id,level);
			if(stateProperties!=null) assetStateProps.AddRange(stateProperties);
			foreach(AssetStateProperty p in assetStateProps){
				levelProps.Add(p.name, p.value);
			}
		}
		return levelProps;
	}



	public BuildingState? ToBuildingState() {
//		return (BuildingState)Enum.Parse(typeof(BuildingState), name, true); 
		if (Utility.StringEquals(this.state , BuildingState.FRAME.ToString())) return BuildingState.FRAME;
		if (Utility.StringEquals(this.state , BuildingState.CONSTRUCT.ToString())) return BuildingState.CONSTRUCT;
		if (Utility.StringEquals(this.state , BuildingState.READY.ToString())) return BuildingState.READY;
		if (Utility.StringEquals(this.state , BuildingState.REGENERATE.ToString())) return BuildingState.REGENERATE;
		if (Utility.StringEquals(this.state , BuildingState.PLANT.ToString())) return BuildingState.PLANT;
		if (Utility.StringEquals(this.state , BuildingState.WATER.ToString())) return BuildingState.WATER;
		if (Utility.StringEquals(this.state , BuildingState.HARVEST.ToString())) return BuildingState.HARVEST;
		return null;
	}

	public bool IsLastState() {
		return name == "last";
	}

	public bool IsFirstState() {
		return name == "first";
	}

	public static int GetExpansionAssetStateId(){
		KDbQuery<AssetState> dbquery = new KDbQuery<AssetState>(new BaseDbOp[]{new DbOpEq("assetId", "expansion"), new DbOpEq("state", "first")});
		List<AssetState> expansionAssetStates = DatabaseManager.GetInstance ().GetDbHelper().QueryForAll<AssetState> (dbquery);
		return expansionAssetStates [0].id;
	}
	/**
	 * Gets all the states after this state, inclusive of this state.
	 * @param assetState
	 * @return
	 */
	public static List<AssetState> GetAfterStates(AssetState assetState){
		List<AssetState> afterStates = new List<AssetState>();	
		AssetState nextState = assetState;
		while(nextState != null && !afterStates.Contains(nextState)) {
			afterStates.Add(nextState);
			nextState = nextState.next;
		}
		return afterStates;
	}

	/**
	 * Gets the state for the given asset with the given state name
	 * @param asset
	 * @param stateName
	 * @return
	 */
	public static AssetState GetStateFromStateName(Asset asset, String stateName) {
		return asset.GetAssetStateByName (stateName);
	}

	/**
	 * Checks if this state comes after the passed state exclusive/inclusive of this state. 
	 * @param state
	 * @return
	 */
	public bool IsAfter(AssetState state, bool inclusive) {
		if(state == null) return false;
		
		if(inclusive && state.Equals(this))
			return true;
		
		List<AssetState> afterStates = new List<AssetState>();
		AssetState nextState = state.next;
		while(nextState != null && !afterStates.Contains(nextState)) {
			if(nextState.Equals(this)) return true;
			afterStates.Add(nextState);
			nextState = nextState.next;
		}
		return false;
	}

	/**
	 * This method returns the rewards for the state completion (both resource and collectables combined)
	 * @param aState
	 * @param level
	 * @return
	 */
	public static Dictionary<IGameResource,int> GetAllStateRewards(AssetState state, int level){
		if(state==null) return null;
		
		Dictionary<IGameResource, int> allRewards = new Dictionary<IGameResource, int>();
		List<IResourceUpdate> resUpdates = null;
		resUpdates = state.GetRewards(level).ConvertAll(x => (IResourceUpdate)x);
		foreach (IResourceUpdate resourceUpdate in resUpdates) {
			allRewards[resourceUpdate.GetResource()] = resourceUpdate.GetQuantity();
		}
		resUpdates = state.GetRewardCollectables(level).ConvertAll(x => (IResourceUpdate)x);
		foreach (IResourceUpdate resourceUpdate in resUpdates) {
			allRewards[resourceUpdate.GetResource()] = resourceUpdate.GetQuantity();
		}
		return allRewards;
		//	ResourceManager.Instance.GetDiffResources( state.GetRewards(level).ConvertAll(x => (IResourceUpdate)x));
	}
	
}
