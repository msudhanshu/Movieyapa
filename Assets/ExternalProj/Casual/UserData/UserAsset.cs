using UnityEngine;
using System.Collections;
using SimpleSQL;

public class UserAsset : IUserAsset {

	public long id {get; set;}
	public int assetStateId {get; set;}
	public int xpos {get; set;}
	public int ypos {get; set;}
	public long stateStartTime {get; set;}
	public long activityStartTime {get; set;}
	public int level {get; set;}

	private Asset _asset;
	public Asset asset {
		get {
			if(_asset==null) {
				assetState = DatabaseManager.GetAssetState(assetStateId);
				_asset = DatabaseManager.GetAsset(assetState.assetId);
			}
            return _asset;
        }
        set {
			_asset = value;
        }
	}   

	private AssetState _assetState;
	public AssetState assetState {
		get {
			if (_assetState == null)
				_assetState = DatabaseManager.GetAssetState(assetStateId);
			return _assetState;
		}
		set {
			_assetState = value;
			assetStateId = value.id;

		}
	}


	[Ignore]
	public virtual string assetId {
		get {
			return asset.id;
		}
		set {
        }
    }

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
    
	//Mapping From UserAsset to BuildingData
	/*public BuildingData GetBuildingData() {
		BuildingData data = new BuildingData();
		data.uid = id.ToString();
		data.assetState = DatabaseManager.GetInstance().GetDbHelper().QueryObjectById<AssetState>(assetStateId);
		data.buildingTypeString = data.assetState.assetId;
		data.state = data.assetState.GetBuildingState();
		data.positionStr = GridPosition.ToString(xpos, ypos);
		data.startTime = Utility.FromUnixTime(stateStartTime);
		//TODO: Parse it from server response
		data.heightStr = "5.35";
		data.storedResources = 0;
		return data;
	}*/


	/**
	 * Time the building started building
	 */
	public virtual long startTime {
		get {return stateStartTime;}
		set {stateStartTime = value;}
	}

	private bool InTransition() {
		if(stateStartTime == 0 || activityStartTime <= 0)
			return false;
		
		if(stateStartTime <= activityStartTime)
			return true;
		
		return false;
	}

	public bool ReadyForNextActivity() {
		return !InTransition() ;//&& !assetState.HasAutoActivity();
		
	}

	virtual public string GetViewPrefabName() {
		if (assetState == null || !asset.id.Contains("Windmill")) 
			return asset.basePrefabName;
		return asset.basePrefabName + "_" + assetState.state;
	}

	/**
	 * util functions to access StateProperies
	 */
	public string GetStatePropertyValue(string name) {
		AssetStateProperty stateProperty = AssetStateProperty.GetStateProperty(assetState.id,level,name);
		if(stateProperty!=null)
			return stateProperty.value;
		return null;
	}
	
	public string GetStatePropertyValueForLevel(string key, int level, string defaultValue=null) {
		AssetStateProperty stateProperty = AssetStateProperty.GetStateProperty(assetState.id,level,key);
		if(stateProperty!=null)
			return stateProperty.value;
		return defaultValue;
	}
	
	public float GetStateFloatProperty(string key, float defaultValue) {
		return assetState.GetFloatProperty(key, defaultValue, level);
	}
	
	public long GetStateLongProperty(string key, long defaultValue) {
		return assetState.GetLongProperty(key, defaultValue, level);
	}
	
	public string GetStateProperty(string key) {
		return assetState.GetProperty(key, level);
	}
	
	public int GetStateIntProperty(string key, int defaultValue) {
        return assetState.GetIntProperty(key, defaultValue, level);
    }


	//TODO : IMPLEMENT USERASSETPROPERTIES ???? IS IT DAO? 

	public void setProperty(string propertyName, string propertyValue) {
	}
	public void saveProperties() {
    }

}
