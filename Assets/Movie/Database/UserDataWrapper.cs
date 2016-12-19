using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Expansion;
using SgUnity;
namespace KiwiCommonDatabase
{
    
[System.Serializable]
public class UserDataWrapper : MarketDataWrapper {

//	public List<UserQuestTask> userQuestTasks;
//	public List<UserQuest> userQuests;
	public List<UserResource> userResources;
    //public List<UserCurrency> userCurrencies;
    //public string userLevels;
    public UserCurrency userCurrency;

    public List<UserPackage> userPackages;

	private long _nextUserAssetId = -1;
	public long nextUserAssetId {
		get {
//			if (_nextUserAssetId < 0) {
//				if (maxUserAssetIdOnServer - (Config.USER_ID * ServerConfig.USER_ASSETS_OFFSET) > 0) {
//					_nextUserAssetId = maxUserAssetIdOnServer;
//				} else {
//					_nextUserAssetId = Config.USER_ID * ServerConfig.USER_ASSETS_OFFSET;
//				}
//			}
			return ++_nextUserAssetId;
		}
		set {
			_nextUserAssetId = value;
		}
	}

		//	public long serverEpochTimeAtSessionStart;
//	public void InitializeTime() {
//		ServerConfig.serverTimeAtSessionStart = serverEpochTimeAtSessionStart;
//		ServerConfig.localTimeAtSessionStart = Utility.ToUnixTime(System.DateTime.Now);
//	}
    
}

}