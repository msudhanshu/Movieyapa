using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradeBuildingButton : MonoBehaviour {

	public Text cost;

	public void PopulateData(UserAsset userAsset){
		AssetState upgradeState = userAsset.asset.GetUpgradeState();
		List<AssetStateCost> costs = upgradeState.GetCosts(userAsset.level);
		cost.text = "";
		if(costs!=null && costs.Count > 0)
			cost.text = "Costs:\n";
		foreach(AssetStateCost stateCost in costs){
			cost.text += stateCost.resourceId + ": <color=cyan>"+ stateCost.quantity + "</color>\n";
		}
	}
		
	public void OnClick() {
		UpgradableBuilding building = BuildingManager3D.ActiveBuilding as UpgradableBuilding;
		building = BuildingManager3D.ActiveBuilding.GetComponent<UpgradableBuilding>();
		building.UpgradeToNextLevel();
	}
}
