//
//  ShopPopup.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class UIShopPopup: UIBasePopup,IBaseItemClickListner {

	private List<UIShopItem> uiShopItems;
	private List<UIBaseItem> uiCategoryItems;
	public GameObject shopItemPrefab;
	public GameObject categoryItemPrefab;

	protected static Dictionary<string, List<Asset>> categoryAssetsMap = new Dictionary<string, List<Asset>>() ;
	protected AssetCategory selectedCategory;
    
	override protected  void InitDefaultPopup() {
		IntializeContents();
		SwitchToCategoryTab();
	}

    private void IntializeContents() {
		List<AssetCategory> assetCategoryList =  AssetCategory.GetAssetCategories();
		/*********************REMOVE NOT TO BE DISPLAYED CATEGORY **************************/
		List<AssetCategory> toBeRemovedCategoryList = new List<AssetCategory>();
		
		foreach(AssetCategory category in assetCategoryList)
			if(category.IsHiddenInMarket())
				toBeRemovedCategoryList.Add(category);
		
		//resources category is part of every location
		//AssetCategory resourcesCategory = AssetCategory.getCategory(AssetCategoryName.RESOURCES);
		//toBeRemovedCategoryList.Remove(resourcesCategory);
		
		foreach(AssetCategory category in toBeRemovedCategoryList)
			assetCategoryList.Remove(category);

		if(assetCategoryList.Count > 0 && categoryAssetsMap.Count <= 0) {
			//if(categoryAssetsMap.get(assetCategoryList.get(0).id) == null) {
				foreach(AssetCategory category in assetCategoryList){
					/*ShopCategory shopCategoryItem = new ShopCategory(skin, category, this);
					categoryContainer.add(shopCategoryItem);
					Cell<Button> cell = shopCategoryItem.getButtonCell();
					initCategoriesMenu(categoryContainer, cell,i++,assetCategoryList.size()) ;
					*/
				//todo : populate subcategories
		
				List<Asset> assetsInCategory= BuildingManager3D.GetInstance().GetAllBuildingTypes().Where(b=>b.assetCategoryEnum==category.type).ToList();
				categoryAssetsMap[category.id] = assetsInCategory;
            }
            //}
		}
	}

	private void SwitchToCategoryTab() {
		List<AssetCategory> assetsCategoryList = new List<AssetCategory>();
		foreach (KeyValuePair<string , List<Asset>> item in categoryAssetsMap) {
			assetsCategoryList.Add( AssetCategory.GetAssetCategory(item.Key));
		}
		List<IUIBaseItemInfo> allAssetCategoryItemData = assetsCategoryList.ConvertAll(x=>x as IUIBaseItemInfo);
		PopulatePopupData(allAssetCategoryItemData);
		backButton.gameObject.SetActive(false);
	}

	virtual public void ItemSelected(UIBaseItem baseItem) {
		//SWITCH TO ASSET TAB
		if(baseItem.baseItemInfo is AssetCategory) {
			//AssetCategory assetCategory = baseItem.asset as AssetCategory;
			Debug.Log("is an assetcategory "+baseItem.baseItemInfo.GetId());
			List<Asset> assets = categoryAssetsMap[baseItem.baseItemInfo.GetId()];
			List<Asset> allAssetCategoryItemData = assets.ConvertAll(x=>x as Asset);
			PopulatePopupData(allAssetCategoryItemData);
			backButton.gameObject.SetActive(true);
		}
	}

	override public void Show() {
		if (uiShopItems != null) {
			foreach (UIShopItem p in uiShopItems) {
				p.UpdateAssetStatus ();
			}
		}
		base.Show();
		//PopupManager.GetInstance().SchedulePopup(PanelType.QUEST, 10000);
	}


	virtual protected void PopulatePopupData(List<IUIBaseItemInfo> allItemData) {
		if(uiCategoryItems==null)
			uiCategoryItems = new List<UIBaseItem>();
		ClearAllItems();
		
		int i = 0;
		foreach(IUIBaseItemInfo type in allItemData) {
			Transform go = CreateItemPrefab(categoryItemPrefab,i);
			UIBaseItem panel = go.GetComponent<UIBaseItem>();
			panel.InitialiseWithAssetData(type,this);
			uiCategoryItems.Add (panel);
			i++;
		}
		ResizeGridLayout(i);
	}

	virtual protected void PopulatePopupData(List<Asset> allItemData) {
		if(uiShopItems==null)
			uiShopItems = new List<UIShopItem>();
		ClearAllItems();

		int i = 0;
		foreach(Asset type in allItemData) {
			Transform go = CreateItemPrefab(shopItemPrefab,i);
			UIShopItem panel = go.GetComponent<UIShopItem>();
			panel.InitialiseWithAssetData(type);
			uiShopItems.Add (panel);
			i++;
		}
		ResizeGridLayout(i);
	}

	private void ClearAllItems() {
		if(uiShopItems!=null) {
		foreach(UIShopItem item in uiShopItems) {
			Pool.Destroy(item.gameObject);
		}
		uiShopItems.Clear();
		}

		if(uiCategoryItems!=null) {
		foreach(UIBaseItem item in uiCategoryItems) {
			Pool.Destroy(item.gameObject);
		}
		uiCategoryItems.Clear();
		}
	}
	
	override public void BackButtonClicked() {
		SwitchToCategoryTab();
	}
}
