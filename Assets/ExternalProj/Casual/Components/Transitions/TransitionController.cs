using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ActivityController))]
public class TransitionController : CoreBehaviour, IActivityTarget
{

	ITransitionListener listener;
	IUserAsset userAsset;

	AssetState prevState;
	public AssetState currState;
	
	virtual public long StartTime { get; set; }
	/*	get {return data.startTime; }
		protected set { data.startTime = value; }
	}*/

	virtual public StateTransition CurrentTransition { get; set; } 
	virtual public StateTransition AutoTransition { get; set; }
	virtual public StateTransition CompletedTransition { get; set; }

	private Dictionary<IGameResource, int> currentDiffResources;
	private Dictionary<IGameResource, int> activityCosts;

	virtual public bool TransitionInProgress {
		get {
			if (CurrentTransition != null || CompletedTransition != null) return true;
			return false;
		}
	}

	public bool InTransition() {
		if(userAsset.stateStartTime == 0 || userAsset.activityStartTime <= 0)
			return false;
		
		if(userAsset.stateStartTime <= userAsset.activityStartTime)
			return true;
		
		return false;
	}

	private void InitializeController(ITransitionListener listener, IUserAsset userAsset) {
		this.activityController.RegisterAsTarget(this);
		this.userAsset = userAsset;
		this.currState = listener.GetAssetState();
		this.listener = listener;
	}

	public void ResumeTransitions(ITransitionListener listener, IUserAsset userAsset) {
		/*
		 * Initializing Transitions from server state
		 * If already in transition, continue from the last activityStartTime
		 */

		InitializeController(listener, userAsset);

		//If in last state and no next state, dont start any transitions
		if (currState.IsLastState() && currState.nextStateName == "NULL") {
			return;
		}

		if (InTransition()) {
			StartTransition(currState, userAsset.activityStartTime, false);
		} else if (currState.HasAutoActivity()) {
			StartTransition(currState, userAsset.stateStartTime, false);
		}

	}

	public void InitializeTransitions(ITransitionListener listener, IUserAsset userAsset) {

		InitializeController(listener, userAsset);
		//StartTransition(currState, Utility.GetServerTime(), true);
	}
	
	virtual public bool StartTransition(string type, long startTime, string supportingId) { 
		return false;
	}
	
	virtual public bool CanTransition(AssetState state) {
		List<AssetStateCost> costs = state.GetCosts(userAsset.level);
		Dictionary<IGameResource, int> diffResources =  ResourceManager.GetInstance().GetDiffResources(costs.ConvertAll(x => (IResourceUpdate)x));
		if (!ResourceManager.GetInstance().CanDeductResources(diffResources))
			return false;
		return true;
	}

	private long CurrentActivityStartTime() {
		if (userAsset.activityStartTime >= userAsset.stateStartTime)
			return userAsset.activityStartTime;
		return -1;
	}

	private long GetActivityStartTime() {
		long startTime = CurrentActivityStartTime();
		return startTime == -1 ? Utility.GetServerTime() : startTime;
	}
	
	//Called from ActivityController
	virtual public void StartActivityTransition() {
		StartActivityTransition(GetActivityStartTime());
	}
	
	virtual public void StartActivityTransition(long startTime) {
		if (!InTransition()) {
			userAsset.activityStartTime = Utility.GetServerTime();
			Debug.LogWarning("Syncing Activity Start Time");
			listener.SyncActivity(activityCosts);
		} else {
			Debug.LogWarning("Not Syncing Activity Start Time since already activity started");
		}

		StartCoroutine (GenericTransition (currState, startTime));
	}
	
	virtual public bool StartTransition(AssetState state, long startTime) {
		return StartTransition(state, startTime, false);
	}

	virtual public bool StartTransition(AssetState state, long startTime, bool syncToServer) {

		activityCosts = ResourceManager.GetInstance().GetDiffResources(state.GetCosts(userAsset.level).ConvertAll(x => (IResourceUpdate)x));

		if (!ResourceManager.GetInstance().CanDeductResources(activityCosts)) {
			activityCosts = null;
			return false;
		}
		
		if (state.HasAutoActivity() || state.activityDuration == 0) {
			//States with Auto activities generally will not have state costs. If they do have, we will have to handle separately
			StartCoroutine (GenericTransition (state, startTime));
		} else {
			bool success = this.gameObject.GetComponent<ActivityController>().CheckPreConditionsAndStartStateTransition(true, state.activityId);
			if (!success)
				return false;
		}

		ResourceManager.GetInstance().DeductResources(activityCosts);

		//Debug.LogWarning("Started Transitioning object : " + userAsset.id + " in " + currState.state);
		return true;
	}


	/**
     *  Start an automatic activity.
     */ 
	virtual public void StartAutomaticTransition(long startTime) {
		//TODO: Right now not using any automatic transition
		//StartCoroutine (AutomaticTransition (startTime));
	}
	
	virtual public void AcknowledgeTransition() {
		if (CompletedTransition != null && currState != null) {
			
			CurrentTransition = null;
			CompletedTransition = null;
			
			StartTime = Utility.GetServerTime();
			StartTransition(currState, StartTime);
			
		} else if (CurrentTransition == null) {
			StartTime = Utility.GetServerTime();
			StartTransition(currState, StartTime);
		}
	}
	
	public void SetNextState() {
		SetNextState(currState.next);
	}
	
	public void SetNextState(AssetState nextState) {
		//currentDiffResources = AssetState.GetAllStateRewards(currState, userAsset.level);
		currentDiffResources = ResourceManager.GetInstance().GetDiffResources(currState.GetRewards(userAsset.level).ConvertAll(x => (IResourceUpdate)x));

		//Add Asset State Reward Collectables
		List<AssetStateRewardCollectable> rewardCollectables = currState.GetRewardCollectables (userAsset.level);
		foreach (AssetStateRewardCollectable asrCollectable in rewardCollectables) {
			ResourceManager.GetInstance().SetCollectableValueDiff(asrCollectable.collectble, asrCollectable.GetQuantity());
		}
		prevState = currState;
		currState = nextState;

		//send the action to the quest system. 
		UserQuestTask.notifyAction(ActivityTaskType.ASSET_ACTIVITY, prevState.asset, prevState.activity);
		//This is required if the user does not perform a speed up task but completes the required action
		UserQuestTask.notifyAction(ActivityTaskType.SPEED_UP, prevState.asset, prevState.activity);
		UserQuestTask.notifyAction(ActivityTaskType.CATEGORY_ACTIVITY, prevState.asset.assetCategory, prevState.activity);
		UserQuestTask.notifyAction(ActivityTaskType.ASSET_STATE, prevState.asset, currState);
		if(currState != null)
			UserQuestTask.notifyAction(ActivityTaskType.CATEGORY_STATE, prevState.asset.assetCategory, currState.name);


		if (currState != null) {
			listener.SetAssetState(currState);	
			listener.SyncState(currentDiffResources);
		} else {
			return;
		}
		

		long now = Utility.GetServerTime();
		userAsset.stateStartTime = now;

		if (currState.HasAutoActivity()) {
			StartTransition(currState, now);
		}


	}

	public float transitionPercentageComplete(){
		if (CurrentTransition != null) {
			return CurrentTransition.PercentageComplete;
		} else {
			return 0.0f;
		}
	}

	private void CreateCompletedTransition(AssetState state, long startTime) {
		CompletedTransition = new StateTransition(state, startTime);
	}

	protected IEnumerator GenericTransition(AssetState state, long startTime) {
		Debug.LogWarning("Creating transition with duration" + state.activityDuration);
		StateTransition transition = new StateTransition(state, startTime);
		CurrentTransition = transition;
		listener.TransitionStarted(transition);
		Debug.LogWarning("transition percentage " + CurrentTransition.PercentageComplete);
		while (CurrentTransition != null && CurrentTransition.PercentageComplete < 1.0f) {
			listener.TransitionProgressed(transition);
		//	Debug.LogWarning("CURRENT TRANSITION IS BEING PROGRESSES" + CurrentTransition.PercentageComplete);
			yield return new WaitForSeconds(Mathf.Max (1.0f, (float)CurrentTransition.RemainingTime / 15.0f));
		}
		
		// If this wasn't triggered by a speed up
		if (CurrentTransition != null) {
			FinishTransition();
		}
	}
	
	virtual public void FinishTransition() {
		if (!CurrentTransition.IsAutoActivity) {
			GetComponent<ActivityController>().OnActivityComplete();
		}
		CompletedTransition = CurrentTransition;
		CurrentTransition = null;
		listener.TransitionCompleted(CompletedTransition);

		Debug.LogWarning("Moving to Next state");
		//Start moving to next state
		SetNextState();
	}

	virtual public void ForceAcknowledge() {
		if (CurrentTransition != null) {
			FinishTransition();
			AcknowledgeTransition();
		}

	}

	virtual public void ForceFinishCurrentTransaction() {
		CurrentTransition = null;
	}

	
	/**
     * Do automatic activity.
     */
	protected IEnumerator AutomaticTransition(long startTime) {
		yield return 0;
		/*AutoTransition = new StateTransition (State, startTime);
		while (StoredResources < asset.generationStorage) {
			yield return new WaitForSeconds((float)AutoTransition.RemainingTime.TotalSeconds);    
			StoredResources += asset.generationAmount;
			if (StoredResources > asset.generationStorage) StoredResources = asset.generationStorage;
			AutoTransition = new StateTransition (StateTransitionType.AUTOMATIC, asset.generationTime, long.Now, "");
		}
		if (StoredResources > asset.generationStorage) StoredResources = asset.generationStorage;
		AutoTransition = null;
		view.SendMessage ("UI_StoreFull");*/
	}

	#region IActivityTarget_Implementation
	private ActivityController _activityController;
	virtual public ActivityController activityController {
		get{
			if(_activityController == null)
				_activityController = GetComponent<ActivityController> ();
			if(_activityController == null)
				_activityController = gameObject.AddComponent<ActivityController>();
			return _activityController;
		}
	}

	virtual public Vector3 targetWorldPosition {
		get {
			return BuildingModeGrid3D.GetInstance().GridPositionToWorldPosition(userAsset.position);
		}
	}
	virtual public GridPosition targetGridPosition {
		get {
			return userAsset.position;
		}
	}

	virtual public bool IsHelperRequired() {
		return true;
	}

	virtual public void StartActivity() {
		StartActivityTransition();
	}
	#endregion
}
