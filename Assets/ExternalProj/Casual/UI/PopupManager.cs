using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : Manager<PopupManager> {

	public UIGamePanel activePanel;
	public Dictionary <PanelType, UIGamePanel> panels = new Dictionary <PanelType, UIGamePanel>();
	private HashSet<PanelType> restrictOtherPanels = new HashSet<PanelType>();

	override public void StartInit () {
		restrictOtherPanels.Add(PanelType.PLACE_BUILDING);
	}
	
	override public void PopulateDependencies () {
	}

	Queue<ScheduledPopup> scheduledPopups = new Queue<ScheduledPopup>();

	public void SchedulePopup(UIGamePanel panel, long delay) {
		scheduledPopups.Enqueue(new ScheduledPopup(panel, delay));

		if(scheduledPopups.Count == 1 && activePanel != null && activePanel.panelType == PanelType.DEFAULT)
			ShowScheduledPopup();
	}

	public void SchedulePopup(PanelType type, long delay = 1) {
		SchedulePopup(panels[type], delay);
	}

	public void RemoveScheduledPopup() {
		//TODO: Based on class name, remove from the scheduled Popups queue
	}

	private void ShowScheduledPopup() {
		StartCoroutine(DoShowScheduledPopup());
	}
	private IEnumerator DoShowScheduledPopup() {
		ScheduledPopup popup = scheduledPopups.Dequeue();
		if (popup == null)
			yield break;
		float secs = popup.GetDelay();
		yield return new WaitForSeconds(popup.GetDelay());
		UIGamePanel panel = popup.GetPanel();
		panel.Show();
	}

	private void HideScheduledPopup() {
		activePanel.Hide();
		if(scheduledPopups.Count > 0)
			ShowScheduledPopup();
	}

	public void AddPanel(PanelType panelType, UIGamePanel panel) {
		panels.Add(panelType, panel);
		if(panelType == PanelType.DEFAULT) {
			activePanel = panel;
		}
	}

	public void ShowPanel(PanelType panelType, bool allowOtherPopups = true) {

		if (panelType == PanelType.DEFAULT) {
			BuildingManager3D.ActiveBuilding = null;
			allowOtherPopups = true;

			if (scheduledPopups.Count > 0) {
				ShowScheduledPopup();
			}
		}

		if (restrictOtherPanels.Contains(activePanel.panelType) && panelType != PanelType.DEFAULT)
			return;

		if(panels.ContainsKey(panelType)) {
			UIGamePanel panel = panels[panelType];
			panel.Show();
		}
	}

	public UIGamePanel getPanel(PanelType type){
		if(panels.ContainsKey(type))
			return panels[type];
		return null;
	}
	public void HideActivePanel() {
		//activePanel.Hide ();
		//ShowScheduledPopup();
	}
	

}
