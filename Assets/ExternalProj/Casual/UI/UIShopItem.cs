//
//  UIShopItem.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//Item fo base/shop popup. It will have only look (name,image,clicklistner)/Iasset of the item.
public class UIShopItem : MonoBehaviour
{
	public Text titleLabel;
//	public UILabel descriptionLabel;
//	public UILabel allowsLabel;
	public Text costLabel;
//	public UILabel levelLabel;
//	public UISprite sprite;
	//public Sprite backgroundSprite;
	public Image itemIcon;
	public Button buildButton;
	public Sprite defaultSprite;
//	public UISprite extraCostSprite;
//	public UILabel extraCostLabel;

	//public string[] spriteNames;
	[HideInInspector]
	public Asset asset;

	public virtual void shopItemClicked() {
		Debug.Log("is an asset ");
		if (ResourceManager.GetInstance().CanBuild (asset)) {
			switch(asset.assetCategoryEnum) {
			case AssetCategoryEnum.HELPER:
				CharacterManager.GetInstance ().CreateCharacter(asset.id);
				PopupManager.GetInstance().ShowPanel (PanelType.DEFAULT);
				break;
			case AssetCategoryEnum.RPGCHARACTER:
				CharacterManager.GetInstance ().CreateCharacter(asset.id);
				PopupManager.GetInstance().ShowPanel (PanelType.DEFAULT);
				break;
			case AssetCategoryEnum.BUILDING:
			case AssetCategoryEnum.TOWNBLDG:
			case AssetCategoryEnum.DECORATION:
			case AssetCategoryEnum.TREE:
			case AssetCategoryEnum.CROP:
                BuildingManager3D.GetInstance ().CreateBuilding (asset.id);
				PopupManager.GetInstance().ShowPanel (PanelType.PLACE_BUILDING);
                break;
            }
		} else {
			PopupManager.GetInstance().ShowPanel (PanelType.JAM);
			Debug.LogWarning("This is where you bring up your in app purchase screen: JamPopup");
		}
	}

	/**
	 * Set up the building with the given type data.
     */
	virtual public void InitialiseWithAssetData(Asset type) {
		this.asset = type;
		titleLabel.text = type.name;
	//	descriptionLabel.text = type.description;
		CalculateCost();
	//	sprite.spriteName = type.spriteName;
	/*	if (type.additionalCosts != null && type.additionalCosts.Count > 0) {
			extraCostLabel.gameObject.SetActive(true);
			extraCostSprite.gameObject.SetActive(true);
			extraCostLabel.text = "" + type.additionalCosts[0].amount;
			extraCostSprite.spriteName = ResourceManager.GetInstance().GetCustomResourceType(type.additionalCosts[0].id).spriteName;
		} else {
			extraCostLabel.gameObject.SetActive(false);
			extraCostSprite.gameObject.SetActive(false);
		}*/
		UpdateAssetStatus ();
	}

	private void CalculateCost() {
		string costsString="";
		Dictionary<IGameResource,int> costsMap = ResourceManager.GetInstance().GetDiffResources( 
		                                         asset.GetAssetCosts().ConvertAll(x => (IResourceUpdate)x));
		if(costsMap == null) {
			costLabel.gameObject.SetActive(false);
			return;
		}
		foreach(KeyValuePair<IGameResource,int> item in costsMap ) {
			if(item.Value>0)
				costsString += item.Key.getCamelName() + "=" + item.Value + ",";
		}
		if(costsString != null && !Utility.StringEmpty(costsString) ) {
			costLabel.gameObject.SetActive(true);
			costLabel.text = costsString;
		} else {
			costLabel.gameObject.SetActive(false);
		}
	}

	/**
	 * Updates the UI (text, buttons, etc), based on if the building type
	 * requirements are met or not.
     */
	virtual public void UpdateAssetStatus() {
//		buildButton.gameObject.SetActive(true);
//		if(itemIcon != null) {
//			itemIcon.sprite = GameManager.LoadSprite(AssetsMarketImageName(),"Market/shop-trees");
//			if(itemIcon.sprite==null)
//				itemIcon.sprite = defaultSprite;
//		}
	}

	private string AssetsMarketImageName() {
		return asset.spriteName;
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