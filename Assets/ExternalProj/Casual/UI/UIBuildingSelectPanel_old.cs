using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * User interface for an individual buildings panel. Shown when
 * selecting which building type to build.
 */
//it changed to UIBuildingItem<-UIShopItem
public class UIBuildingSelectPanel_old : MonoBehaviour
{
	public UILabel titleLabel;
	public UILabel descriptionLabel;
	public UILabel allowsLabel;
	public UILabel costLabel;
	public UILabel levelLabel;
	public UISprite sprite;
	public UISprite backgroundSprite;
	public BuildBuildingButton buildButton;
	public UISprite extraCostSprite;
	public UILabel extraCostLabel;

	public string[] spriteNames;

	private AssetData type;

	/**
	 * Set up the building with the given type data.
     */
	virtual public void InitialiseWithBuildingType(AssetData type) {
		this.type = type;
		titleLabel.text = type.name;
		descriptionLabel.text = type.description;
		costLabel.text = type.cost.ToString();
		sprite.spriteName = type.spriteName;
		if (type.additionalCosts != null && type.additionalCosts.Count > 0) {
			extraCostLabel.gameObject.SetActive(true);
			extraCostSprite.gameObject.SetActive(true);
			extraCostLabel.text = "" + type.additionalCosts[0].amount;
			extraCostSprite.spriteName = ResourceManager.GetInstance().GetCustomResourceType(type.additionalCosts[0].id).spriteName;
		} else {
			extraCostLabel.gameObject.SetActive(false);
			extraCostSprite.gameObject.SetActive(false);
		}
		UpdateBuildingStatus ();
	}

	/**
	 * Updates the UI (text, buttons, etc), based on if the building type
	 * requirements are met or not.
     */
	virtual public void UpdateBuildingStatus() {
		// Level indicator
		if (type.level == 0) {
			levelLabel.text = "";
		} else if (type.level <= ResourceManager.GetInstance().Level) {
			levelLabel.text = "[000000]Level " + type.level;
		} else {
			levelLabel.text = "[ff0000]Requires Level " + type.level;
		}

		// We don't check for the amount of resources here, becaue we want to prompt users
		// to buy resources. However you could add a check for resources if you preferred.
		// To do this check that ResourceManager.Instance.Resource >= type.cost.
		if (BuildingManager3D.GetInstance().CanBuildBuilding(type.id)) {
			allowsLabel.text = "[000000]Allows: " + FormatIds(type.allowIds, false);
			backgroundSprite.spriteName = spriteNames[0];
			buildButton.gameObject.SetActive(true);
			buildButton.Init(type.id);
		} else {
			allowsLabel.text = "[000000]Requires: " + FormatIds(type.requireIds, true);
			backgroundSprite.spriteName = spriteNames[1];
			buildButton.gameObject.SetActive(false);
		}
	}

	/**
	 * Formats the allows/required identifiers to be nice strings, coloured correctly.
	 * Returns the identifiers.
     */
	virtual protected string FormatIds(List<string> allowIds, bool redIfNotPresent) {
		BuildingManager3D manager = BuildingManager3D.GetInstance();
		string result = "";
		foreach (string id in allowIds) {
			if (redIfNotPresent && !manager.PlayerHasBuilding(id) && !OccupantManager.GetInstance().PlayerHasOccupant(id)) {
				result += "[ff0000]";
			} else {
				result += "[000000]";
			}
			Asset type = manager.GetBuildingTypeData(id);
			OccupantTypeData otype = OccupantManager.GetInstance().GetOccupantTypeData(id);
			if (type != null) {
				result += type.name + ", ";
			} else if (otype != null) {
				result += otype.name + ", ";
			} else {
				Debug.LogWarning("No building or occupant type data found for id:" + id);
				result += id + ", ";
			}
		}
		if (result.Length > 2) {
			result = result.Substring(0, result.Length - 2);
		} else {
			return "Nothing";
		}
		return result;
	}
}

