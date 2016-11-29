using UnityEngine;
using System.Collections;

public class MoveBuildingButton : MonoBehaviour {
		
	public void OnClick() {
		BuildingManager3D.ActiveBuilding.StartMoving ();
		PopupManager.GetInstance().ShowPanel (PanelType.PLACE_BUILDING);
		GameEventTask.notifyAction("discovered");
	}
}
