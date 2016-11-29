using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Represents a building in the game. 
 */

[RequireComponent(typeof(TransitionController))]
public class Building : UserAssetController, ITransitionListener
{
	/**
     * Wrapper holding the data for this building that is persisted.
     */ 
	public BuildingData data { 
		get {
			return userAsset as BuildingData;
		}
		set {
			data = value;
		}
	}

	virtual public BuildingData BuildingData {
		get { return data; }    
	}

	/**
     * Unique identifier for the buidling.
     */ 
	virtual public string uid {
		get { return data.uid; }
		set { data.uid = value; }
	}

	protected override void CreateEmptyUserAsset() {
		userAsset = new BuildingData();
	}
	
	/**
     * State of the building.
     */
	virtual public AssetState State {
		get { return data.assetState; }
		set { 
			data.assetState = value;
			data.stateStartTime = Utility.GetServerTime();
			view.SendMessage("UI_UpdateState");
	//		GameEventTask.notifyAction("state_"+data.assetId+"_"+data.assetState.state);
		}
	}
	
	/**
     * Time the building started building
     */
	virtual public long StartTime {
		get {return data.startTime; }
		protected set { data.startTime = value; }
	}
		
	/**
     * The number of auto generated resources stored in this building, ready to be collected.
     */ 
	virtual public int StoredResources {
		get { return data.storedResources; }
		protected set { data.storedResources = value; }
	}

	/**
     * Returns true if the store is full.
     */ 
	virtual public bool StoreFull {
		get {
			if (_asset.generationAmount > 0 && StoredResources >= _asset.generationStorage) return true;
			return false;
		}
	}
	
	/**
     * Gets a list of the buildings occupants. WARNING: This is not a copy.
     */ 
	virtual public List<OccupantData> Occupants {
		get { return data.occupants; }
	}

	private TransitionController _transitionController;
	virtual public TransitionController transitionController {
		get { 
			if (_transitionController == null)
				_transitionController = GetComponent<TransitionController>();
			if (_transitionController == null)
				_transitionController = gameObject.AddComponent<TransitionController>();
			return _transitionController;
		}
	}

	
	virtual public StateTransition CurrentTransition { 
		get {
			return transitionController.CurrentTransition; 
		} 
	} 

	virtual public StateTransition CompletedTransition { 
		get {
			return transitionController.CompletedTransition; 
		} 
	} 

	virtual public bool TransitionInProgress {
		get {
			return transitionController.TransitionInProgress; 
		}
	}

	virtual public StateTransition AutoTransition { get; set; }

	protected override void InitView(Asset type) {
		base.InitView(type);
		State = data.asset.GetFirstState();
		view.SendMessage("UI_UpdateDragState", DragState.PLACING);
		view.SendMessage ("UI_UpdateState");
		transitionController.InitializeTransitions(this, data);
    }

	/**
     * Create a building from data. Uses a coroutine to ensure view can be synced with data.
     */ 
	protected override  void PostInit() {
		view.SendMessage("UI_UpdateDragState", DragState.PLACED);
		view.SendMessage ("UI_UpdateState");
		
		// Ensure occupant type references are loaded
		if (data.occupants != null) {
			foreach (OccupantData o in data.occupants) {
				o.Type = OccupantManager.GetInstance().GetOccupantTypeData(o.occupantTypeString);    
				OccupantManager.GetInstance().RecruitedOccupant(o);
			}
		}

		transitionController.ResumeTransitions(this, data);
	}


	/**
     * Place the building.
     */ 
	virtual public void Place() {
		State = data.asset.GetFirstState();// GetAssetState(BuildingState.CONSTRUCT);		
		view.SendMessage("UI_UpdateDragState", DragState.PLACED);
		
		StartTime = Utility.GetServerTime();
		view.SendMessage("UI_UpdateState");

		transitionController.StartTransition(State, StartTime, true);
	}

	
	private bool CheckCollectableRequirementCondition(){
		AssetState state = this.userAsset.assetState;
		List<AssetStateCollectable> assetStateCollectables = state.GetAllCollectables (this.userAsset.level);
		
		return true;


	}

	
	/**
     * Acknowledge the building.
     */ 
	virtual public void Acknowledge() {

		AssetState state = transitionController.currState;
		List<AssetStateCollectable> assetStateCollectables = state.GetAllCollectables (this.userAsset.level);

		if (assetStateCollectables != null && assetStateCollectables.Count > 0) {

			UIBuildingCompletePanel panel = ((UIBuildingCompletePanel)PopupManager.GetInstance ().getPanel (PanelType.BUILDING_COMPLETE));
			panel.InitialiseWithData (this, transitionController.currState);

			PopupManager.GetInstance ().ShowPanel(PanelType.BUILDING_COMPLETE);
		} else
			this.AcknowledgeAfterPreConditionsMet ();

	}

	public void AcknowledgeAfterPreConditionsMet(){
		transitionController.AcknowledgeTransition();
	}
	
	/**
     * Speed up activity for a certian amount of gold.
     */ 
	virtual public void SpeedUp() {
		if (CurrentTransition == null) {
			Debug.LogError ("Tried to speed up but no activity in progress");
			return;
		}
		transitionController.ForceAcknowledge();
	}

	//Deprecated
	virtual public bool StartTransition(string type, System.DateTime startTime, string supportingId) { 
		return transitionController.StartTransition(State, Utility.ToUnixTime(startTime));
		
	}

	/**
     * Create a new occupant and add it to this building
     */ 
	protected void CreateOccupant(string supportingId) {
		OccupantData occupant = new OccupantData();
		occupant.uid = System.Guid.NewGuid().ToString();
		occupant.Type = OccupantManager.GetInstance().GetOccupantTypeData(supportingId);
		occupant.occupantTypeString = occupant.Type.id;
		if (data.occupants == null) data.occupants = new List<OccupantData>();
		data.occupants.Add (occupant);
		OccupantManager.GetInstance().RecruitedOccupant(occupant);
	}
	
	/**
     * Start moving the building.
     */ 
	virtual public void StartMoving(){
		view.SendMessage("UI_UpdateDragState", DragState.MOVING);
	}
	
	/**
     * Start moving the building.
     */ 
	virtual public void FinishMoving(){
		view.SendMessage("UI_UpdateDragState", DragState.PLACED);
	}
	
	/**
     * Collect stored resources.
     */
	virtual public void Collect() {
		switch (asset.generationType) {
		case RewardType.GOLD :
			ResourceManager.GetInstance().AddGold(StoredResources);
			break;
		case RewardType.RESOURCE :
			ResourceManager.GetInstance().AddSilver(StoredResources);
			break;                
		}
		StoredResources = 0;
		// If the store was full restart the auto activity
		if ((int)BuildingManager3D.GetInstance ().saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
	}

	/**
     * Returns true if there is enough room in this building to fit the given occupant.
     */ 
	virtual public bool CanFitOccupant(int occupantSize) {
		int currentSize = 0;
		if (data.occupants != null) {
			foreach (OccupantData o in data.occupants) {
				currentSize += OccupantManager.GetInstance().GetOccupantTypeData(o.occupantTypeString).occupantSize;
			}
		}
		if ((currentSize + occupantSize) <= _asset.occupantStorage) return true;
		return false;
	}
	
	/**
     * Dismiss (remove) the occupant. Returns true if occupant found and removed, otherwise false;
     */ 
	virtual public bool DismissOccupant(OccupantData occupant) {
		if (data.occupants != null)    {
			if (data.occupants.Contains(occupant)) {
				data.occupants.Remove(occupant);
				OccupantManager.GetInstance().DismissedOccupant(occupant);
				view.SendMessage ("UI_DismissOccupant", SendMessageOptions.DontRequireReceiver);
				return true;        
			}
		}
		return false;
	}
	
	/**
     * Finish building.
     */ 
	virtual public void CompleteBuild() {
		State = State.next;
		view.SendMessage ("UI_UpdateState");
	}

	virtual public bool IsUpgradable() {
		if (data.asset.GetAssetState("upgrade") == null)
			return false;
		//TODO: Add a column with upgrade_id to see if this can transition to Upgrade
		if (data.assetState.state == "ready") 
			return true;
		return false;
	}

	virtual public bool IsRegenerateable() {
		if (data.asset.GetAssetState("regenerate") == null)
			return false;
		//TODO: Add a column with upgrade_id to see if this can transition to Upgrade
		if (data.assetState.state == "ready") 
			return true;
		return false;
	}

	public bool shouldAcknowledge() {
		bool hasCompletedTransition = (CompletedTransition != null);
		bool isReadyForTransition = (userAsset as UserAsset).ReadyForNextActivity();
		return hasCompletedTransition || isReadyForTransition;
	}


	/*
	 * Implementations for ITransitionListener
	 */

	virtual public AssetState GetAssetState() {
		return State;
	}

	virtual public void SetAssetState(AssetState state) {
		State = state;
		view.SendMessage ("UI_UpdateState");
	}
	
	virtual public void TransitionStarted(StateTransition transition) {
		view.SendMessage ("UI_StartTransition", transition);
	}

	virtual public void TransitionProgressed(StateTransition transition) {
		view.SendMessage ("UI_UpdateProgress" , transition);
	}

	virtual public void TransitionCompleted(StateTransition transition) {
		view.SendMessage("UI_CompleteTransition", CompletedTransition);
	}

	virtual public void SyncState(Dictionary<IGameResource, int> diffResources) {
		EAction action = EAction.UPDATE;
		if (State.Equals(BuildingState.CONSTRUCT)) {
			action = EAction.ADD;
		}

		ServerAction.takeAction(action, data, ServerSyncManager.GetInstance().serverNotifier, diffResources); 
	}

	virtual public void SyncActivity(Dictionary<IGameResource, int> diffResources) {
		Debug.LogWarning("Activity start time being updated");
		ServerAction.takeAction(EAction.ACTIVITY_UPDATE, data, ServerSyncManager.GetInstance().serverNotifier, diffResources); 
	}

	/**
     * Start Expansion activity on the expansion building.
     */ 
	public void Expand() {
		if (!this.asset.IsExpansionAsset ())
			return;
		State = data.GetAssetState(BuildingState.FIRST);		
		StartTime = Utility.GetServerTime();
		//view.SendMessage("UI_UpdateState");
		transitionController.StartTransition(State, StartTime, true);
	}

	protected void OnExpansion(){
	}
	
}
