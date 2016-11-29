//
//  UIBaseItem.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//Item fo base/shop popup. It will have only look (name,image,clicklistner)/IBaseItemInfo of the item.
public class UIBaseItem : MonoBehaviour
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
	IBaseItemClickListner itemClickListner;

//	public UISprite extraCostSprite;
//	public UILabel extraCostLabel;

	//public string[] spriteNames;
	[HideInInspector]
	public IUIBaseItemInfo baseItemInfo;

	public virtual void buildButtonCallback() {
		/*if (ResourceManager.Instance.CanBuild (BuildingManager3D.GetInstance().GetBuildingTypeData(assetData.id))) {
			BuildingManager3D.GetInstance ().CreateBuilding (assetData.id);
			PopupManager.GetInstance().ShowPanel (PanelType.PLACE_BUILDING);
		} else {
			Debug.LogWarning("This is where you bring up your in app purchase screen");
		}
		*/
		if(itemClickListner!=null) itemClickListner.ItemSelected(this);
	}

	/**
	 * Set up the building with the given type data.
     */
	virtual public void InitialiseWithAssetData(IUIBaseItemInfo type, IBaseItemClickListner itemClickListner=null) {
		this.itemClickListner = itemClickListner;
		this.baseItemInfo = type;
		titleLabel.text = type.GetName();
	//	descriptionLabel.text = type.description;
		CalculateCost();
	//	sprite.spriteName = type.spriteName;
	/*	if (type.additionalCosts != null && type.additionalCosts.Count > 0) {
			extraCostLabel.gameObject.SetActive(true);
			extraCostSprite.gameObject.SetActive(true);
			extraCostLabel.text = "" + type.additionalCosts[0].amount;
			extraCostSprite.spriteName = ResourceManager.Instance.GetCustomResourceType(type.additionalCosts[0].id).spriteName;
		} else {
			extraCostLabel.gameObject.SetActive(false);
			extraCostSprite.gameObject.SetActive(false);
		}*/
		UpdateAssetStatus ();
	}

	private void CalculateCost() {
		string costsString="";
		Dictionary<IGameResource,int> costsMap = baseItemInfo.GetItemCosts();
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
//			itemIcon.sprite = GameManager.LoadSprite(baseItemInfo.GetItemImageName(),"Market/shop-trees");
//			if(itemIcon.sprite==null)
//				itemIcon.sprite = defaultSprite;
//		}
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

//TODO : Implement
// asset and assetcategory should implement this... and UIBaseItem will interact with this to show the item in popup.
public interface IUIBaseItemInfo {
	string GetId();
	string GetName();
	string GetDescription();
	string GetItemImageName();
//	class Getclazz();
	//string GetCost();
    //string GetCostIconImageName();
	//TODO : OR/AND ... i.e cost will be two resources combined or either of them.
	Dictionary<IGameResource,int> GetItemCosts();
}

//may be just id would be sufficient
//this interface is to handle all item selection from the popupclass itself.
public interface IBaseItemClickListner {
	void ItemSelected(UIBaseItem UIBaseItem);
}
