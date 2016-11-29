using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Object = System.Object;

public class TaskMap : Dictionary<Object, Dictionary<Object, List<UserQuestTask>>> {

	private List<UserQuestTask> getTaskList(UserQuestTask questTask) {
		IActivityTask activityTask = questTask.activityTask;
		Object target = activityTask.GetTarget();
		if(!this.ContainsKey(target))
			this[target] = new Dictionary<Object, List<UserQuestTask>>();
		Object action = activityTask.GetAction();
		if(!this[target].ContainsKey(action)){
			this[target][action] = new List<UserQuestTask>();
		}
		return this[target][action];
	}
	
	public void Add(UserQuestTask questTask) {
		List<UserQuestTask> taskList = this.getTaskList(questTask);
		//check if the quest task has already been added .. SAFE CASE
		if(!taskList.Contains(questTask)){
			taskList.Add(questTask);
		}
	}
	
	//returns a bool depending on whether the quest task has been added
	//in the map
	public bool exists(UserQuestTask questTask){
		List<UserQuestTask> taskList = this.getTaskList(questTask);
		return taskList.Contains(questTask);
	}
	
	private List<UserQuestTask> getTasks(Object target, Object action) {
		if(target!=null && ContainsKey(target)){
			if(action!=null && this[target].ContainsKey(action))
				return this[target][action];
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void finish(Object target, Object action, int quantity, UserQuestTask questTask) {
		
		//Ranjeet: Fix for sink actor quest issue.
		//If questTask is not null complete only that quest task and not others.

		List<UserQuestTask> questTaskList = getTasks(target, action);
		if (questTaskList != null) {
			// Initialize a new list as the list is getting modified if the quest
			// task gets completed
			List<UserQuestTask> duplicateList = new List<UserQuestTask>(questTaskList);
			foreach(UserQuestTask task in duplicateList) {
				if(questTask!=null) {
					if(Utility.StringEquals(task.questTaskId,questTask.questTaskId)) {
						//TODO: Ranjeet Should we return once done with one questtask.
						if(task.finish(quantity)){
							questTaskList.Remove(task);
						}
					}
				}else {
					if(task.finish(quantity)){
						questTaskList.Remove(task);
					}
						
				}
			}
		}
	}
	
	//finishes a quest task with the given quantity
	//sendNotification : whether to send this event to the server
	//forceFinish : whether it needs to be finished forcefully
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void finish(UserQuestTask questTask, int quantity, 
	                                bool sendNotificationToServer, 
	                                bool forceFinish, bool isExpired) {
		IActivityTask activityTask = questTask.activityTask;
		List<UserQuestTask> questTaskList = getTasks(activityTask.GetTarget(), activityTask.GetAction());
		if (questTaskList != null) {
			if(questTask.finish(quantity, sendNotificationToServer, forceFinish, isExpired))
				questTaskList.Remove(questTask);
		}
	}
	
	//finishes a quest task with the given quantity
	public void finish(UserQuestTask questTask, int quantity) {
		this.finish(questTask, quantity, true, false, false);
	}
	
	//finishes a quest task with the given quantity
	public void finish(UserQuestTask questTask, int quantity, bool sendNotificationToServer) {
		this.finish(questTask, quantity, sendNotificationToServer, false, false);
	}
	
	public void forceFinish(UserQuestTask questTask, int quantity, bool sendNotificationToServer){
		this.finish(questTask, quantity, sendNotificationToServer, true, false);
	}
	
	//force finished a quest task, sets the current quantity as the required quantity
	public void forceFinish(UserQuestTask questTask, bool sendNotificationToServer, bool isExpired){
		this.finish(questTask, questTask.questTask.requiredQuantity, sendNotificationToServer, true, isExpired);
	}
	
}

