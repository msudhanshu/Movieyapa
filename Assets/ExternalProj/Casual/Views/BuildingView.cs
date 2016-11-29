
using UnityEngine;
using System.Collections;

/**
 * A building view built using NGUI.
 */ 
public class BuildingView : UpgradableGridObject {

	/**
	 * Building Sprite
	 */ 
	public UISprite buildingSprite;

	/**
	 * Activity icon.
	 */ 
	public UISprite currentActivity;

	/**
	 * Game object wrapping the various
	 * widgets used to indicate progress. Also
	 * acts as a button.
	 */ 
	public AcknowledgeBuildingButton progressIndicator;

	/**
	 * Sprites showing progress of current activity.
	 */ 
	public UISprite[] progressRings;

	/* The UIWidget that is being dragged */
	protected UIWidget widget;
	
	/**
	 * Reference to the camera showing this object.
	 */ 
	protected UIDraggableCamera draggableCamera;

//	/**
//	 * Building this view references.
//	 */ 
//	protected Building building;

	
	/**
	 * Internal initialisation.
	 */
	void Awake(){
		target = transform;	
		myPosition = target.position;
		grid = GameObject.FindObjectOfType(typeof(AbstractGrid)) as AbstractGrid;
		draggableCamera = GameObject.FindObjectOfType(typeof(UIDraggableCamera)) as UIDraggableCamera;
	}

	/**
	 * Initialise the building view.
	 */ 
	virtual public void UI_Init(UpgradableBuilding building) {
		this.dragState = DragState.PLACING;
		this.building = building;
		widget = buildingSprite;
		buildingSprite.color = Color.white;
		if 	(building.State.Equals(BuildingState.CONSTRUCT) || building.State.Equals(BuildingState.READY) ) {
			buildingSprite.spriteName = "InProgress2x2";
		} else {
			buildingSprite.spriteName = building.asset.spriteName;
		}
		buildingSprite.MakePixelPerfect();
		progressIndicator.building = building;
		SetColor (building.Position);
		myPosition = transform.localPosition;
		SnapToGrid();
		// Force in front
		if 	(dragState == DragState.MOVING || dragState == DragState.PLACING)  widget.depth = 5000;
	}

	/**
	 * Update objects position
	 */ 
	override public void SetPosition(GridPosition pos) {
		Vector3 position = grid.GridPositionToArenaPosition(pos);
		widget.depth = 1000 - (int)position.z;
		position.z = target.localPosition.z;
		target.localPosition = position;
		myPosition = target.localPosition;
		// Force in front
		if 	(dragState == DragState.MOVING || dragState == DragState.PLACING)  widget.depth = 5000;
	}

	/**
	 * When object is dragged, move object then snap it to the grid.
	 */ 
	void OnDrag (Vector2 delta) {
		if (CanDrag && enabled && target != null) {
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			myPosition = myPosition + (Vector3)(delta * (draggableCamera != null ? draggableCamera.scale.x : 1));
			GridPosition pos = SnapToGrid();
			PostDrag(pos);
		} else {
			draggableCamera.Drag(delta);
		}
	}

	/**
	 * NGUI Click event... building clicked.
	 */ 
	public void OnClick() {
		if (building.asset.isObstacle) {
			if (building.CurrentTransition == null && building.CompletedTransition == null) {
				PopupManager.GetInstance().ShowPanel (PanelType.OBSTACLE_INFO);
				if (UIGamePanel.activePanel is UIBuildingInfoPanel) ((UIBuildingInfoPanel)UIGamePanel.activePanel).InitialiseWithBuilding(building);
			}
		} else if (building.State.Equals(BuildingState.REGENERATE)) {
			PopupManager.GetInstance().ShowPanel (PanelType.BUILDING_INFO);
			if (UIGamePanel.activePanel is UIBuildingInfoPanel) ((UIBuildingInfoPanel)UIGamePanel.activePanel).InitialiseWithBuilding(building);
		}
	}

	/**
	 * Snap object to grid. 
	 */
	override protected GridPosition SnapToGrid() {
		GridPosition pos = grid.ArenaPositionToGridPosition(myPosition);
		Vector3 position = grid.GridPositionToArenaPosition(pos);
		widget.depth = 1000 - (int)position.z;
		position.z = target.localPosition.z;
		target.localPosition = position;
		return pos;
	}

	/**
	 * Can we drag this object.
	 */
	override protected bool CanDrag {
		get {
			if (dragState == DragState.PLACING) return true;
			if (dragState == DragState.MOVING) return true;
			return false;
		}
		set {
			// Do nothing
		}
	}



	/**
	 * Building state changed, update view.
	 */ 
	override public void UI_UpdateState() {
		switch (building.State.ToBuildingState()) {
			case BuildingState.CONSTRUCT :
				// Snap to grid will ensure correct depth after dragging.
				SnapToGrid();
				progressIndicator.gameObject.SetActive(true);
				buildingSprite.color = Color.white;
				buildingSprite.spriteName = "InProgress2x2";
				progressIndicator.gameObject.SetActive(true);
				currentActivity.color = UIColor.DESATURATE;
				progressRings[0].color = UIColor.BUILD;
				foreach (UISprite progressRing in progressRings) {
					progressRing.fillAmount = 0.0f;
				}
				currentActivity.spriteName = "build_icon";
				break;
			case BuildingState.READY :
				// Snap to grid will ensure correct depth after dragging.
				SnapToGrid();
				progressIndicator.gameObject.SetActive(true);
				currentActivity.color = Color.white;
				buildingSprite.color = Color.white;
				progressRings[0].color = UIColor.BUILD;
				foreach (UISprite progressRing in progressRings) {
					progressRing.fillAmount = 1.0f;
				}
				StartCoroutine("DoBobble");
				break;
			case BuildingState.REGENERATE :
				// Snap to grid will ensure correct depth after dragging.
				SnapToGrid(); 
				if ((building.CurrentTransition == null || building.CurrentTransition.Type == "BUILD") &&
			    	(building.CompletedTransition == null || building.CompletedTransition.Type == "BUILD")) {
					progressIndicator.gameObject.SetActive(false);
				}
				buildingSprite.color = Color.white;
				buildingSprite.spriteName = building.asset.spriteName;
				buildingSprite.color = Color.white;
				buildingSprite.MakePixelPerfect();
				break;
//			case BuildingState.MOVING :
//				SetColor (building.Position);
//				widget.depth = 5000;
//				break;
		}
		// StartCoroutine(WaitThenRefreshPanel());
	}


	/**
	 * Building dragging state changed, update view.
	 */ 
	override public void UI_UpdateDragState(DragState state) {
		dragState = state;
		switch(dragState){
		case DragState.MOVING :
			SetColor (building.Position);
			widget.depth = 5000;
			break;

		case DragState.PLACED :
			SetColor (building.Position);
			widget.depth = 5000;
			break;
			
		case DragState.PLACING_INVALID :
			SetColor (building.Position);
			widget.depth = 5000;
			break;
		}
	}


	/**
	 * Activity completed.
	 */ 
	public void UI_StartTransition(StateTransition activity) {
		if (activity.Type != StateTransitionType.CONSTRUCT) {
			progressIndicator.gameObject.SetActive(true);
			StopCoroutine("DoBobble");
			currentActivity.color = UIColor.DESATURATE;
			progressRings[0].color = UIColor.GetColourForActivityType (activity.Type);
			foreach (UISprite progressRing in progressRings) {
				progressRing.fillAmount = activity.PercentageComplete;
			}
			currentActivity.spriteName = activity.Type.ToString().ToLower() + "_icon";
		}
	}


	/**
	 * Indicate progress on the progress ring.
	 */
	public void UI_UpdateProgress(StateTransition activity) {
		foreach (UISprite progressRing in progressRings) {
			progressRing.fillAmount = activity.PercentageComplete;
		}
	}

	/**
	 * Activity completed.
	 */ 
	public void UI_CompleteActivity(string type) {
		progressIndicator.gameObject.SetActive(true);
		currentActivity.color = Color.white;
		progressRings[0].color = UIColor.GetColourForActivityType(type);
		foreach (UISprite progressRing in progressRings) {
			progressRing.fillAmount = 1.0f;
		}
		StartCoroutine("DoBobble");
	}
	
	/**
	 * Store of auto generated rewards is full.
	 */ 
	public void UI_StoreFull() {
		if (!building.TransitionInProgress) {
			progressIndicator.gameObject.SetActive(true);
			currentActivity.spriteName = building.asset.generationType.ToString().ToLower() + "_icon";
			currentActivity.color = Color.white;
			progressRings[0].color = UIColor.GetColourForRewardType(building.asset.generationType);
			foreach (UISprite progressRing in progressRings) {
				progressRing.fillAmount = 1.0f;
			}
			StartCoroutine("DoBobble");
		}
	}
	
	/**
	 * Activity acknowledged.
	 */ 
	virtual public void UI_AcknowledgeActivity() {
		if (!building.TransitionInProgress) {
			if (building.StoreFull) UI_StoreFull ();		
			else progressIndicator.gameObject.SetActive(false);	
		}
	}

	/**
	 * After object dragged set color
	 */ 
	override protected void PostDrag (GridPosition pos) {
		base.PostDrag (pos);
		// Move forward so always clickable
		widget.depth = 5000;
	}

	virtual protected void SetColor(GridPosition pos) {
		if (BuildingModeGrid.GetInstance ().CanObjectBePlacedAtPosition (building, pos)) {
			buildingSprite.color = UIColor.PLACING_COLOR;
		} else {
			buildingSprite.color = UIColor.PLACING_COLOR_BLOCKED;
		}
	}
	
	protected IEnumerator DoBobble() {
		while (progressIndicator.gameObject.activeInHierarchy) {
			iTween.PunchPosition(progressIndicator.gameObject, new Vector3(0, 0.035f, 0), 1.5f);
			yield return new WaitForSeconds(Random.Range (1.0f, 1.5f));
		}
	}

	// TODO Don't think I need this anymore
	protected IEnumerator WaitThenRefreshPanel() {
		yield return true;
		if (buildingSprite != null && buildingSprite.panel != null) buildingSprite.panel.Refresh();
	}

}
