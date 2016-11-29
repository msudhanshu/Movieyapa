using System.Collections.Generic;
using SimpleSQL;

[System.Serializable]
public class BuildingData : UserAsset {

	private static Dictionary<AssetState, List<BuildingData>> assetStateBuildingDataMap = new Dictionary<AssetState, List<BuildingData>>();
	private static Dictionary<AssetCategory, List<BuildingData>> assetCategoryBuildingDataMap = new Dictionary<AssetCategory, List<BuildingData>>();
	
	/**
	 * Unique identifier for the buidling.
	 */ 
	public virtual string uid {
		get{return id.ToString();} 
		set{}
	}


	/**
	 * The string defining the type of this building.
	 */ 
	public virtual string buildingTypeString {
		get {return assetId;} 
		set {}
	}
	
	/**
	 * The number of auto generated resources stored in this building, ready to be collected.
	 */ 
	public virtual int storedResources {get; set;}
	
	/**
	 * List of all occupants in this building.
	 */ 
	public virtual List<OccupantData> occupants {get; set;}



	public int GetBuildTime() {
		if (assetState.Equals(BuildingState.CONSTRUCT))
			return assetState.activityDuration;
		return asset.GetAssetState(BuildingState.CONSTRUCT.ToString().ToLower()).activityDuration;
	}

	public AssetState GetAssetState(BuildingState state) {
		return asset.GetAssetState(state.ToString().ToLower());
    }

	
	public bool InTransition() {
		if(stateStartTime == 0 || activityStartTime <= 0)
			return false;
		
		if(stateStartTime <= activityStartTime)
			return true;
		else
			return false;
	}

		
	override public string ToString() {
		return "Building(" + uid + "): " + assetState + " " + startTime.ToString() + " ";
	}

	public static void AddToAssetStateMap(BuildingData data){
		AssetState assetState = data.assetState;
		if (assetState != null) {
			if(!assetStateBuildingDataMap.ContainsKey(assetState)){
				assetStateBuildingDataMap.Add(assetState, new List<BuildingData>());
			}
			assetStateBuildingDataMap [assetState].Add (data);
		}
	}

	public static int GetAssetStateCount(AssetState assetState) {
		if (assetStateBuildingDataMap.ContainsKey (assetState)) {
			List<BuildingData> userAssetList = assetStateBuildingDataMap [assetState];
			if (userAssetList != null)
				return userAssetList.Count;
		}
		return 0;
	}

	public static void AddToAssetCategoryMap(BuildingData data){
		AssetCategory assetCategory = data.asset.assetCategory;
		if (assetCategory != null) {
			if(!assetCategoryBuildingDataMap.ContainsKey(assetCategory)){
				assetCategoryBuildingDataMap.Add(assetCategory, new List<BuildingData>());
			}
			assetCategoryBuildingDataMap [assetCategory].Add (data);
		}
	}

	public static List<BuildingData> GetBuildingDataForCategory(AssetCategory assetCategory){
		if(!assetCategoryBuildingDataMap.ContainsKey(assetCategory))
			assetCategoryBuildingDataMap.Add(assetCategory, new List<BuildingData>()); 
		return assetCategoryBuildingDataMap[assetCategory];
	}

}
