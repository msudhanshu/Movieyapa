using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KiwiCommonDatabase;
using SimpleSQL;

[System.Serializable]
public class Quest : BaseDbModel{
	[PrimaryKey]
	public string id {get; set;}
	public int version {get; set;}
	public string name {get; set;}
	[MaxLength(150)]
	public string description{get; set;}
	[MaxLength(150)]
	public string finishedDescription{get; set;}
	public bool visible{get; set;}
	public int priority{get; set;}
	public string questIcon{get; set;}
	public string questAnnouncer{get; set;}
	public string questOutroAnnouncer{get; set;}
	public string subtitle{get; set;}
	public string questCompleteText{get; set;}
	public string actualStartTime{get; set;}
	public string userStartTime{get; set;}
	public string userEndTime{get; set;}
	[Default(0)]
	public int deltaEndTime{get; set;} // In Hours
	[MaxLength(150)]
	public string specialDescription{get; set;} 
	public string specialGoldCost{get; set;}

	private List<QuestReward> rewards;
	private List<QuestTask> questTasks;
	private List<string> dependsOnQuestIdList;

	public List<QuestTask> getQuestTasks() {
		if (this.questTasks == null) {
			this.questTasks = DatabaseManager.GetInstance().GetDbHelper().QueryForAll<QuestTask>(new KDbQuery<QuestTask>(new DbOpEq("questId", id)));
			foreach(QuestTask task in questTasks)
				task.quest = this;
		}
		return this.questTasks;
	}
	
	public List<QuestReward> getRewards() {
		if (this.rewards == null)
			this.rewards = DatabaseManager.GetInstance().GetDbHelper().QueryForAll<QuestReward>(new KDbQuery<QuestReward>(new DbOpEq("questId", id)));
		return this.rewards;
	}
	
	public List<string> getDependsOnQuestIdList() {
		if (this.dependsOnQuestIdList == null || this.dependsOnQuestIdList.Count == 0){
			dependsOnQuestIdList = new List<string>();
			List<QuestDependency> questDependencies = DatabaseManager.GetInstance().GetDbHelper().QueryForAll<QuestDependency>(new KDbQuery<QuestDependency>(new DbOpEq("quest", id)));
			foreach(QuestDependency questDependency in questDependencies){
				if(!dependsOnQuestIdList.Contains(questDependency.getDependsOnQuestId()))
					dependsOnQuestIdList.Add(questDependency.getDependsOnQuestId());
			}
		}
		return this.dependsOnQuestIdList;
	}
	
	public int getDependsOnQuestCount() {
		if (getDependsOnQuestIdList() != null)
			return getDependsOnQuestIdList().Count;
		else
			return 0;
	}
}