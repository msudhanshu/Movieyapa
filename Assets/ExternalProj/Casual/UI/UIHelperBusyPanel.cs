using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * Panel for selling a building.
 */ 
public class UIHelperBusyPanel: UIGamePanel {

	public Text goldLabel;
	public Text resourceLabel;
//	public UISprite buildingSprite;
	public Text messageLabel;
	public GameObject sellForGoldButton;
	
	override protected void Init() {
		base.Init ();
		// Force content to normal position then update cancel button position
//		content.transform.position = showPosition;	
	}
	
	/**
	 * Set up the building with the given building.
     */
	override public void InitialiseWithBuilding(Building building) {
		resourceLabel.text = ((int)building.asset.cost * BuildingManager3D.RECLAIM_PERCENTAGE).ToString();
		goldLabel.text =  ((int)Mathf.Max(1.0f, (int)(building.asset.cost * BuildingManager3D.GOLD_SELL_PERCENTAGE))).ToString ();
		//buildingSprite.spriteName = building.asset.spriteName;
		messageLabel.text = string.Format ("Are you sure you want to sell your {0} for {1} resources?", building.asset.name, (BuildingManager3D.GOLD_SELL_PERCENTAGE <= 0 ? "": "gold or "));
		if (BuildingManager3D.GOLD_SELL_PERCENTAGE <= 0) sellForGoldButton.SetActive(false);
	}
	
	override public void Show() {
		if (activePanel == null || activePanel.panelType == openFromPanelOnly || openFromPanelOnly == PanelType.NONE) {
			if (activePanel != null) activePanel.Hide ();
			StartCoroutine(DoShow());
			activePanel = this;
		}
	}

	override public void Hide() {
		StartCoroutine(DoHide());
	}
	
	new protected IEnumerator DoShow() {
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		content.SetActive(true);
		if(animator!=null)
			animator.SetTrigger("Opening");
	}
	
	new protected IEnumerator DoHide() {
		content.SetActive(false);
		yield return true;
	}
	
}
