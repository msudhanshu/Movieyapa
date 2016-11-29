using UnityEngine;
using System.Collections;

/**
 * Acknowledge building turning it from ready to built.
 */ 
public class AcknowledgeBuildingButton : MonoBehaviour {

	public Building building;

	public void OnClick() {
		if (building.State.Equals(BuildingState.READY)) {
			BuildingManager3D.GetInstance ().AcknowledgeBuilding (building);
		} else if (building.State.Equals(BuildingState.CONSTRUCT) || building.CurrentTransition != null) {
			PopupManager.GetInstance().ShowPanel(PanelType.SPEED_UP);
			if (UIGamePanel.activePanel is UISpeedUpPanel) ((UISpeedUpPanel)UIGamePanel.activePanel).InitialiseWithBuilding(building);
		} else {
			building.Acknowledge();
		}
	}
}