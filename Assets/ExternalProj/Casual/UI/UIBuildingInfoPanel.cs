#define USE_NGUI_2X

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/**
 * User interface shown when a building is selected.
 */ 
public class UIBuildingInfoPanel : UIGamePanel {
	
	/**
	 * Collect button (which is optional depending on buildings auto activity).
	 */ 
//	public CollectButton collectButton;
	
	/**
	 * Button for recruiting occupants.
	 */ 
//	public RecruitButton recruitButton;
	
	/**
	 * Button for viewing occupants.
	 */ 
//	public UIButtonSwitchPanel occupantButton;

	public Text name;
	public Text level;

	public UpgradeBuildingButton upgradeButton;

	/**
	 * List of buttons which are set up with a function depending on the building type.
	 */
	public ActivityButton[] buttonTemplates;

	public Building building;

	/**
	 * Set up the building with the given building.
     */
	override public void InitialiseWithBuilding(Building building) {
		BuildingManager3D.ActiveBuilding = building;
		this.building = building;
	}

	public void PopulateData(){
		/*		if (building.IsRegenerateable()) {
			collectButton.gameObject.SetActive(true);	
			collectButton.Init(building);
		} else {
			collectButton.gameObject.SetActive(false);
		}
*/
		name.text = building.asset.name;
		level.text = "Level "+ building.userAsset.level;

		if (building.IsUpgradable()) {
			upgradeButton.gameObject.SetActive(true);
			upgradeButton.PopulateData(building.userAsset as UserAsset);
		} else {
			upgradeButton.gameObject.SetActive(false);
		}
		
		/*		if (OccupantManager.GetInstance().CanBuildingRecruit(building.asset.id) && building.CurrentTransition == null && building.CompletedTransition == null){
			recruitButton.gameObject.SetActive(true);	
		} else {
			if (recruitButton != null) recruitButton.gameObject.SetActive(false);
		}
		if (OccupantManager.GetInstance().CanBuildingHoldOccupants(building.asset.id)){
			occupantButton.gameObject.SetActive(true);	
		} else {
			if (occupantButton != null) occupantButton.gameObject.SetActive(false);
		}*/
		if (building.CurrentTransition != null || building.CompletedTransition != null) {
			for (int i = 0; i < buttonTemplates.Length; i++){
				buttonTemplates[i].gameObject.SetActive(false);
			}
		} else if (buttonTemplates.Length > 0) {
			int i = 0;
			foreach (string activityType in building.asset.activities) {
				// Add special cases for special activities here
				buttonTemplates[i].gameObject.SetActive(true);
				buttonTemplates[i].InitWithActivityType(DoActivity, activityType,  "");
				i++;
			}
			for (int j = i; j < buttonTemplates.Length; j++){
				buttonTemplates[j].gameObject.SetActive(false);
			}
		}
	}
	/**
	 * Show the panel.
	 */ 
	override protected IEnumerator DoShow() {
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		content.SetActive (true);
		PopulateData();
		if(animator!=null)
			animator.SetTrigger("Opening");

	//	content.GetComponent<UIGrid>().Reposition();
	//	iTween.MoveTo(content, showPosition, UI_DELAY);
	}
	
			
	/**
	 * Reshow the panel (i.e. same panel but for a different object/building).
	 */ 
	override protected IEnumerator DoReShow() {
	//	iTween.MoveTo(content, hidePosition, UI_DELAY);
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		PopulateData();
		if(animator!=null)
			animator.SetTrigger("Opening");

	//	content.GetComponent<UIGrid>().Reposition();
	//	iTween.MoveTo(content, showPosition, UI_DELAY);
	}
	
	/**
	 * Start the generic activty function.
	 */ 
	public void DoActivity(string type, string supportingId) {
		building.StartTransition (type, System.DateTime.Now, supportingId);
		PopupManager.GetInstance().ShowPanel (PanelType.DEFAULT);
	}

	public UIBuildingInfoPanel Instance {
		get; private set;
	}
}

/**
 * Delegate type used by the buttons.
 */ 
public delegate void ActivityDelegate(string type, string supportingId);