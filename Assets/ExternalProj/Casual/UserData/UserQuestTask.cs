
using System.Collections.Generic;
using Object = System.Object;
//
//[System.Serializable]

public class UserQuestTask {

	public string questTaskId{get; set;}
	public int currentCount{get; set;}
	private QuestTask _questTask;
	public QuestTask questTask{
		get{
			if(_questTask==null && questTaskId!=null)
				_questTask = DatabaseManager.GetQuestTask(questTaskId);
			return _questTask;
		}
		set{}
	}
	public UserQuest userQuest;
	public IActivityTask activityTask;
	private bool isActivated = false;
	private int userQuestTaskCount = 0;

	private static Dictionary<ActivityTaskType, TaskMap> activityTaskMap = new Dictionary<ActivityTaskType, TaskMap>();

	public UserQuestTask(){
	}
	
	public UserQuestTask(string questTaskId, int currentCount){
		this.questTaskId = questTaskId;
		this.currentCount = currentCount;
	}

	public UserQuestTask(string questTaskId){
		this.questTaskId = questTaskId;
		this.currentCount = 0;
	}

/*	public static bool notifyAction(ActivityTaskType activityTaskType, Object target, Object action, UserQuestTask questTask) {
		TaskMap taskMap = activityTaskMap[activityTaskType];
		if(taskMap != null){//There is no task for this target and action
			taskMap.finish(target, action, 1, questTask);
			return true;
		}else
			return false;
	}*/
	
	public static bool notifyAction(ActivityTaskType activityTaskType, Object target, Object action) {
		return notifyAction(activityTaskType, target, action, 1);
	}
	
	/**
	 * If the notifcations from the game is not responded by the quest system means no quest 
	 * is active for this action
	 * @param activityTaskType
	 * @param target
	 * @param action
	 * @param quantity
	 * @return
	 */
	public static bool notifyAction(ActivityTaskType activityTaskType, Object target, Object action, int quantity) {
		if(activityTaskMap.ContainsKey(activityTaskType)){//There is no task for this target and action
			activityTaskMap[activityTaskType].finish(target, action, quantity, null);
			return true;
		}else
			return false;
	}

	private TaskMap getTaskMap() {
		ActivityTaskType activityTaskType = questTask.taskType;
		//FIXME:Anuj
		//		if(activityTaskType == ActivityTaskType.GUIDED) 
		//			activityTaskType =  ((GuidedTask)this.activityTask).getBaseActivityTaskType();
		if(!activityTaskMap.ContainsKey(activityTaskType))
			activityTaskMap.Add(activityTaskType, this.activityTask.GetNewTaskMap());
		return activityTaskMap[activityTaskType];
	}
	
	public bool activate(bool sendNotification) {
		ActivityTaskType activityTaskType = questTask.taskType;
		IActivityTask activityTask = DatabaseManager.getActivityTask(activityTaskType, questTask.taskActivity);
		if(activityTask != null) {
			this.activityTask = activityTask;
			//		Object target = this.activityTask.getTarget();
			//Activate the quest tasks only if we in the current location
			
			if(BuildingManager3D.userQuestTaskMap.ContainsKey(this.questTaskId)){
				userQuestTaskCount = BuildingManager3D.userQuestTaskMap[this.questTaskId]; 
			}
			
			TaskMap taskMap = this.getTaskMap();
			if(taskMap.exists(this))
				return true;
			
			this.activityTask.Activate(questTask.taskType, questTask.requiredQuantity, this.questTask);
			taskMap.Add(this);
			
			int initialQuantity = getInitialQuantity();
			
			// if initial quantity is greater than zero, we need to call finish
			if(initialQuantity > 0)
				taskMap.finish(this, initialQuantity, true);
			else if(questTask.requiredQuantity <= 0)
				taskMap.finish(this, 1, sendNotification);
			
			isActivated = true;
			
			if(BuildingManager3D.userQuestTaskMap.ContainsKey(questTaskId)){
				if(userQuestTaskCount > this.currentCount)
					this.setQuantityOnStartup(userQuestTaskCount - this.currentCount);
				else if(userQuestTaskCount < this.currentCount)
					this.setQuantityOnStartup(this.currentCount - userQuestTaskCount, true);
			}
		}
		return isActivated;
	}
	
	public void activateOnRestore() {
		this.activityTask.ActivateOnRestore(questTask.taskType, questTask.requiredQuantity, this.questTask); //FIXME: defn should be UserQuestTask
	}
	
	public void setQuantityOnStartup(int quantity){
		//send notification to server is false
		this.setQuantityOnStartup(quantity, false);
	}
	
	public void setQuantityOnStartup(int quantity, bool sendNotificationToServer){
		getTaskMap().forceFinish(this, quantity, sendNotificationToServer);
	}
	
	/**
	 * Updates the current quantity of the questtask
	 * @param quantity
	 */
	protected void updateQuantity(int quantity, bool sendNotificationToServer){
		this.currentCount = this.currentCount + quantity;
		
		// Update quest task status on server
		//TODO MANJEET
		//		if(sendNotificationToServer)
//			ServerAction.takeAction(Action.QUEST_TASK_UPDATE, this, ServerSyncManager.GetInstance().serverNotifier);
		
		//Updating the UI if the Quest Task Popup is visible
		//FIXME:Anuj UI
		//		if(this.questTaskUI != null)
		//			this.questTaskUI.updateTaskQuantity(this.currentQuantity);
	}
	
	/**
	 * Returns the completion status of this quest task.
	 * @return
	 */
	public bool finish(int quantity){
		return this.finish(quantity, true, false, false);
	}
	
	public bool finish(int quantity, bool sendNotificationToServer, bool forceFinished, bool isExpired){
		UserQuest backedQuest = this.userQuest ;
		this.updateQuantity(quantity, sendNotificationToServer);
		this.activityTask.OnFinish(quantity);
		
		//notify quest icon of the progress to show the progress animation only if the finish 
		//was not called forcefully
		//		if((!forceFinished || !(this.requiredQuantity <= 0)))
		//			this.quest.getQuestUI().showProgressAnimation(); //FIXME:Anuj UI
		
		if(this.isCompleted()) {
			//if(sendNotificationToServer)
			//		checkAndAddCollectables(forceFinished,isExpired); /FIXME:Anuj
			this.activityTask.OnComplete(questTask.taskType);
			this.userQuest.notifyQuestTaskCompletion(this, isExpired);
		}
		return this.isCompleted();
	}
	
	/**
	 * This will be called when a quest task has been completed in the last session
	 * Called from initialize quest system
	 */
	public void finishOnStartUp(){
		forceFinishQuestTask(false, false);
	}
	
	public void finishOnActualExpired(){
		this.finish(questTask.requiredQuantity, false, true, true);
	}
	
	/**
	 * Called from quest task popup when user clicks the "Skip Task" button
	 * and have enough resource for this action
	 */
	public void forceFinishQuestTask(bool sendNotificationToServer, bool isExpired){
		//have to call gettaskmap .. cannot call get from the map
		//bcz if it is guided the map will return null so have to get the
		//base activity type map
		TaskMap taskMap = this.getTaskMap();
		if(taskMap != null) //There is no task for this target and action
			taskMap.forceFinish(this, sendNotificationToServer, isExpired);
	}
	
	public bool isCompleted() {
		return (this.currentCount >= questTask.requiredQuantity) || (questTask.requiredQuantity <= 0);
	}
	
	/**
	 * If the quest task is not activated it should be non-skippable
	 * @return
	 */
	public int getSkipCost(){
		if(!this.isCompleted() && this.isActivated)
			return questTask.skipCost;
		else
			return 0;
	}

	public int getInitialQuantity(){
		return this.activityTask.GetInitialQuantity(questTask.taskType);
	}

}
