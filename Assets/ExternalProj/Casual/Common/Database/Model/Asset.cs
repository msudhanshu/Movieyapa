using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Asset : BaseDbModel {

	[PrimaryKey]
	public string id {get; set;}

	public string name {get; set;}

	public string description {get; set;}

	public string assetCategoryId {get; set;}

	/*[Ignore]
	public AssetCategory assetCategory { 
		get{
			return (AssetCategory) base.getRelationalFieldValue("assetCategory", "assetCategoryId");
		} 
		set{
			base.setRelationalFieldValue("assetCategory", "assetCategoryId", value);
		}
	}*/
	
	private AssetCategory _assetCategory;
	[Ignore]
	public AssetCategory assetCategory {
		get {
			if (_assetCategory == null)
				_assetCategory = DatabaseManager.GetAssetCategory(assetCategoryId);
			return _assetCategory;
		}
		set {
			assetCategoryId = value.id;
		}
	}

	//public AssetSubCategory assetSubCategory {get; set;}
	
	public int minLevel {get; set;}
	
	public int maxInstances {get; set;}
	//[Default(1)]
	public int numTilesX {get; set;}
	//[Default(1)]
	public int numTilesY {get; set;}
	public int numTilesZ {get; set;}
	//[Default(false)]
	public bool canFlip {get; set;}
	//[Default(-1)]
	public long expiryTime1 {get; set;}
	
	public int displayOrder {get; set;}
	//[Default(false)]
	public bool isPremium {get; set;}
	
	public string marketImageMd5 {get; set;}
	
	public string imageZipMd5 {get; set;}
	//[Default(null)]
	public string material {get; set;}
	//[Default(0)]
	public int saleDiscount {get; set;}
	
	public int maxRegenerateCount {get; set;}
	//[Default("land")]
	public string tileType {get; set;}
	
	public string bundleid {get; set;}
	
	public string neighborHelperCount {get; set;}
	
	public int flags {get; set;}

	List<GridPosition> _shape;
	[Ignore]
	public virtual List<GridPosition> shape {		
		get {
			if (_shape == null) 
				_shape = GridPosition.GetGridPositions(numTilesX, numTilesY);
			return _shape;
		} 
		set{}
	}	// Shape of the building in the isometric grid.

	private Dictionary<string,AssetState> stateMap = null;
	public AssetState GetAssetState(string state) {
		if(stateMap==null)
			stateMap = new Dictionary<string, AssetState>();
		if(!stateMap.ContainsKey(state)){
			KDbQuery<AssetState> query = new KDbQuery<AssetState>(new BaseDbOp[]{new DbOpEq("assetId", id), new DbOpEq("state", state)});
			AssetState assetState = DatabaseManager.GetInstance().GetDbHelper().QueryForFirst<AssetState>(query);
			stateMap[state] = assetState;
		}
		return stateMap[state];
	}

	public AssetState GetAssetStateByName(string name) {
		KDbQuery<AssetState> query = new KDbQuery<AssetState>(new BaseDbOp[]{new DbOpEq("assetId", id), new DbOpEq("name", name)});
		return DatabaseManager.GetInstance().GetDbHelper().QueryForFirst<AssetState>(query);
	}

	public AssetState GetUpgradeState() {
		return GetAssetState("upgrade");
		//return GetStateFromActivity(ActivityName.UPGRADE);
	}

	public AssetState GetStateFromActivity(ActivityName activity) {
		return GetStateFromActivityName(activity.ToString().ToLower());
	}

	public AssetState GetStateFromActivityName(string activity) {
		KDbQuery<AssetState> query = new KDbQuery<AssetState>(new BaseDbOp[]{new DbOpEq("assetId", id), new DbOpEq("activityId", activity)});
		return DatabaseManager.GetInstance().GetDbHelper().QueryForFirst<AssetState>(query);
	}

	public AssetData ConvertToAssetData() {
		AssetData data = new AssetData ();
		data.id = id;
		data.name = name;
		data.spriteName = name;
		data.description = description;
		data.sizex = numTilesX;
		data.sizey = numTilesY;
		data.sizeHeight = 9; //Add a new column in asset sheet
		data.cost = 10;  //AssetCost
		data.buildTime = 20; //AssetState
		//data.assetCategory = (assetCategory.name == "houses") ? AssetCategoryEnum.BUILDING : AssetCategoryEnum.HELPER;
		data.assetCategory = AssetCategoryEnum.BUILDING ;
		data.generationStorage = maxRegenerateCount;
		data.generationTime = 5; //AssetState - activityDuration
		data.generationType = RewardType.RESOURCE; //AssetStateRewards

		return data;
	}

	List<string> _allowIds;
	List<string> _requireIds;
	List<string> _activities;
	List<CustomResource> _additionalCosts;
	string allowIdsStr {get; set;}
	string requireIdsStr {get; set;}
	string activitiesStr {get; set;}

	[Ignore]
	public virtual int buildTime {get{return 5;} set{}}	
	[Ignore]
	public virtual int sizex {get{return numTilesX;} set{}}	
	[Ignore]
	public virtual int sizey {get{return numTilesY;} set{}}	
	[Ignore]
	public virtual int sizeHeight {
		get{
			//return this.GetIntProperty("height");
			return numTilesZ;
			} 
		set{}
	}	

	[Ignore]
	public virtual string spriteName {
		get{
			return "Market/"+assetCategory.id+"/"+id+"_market";
		}
	}

	[Ignore]
	public virtual string basePrefabName {
		get{
			return assetCategory.id+"/"+"3D-"+id;
		}
	}

	[Ignore]
	public virtual int generationTime {get{return 5;} set{}}				// Time to generate the reward.			
	[Ignore]
	public virtual int generationAmount {get { return 5;} set{}}				// Amount of reward to generate each time interval. For obstacles this is reward amount for clearing.
	[Ignore]
	public virtual int generationStorage {get { return maxRegenerateCount;} set{}}			// Maximum amount of generated reward to store in this building. Acknowledgement indicator will appear once this value is reached.
	[Ignore]
	public virtual AssetCategoryEnum assetCategoryEnum {get { return assetCategory.type;} set{}}		//TODO : It should be a seperate table rather than enum.


	[Ignore]
	public virtual int cost {get{return 0;} set{}}	
	[Ignore]
	public virtual bool isObstacle {get{return false;} set{}}					// If this is the true the building is an obstacle. It can't be built by players only cleared form the scene.
	[Ignore]
	public virtual bool isPath {get{return false;} set{}}						// If this is the true the building is a path. It builds insantly and is handled by the Path Manager.
	[Ignore]
	public virtual int level {get{return 1;} set{}}						// Level required to build.
	[Ignore]
	public virtual List<string> allowIds {
		get {
			if (_allowIds ==null) 
				_allowIds = Utility.StringToList(allowIdsStr);
			return _allowIds;
		} 
		set{}
	}			// Ids of the buildings and units that this building allows.
	
	[Ignore]
	public virtual List<string> requireIds {
		get {
			if (_requireIds ==null) 
				_requireIds = Utility.StringToList(allowIdsStr);
			return _requireIds;
		} 
		set{}
	}	// Ids of the buildings required before this building can be built.
	
	[Ignore]
	public virtual List<string> activities {
		get {
			if (_activities ==null) 
				_activities = Utility.StringToList(activitiesStr);
			return _activities;
		} 
		set{}
	}  	// Types of activities this building allows.
	[Ignore]
	public virtual RewardType generationType {get; set;}		// Type of reward automatically generated by this building. Ignored if generation amount = 0. For obstacles this is reward type for clearing.
	[Ignore]
	public virtual int occupantStorage {get; set;}				// The space for holding occupants. Note that occupants size can be variable (for example a building could hold two tigers with a size of 1, but only one elephant whic has a size of 2).
	[Ignore]
	public virtual List<CustomResource> additionalCosts {get {return _additionalCosts;} set{ if (value != null) _additionalCosts = value;}}	// Additional resource costs for the building.

	public bool IsRpgCharacter() {
		return assetCategory.name == "Rpgcharacters";
	}
	public bool IsHelper() {
		return assetCategory.name == "Helpers";
	}
	public bool IsBuilding() {
		return assetCategory.name == "Houses";
	}

	public AssetState GetFirstState() {
		AssetState  state = GetAssetStateByName("first");
		if(state ==  null) {
			state = GetLastState();
		}
		return state;
	}

	public AssetState GetLastState() {
		return GetAssetStateByName("last");
	}

	private Dictionary<String, String> _properties;
	/* Access Asset Properties */
	private Dictionary<String, String> properties {
		get {
		if(_properties == null){
			_properties = new Dictionary<String, String>();
			List<AssetProperty> assetProps = AssetProperty.GetProperties(this.id);
			if(assetProps != null) {
				foreach(AssetProperty p in assetProps) {
					if(p.value != null && ! Utility.StringEquals(p.value.Trim(),"") )
						_properties.Add(p.name, p.value);
				}
			}
		}
		return _properties;
		}
	}

	public string GetProperty(string key){
		return properties[key];
	}
	
	public bool HasProperty(string key) {
		return properties.ContainsKey(key);
	}
	
	public int GetIntProperty(string key){
		return Convert.ToInt32(GetProperty(key));
	}
	
	public int GetIntProperty(string key, int defaultValue){
		return this.HasProperty(key) ? this.GetIntProperty(key) : defaultValue;
	}
	public bool GetBoolProperty(string key) {
		if(this.HasProperty(key)) {
			return this.GetIntProperty(key) == 1;
		}
		return false;
	}
	public float GetFloatProperty(string key, float defaultValue) {
		return this.HasProperty(key) ? Convert.ToSingle(GetProperty(key)) : defaultValue;
	}

	public List<AssetCost> GetAssetCosts() {
		KDbQuery<AssetCost> dbquery = new KDbQuery<AssetCost>(new BaseDbOp[]{new DbOpEq("assetId", id)});
		return DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetCost> (dbquery);
	}

	public Dictionary<IGameResource, int> GetCostDiff() {
		Dictionary<IGameResource, int> costDiff = ResourceManager.GetInstance().GetDiffResources(GetAssetCosts().ConvertAll(x => (IResourceUpdate)x ));
		List<IGameResource> keys = new List<IGameResource>(costDiff.Keys);
		for (int i = 0; i < keys.Count; i++) {
			if (costDiff[keys[i]] == -1)
				costDiff[keys[i]] = 0;
			else 
				costDiff[keys[i]] *= -1;
		}

		return costDiff;
	}

	public bool IsExpansionAsset(){
		return Utility.StringEquals(this.id, "expansion");
	}

	public static Asset GetExpansionAsset(){
		return DatabaseManager.GetAsset("expansion");
	}

	public Dictionary<IGameResource, int> GetSellValue() {
		//TODO: Get the sell value of the asset
		return null;
	}
}
