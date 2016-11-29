using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

[System.Serializable]
public class AssetStateRewardCollectable : BaseDbModel, IResourceUpdate {

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
	//TODO: verify and change to collectableId in schema and then change here
	public string collectable {get; set;}	

	[Ignore]
	private Collectable _collectable;
	[Ignore]
	public Collectable collectble {
		get {
			if (_collectable == null)
				_collectable = DatabaseManager.GetCollectable(collectable);
			return _collectable;
		}
		set {
			collectable = value.id;
		}
	}

	public int quantity {get; set;}

	public float probability {get; set;}

	#region IResourceUpdate implementation

	public IGameResource GetResource ()
	{
		return collectble;
	}

	public int GetQuantity ()
	{
		return quantity;
	}
	#endregion

}
