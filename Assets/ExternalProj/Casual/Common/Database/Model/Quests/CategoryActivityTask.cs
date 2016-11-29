using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

public class CategoryActivityTask : BaseDbModel, IActivityTask
{
	[PrimaryKey]
	public int id {get; set;}
	public string assetCategoryId {get; set;}
	public string activityId {get; set;}

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

	private Activity _activity;
	[Ignore]
	public Activity activity {
		get {
			if (_activity == null)
				_activity = DatabaseManager.GetActivity(activityId);
			return _activity;
		}
		set {
			activityId = value.id;
		}
	}

	public CategoryActivityTask(){

	}

	public object GetTarget(){
		return this.assetCategory;	
	}
	
	public object GetAction(){
		return this.activity;
	}
	
	public string GetTargetId(){
		return this.assetCategoryId;
	}
	
	public int GetInitialQuantity(ActivityTaskType type){
		return 0;
	}
	
	public TaskMap GetNewTaskMap(){
		return new TaskMap();
	}

	public bool Activate(ActivityTaskType type,int quantity){
		return true;
	}

	
	public bool Activate(ActivityTaskType type,int quantity, QuestTask questTask){
		return this.Activate(type,quantity);
	}
	
	public bool ActivateOnRestore(ActivityTaskType type,int quantity, QuestTask questTask){
		return false;
	}
	
	public void OnComplete(ActivityTaskType type){}
	
	public void OnFinish(int quantity){}
	
	public void OnVisitingNeighbor(){}
	
	public void OnReturningHome(){}
}

