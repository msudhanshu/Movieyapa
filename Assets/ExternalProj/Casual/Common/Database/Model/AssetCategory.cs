using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;
using System.Collections.Generic;

[System.Serializable]
public class AssetCategory : BaseDbModel,IUIBaseItemInfo {

	[PrimaryKey]
	public string id {get; set;}

	public string name {get; set;}

	public string description {get; set;}

	public string resourceId {get; set;}
	[Ignore]
	private DbResource _resource;
	[Ignore]
	public DbResource resource {
		get {
			if (_resource == null)
				_resource = DatabaseManager.GetDbResource(resourceId);
			return _resource;
		}
		set {
			resourceId = value.id;
		}
	}
	
	public string spedResourceId {get; set;}	
	[Ignore]
	private DbResource _spedResource;
	[Ignore]
	public DbResource spedResource {
		get {
			if (_spedResource == null)
				_spedResource = DatabaseManager.GetDbResource(spedResourceId);
			return _spedResource;
		}
		set {
			spedResourceId = value.id;
		}
	}

	public int displayOrder {get; set;}

	public bool IsHiddenInMarket() {
		if(displayOrder == -1) return true;
		else return false;
	}

	public static List<AssetCategory> GetAssetCategories() {
		KDbQuery<AssetCategory> dbquery = new KDbQuery<AssetCategory>(
			new BaseDbOp[]{});
		return DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetCategory>(dbquery);
	}

	public static AssetCategory GetAssetCategory(string assetCategory_id) {
		return DatabaseManager.GetInstance ().GetDbHelper ().QueryObjectById<AssetCategory> (assetCategory_id);
	}

	public AssetCategoryEnum type {
		get {
			if (id == "houses")
				return AssetCategoryEnum.BUILDING;
			else if (id == "boundhelpers")
				return AssetCategoryEnum.HELPER;
			else if (id == "rpgcharacters")
				return AssetCategoryEnum.RPGCHARACTER;
			else if (id == "decorations")
				return AssetCategoryEnum.DECORATION;
			else if (id == "crops")
				return AssetCategoryEnum.CROP;
			else if (id == "trees")
				return AssetCategoryEnum.TREE;
			else if (id == "townbldgs")
				return AssetCategoryEnum.TOWNBLDG;
			else if (id == "expansion")
				return AssetCategoryEnum.EXPANSION;
			else
				return AssetCategoryEnum.NONE;
		} 
		set {
		}
	}

	public string PrefabName() {
		string name= "AssetCategory/";
		if(Config.CategoryPrefabNameMap.ContainsKey(type)) {
			name+=Config.CategoryPrefabNameMap[type];
		} else {
			name+= Config.DefaultAssetCategoryPrefabName;
		}
		return name;
	}

	#region IUIBaseItemInfo Implementation
	public string GetId() {
		return id;
	}
	public string GetName() {
		return name;
	}
	public string GetDescription() {
		return name;
	}
	public string GetItemImageName() {
		return "Market/shop-category/shop-"+id;
	}

	public Dictionary<IGameResource,int> GetItemCosts() {
		return null;
	}
	#endregion
}


public enum AssetCategoryEnum  {
	HELPER,
	RPGCHARACTER,
	BUILDING,
	CROP,
	DECORATION,
	TREE,
	TOWNBLDG,
	EXPANSION,
	NONE
}
