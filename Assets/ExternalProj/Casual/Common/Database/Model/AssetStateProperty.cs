using UnityEngine;
using System.Collections;
using KiwiCommonDatabase ;
using SimpleSQL;
using System.Collections.Generic;

[System.Serializable]
public class AssetStateProperty : BaseDbModel {

	[PrimaryKey]
	public int id {get; set;}

	public int assetStateId {get; set;}
	[Ignore]
	private AssetState _assetState;
	[Ignore]
	public AssetState assetState {
		get {
			if (_assetState == null)
				_assetState = DatabaseManager.GetAssetState(assetStateId);
			return _assetState;
		}
		set {
			assetStateId = value.id;
		}
	}

	public int level {get; set;}

	public string name  {get; set;}

	public string value {get; set;}

	public static AssetStateProperty GetStateProperty(int assetStateId, int level, string name) {
		KDbQuery<AssetStateProperty> dbquery = new KDbQuery<AssetStateProperty>(
			new BaseDbOp[]{new DbOpEq("assetStateId", assetStateId), new DbOpEq("name", name), new DbOpEq("level", level)});
		return DatabaseManager.GetInstance().GetDbHelper().QueryForFirst<AssetStateProperty>(dbquery);
	}

	public static List<AssetStateProperty> GetStateProperties(int assetStateId, int level) {
		KDbQuery<AssetStateProperty> dbquery = new KDbQuery<AssetStateProperty>(
			new BaseDbOp[]{new DbOpEq("assetStateId", assetStateId), new DbOpEq("level", level)});
		return DatabaseManager.GetInstance().GetDbHelper().QueryForAll<AssetStateProperty>(dbquery);
	}
}
