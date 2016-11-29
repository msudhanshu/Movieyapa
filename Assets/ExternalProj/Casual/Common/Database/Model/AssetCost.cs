using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

[System.Serializable]
public class AssetCost : BaseDbModel, IResourceUpdate{

	[PrimaryKey]
	public int id {get; set;}

	public string assetId {get; set;}
	private Asset _asset;
	[Ignore]
	public Asset asset {
		get {
			if (_asset == null)
				_asset = DatabaseManager.GetAsset(assetId);
			return _asset;
		}
		set {
			assetId = value.id;
		}
	}

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

	public int quantity {get; set;}

	#region IResourceUpdate implementation
	
	public IGameResource GetResource ()
	{
		return resource;
	}
	
	public int GetQuantity ()
	{
		return quantity;
	}
	
	#endregion
}
