using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    public enum EAction
{
	ADD,
	DELETE,
	PURCHASE,
	UPDATE,
	SELL,
	QUEST_TASK_UPDATE,
	QUEST_TASK_SKIP,
	QUEST_UPDATE,
	QUEST_UPDATE_STATE,
	QUEST_SKIP,
	HELPER_PURCHASE,
	HELPER_REMOVE,
	HELPER_ADD,
	HELPER_MOVE,
	HELPER_ACT,
	HELPER_FINISH_ACT,
	HELPER_OUTFIT_PURCHASE_AND_UPDATE,
	HELPER_OUTFIT_UPDATE,
	ACTIVITY_UPDATE,
	EXPANSION,
	COLLECTABLE_UPDATE,
	LEVEL_UPDATE,
	GETNEXTQUESTION
};

public class ServerAction {

	public static Dictionary<EAction, String> Urls = new Dictionary<EAction,String>() {
		{EAction.GETNEXTQUESTION, "/movie/get?"}
	};

	public static void Init(){
		//Urls.Add(EAction.GETNEXTQUESTION, "/game/get?");
		//Urls.Add(EAction.ADD, "/assettests/add?");
		//Urls.Add(EAction.ADD, "/assets/add?");
	}

	public static void takeAction(EAction action, UserQuest quest, ServerNotifier notifier, Dictionary<IGameResource, int> diffResources = null, Boolean sync = false){
		if(!ServerConfig.SERVER_ENABLED)
			return;
		string url = ServerConfig.BASE_URL + Urls[action];

		url +=  "user_id=" + Config.USER_ID + "&quest_id=" + quest.questId + "&state=" + quest.questStatus.ToString().ToLowerInvariant()
			+ "&depends_on_completed=" + quest.dependsonCompletedCount;

		url += ResourceManager.GetInstance().GetResourceString(diffResources);

		if(sync)
			ServerSyncManager.GetInstance().GetResponseSync(action, notifier, url);
		else
			ServerSyncManager.GetInstance().GetResponseAsync(action, notifier,url);
	}

	public static void takeAction(EAction action, ServerNotifier notifier){
		if(!ServerConfig.SERVER_ENABLED)
			return;
		string url = ServerConfig.BASE_URL + Urls[action];
		ServerSyncManager.GetInstance().GetRawResponse(action, notifier, url);
	}






	//OLD
	
	public static void takeAction(EAction action, UserQuest quest, List<UserQuest> dependentQuests, ServerNotifier notifier, Dictionary<IGameResource, int> diffResources = null, Boolean sync = false){
		if(!ServerConfig.SERVER_ENABLED)
			return;
		string url = ServerConfig.BASE_URL + Urls[action];
		
		string questStatusString = "&quests=";
		questStatusString = questStatusString + quest.questId + ";" + quest.questStatus.ToString().ToLowerInvariant() + ";" + quest.dependsonCompletedCount + ";";
		foreach(UserQuest dependentQuest in dependentQuests){
			questStatusString = questStatusString + ":" + dependentQuest.questId + ";" + dependentQuest.questStatus.ToString().ToLowerInvariant() + ";" + dependentQuest.dependsonCompletedCount + ";";
			List<QuestTask> questTasks = dependentQuest.quest.getQuestTasks();
			if (questTasks!=null && questTasks.Count > 0) {
				foreach(QuestTask questTask in questTasks){
					questStatusString += questTask.id + ",";
				}
				questStatusString = questStatusString.Substring(0, questStatusString.Length-1);
			}
		}
		
		url +=  "user_id=" + Config.USER_ID + questStatusString;
		url += ResourceManager.GetInstance().GetResourceString(diffResources);
		
		if(sync)
			ServerSyncManager.GetInstance().GetResponseSync(action, notifier, url);
		else
			ServerSyncManager.GetInstance().GetResponseAsync(action, notifier,url);
	}
	
	public static void takeAction(EAction action, UserQuestTask qTask, ServerNotifier notifier, Dictionary<IGameResource, int> diffResources = null, Boolean sync = false){
		if(!ServerConfig.SERVER_ENABLED)
			return;
		string url = ServerConfig.BASE_URL + Urls[action];
		url += "user_id=" + Config.USER_ID + "&quest_task_id="+ qTask.questTaskId + 
			"&required_count=" + qTask.questTask.requiredQuantity + "&current_count="+qTask.currentCount +
				"&completed=" + (qTask.isCompleted()?1:0) + "&quest_id=" + qTask.questTask.questId;
		
		url += ResourceManager.GetInstance().GetResourceString(diffResources);
		
		if(sync)
			ServerSyncManager.GetInstance().GetResponseSync(action, notifier, url);
		else
			ServerSyncManager.GetInstance().GetResponseAsync(action, notifier,url);
	}
	
	public static void takeAction(EAction action, BuildingData data, ServerNotifier notifier, Dictionary<IGameResource, int> diffResources = null, Boolean sync = false){
		if(!ServerConfig.SERVER_ENABLED)
			return;
		string url = ServerConfig.BASE_URL + Urls[action];
		
		url += "user_id=" + Config.USER_ID + "asset_id="+data.assetId+"&user_asset_id="+data.id+"&asset_state_id="+data.assetStateId+
			"&xpos="+data.position.x+"&ypos="+data.position.y+"&level="+data.level; 
		
		url += ResourceManager.GetInstance().GetResourceString(diffResources);
		
		if(sync)
			ServerSyncManager.GetInstance().GetResponseSync(action, notifier, url);
		else
			ServerSyncManager.GetInstance().GetResponseAsync(action, notifier,url);
		
	}
	
	public static void takeAction(EAction action, string urlString, ServerNotifier notifier, Boolean sync = false){
		if(!ServerConfig.SERVER_ENABLED)
			return;
		string url = ServerConfig.BASE_URL + Urls [action] + urlString;
		if(sync)
			ServerSyncManager.GetInstance().GetResponseSync(action, notifier, url);
		else
			ServerSyncManager.GetInstance().GetResponseAsync(action, notifier,url);
	}
	
	public static void MakeCallToServer(EAction action, ServerNotifier notifier, string url, string batchUrlData, bool sync) {
		if(sync)
			ServerSyncManager.GetInstance().GetResponseSync(action, notifier, url);
		else
			ServerSyncManager.GetInstance().GetResponseAsync(action, notifier,url);
	}
	
	public static void takeAction(EAction action, UserAssetController helper, PlaceableGridObject workActor = null, Dictionary<Resource, int> resourceDifferences = null, bool async=true){
		if (! ServerConfig.SERVER_ENABLED) return;
		String url= AnimalHelperURL(action, helper, workActor, resourceDifferences);
		//	MakeCallToServer(action,  helper, url, null, !async); //helper implements gameservernotifier interface
		MakeCallToServer(action,  ServerSyncManager.GetInstance().serverNotifier, url, null, !async);
	}
	
	private static String AnimalHelperURL(EAction action, UserAssetController helper, PlaceableGridObject workActor = null, Dictionary<Resource, int> resourceDifferences = null){
		string url = ServerConfig.BASE_URL + Urls[action];
		switch(action){
		case EAction.HELPER_PURCHASE:
			url +="user_id="+Config.USER_ID+"&helper_name="+ helper.asset.id +
				"&user_animal_helper_id="+ helper.userAssetId + "&xpos="+helper.Position.x+
					"&ypos="+helper.Position.y;
			break;
			
		default: 
			Debug.Log("Undefined EAction - " + action); 
			return null;
		}	
		//return finalUrlWithResourcesAndTimestamp(url, action, resourceDifferences);
		return url;
	}
	
	public static void SendCollectableUpdate(Collectable collectable, int diffCount, int count){
		string url = ServerConfig.BASE_URL + Urls [EAction.COLLECTABLE_UPDATE];
		
		url += "user_id=" + Config.USER_ID + "&collectable_id=" + 
			collectable.id + "&count=" + count + "&diff_count=" + diffCount;
		
		MakeCallToServer(EAction.COLLECTABLE_UPDATE,  ServerSyncManager.GetInstance().serverNotifier, url, null, false);
	}
	
	public static void takeAction(EAction action, String levelString, String level_type, int newLevel,
	                              Dictionary<IGameResource, int> diffResources, Boolean sync = false){
		if (! ServerConfig.SERVER_ENABLED) return;
		String url= ServerConfig.BASE_URL + Urls[EAction.LEVEL_UPDATE] +"user_id="+Config.USER_ID+
			"&level_id="+levelString + "&level_xp="+ResourceManager.GetInstance().Level+ "&level_type=" + level_type +
				"&new_level=" + newLevel;
		//url += ResourceManager.GetInstance().GetResourceString(diffResources);
		
		MakeCallToServer(EAction.LEVEL_UPDATE, ServerSyncManager.GetInstance().serverNotifier, url, null, false);
		
	}
}

