using UnityEngine;
using System.Collections;

/**
 * Place the building on the map.
 */ 
public class PlaceBuildingButton : MonoBehaviour {

	public void OnClick() {
		BuildingManager3D.ActiveBuilding.SetView();
	}
}
