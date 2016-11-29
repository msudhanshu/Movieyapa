using UnityEngine;
using System.Collections;

/**
 * Panel for speeding up an activity by paying gold.
 */ 
public class UISpeedUpPanel : UIGamePanel {

	public UILabel goldLabel;
	public UISprite buildingSprite;
	public UILabel timeLabel;
	public UISprite headerSprite;
	public UISprite headerRing;
	
	//private Building building;
	
	override protected void Init() {
		// Force content to normal position then update cancel button position
//		content.transform.position = showPosition;	
	}
	
	/**
	 * Set up the building with the given building.
     */
	override public void InitialiseWithBuilding(Building building) {
	//	this.building = building;
		if (building.CurrentTransition != null) {
			BuildingManager3D.ActiveBuilding = building;
			timeLabel.text = string.Format("Time Remaining: {0} minutes {1} second{2}", (int)building.CurrentTransition.RemainingTime/60, building.CurrentTransition.RemainingTime, (building.CurrentTransition.RemainingTime != 1 ? "s" : ""));
			goldLabel.text =  ((int)Mathf.Max (1, (float) (building.CurrentTransition.RemainingTime + 1 ) / (float)BuildingManager3D.GOLD_TO_SECONDS_RATIO)).ToString ();
			buildingSprite.spriteName = building.asset.spriteName;
			headerSprite.spriteName = building.CurrentTransition.Type.ToString().ToLower() + "_icon";
			headerRing.color = UIColor.GetColourForActivityType(building.CurrentTransition.Type);
			StartCoroutine(UpdateLabels());
		} else {
			Debug.LogError ("Can't speed up a building with no activity");	
		}
		// Make sure we close if its zero
		if (building.CurrentTransition.RemainingTime < 1) {
			PopupManager.GetInstance().ShowPanel (PanelType.DEFAULT);
		}
	}
	
	override public void Show() {
		if (activePanel == null || activePanel.panelType == openFromPanelOnly || openFromPanelOnly == PanelType.NONE) {
			if (activePanel != null) activePanel.Hide ();
			StartCoroutine(DoShow());
			activePanel = this;
		}
	}

	override public void Hide() {
		StopAllCoroutines();
		StartCoroutine(DoHide());
	}
	
	/**
	 * Update the labels as time passes.
	 */ 
	protected IEnumerator UpdateLabels() {
		while (BuildingManager3D.ActiveBuilding != null && BuildingManager3D.ActiveBuilding.CurrentTransition != null && BuildingManager3D.ActiveBuilding.CurrentTransition.RemainingTime > 1) {
			timeLabel.text = string.Format("Time Remaining: {0} minutes {1} second{2}", (int)BuildingManager3D.ActiveBuilding.CurrentTransition.RemainingTime/60, BuildingManager3D.ActiveBuilding .CurrentTransition.RemainingTime, (BuildingManager3D.ActiveBuilding.CurrentTransition.RemainingTime != 1 ? "s" : ""));
			goldLabel.text = ((int)Mathf.Max (1, (float) (BuildingManager3D.ActiveBuilding .CurrentTransition.RemainingTime + 1 ) / (float)BuildingManager3D.GOLD_TO_SECONDS_RATIO)).ToString ();
			yield return true;
		}
		// Finished
		PopupManager.GetInstance().ShowPanel (PanelType.DEFAULT);
	}
	
	new protected IEnumerator DoShow() {
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		content.SetActive(true);
	}
	
	new protected IEnumerator DoHide() {
		content.SetActive(false);
		yield return true;
	}
	
}
