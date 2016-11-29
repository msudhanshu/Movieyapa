using UnityEngine;
using System.Collections;

public class SellBuildingButton : MonoBehaviour {
	
	public void OnClick() {
		PopupManager.GetInstance().ShowPanel (PanelType.SELL_BUILDING);
		((UISellBuildingPanel)UIGamePanel.activePanel).InitialiseWithBuilding(BuildingManager3D.ActiveBuilding);
		GameEventTask.notifyAction("03state2");
	}

}
