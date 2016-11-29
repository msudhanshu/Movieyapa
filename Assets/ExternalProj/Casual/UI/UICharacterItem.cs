using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * User interface for an individual buildings panel. Shown when
 * selecting which building type to build.
 */
public class UICharacterItem : UIBaseItem
{
	public override void buildButtonCallback() {
		if (ResourceManager.GetInstance().CanBuild (BuildingManager3D.GetInstance().GetBuildingTypeData(baseItemInfo.GetId()))) {
			CharacterManager.GetInstance().CreateCharacter(baseItemInfo.GetId());
			PopupManager.GetInstance().ShowPanel (PanelType.DEFAULT);
		} else {
			Debug.LogWarning("This is where you bring up your in app purchase screen");
		}
	}
}

