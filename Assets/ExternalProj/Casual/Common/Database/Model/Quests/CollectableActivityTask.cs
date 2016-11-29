using UnityEngine;
using System;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

public class CollectableActivityTask : BaseDbModel, IActivityTask
{
	[PrimaryKey]
	public int id {get; set;}
	public string collectableId {get; set;}
	public string type {get; set;}

	private GameResourceActivityTaskType collectableActivityTaskType;

	private Collectable _collectable;
	[Ignore]
	public Collectable collectable {
		get {
			if (_collectable == null)
				_collectable = DatabaseManager.GetCollectable(collectableId);
			return _collectable;
		}
		set {
			collectableId = value.id;
		}
	}

	public CollectableActivityTask(){

	}

	private GameResourceActivityTaskType getType() {
		if(this.collectableActivityTaskType == null)
			this.collectableActivityTaskType = (GameResourceActivityTaskType) Enum.Parse(typeof(GameResourceActivityTaskType), this.type.ToUpper());
		return this.collectableActivityTaskType;
	}

	public object GetTarget(){
		return this.collectable;	
	}
	
	public object GetAction(){
		return this.getType ();
	}
	
	public string GetTargetId(){
		return this.collectableId;
	}
	
	public int GetInitialQuantity(ActivityTaskType type){
		switch (this.getType()) {
		case GameResourceActivityTaskType.POSSESS:
			return ResourceManager.GetInstance().GetCollectableValue(this.collectable);
		default:
			return 0;
		}
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

