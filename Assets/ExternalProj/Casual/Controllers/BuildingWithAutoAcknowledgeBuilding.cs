using UnityEngine;
using System.Collections;

/**
 * A building implementation that automatically finishes all tasks.
 */ 
public class BuildingWithAutoAcknowledgeBuilding : UpgradableBuilding
{
	/**
	 * Finish building auto acknolwedge.
	 */ 
	override public void CompleteBuild() {
		base.CompleteBuild ();
//		State = BuildingState.READY;
//		this.gameObject.layer = BuildingManager3D.BUILDING_LAYER;
//		BuildingManager3D.GetInstance().AcknowledgeBuilding (this);
//		Acknowledge();
		// view.SendMessage ("UI_UpdateState");
	}
}

