using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;
using System.Collections.Generic;

[System.Serializable]
public class AssetProperty : BaseDbModel {

	[PrimaryKey]
	public int id {get; set;}

	public string assetId {get; set;}
	private Asset _asset;
	[Ignore]
	private Asset asset {
		get {
			if (_asset == null)
				_asset = DatabaseManager.GetAsset(assetId);
			return _asset;
		}
		set {
			assetId = value.id;
			_asset = null;
		}
	}

	public string name {get; set;}

	public string value {get; set;}

	public static AssetProperty GetProperty(string assetId, string name) {
		KDbQuery<AssetProperty> dbquery = new KDbQuery<AssetProperty>(
			new BaseDbOp[]{new DbOpEq("assetId", assetId), new DbOpEq("name", name) });
		return DatabaseManager.GetInstance().GetDbHelper().QueryForFirst<AssetProperty>(dbquery);
	}

	public static List<AssetProperty> GetProperties(string assetId) {
		KDbQuery<AssetProperty> dbquery = new KDbQuery<AssetProperty>(
			new BaseDbOp[]{new DbOpEq("assetId", assetId) });
		return DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetProperty>(dbquery);
	}
}
