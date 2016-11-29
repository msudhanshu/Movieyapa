using UnityEngine;
using System.Collections;

public class DoSellBuildingButton : MonoBehaviour {

	public bool isGold = false;

	public void OnClick() {
		if (isGold) {
			BuildingManager3D.GetInstance ().SellBuildingForGold (BuildingManager3D.ActiveBuilding);
		} else {
			BuildingManager3D.GetInstance ().SellBuilding (BuildingManager3D.ActiveBuilding, false);
		}
		PopupManager.GetInstance().ShowPanel (PanelType.DEFAULT);
		BuildingManager3D.ActiveBuilding = null;
	}
}
