using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Expansion;

[System.Serializable]
public class UserDataWrapper : MarketDataWrapper {

	public List<UserQuestTask> userQuestTasks;
	public List<UserQuest> userQuests;
	public List<UserResource> userResources;
	public List<UserAnimalHelper> userAnimalHelpers;
	public List<UserExpansionEdge> userExpansions;
	
	public List<UserCollectable> userCollectables;



	//TODO: Remove Building Data and move everything to UserAsset
	public List<BuildingData> userAssets;
	public string userLevels;
	public long maxUserAssetIdOnServer {get; set;}
	public long serverEpochTimeAtSessionStart;

	public List<BuildingData> userBuildings = new List<BuildingData>();


	private long _nextUserAssetId = -1;
	public long nextUserAssetId {
		get {
			if (_nextUserAssetId < 0) {
				if (maxUserAssetIdOnServer - (Config.USER_ID * ServerConfig.USER_ASSETS_OFFSET) > 0) {
					_nextUserAssetId = maxUserAssetIdOnServer;
				} else {
					_nextUserAssetId = Config.USER_ID * ServerConfig.USER_ASSETS_OFFSET;
				}
			}
			return ++_nextUserAssetId;
		}
		set {
			_nextUserAssetId = value;
		}
	}

	public void InitializeTime() {
		ServerConfig.serverTimeAtSessionStart = serverEpochTimeAtSessionStart;
		ServerConfig.localTimeAtSessionStart = Utility.ToUnixTime(System.DateTime.Now);
	}

	public void PopulateUserAssets(UserDataWrapper wrapper) {

		if (userAssets == null || userAssets.Count == 0)
			return;

		foreach(BuildingData userAsset in userAssets) {

			if (userAsset.asset.assetCategory.id == "houses") {
				userBuildings.Add(userAsset);
			}
		}
	

	}
}
