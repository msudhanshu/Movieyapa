using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class DraggableGridObject : PlaceableGridObject
{
	/* Drag state of the building. */

	private DragState _dragState;

	virtual public DragState dragState {
		get { return _dragState; }
		protected set { _dragState = value; }
	}
	
	public bool isMoving(){
		return dragState == DragState.MOVING;
	}
	
	public void OnDragEvent (Vector3 newPosition) {
		if (CanDrag && enabled) {
			myPosition = newPosition;
			//GridPosition pos = SnapToGrid();
			GridPosition pos;
			GridHeight height;
			SnapToGrid(out pos, out height);
			PostDrag(pos, height);
		}
	}


	/**
	 * Can we drag this object.
	 */
	virtual protected bool CanDrag {
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
	 * After object dragged set color
	 */ 
	virtual protected void PostDrag (GridPosition pos) {
		if (!isMoving()) {
			assetController.Position = pos;

		} else {
			assetController.MovePosition = pos;
		}
		SetColor (pos);
	}

	/**
	 * After object dragged set color
	 */ 
	virtual protected void PostDrag (GridPosition pos, GridHeight height) {
		if (!isMoving()) {
			assetController.Position = pos;
			assetController.Height = height;
			
		} else {
			assetController.MovePosition = pos;
			assetController.MoveHeight = height;
		}
		SetColor (pos);
	}

	/**
	 * Building dragging state changed, update view.
	 */ 
	virtual public void UI_UpdateDragState(DragState dragState) {
		this.dragState = dragState;
		switch(dragState){
		case DragState.PLACING :
			this.gameObject.layer = BuildingManager3D.SELECTION_LAYER;
			//updateActiveComponent(1.0f);
			GetActivityStatus().gameObject.SetActive(false);
			SetEditModeColor();
			if (particles != null) particles.Stop ();
			break;

		case DragState.MOVING :
			this.gameObject.layer = BuildingManager3D.SELECTION_LAYER;
			GetActivityStatus().gameObject.SetActive(false);
			//updateActiveComponent(1.0f);
			SetEditModeColor();
			if (particles != null) particles.Stop ();
			break;

		case DragState.PLACED :
			this.gameObject.layer = BuildingManager3D.BUILDING_LAYER;
			SetNormal();
			GetActivityStatus().gameObject.SetActive(true);
			//updateActiveComponent(0.0f);
			//updateActiveComponent(building.transitionPercentageComplete());
			if (particles != null) particles.Play ();
			break;

		case DragState.PLACING_INVALID :
			SetEditModeColor();
			GetActivityStatus().gameObject.SetActive(false);
			//updateActiveComponent(1.0f);
			if (particles != null) particles.Stop ();
			break;
		}
	}

	/**
	 * Resets view if moving/placing
	 */
	public void Reset(){
		if(dragState == DragState.MOVING ) {
			SetPosition(assetController.Position);
			SetHeight(assetController.Height);
			UI_UpdateDragState(DragState.PLACED);
		} else if(dragState == DragState.PLACING){
			dragState = DragState.PLACING_INVALID; // FIXME: hack fix for the case when reset view is being called in Sell
			BuildingManager3D.GetInstance().SellBuilding(assetController as Building, false);
		}
	}

	public void Set(){
		if (dragState == DragState.MOVING) {
			// Moving
			if (grid.CanObjectBePlacedAtPosition (assetController, assetController.MovePosition)) {
				if (BuildingManager3D.GetInstance().MoveBuilding()) {
					PopupManager.GetInstance().ShowPanel(PanelType.DEFAULT);
				}
			}
		} else {
			// Placing
			if (grid.CanObjectBePlacedAtPosition (assetController, assetController.Position)) {
				if (BuildingManager3D.GetInstance().PlaceBuilding()) {
					PopupManager.GetInstance().ShowPanel(PanelType.DEFAULT);
				}
			}
		}
	}

}

