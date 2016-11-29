//
//  UserAnimalHelper.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using System.Collections.Generic;
using SimpleSQL;

[System.Serializable]
public class UserAnimalHelper : IUserAsset{

	private const int FREE_HELPER_USER_ASSET_ID = -1;
	
	public virtual long id {get; set;}
	
	public virtual string helperId {get; set;}
	
	public virtual int xpos {get; set;}
	
	public virtual int ypos {get; set;}
	
	public virtual long userAssetId {get; set;}

	//[Default(1)]
	public virtual int activeOutfitId {get; set;}

	//[Default("1")]
	public virtual string purchasedOutfits {get; set;}

	//Animal helpers are not upgradable
	public virtual int level { get { return 1;} set {}}

	public virtual long stateStartTime {get; set;}
	public virtual long activityStartTime {get; set;}

	[Ignore]
	public virtual string assetId {
		get {
			return helperId;
		}
		set {
			helperId = value;
		}
	}

	private Asset _asset=null;
	public Asset asset {
		get {
			if(_asset==null)
				_asset = DatabaseManager.GetAsset(helperId);
			return _asset;
		}
		set {
			_asset = value;
		}
	}

	public AssetState assetState { get; set;}

	private GridPosition? _position=null;
	[Ignore]
	public virtual GridPosition position { 
		get {
			if (!_position.HasValue) {
				_position = new GridPosition(xpos,ypos);
			}
			return _position??new GridPosition(0,0);
		}
		set { 
			_position = value;
		}
	}


	//TODO : PUT THIS IS USERHELPER TABLE , OR DON'T USE THIS HEIGHT BUT PLACE THE CHARACTER BY CALCULATING TERRAIN HEIGHT.
	/**
	 * Current position of the building
	 */
	private GridHeight? _height=null;
	[Ignore]
	public virtual GridHeight height {
		get {
			if (!_height.HasValue) {
				_height = new GridHeight(0);
			}
			return _height??new GridHeight(0);
		}
		set { 
			_height = value;
		}
	}

	virtual public string GetViewPrefabName() {
		return asset.basePrefabName;
	}
}
