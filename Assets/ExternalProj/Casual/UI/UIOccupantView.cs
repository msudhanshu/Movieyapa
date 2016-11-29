using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Panel for viewing the occupants of a building.
 */ 
public class UIOccupantView : UIGamePanel {

	public GameObject occupantScrollPanel;
	public GameObject occupantPanelPrefab;
	public GameObject cancelButton;
	
	private bool initialised = false;
	private List<UIOccupantViewPanel> occupantViewPanels;
	
	override protected void Init() {
		// Force content to normal position then update cancel button position
/*		content.transform.position = showPosition;	
		showPosition = cancelButton.transform.position;
		hidePosition = cancelButton.transform.position + new Vector3(0, UI_TRAVEL_DIST, 0);
		cancelButton.transform.position = hidePosition;	*/
	}
	
	override public void InitialiseWithBuilding(Building building) {
		if (!initialised) {
			List <OccupantData> data = building.Occupants;
			occupantViewPanels = new List<UIOccupantViewPanel>();
			//FIXME: Anuj - Commented to make the build work
//			occupantScrollPanel.GetComponent<UIScrollView>().ResetPosition();
			if (data != null)  {
				foreach(OccupantData o in data) {
					AddOccupantPanel(o, false);
				}
			}
			if ((building.CurrentTransition != null && building.CurrentTransition.Type == StateTransitionType.RECRUIT) || ((building.CompletedTransition != null && building.CompletedTransition.Type == StateTransitionType.RECRUIT) )) {
				OccupantData no = new OccupantData();
				no.Type = OccupantManager.GetInstance().GetOccupantTypeData(building.CurrentTransition.SupportingId);
				AddOccupantPanel(no, true);
				// TODO Coroutine to allow constant update of this panel (or maybe it should be in the panel itself?)
			}
			occupantScrollPanel.GetComponent<UIGrid>().Reposition();
			initialised = true;
		}
	}

	override public void Show() {
		if (activePanel == this) {
			StartCoroutine(ClearThenReshow());
		} else {
			if (activePanel != null) activePanel.Hide ();
			StartCoroutine(ClearThenReshow());
			StartCoroutine(DoShow ());
			activePanel = this;
		}
	}
	
	protected IEnumerator ClearThenReshow() {
		if (occupantViewPanels != null) {
			foreach (UIOccupantViewPanel o in occupantViewPanels) {
				Destroy(o.gameObject);
			}
		}
		yield return true;
		initialised = false;
		InitialiseWithBuilding(BuildingManager3D.ActiveBuilding);
	}
	
	override public void Hide() {
		StartCoroutine(DoHide ());
	}
	
	override protected IEnumerator DoShow() {
		content.SetActive(true);
		yield return true;
		GetComponent<UIPanel>().Refresh();
		//FIXME: Anuj - Commented to make the build work
//		occupantScrollPanel.GetComponent<UIScrollView>().ResetPosition();
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		cancelButton.SetActive (true);
		yield return true;
		GetComponent<UIPanel>().Refresh();
//		iTween.MoveTo(cancelButton, showPosition, UI_DELAY);
	}
	
	override protected IEnumerator DoHide() {
		content.SetActive(false);
//		iTween.MoveTo(cancelButton, hidePosition, UI_DELAY);
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		cancelButton.SetActive (false);
	}
	
	private void AddOccupantPanel(OccupantData data, bool inProgress) {
		GameObject panelGo = (GameObject) NGUITools.AddChild(occupantScrollPanel, occupantPanelPrefab);
		UIOccupantViewPanel panel = panelGo.GetComponent<UIOccupantViewPanel>();
		panel.InitialiseWithOccupant(data, inProgress);
		occupantViewPanels.Add (panel);
	}
}
