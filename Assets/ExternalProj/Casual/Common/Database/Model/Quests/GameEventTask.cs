using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

public class GameEventTask: BaseDbModel, IActivityTask{

	public static string TARGET = "KiwiGame";

	[PrimaryKey]
	public int id {get; set;}
	public string gameEvent{get; set;}

	/**
	 * Default Constructor
	 */
	public GameEventTask() {
		
	}
	
	public GameEventTask(int id, string gameEvent) {
		this.id = id;
		this.gameEvent = gameEvent;
	}

	virtual public object GetTarget(){
		return TARGET;
	}
	
	virtual public object GetAction(){
		return gameEvent;
	}

	virtual public string GetTargetId(){
		return TARGET;
	}
	
	virtual public int GetInitialQuantity(ActivityTaskType type){
		return 0;
	}
	
	virtual public TaskMap GetNewTaskMap(){
		return new TaskMap();
	}

	public static void notifyAction(string evnt) {
		UserQuestTask.notifyAction(ActivityTaskType.GAME_EVENT, TARGET, evnt, 1);
	}
	

	virtual public bool Activate(ActivityTaskType type,int quantity){
		return true;
	}

	virtual public bool Activate(ActivityTaskType type,int quantity, QuestTask questTask){
		return this.Activate(type,quantity);
	}
	
	virtual public bool ActivateOnRestore(ActivityTaskType type,int quantity, QuestTask questTask){
		return false;
	}
	
	virtual public void OnComplete(ActivityTaskType type){}
	
	virtual public void OnFinish(int quantity){}
	
	virtual public void OnVisitingNeighbor(){}
	
	virtual public void OnReturningHome(){}
	
}
