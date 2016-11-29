using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KiwiCommonDatabase;
using SimpleSQL;
using Object = System.Object;

[System.Serializable]
public class QuestTask : BaseDbModel{
	[PrimaryKey]
	public string id{get; set;}
	public string questId{ get; set;}
	private Quest _quest;
	[Ignore]
	public Quest quest { 
		get {
//			if (questId != null && _quest == null)
//				_quest = DatabaseManager.GetQuest(questId);
			return _quest;
		}
		set {
			if(value!=null){
				questId = value.id;
				_quest = value;
			//	if (questId != null && _quest == null)
			//		_quest = DatabaseManager.GetQuest(questId);
			}
		}
	}
	public ActivityTaskType taskType{get; set;}
	public int taskActivity{get; set;}
	public int requiredQuantity{get; set;}
	public int skipCost{get; set;}
	public int version{get; set;}
	public string frontDescription{get; set;}
	public string backDescription{get; set;}
	public bool linkToMarket{get; set;}
	public string iconImage{get; set;}
	[Default(0)]
	public int displayOrder{get; set;}
	/**
	 * Default Constructor
	 */
	public QuestTask() {
		
	}
	
	public QuestTask(string id, Quest quest, ActivityTaskType activityType, int activityTaskId, 
	                 int quantity, int skipCost,string frontDescription,string backDescription) {
		this.id = id;
		this.quest = quest;
		this.taskType = activityType;
		this.taskActivity = activityTaskId;
		this.requiredQuantity = quantity;
		this.frontDescription = frontDescription;
		this.backDescription = backDescription;
		this.skipCost = skipCost;
	}

}