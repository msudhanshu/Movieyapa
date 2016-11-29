using UnityEngine;
using System.Collections;

/**
 * A button which can perform the clear activity.
 */ 
public class ClearObstacleButton : CollectButton {
	
	/**
	 * Set up the visuals.
	 */
	override public void Init(Building building) {
		myBuilding = building;
//		resourceLabel.text = "" + building.asset.cost; 
	}
	
	/**
	 * Button clicked, start activity.
	 */ 
	override public void OnClick() {
		if (ResourceManager.GetInstance().Silver > myBuilding.asset.cost) {
			PopupManager.GetInstance().ShowPanel(PanelType.DEFAULT);
		}
	}
}
