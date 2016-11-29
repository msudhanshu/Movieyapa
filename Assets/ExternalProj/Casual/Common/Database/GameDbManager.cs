using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleSQL;
using KiwiCommonDatabase;
using System.Reflection;
using System.Linq;

partial class DatabaseManager
{
	public static Activity GetActivity(string activity_id) {
		return GetInstance ().GetDbHelper ().QueryObjectById<Activity> (activity_id);
	}

	public static Asset GetAsset(string asset_id) {
		return GetInstance ().GetDbHelper ().QueryObjectById<Asset> (asset_id);
	}

	public static AssetState GetAssetState(int assetState_id) {
		return GetInstance ().GetDbHelper ().QueryObjectById<AssetState> (assetState_id);
	}

	public static AssetCategory GetAssetCategory(string assetCategory_id){
		return GetInstance ().GetDbHelper().QueryObjectById<AssetCategory>(assetCategory_id);
	}

	public static DbResource GetDbResource(string id) {
		return GetInstance().GetDbHelper().QueryObjectById<DbResource>(id);
	}

	public static Collectable GetCollectable(string id) {
		return GetInstance().GetDbHelper().QueryObjectById<Collectable>(id);
	}

	public static Level GetLevel(int id) {
		return GetInstance().GetDbHelper().QueryObjectById<Level>(id);
	}

	public static Quest GetQuest(string id) {
		return GetInstance().GetDbHelper().QueryObjectById<Quest>(id);
	}

	public static QuestTask GetQuestTask(string id) {
		return GetInstance().GetDbHelper().QueryObjectById<QuestTask>(id);
	}



	public static IActivityTask getActivityTask(ActivityTaskType type, int activityTaskId) {
		switch (type) {
		case ActivityTaskType.ASSET_ACTIVITY:
			//case SPEED_UP:
			//case ASSET_MOVE:
			return (IActivityTask) GetInstance().GetDbHelper().QueryObjectById<AssetActivityTask>(activityTaskId);
		case ActivityTaskType.ASSET_STATE:
		case ActivityTaskType.EXPANSION:
			return (IActivityTask)GetInstance().GetDbHelper().QueryObjectById<AssetStateTask>(activityTaskId);
			//case RESOURCE_ACTIVITY:
			//	return (IActivityTask)AssetHelper.getResourceActivityTaskDao().queryForId(activityTaskId);
		case ActivityTaskType.GAME_EVENT:
			return (IActivityTask)GetInstance().GetDbHelper().QueryObjectById<GameEventTask>(activityTaskId);
			/*case LEVEL_UP:
				return (IActivityTask)AssetHelper.getLevelUpTaskDao().queryForId(activityTaskId);
			case WIDGET_ACTIVITY:
				return (IActivityTask)AssetHelper.getWidgetActivityTaskDao().queryForId(activityTaskId);
			case GUIDED:
				return (IActivityTask)AssetHelper.getGuidedTaskDao().queryForId(activityTaskId);*/
		case ActivityTaskType.CATEGORY_ACTIVITY:
			return (IActivityTask)GetInstance().GetDbHelper().QueryObjectById<CategoryActivityTask>(activityTaskId);
		case ActivityTaskType.CATEGORY_STATE:
			return (IActivityTask)GetInstance().GetDbHelper().QueryObjectById<CategoryStateTask>(activityTaskId);
		case ActivityTaskType.COLLECTABLE_ACTIVITY:
			return (IActivityTask)GetInstance().GetDbHelper().QueryObjectById<CollectableActivityTask>(activityTaskId);
			/*case ASSET_INSPECT:
				return (IActivityTask)AssetHelper.getAssetInspectTaskDao().queryForId(activityTaskId);
			case SINK_ACTIVITY :
				return (IActivityTask)AssetHelper.getSinkActivityTaskDao().queryForId(activityTaskId);
			case SOCIAL_ACTIVITY :
				return (IActivityTask)AssetHelper.getSocialActivityTaskDao().queryForId(activityTaskId);
			case QUEST_STATUS :
				return (IActivityTask)AssetHelper.getQuestStatusTaskDao().queryForId(activityTaskId);
			case MINI_GAME_ACTIVITY :
				return (IActivityTask)AssetHelper.getMiniGameActivityTaskDao().queryForId(activityTaskId);
			case POPUP_DESC :
				return (IActivityTask)AssetHelper.getPopupDescTaskDao().queryForId(activityTaskId);*/
			
		default:
			return null;
		}
		return null;
	}

	public static Level GetLevelObject(int level, DbResource res) {
		try{
			KDbQuery<Level> dbquery = new KDbQuery<Level>(new BaseDbOp[]{
				new DbOpEq("resourceId", res.id), 
				new DbOpEq("level", level)});
			List<Level> resultsList = GetInstance ().GetDbHelper ().QueryForAll<Level>(dbquery);
			if(resultsList.Count > 0){
				return resultsList[0];
			}
		} catch(Exception ex){
			Debug.LogException (ex);
		}
		return null;
	}
	
	public static List<LevelReward> GetLevelRewardFC(Level level){
		try{
			KDbQuery<LevelReward> dbquery = new KDbQuery<LevelReward>(new DbOpEq(LevelReward.LEVEL_COLUMN, level.id));
			List<LevelReward> resultsList = GetInstance ().GetDbHelper ().QueryForAll<LevelReward>(dbquery);
			return resultsList;
		} catch(Exception ex){
			Debug.LogException (ex);
		}
		return null;
	}

	public static List<LevelSpecialReward> GetLevelSpecialRewardFC(Level level){
		try{
			KDbQuery<LevelSpecialReward> dbquery = new KDbQuery<LevelSpecialReward>(new DbOpEq(LevelSpecialReward.LEVEL_COLUMN, level.id));
			List<LevelSpecialReward> resultsList = GetInstance ().GetDbHelper ().QueryForAll<LevelSpecialReward>(dbquery);
			return resultsList;
		} catch(Exception ex){
			Debug.LogException (ex);
		}
		return null;
	}

}

