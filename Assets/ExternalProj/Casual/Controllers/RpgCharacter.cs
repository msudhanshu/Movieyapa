//
//  RpgCharacter.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Represents a building in the game. 
 */
using System;


public class RpgCharacter : Character, ITransitionListener
{	
	protected override void CreateEmptyUserAsset() {
		userAsset = new BuildingData();
	}


	public BuildingData data { 
		get{
			return userAsset as BuildingData;
		}
		set {
			data = value;
		}
	}

	virtual public AssetState State {
		get { return data.assetState; }
		set { 
			data.assetState = value;
			data.stateStartTime = Utility.GetServerTime();
			view.SendMessage("UI_UpdateState");
		}
	}

	private int _Health=10;
	public int Health {
		get {
			string val = data.GetStatePropertyValue("health");
			if (val != null) 
				_Health =  Convert.ToInt32(val);

			return _Health;
		}
	}

	private int _Walkspeed=1;
	public int Walkspeed {
		get {
			string val = data.GetStatePropertyValue("walkspeed");
			if (val != null)
				_Walkspeed = Convert.ToInt32(val);
			return _Walkspeed;
		}
	}

	/**
     * Time the building started building
     */
	virtual public long StartTime {
		get {return data.startTime; }
		protected set { data.startTime = value; }
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
		//view.SendMessage("UI_UpdateDragState", DragState.PLACING);
		view.SendMessage ("UI_UpdateState");
		transitionController.InitializeTransitions(this, data);
	}
	
	/**
     * Create a building from data. Uses a coroutine to ensure view can be synced with data.
     */ 
	protected override  void PostInit() {

		view.SendMessage ("UI_UpdateState");
		transitionController.ResumeTransitions(this, data);
	}
	
	
	/**
     * Place the building.

	virtual public void Place() {
		State = data.GetAssetState(BuildingState.CONSTRUCT);		
		view.SendMessage("UI_UpdateDragState", DragState.PLACED);
		
		StartTime = System.DateTime.Now;
		view.SendMessage("UI_UpdateState");
		
		transitionController.StartTransition(State, StartTime, true);
	}
	*/

	/**
     * Finish building.
     */ 
	virtual public void CompleteBuild() {
		State = State.next;
		view.SendMessage ("UI_UpdateState");
	}
	
	virtual public bool IsUpgradable() {
		if (data.asset.GetUpgradeState() == null)
			return false;
		//TODO: Add a column with upgrade_id to see if this can transition to Upgrade
		if (data.assetState.state != "regenerate") 
			return false;
		return true;
	}

	virtual public void TryUpgradingToNextLevel() {
		bool res = UpgradeToNextLevel();
		if(!res) Debug.LogWarning("There is nothing to upgrade");
	}

	string MIN_XP_LEVEL_FOR_UPGRADE_LEVEL = "min_game_level";
	public bool UpgradeToNextLevel() {
		AssetProperty property = AssetProperty.GetProperty(data.assetId, MIN_XP_LEVEL_FOR_UPGRADE_LEVEL);
		int minLevel = Convert.ToInt32(property != null ? property.value : "1");
		if (ResourceManager.GetInstance().Level < minLevel)
			return false;
		
		AssetState upgradeState = data.asset.GetUpgradeState();
		if (!transitionController.CanTransition(upgradeState)) {
			Debug.LogError("Not enough resources to upgrade the building");
			return false;
			//Show Jam Popup
		} 
		
		//Clear any current transition
		if (CurrentTransition != null) {
			//Collect();
			transitionController.ForceFinishCurrentTransaction();
		}
		
		//Force the transition Controller to upgrade state
		transitionController.SetNextState(upgradeState);
		transitionController.AcknowledgeTransition();
		return true;
	}

	#region ITransitionListener_Implementations

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

		
		if (data.asset.GetUpgradeState().state == State.state)
		//if ( Utility.StringEquals( State.state , "upgrade")  )
			data.level++;
		//TODO : Start moving to next state - who should do this ?
		//SetNextState();
	}
	
	virtual public void SyncState(Dictionary<IGameResource, int> diffResources) {
		EAction action = EAction.UPDATE;
		if (State.Equals("first")) {
			action = EAction.ADD;
		}

		ServerAction.takeAction(action, data, ServerSyncManager.GetInstance().serverNotifier, diffResources); 
	}
	
	virtual public void SyncActivity(Dictionary<IGameResource, int> diffResources) {
		string resourceString = ResourceManager.GetInstance().GetResourceString(diffResources);
		ServerAction.takeAction(EAction.ACTIVITY_UPDATE, data, ServerSyncManager.GetInstance().serverNotifier, diffResources); 
	}

	#endregion
}
