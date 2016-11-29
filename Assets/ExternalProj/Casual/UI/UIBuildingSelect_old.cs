using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIBuildingSelect_old : UIGamePanel {

	public GameObject buildingScrollPanel;
	public GameObject buildingPanelPrefab;
	public GameObject cancelButton;
	
	private bool initialised = false;
	private List<UIShopItem> buildingSelectPanels;

	override protected void Init() {
	}
	
	void Start() {
		if (!initialised) {
//FIXME: Anuj - Commented to make the build work
//			buildingScrollPanel.GetComponent<UIScrollView>().ResetPosition();
			List <Asset> types = BuildingManager3D.GetInstance().GetAllBuildingTypes().Where(b=>!b.isObstacle && !b.isPath).ToList();
			buildingSelectPanels = new List<UIShopItem>();
			foreach(Asset type in types) {
				AddBuildingPanel(type);
			}
			buildingScrollPanel.GetComponent<UIGrid>().Reposition();
			initialised = true;
		}
		// Force content to normal position then update cancel button position
/*		content.transform.position = showPosition;	
		showPosition = cancelButton.transform.position;
		hidePosition = cancelButton.transform.position + new Vector3(0, UI_TRAVEL_DIST, 0);
		cancelButton.transform.position = hidePosition;*/
	}

	override public void Show() {
		if (activePanel != null) activePanel.Hide ();
		foreach(UIShopItem p in buildingSelectPanels) {
			p.UpdateAssetStatus();
		}
		StartCoroutine(DoShow ());
		activePanel = this;
	}
	
	override public void Hide() {
		StartCoroutine(DoHide ());
	}
	
	new protected IEnumerator DoShow() {
		content.SetActive(true);
		yield return true;
		GetComponent<UIPanel>().Refresh();
		//FIXME: Anuj - Commented to make the build work
//		buildingScrollPanel.GetComponent<UIScrollView>().ResetPosition();
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		cancelButton.SetActive (true);
		yield return true;
		GetComponent<UIPanel>().Refresh();
//		iTween.MoveTo(cancelButton, showPosition, UI_DELAY);
	}
	
	new protected IEnumerator DoHide() {
		content.SetActive(false);
//		iTween.MoveTo(cancelButton, hidePosition, UI_DELAY);
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		cancelButton.SetActive (false);
	}
	
	private void AddBuildingPanel(Asset type) {
		GameObject panelGo = (GameObject) NGUITools.AddChild(buildingScrollPanel, buildingPanelPrefab);
		UIShopItem panel = panelGo.GetComponent<UIShopItem>();
		panel.InitialiseWithAssetData(type);
		buildingSelectPanels.Add (panel);
	}
}
