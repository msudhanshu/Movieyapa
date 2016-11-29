#define USE_NGUI_2X

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGamePanel : MonoBehaviour {
	
	public const float UI_DELAY = 0.75f;
	public const float UI_TRAVEL_DIST = 0.6f;
	
	public GameObject content;
	public PanelType panelType;
	public PanelType openFromPanelOnly;

	protected Animator animator;
	/**
	 * Position of the buttons when visible.
	 */ 
//	protected Vector3 showPosition;
	
	/**
	 * Position of the buttons when hidden.
	 */ 
//	protected Vector3 hidePosition;
	
	public static Dictionary <PanelType, UIGamePanel> panels;

	void Awake() {
		PopupManager.GetInstance().AddPanel(panelType, this);
		Init();
	}

	virtual protected void Init() {
		animator = this.gameObject.GetComponent<Animator>();
	}
	
	virtual public void InitialiseWithBuilding(Building building) {
	}
	
	virtual public void Show() {
		if (panelType == PanelType.DEFAULT && GridView.Instance != null) GridView.Instance.NormalMode(); 
		if (panelType == PanelType.PLACE_BUILDING && GridView.Instance != null) GridView.Instance.BuildingMode(); 
		if (panelType == PanelType.EDIT_PATHS && GridView.Instance != null) GridView.Instance.PathMode(); 
		if (activePanel == this) {
			StartCoroutine(DoReShow());
		} else if (activePanel == null || activePanel.panelType == openFromPanelOnly || openFromPanelOnly == PanelType.NONE) {
			if (activePanel != null) activePanel.Hide ();
			StartCoroutine(DoShow());
			activePanel = this;
		}
	}

	virtual public void Hide() {
		StartCoroutine(DoHide());
	}

	public static UIGamePanel activePanel {
		get { return PopupManager.GetInstance().activePanel; }
		set { PopupManager.GetInstance().activePanel = value; }
	}
		
	/**
	 * Reshow the panel (i.e. same panel but for a different object/building).
	 */ 
	virtual protected IEnumerator DoReShow() {
	//	iTween.MoveTo(content, hidePosition, UI_DELAY);
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
	//	iTween.MoveTo(content, showPosition, UI_DELAY);
	}
	
	
	/**
	 * Show the panel.
	 */ 
	virtual protected IEnumerator DoShow() {
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		content.SetActive (true);
		if(animator!=null)
			animator.SetTrigger("Opening");

		#if USE_NGUI_3X
		yield return true;
		GetComponent<UIPanel>().Refresh();
#endif
	//	iTween.MoveTo(content, showPosition, UI_DELAY);
	}
	
	/**
	 * Hide the panel. 
	 */
	virtual protected IEnumerator DoHide() {
	//	iTween.MoveTo(content, hidePosition, UI_DELAY);
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		content.SetActive (false);
	}
}

public enum PanelType {
	NONE,
	DEFAULT,
	BUY_BUILDING,
	PLACE_BUILDING,
	RESOURCE,
	BUY_RESOURCES,
	BUILDING_INFO,
	SELL_BUILDING,
	OBSTACLE_INFO,
	SPEED_UP,
	RECRUIT_OCCUPANTS,
	VIEW_OCCUPANTS,
	TARGET_SELECT,
	BATTLE_RESULTS,
	EDIT_PATHS,
	SHOP,
	BUY_HELPER,
	HELPER_BUSY,
	FPS_EXIT,
	QUEST,
	QUEST_TASKS,
	QUEST_COMPLETE,
	LEVEL_UP,
	UPGRADE_BUILDING,
	BUILDING_COMPLETE,
	EXPAND,
	JAM
}
