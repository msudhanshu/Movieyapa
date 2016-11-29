using UnityEngine;
using System.Collections;

/**
 * Cancel placing the building on the map and give the
 * resources back.
 */ 
public class CancelBuildingButton : MonoBehaviour {
	public void OnClick() {
		if (BuildingManager3D.ActiveBuilding != null) {
			BuildingManager3D.ActiveBuilding.ResetView();
			PopupManager.GetInstance().ShowPanel(PanelType.DEFAULT);
			GameEventTask.notifyAction("01state1");
		}
	}
}
