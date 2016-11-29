using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

[System.Serializable]
public class AssetStateCollectable : BaseDbModel, IResourceUpdate {

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

	public int level  {get; set;}

	public string collectableId {get; set;}	
	[Ignore]
	private Collectable _collectable;
	[Ignore]
	public Collectable collectable {
		get {
			if (_collectable == null)
				_collectable = DatabaseManager.GetCollectable(collectableId);
			return _collectable;
		}
		set {
			collectableId = value.id;
		}
	}
	
	public int quantity {get; set;}


	#region IResourceUpdate implementation

	public IGameResource GetResource ()
	{
		return collectable;
	}

	public int GetQuantity ()
	{
		return quantity;
	}

	#endregion
}
