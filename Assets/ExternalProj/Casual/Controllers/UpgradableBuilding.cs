using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class UpgradableBuilding : Building
{
	string MIN_XP_LEVEL_FOR_UPGRADE_LEVEL = "min_game_level";

	virtual public bool UpgradeToNextLevel() {
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
			Collect();
			transitionController.ForceFinishCurrentTransaction();
		}

		//Force the transition Controller to upgrade state
		transitionController.SetNextState(upgradeState);
		transitionController.AcknowledgeTransition();
		return true;
	}

	override public void TransitionCompleted(StateTransition transition) {
		view.SendMessage("UI_CompleteTransition", transition);
		AssetState upgradeState = data.asset.GetUpgradeState();
		if (upgradeState!=null && upgradeState.state == State.state)
			data.level++;
		//TODO : Start moving to next state - who should do this ?
		//SetNextState();
		if (this.asset.IsExpansionAsset ()) {
			this.OnExpansion();
		}

	}

}
