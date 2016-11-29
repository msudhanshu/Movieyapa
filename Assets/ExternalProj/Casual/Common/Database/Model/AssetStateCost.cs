using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

[System.Serializable]
public class AssetStateCost : BaseDbModel, IResourceUpdate {

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

	public int level  {get; set;}

	public int version {get; set;}

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
