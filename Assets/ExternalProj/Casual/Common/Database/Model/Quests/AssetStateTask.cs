using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KiwiCommonDatabase;
using SimpleSQL;

public class AssetStateTask : BaseDbModel, IActivityTask
{
	private enum AssetStateTaskType {
		COUNT_NEW,
		COUNT_ALL,
		EXPANSION
	};

	[PrimaryKey]
	public int id {get; set;}
	public int assetStateId {get; set;}
	public int countAll { get; set;}
	
	private AssetState _assetState;
	[Ignore]
	public AssetState assetState {
		get {
			if (_assetState == null)
				_assetState = DatabaseManager.GetAssetState(assetStateId);
			return _assetState;
		}
		set {
			assetStateId = value.id;
		}
	}

	public AssetStateTask(){

	}

	private AssetStateTaskType GetType(){
		switch (this.countAll) {
		case 0:
			return AssetStateTaskType.COUNT_ALL;
		case -1:
			return AssetStateTaskType.COUNT_NEW;
		default:
			return AssetStateTaskType.EXPANSION;
		}
	}
	
	public object GetTarget(){
		return this.assetState.asset;	
	}
	
	public object GetAction(){
		return this.assetState;
	}

	public string GetTargetId(){
		return this.assetState.assetId;
	}

	public int GetInitialQuantity(ActivityTaskType type){
		int totalQuantity = 0;
		if(this.GetType() == AssetStateTaskType.COUNT_ALL || this.GetType() == AssetStateTaskType.EXPANSION){
			List<AssetState> afterStates = AssetState.GetAfterStates(assetState);
			foreach(AssetState state in afterStates)
				totalQuantity = totalQuantity + BuildingData.GetAssetStateCount(state);
		}
		return totalQuantity;
	}

	public TaskMap GetNewTaskMap(){
		return new TaskMap();
	}

	private bool isExpansion = false;

	public bool IsExpansionTask(){
		return isExpansion;
	}

	public bool Activate(ActivityTaskType type,int quantity){
		this.isExpansion = (type == ActivityTaskType.EXPANSION);
		if(this.isExpansion) {
			if(this.countAll <= 0) 
				this.countAll = 1;//at least 1 expansion should happen
			ExpansionHandler.Prioritize(this.GetAction() as AssetState, quantity, this.countAll);
		}
		return true;
	}

	public bool Activate(ActivityTaskType type, int quantity, QuestTask questTask){
		return this.Activate(type, quantity);
	}
	
	public bool ActivateOnRestore(ActivityTaskType type, int quantity, QuestTask questTask){
		return false;
	}
	
	public void OnComplete(ActivityTaskType type){}

	public void OnFinish(int quantity){
		if(this.isExpansion)
			ExpansionHandler.Deprioritize(this.GetAction() as AssetState, quantity);
	}
	
	public void OnVisitingNeighbor(){}
	
	public void OnReturningHome(){}
}

