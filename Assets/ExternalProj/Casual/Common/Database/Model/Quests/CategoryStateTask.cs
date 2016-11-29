using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KiwiCommonDatabase;
using SimpleSQL;

public class CategoryStateTask : BaseDbModel, IActivityTask
{
	private enum CategoryStateTaskType {
		COUNT_NEW,
		COUNT_ALL,
	};

	[PrimaryKey]
	public int id {get; set;}
	public string assetCategoryId {get; set;}
	public string categoryStateName { get; set;}
	public int countAll { get; set;}
	
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

	public CategoryStateTask(){

	}

	private CategoryStateTaskType GetType(){
		switch (this.countAll) {
		case 0:
			return CategoryStateTaskType.COUNT_ALL;
		case -1:
			return CategoryStateTaskType.COUNT_NEW;
		default:
			return CategoryStateTaskType.COUNT_NEW;
		}
	}

	public object GetTarget(){
		return this.assetCategory;
	}
	
	public object GetAction(){
		return this.categoryStateName;
	}

	public string GetTargetId(){
		return this.assetCategoryId;
	}

	public int GetInitialQuantity(ActivityTaskType type){
		int totalQuantity = 0;
		if(this.GetType() == CategoryStateTaskType.COUNT_ALL){
			List<BuildingData> userAssetsInCategory = BuildingData.GetBuildingDataForCategory(this.assetCategory);
			foreach(BuildingData userAsset in userAssetsInCategory){
				AssetState state = userAsset.assetState;
				if(state != null) {
					if(state.IsAfter(AssetState.GetStateFromStateName(userAsset.asset, this.categoryStateName), true))
						totalQuantity = totalQuantity + 1;
				}
			}
		}
		return totalQuantity;
	}
	
	public TaskMap GetNewTaskMap(){
		return new TaskMap();
	}

	public bool Activate(ActivityTaskType type,int quantity){
		return true;
	}

	public bool Activate(ActivityTaskType type, int quantity, QuestTask questTask){
		return this.Activate(type, quantity);
	}
	
	public bool ActivateOnRestore(ActivityTaskType type, int quantity, QuestTask questTask){
		return false;
	}
	
	public void OnComplete(ActivityTaskType type){}
	
	public void OnFinish(int quantity){}
	
	public void OnVisitingNeighbor(){}
	
	public void OnReturningHome(){}
}

