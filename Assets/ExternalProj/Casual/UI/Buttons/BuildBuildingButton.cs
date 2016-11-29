using UnityEngine;
using System.Collections;

/**
 * Button on the select building panel which starts the
 * PLACING process.
 */ 
public class BuildBuildingButton : MonoBehaviour {

	private string buildingTypeId;

	public void Init(string buildingTypeId) {
		this.buildingTypeId = buildingTypeId;
	}

	public void OnClick() {
		if (ResourceManager.GetInstance().CanBuild (BuildingManager3D.GetInstance().GetBuildingTypeData(buildingTypeId))) {
			BuildingManager3D.GetInstance ().CreateBuilding (buildingTypeId);
			PopupManager.GetInstance().ShowPanel (PanelType.PLACE_BUILDING);
		} else {
			Debug.LogWarning("This is where you bring up your in app purchase screen");
		}
	}
}
