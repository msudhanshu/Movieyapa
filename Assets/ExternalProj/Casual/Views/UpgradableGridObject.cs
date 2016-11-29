
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class UpgradableGridObject : DraggableGridObject {


	protected Building building {
		get { return assetController as Building;}
		set {}
	}

	/**
	 * Building state changed, update view.
	 */ 
	virtual public void UI_UpdateState() {
		//BuildingState? buildingState = building.State.ToBuildingState();
		//Debug.LogWarning("StateTransition : updatestate="+building.State.state);

		InitializeView();
		/*
		switch (buildingState) {
		case BuildingState.FRAME :
			SetModeEffect(components,ModeEffect.PLACING);
			break;
		case BuildingState.CONSTRUCT :
			if (particles != null) particles.Stop ();
			break;
			
		case BuildingState.READY :
			foreach(GameObject go in components) {
				go.renderer.material.shader = Shader.Find("Diffuse");
				go.renderer.material.color = new Color(1,1,1,1);
				go.SetActive (true);
			}

			if (particles != null) particles.Stop ();
			break;
			
		case BuildingState.REGENERATE :
			foreach(GameObject go in components) {
				go.renderer.material.shader = Shader.Find("Diffuse");
				go.renderer.material.color = new Color(0,0,0,1);
				go.SetActive (true);
			}
			if (particles != null) particles.Play();
			break;

		}*/
		ShowCallout(building.State);

	}
	
	/**
	 * transition completed.
	 */ 
	virtual public void UI_StartTransition(StateTransition transition) {
		Debug.LogWarning("StateTransition : start state="+building.State.state);
		GetActivityStatus().ShowProgressBar();	
		if (transition.State.Equals(BuildingState.CONSTRUCT)) {
			if (components == null)
				return;
			foreach(GameObject go in components) {
				//start transition modeeffect
				//SetColor(go, new Color(1,1,1,1));
				//stop all animation and object's particle effect
				SetModeEffect(go,ModeEffect.UPGRADING);
				
				if(isLevelUpgradable)
					go.SetActive (false);
			}
			
			if(isLevelUpgradable){
				if(activeComponent == null){
					activeComponent = components[0];
					currentLevel = 0;
				}
				activeComponent.SetActive (true);
			}
		}
	}
	

	/**
	 * Indicate progress on the progress ring.
	 */
	virtual public void UI_UpdateProgress(StateTransition transition) {
		if (transition.State.Equals(BuildingState.CONSTRUCT)) {
			updateActiveComponent(transition.PercentageComplete);
		}
		GetActivityStatus().SetText(building.data.level + ":" + building.State.state + ":" + transition.RemainingTime);
		GetActivityStatus().UpdateProgressUI(transition.PercentageComplete);
	}
	
	/**
	 * transition completed.
	 */ 
	virtual public void UI_CompleteTransition(StateTransition transition) {
		Debug.LogWarning("StateTransition : complete state="+building.State.state);
		GetActivityStatus().UpdateProgressUI(1);
		if(UIGamePanel.activePanel.GetType() == typeof(UIBuildingInfoPanel) && ((UIBuildingInfoPanel)UIGamePanel.activePanel).building == building)
			UIGamePanel.activePanel.Show ();
		Dictionary<IGameResource, int> allRewards = DoDoobersWork(transition.State, building.BuildingData.level);

		if(transition.State.next != null)
			ShowCallout(transition.State);

		if (transition.State.Equals(BuildingState.CONSTRUCT)) {
			foreach(GameObject go in components) {
				if(!isLevelUpgradable) {
					//go.renderer.material.color = new Color(1,1,1,1); 
					SetModeEffect(go,ModeEffect.NORMAL);
				}
				go.SetActive (true);
			}
		}

		//if NextAsset State is null, then just delete the assets...like crops.
		DeleteBuildingIfAllStateCompleted(transition, allRewards);
	}

	//FOR CROP HARVEST
	protected void DeleteBuildingIfAllStateCompleted(StateTransition transition, Dictionary<IGameResource, int> rewards = null) {
		AssetState a= transition.State.next;
		if(a==null) {
			if (transition.State.name == "last" && transition.State.state == "harvest")
				BuildingManager3D.GetInstance().DeleteBuilding(building, rewards);
		}
	}

	 public override void OnPointerClick (PointerEventData eventData){
		//	if (CameraMovement.IS_FPSVIEW_ENABLED()) {
	//		return;
	//	}
//
//		if (GameManager.GetInstance().GetFpsCameraComponent().IS_FPSVIEW_ENABLED_NOW() || this.gameObject.layer == BuildingManager3D.SELECTION_LAYER)
//			return;

		/*if (building.shouldAcknowledge()) {
			building.Acknowledge();
		} else {*/
			PopupManager.GetInstance().ShowPanel(PanelType.BUILDING_INFO);
			if (UIGamePanel.activePanel is UIBuildingInfoPanel) ((UIBuildingInfoPanel)UIGamePanel.activePanel).InitialiseWithBuilding(building);
		//}
	}
	
	/** TODO : Implement callout (may be as seperate component)
	 * Callback when the activity status callout is clicked.
	 */
	public override void OnCallOutClick() {
		//activityController.CheckPreConditionsAndStartStateTransition (true,ActivityName.CONSTRUCT);
	//	bool res = building.UpgradeToNextLevel();

		if (building.shouldAcknowledge()) {
			//GetActivityStatus().CallOutClicked();
			building.Acknowledge();
		}

		//if(!res) Debug.LogWarning("There is nothing to upgrade");
		//else GetActivityStatus().CallOutClicked();

	//	doDoobersWork(null,1);
	}

	
	/**
	 * progress: >=0 and <= 1
	 * */
	//FIXME : DONT USE THIS FUNCTION FOR PROGRESS=1 , RAHTER CREATE ONE MORE FLAG OR DIFF FUNCTION
	// currently this function is being called from many places.
	protected void updateActiveComponent(float progress){
		int max = (int)((float)(components.Count) * progress);
		
		if(isLevelUpgradable){
			int NUM_LEVELS_FACTORY = 3;	//TEMP: Should come from static cms data later on
			//TEMP: In READY state, show n-1 state. Show last complete state only after user acknowledge in built state.
			if(building.State.Equals(BuildingState.REGENERATE)){	
				NUM_LEVELS_FACTORY = 4;
			}
			max = (int)((float)(NUM_LEVELS_FACTORY - 1) * progress);
			Debug.LogWarning("UI_UpdateProgress - Transition: index" + max + " PercentageComplete: " + progress);
			
			if(currentLevel != max || activeComponent == null){
				if(activeComponent != null)
					activeComponent.SetActive(false);
				
				string newStatePrefabName = "3D-" + building.asset.spriteName + "_l" + max;
				Debug.Log("Building newStatePrefabName = " + newStatePrefabName);
				GameObject newStatePrefab = (GameObject) Resources.Load(newStatePrefabName, typeof(GameObject));
				if(newStatePrefab != null){
					GameObject newStateBuildingView = (GameObject) GameObject.Instantiate(newStatePrefab);
					newStateBuildingView.transform.parent = objectRootView.transform;
					newStateBuildingView.transform.localPosition = Vector3.zero;
					activeComponent = newStateBuildingView;
				}
				SetColor(activeComponent, new Color(1,1,1,0.75f));
				activeComponent.SetActive (true);	//Enable new component; Only one component is enable at any time
				
				particles = (ParticleSystem) objectRootView.GetComponentInChildren<ParticleSystem>();
				
				currentLevel = max;
			}
		}else{
			SetProgressHeight(progress);
			for (int i = 0; i < max && i < components.Count; i++) {
				//components[i].renderer.material.color = new Color(1,1,1,0.75f); 
				SetModeEffect(components[i],ModeEffect.NORMAL);
				components[i].SetActive (true);
			}
		}

	}

	private void SetProgressHeight(float progress) {
		progress *= this.building.asset.sizeHeight;
		foreach (GameObject go in components) {
			//components[i].renderer.material.color = new Color(1,1,1,0.75f); 
			if(go.GetComponent<Renderer>().material.shader == Shader.Find("Cutaways")) { //save modeeffect globaly and check with that
				go.GetComponent<Renderer>().material.SetFloat("_height",progress);
			}
		}
	}


   private void ShowCallout(AssetState state) {
		if (state !=null && state.activity!=null && dragState == DragState.PLACED) {
			GetActivityStatus().gameObject.SetActive(true);
			GetActivityStatus().ShowCallOut(state.activity.GetActionIconSprite());
			GetActivityStatus().SetText(building.data.level + ":" + building.State.state);
		} else {
			GetActivityStatus().gameObject.SetActive(false);
		}
	}
}

