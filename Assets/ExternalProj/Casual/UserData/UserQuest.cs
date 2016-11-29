
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;
using System.Runtime.CompilerServices;
//
//[System.Serializable]

public class UserQuest {
	public string questId{get; set;}
	public int dependsonCompletedCount{get; set;}
	public QuestStatus questStatus{get; set;}
	public long actualExpiryTime ; // If LE Quest this field contains actual Expiry Time 
	private Quest _quest;
	public Quest quest{
		get{
			if(_quest==null && questId!=null)
				_quest = DatabaseManager.GetQuest(questId.Trim());
			return _quest;
		}

		set{}
	}
	private bool questTasksActivated = false;
	/**
	 * Set to keep track of quest tasks which are still pending to complete
	 */
	public List<UserQuestTask> pendingQuestTasks = new List<UserQuestTask>();
	private List<UserQuest> dependentQuestList;

	private static Dictionary<string,UserQuest> questMap = new Dictionary<string, UserQuest>();
	public static List<UserQuest> activeQuests = new List<UserQuest>(); 	// List of active quests
	public static List<UserQuest> conditionalActiveQuests = new List<UserQuest>(); // List of inactive visible quests
	/*
	 * Map of initialized quests id with the respective quest
	 * A Quest is considered as initialized if any one of the parent quest gets
	 * completed
	 */
	private static Dictionary<string, UserQuest> initializedQuests = new Dictionary<string, UserQuest>();
	public static Dictionary<UserQuest, long> pendingOutroQuests = new Dictionary<UserQuest, long>();
	public static List<UserQuest> pendingCompletionQuests = new List<UserQuest>();

	public UserQuest(){
	}
	
	public UserQuest(string questId, int dependsonCompletedCount, QuestStatus questStatus, long actualExpiryTime){
		this.questId = questId;
		this.dependsonCompletedCount = dependsonCompletedCount;
		this.questStatus = questStatus;
		this.actualExpiryTime = actualExpiryTime;
	}

	private List<UserQuest> getDependentQuestsList() {
		if (this.dependentQuestList == null){
			this.dependentQuestList = new List<UserQuest>();
			List<QuestDependency> questDependencies = DatabaseManager.GetInstance().GetDbHelper().QueryForAll<QuestDependency>(new KDbQuery<QuestDependency>(new DbOpEq("dependsOnQuest", questId)));
			foreach(QuestDependency questDependency in questDependencies){
				UserQuest quest = null;
				if(questMap.ContainsKey(questDependency.quest)){
					quest = questMap[questDependency.quest];
				}
				else{
					quest = new UserQuest(questDependency.quest, 0, QuestStatus.NONINITIALIZED, 0);
					questMap.Add (questDependency.quest, quest);
				}
				if(!dependentQuestList.Contains(quest))
					dependentQuestList.Add(quest);
			}
		}
		return this.dependentQuestList;
	}
	
	public void populate(){
		if(quest == null)
			return ;

		if(questMap.ContainsKey(questId))
			questMap[questId] = this;//return; //FIXME Could be an issue;
		else
			questMap.Add (questId, this);
		/************************************/
		
		//if equals that means the quest was activated
		//greater than equal has been put for cheat console, we put num of depends on quest completed as 100
		//		if(quest.getNumDependsOnQuestCompleted() >= quest.getDependsOnQuestCount()){
		switch (questStatus) {
		case QuestStatus.ACTIVATED :
			//		case FORCE_ACTIVATED :
			//		case USER_EXPIRED :
			//		case READY_FOR_ACTUAL_EXPIRY :	
			//Expire quests belonging to expired content bundles
			/*			if(quest.bundleid!=null && !quest.bundleid.equals("") && !ContentBundle.isLiveBundle(quest.bundleid)){
				
				if(!ContentBundle.isExpiredBundle(quest.bundleid))
					break;
				Map<Resource, Integer> resourcesDifference = User.getNewResourceDifferenceMap();
				//mark the quest as actual expired
				quest.setStatus(QuestStatus.ACTUAL_EXPIRED, false);
				// Don't activate the dependent quests on the server
				ServerApi.takeAction(ServerAction.QUEST_UPDATE, quest, new ArrayList<Quest>(),
				                     resourcesDifference, true);
				break;
			}*/
			
			bool questActivated = false;
			
			//			if(quest.isNotLockedQuest()){
			questActivated = activate(true, false);
			//			}else{
			//				Quest.addToConditionalQuest(quest, true);
			//			}
			
			//Quest Already activated so only server action is to be send
			foreach(UserQuestTask questTask in pendingQuestTasks){
				if(!BuildingManager3D.userQuestTaskMap.ContainsKey(questTask.questTaskId) && questActivated){
					if(questTask.currentCount>0){
						ServerAction.takeAction(EAction.QUEST_TASK_UPDATE, questTask, ServerSyncManager.GetInstance().serverNotifier);
					}
				}
				
			}
			break ;
			
			/*		case PRE_ACTUAL_START :
			
			if(quest.bundleid!=null && !quest.bundleid.equals("")){
				if(ContentBundle.isLiveBundle(quest.bundleid)){
					quest.setStatus(QuestStatus.ACTIVATED, true);
					quest.activate(true);
				}
				break;
			}
			
			long currentEpochTime = System.DateTime.UtcNow.Ticks;
			if((currentEpochTime - quest.getSpecialTime(Quest.ACTUAL_START_TIME) >= 0) &&
			   (currentEpochTime - quest.getSpecialTime(Quest.USER_START_TIME) < 0)) {
				quest.setStatus(QuestStatus.PRE_ACTIVATED, true) ;
				Quest.addToConditionalQuest(quest, true) ;
			} else if((currentEpochTime - quest.getSpecialTime(Quest.USER_START_TIME) >= 0) &&
			          (currentEpochTime - quest.getSpecialTime(Quest.USER_END_TIME) < 0)) {
				quest.setStatus(QuestStatus.ACTIVATED, true) ;
				quest.activate(true) ;
			} 
			else if(quest.getDeltaEndTime()==0 && (currentEpochTime - quest.getSpecialTime(Quest.USER_END_TIME)) >= 0)
				quest.setStatus(QuestStatus.ACTUAL_EXPIRED, true) ;
			else if((currentEpochTime - quest.getSpecialTime(Quest.USER_END_TIME) >= 0)) {
				quest.setStatus(QuestStatus.USER_EXPIRED, true) ;
				quest.activate(true) ;
			} else {
				Quest.addToInitializedQuest(quest);
			}
			break ;
		case PRE_ACTIVATED :
			Quest.addToConditionalQuest(quest, true) ;
			break ;
		case ACTUAL_EXPIRED :
			// Don't activate the dependent quests on the server
			ServerApi.takeAction(ServerAction.QUEST_UPDATE, quest, new ArrayList<Quest>(),
			                     null, true);
			break;*/
		case QuestStatus.COMPLETED :
			Debug.LogError("User Quests Cannot Contain COMPLETED QUESTS") ;
			break;
			/*case HIBERNATE:
			break;*/
		default :
			UserQuest.addToInitializedQuest(this);
			break ;
		}		
	}	
	
	public void notifyQuestTaskCompletion(UserQuestTask questTask, bool isExpired) {
		this.pendingQuestTasks.Remove(questTask);
		if (this.pendingQuestTasks.Count <=0){
			this.questCompleted(isExpired);
			//FIXME: Anuj UI should be called after we click on OK in questComplete
			((UIQuestCompletePanel)PopupManager.GetInstance().getPanel(PanelType.QUEST_COMPLETE)).InitialiseWithQuest(this);
			PopupManager.GetInstance().SchedulePopup(PanelType.QUEST_COMPLETE);
			this.activateDependentQuest();
		}
	}
	
	public void setStatus(QuestStatus qStatus, bool sendNotification) {
		questStatus = qStatus;
		if (sendNotification)
			ServerAction.takeAction(EAction.QUEST_UPDATE_STATE, this, ServerSyncManager.GetInstance().serverNotifier);
	}
	
	/**
	 * This will make the quest active and put it in the list of active quests
	 * Will also populate the map in the corresponding [*] activity tasks
	 */

	public bool activate(bool gameStart, bool sendNotification = true, bool activateQuestTasks = true) {
		
		/**
		 * 1) First remove from conditional quest queue
		 */
		UserQuest.removeFromConditionalQuest(this);
		bool success = UserQuest.addToActiveQuest(this);
		
		//quest was already activated by some other means
		if(!success){
			return false;
		}
		/*else {
			//notify the guided task group
			KiwiGame.uiStage.getGuidedTaskGroup().onQuestActivation(this);
		}*/
		
		// if this is an invisible quest then show its intro popup
		//	if (!this.visible)
		//	this.showIntro(); //FIXME:Anuj UI
		
		// initialize the pending quest tasks
		if (quest.getQuestTasks() != null && quest.getQuestTasks().Count > 0){
			foreach(QuestTask task in quest.getQuestTasks()){
				UserQuestTask userTask = new UserQuestTask(task.id);
				pendingQuestTasks.Add(userTask);
				userTask.userQuest = this;
			}
		}
		else {
			this.questCompleted(false);// Safe case where quest doesn't have any
			// quest tasks
			activateDependentQuest();
		}
		
		if (activateQuestTasks) {
			bool questVisibility = this.activateQuestTasks(sendNotification);
			//		questVisibility = questVisibility || (Config.CURRENT_LOCATION==GameLocation.DEFAULT);
			questTasksActivated = true;
			//		visible = visible && questVisibility;
		}
		
		if (!gameStart && quest.visible) {
			//		this.getQuestUI().populateQuestQueueUI(); //FIXME:Anuj UI
		}
		//	KiwiGame.uiStage.market.onActiveQuestModification(this, true);
		// Once the quest becomes active remove it from the map initialized
		// quests
		UserQuest.removeFromInitializedQuest(questId);
		return true;
		
	}
	
	public bool activateQuestTasks(bool sendNotification){
		bool questVisibility = false;
		List<UserQuestTask> duplicateList = new List<UserQuestTask>(pendingQuestTasks);
		foreach (UserQuestTask questTask in duplicateList){
			questVisibility = questTask.activate(sendNotification) || questVisibility;
		}
		return questVisibility;
	}
	
	public void activateQuestTasksOnRestore() {
		foreach (UserQuestTask questTask in pendingQuestTasks)
			questTask.activateOnRestore();
	}
	
	/**
	 * To activate the dependent quests
	 */
	public void activateDependentQuest() {
		foreach (UserQuest dependentQuest in getDependentQuestsList()) {
			switch (dependentQuest.questStatus) {
			case QuestStatus.ACTIVATED:
				//			case FORCE_ACTIVATED:
				//	if (dependentQuest.isLockedQuest())
				//		dependentQuest.hasDependentLockQuest();
				//	if (!dependentQuest.questLockStatus) {
				dependentQuest.activate(false);
				//		if(KiwiGame.uiStage.secondRenderComplete)
				//			dependentQuest.checkAndShowPopupDesc();
				//	} else {
				//		Quest.addToConditionalQuest(dependentQuest, false);
				//	}
				//	if (dependentQuest.isChallengeQuest())
				//		TeamChallenge.onChallengeActivation(dependentQuest);
				break;
				/*	case ACTUAL_EXPIRED:
				dependentQuest.questCompleted(true);
				break;
			case PRE_ACTIVATED:
				showHudIconIfAny();
				break;*/
			default:
				break;
			}
			//	if(dependentQuest.isChallengeQuest())
			//		KiwiGame.uiStage.activeModeHud.resetSocialWidget();	
		}
		
	}
	
	/**
	 * Will be called when all the quest tasks of the quest gets completed
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void questCompleted(bool isExpired) {
		if (QuestStatus.COMPLETED == questStatus) {
			Debug.LogError("Quest was already completed !!!!!. Being called two times");
			return;
		}
		/*	if (this.isVisible() && !this.isSeen()) {
			addToPendingCompletionQuests(this);
			return;
		}*/
		
		// remove from active quests
		UserQuest.removeFromActiveQuests(this);
		
		//KiwiGame.uiStage.getGuidedTaskGroup().onQuestComplete(this); //FIXME:Anuj Guided
		
		//		if(this.isVisible())
		//			this.getQuestUI().populateQuestQueueUI(); //FIXME:Anuj UI
		
		// Show Outro UI if this quest is not expired
		//		if (!isExpired) {
		this.setStatusForDependentQuests();
		//TODO: RanjeetGuidedTask Not needed as touchFocus is kept set according to nextquest is forced or not
		/*		if(this.getQuestUI().hasQuestOutroPopup() && !this.visible && this.hasGuidedTasks()){
				addToPendingOutroQuests(this);
				//GuidedTaskGroup.forceDeactivateGame();
			}
			else
				this.getQuestUI().showQuestOutroPopup();
*/
		questStatus = QuestStatus.COMPLETED;

		Dictionary<IGameResource, int> currentDiffResources = ResourceManager.GetInstance().GetDiffResources(quest.getRewards().ConvertAll(x => (IResourceUpdate)x));
		// Give resources if the quest is not expired

		ResourceManager.GetInstance().AddResources(currentDiffResources);
		// Update initialized state and numberOfDependsOnQuestCompleted
		// count on server
		ServerAction.takeAction(EAction.QUEST_UPDATE, this, this.getDependentQuestsList(), ServerSyncManager.GetInstance().serverNotifier, currentDiffResources);
		//if(this.isVisible())
		//	QuestTask.notifyAction(ActivityTaskType.QUEST_STATUS, QuestStatusTask.DEFAULT_TARGET,QuestStatus.COMPLETED); //FIXME:Anuj
		/*} else {
			// mark the quest as actual expired
			status = QuestStatus.ACTUAL_EXPIRED;
			// Don't activate the dependent quests on the server
			ServerApi.takeAction(ServerAction.QUEST_UPDATE, this,
			                     new ArrayList<Quest>(), resourcesDifference, true);
		}*/
		
	}
	
	private static void addToPendingOutroQuests(UserQuest quest) {
		UserQuest.pendingOutroQuests.Add(quest, System.DateTime.UtcNow.Ticks);
	}
	
	private static void addToPendingCompletionQuests(UserQuest quest) {
		UserQuest.pendingCompletionQuests.Add(quest);
	}

	// This will also initialize the dependent quests
	private void setStatusForDependentQuests() {
		
		foreach(UserQuest dependentQuest in getDependentQuestsList()) {
			// Checks if the quest is already initialized or not
			UserQuest quest = null;//Quest.initializedQuests[dependentQuest.id];
			if (!UserQuest.initializedQuests.ContainsKey(dependentQuest.questId)) {//quest == null) {
				UserQuest.initializedQuests.Add(dependentQuest.questId, dependentQuest);
				quest = dependentQuest;
			}
			else
				quest = UserQuest.initializedQuests[dependentQuest.questId];
			
			quest.dependsonCompletedCount++;
			
//			long currentEpochTime = System.DateTime.UtcNow.Ticks;
			
			if (quest.dependsonCompletedCount >= quest.quest.getDependsOnQuestCount()) {
				/*if (quest.isSpecialQuest()) {
					if (this.getStatus().equals(QuestStatus.FORCE_ACTIVATED)) {
						quest.status = QuestStatus.FORCE_ACTIVATED;
						continue;
					}
					
					if (currentEpochTime
					    - quest.getSpecialTime(ACTUAL_START_TIME) < 0) {
						quest.status = QuestStatus.PRE_ACTUAL_START;
						Quest.addToInitializedQuest(quest);
						continue;
					} else if (currentEpochTime
					           - quest.getSpecialTime(USER_START_TIME) < 0) {
						quest.status = QuestStatus.PRE_ACTIVATED;
						Quest.addToConditionalQuest(quest, false);
						continue;
					} else if (currentEpochTime
					           - quest.getSpecialTime(USER_END_TIME) >= 0) {
						quest.status = QuestStatus.ACTUAL_EXPIRED;
						Quest.removeFromInitializedQuest(quest.id);
						continue;
					}
				}
				// Expire quests belonging to expired content bundles
				if (quest.bundleid != null && !quest.bundleid.equals("")
				    && !ContentBundle.isLiveBundle(quest.bundleid)) {
					if (ContentBundle.isExpiredBundle(quest.bundleid)) {
						quest.status = QuestStatus.ACTUAL_EXPIRED;
						Quest.removeFromInitializedQuest(quest.id);
					} else {
						quest.status = QuestStatus.PRE_ACTUAL_START;
					}
					continue;
				}*/
				quest.questStatus = QuestStatus.ACTIVATED;
			} else {
				quest.questStatus = QuestStatus.INITIALIZED;
			}
		}
	}
	
	/**
	 * Called from quest task popup when user clicks the "Complete Quest" button
	 * and have enough resource for this action
	 */
	public void forceComplete(bool isExpired) {
		if (!this.questTasksActivated) {
			this.questCompleted(isExpired);
		} else {
			foreach (UserQuestTask questTask in pendingQuestTasks) {
				questTask.forceFinishQuestTask(true, isExpired);
			}
		}
	}
	
	public static void removeFromActiveQuests(UserQuest quest) {
		UserQuest.activeQuests.Remove(quest);
		//	KiwiGame.uiStage.market.onActiveQuestModification(quest, false); //FIXME:Anuj UI
	}
	
	// location check removed as they are slow in BA
	public static bool addToActiveQuest(UserQuest quest) {
		if (!UserQuest.activeQuests.Contains(quest)) {
			UserQuest.activeQuests.Add(quest);
			return true;
		} else
			return false;
	}
	
	public static bool addToConditionalQuest(UserQuest quest, bool gameStart) {
		if (!UserQuest.conditionalActiveQuests.Contains(quest)) {
			UserQuest.conditionalActiveQuests.Add(quest);
			return true;
		} else
			return false;
	}
	
	private static void removeFromConditionalQuest(UserQuest quest) {
		UserQuest.conditionalActiveQuests.Remove(quest);
	}
	
	public static void addToInitializedQuest(UserQuest quest) {
		UserQuest.initializedQuests.Add(quest.questId, quest);
	}
	
	private static void removeFromInitializedQuest(string questId) {
		UserQuest.initializedQuests.Remove(questId);
	}
	/*
	public override int GetHashCode ()
	{
		int prime = 31;
		int result = 1;
		result = prime * result + ((id == null) ? 0 : id.GetHashCode());
		return result;
	}

	public  bool Equals(object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (this.GetType() != obj.GetType())
			return false;
		Quest other = (Quest) obj;
		if (id == null) {
			if (other.id != null)
				return false;
		} else if (!id.Equals(other.id))
			return false;
		return true;
	} */
	
	public static bool isQuestSystemActivated() {
		return (activeQuests.Count > 0) || (initializedQuests.Count > 0);
	}
}
