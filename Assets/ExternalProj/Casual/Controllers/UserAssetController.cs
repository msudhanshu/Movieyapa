//
//  AssetController.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UserAssetController : CoreBehaviour, IGridObject
{
	/**
	 * Wrapper holding the data for this building that is persisted.
	 */ 
	public IUserAsset userAsset;
	
	/**
	 * View for this building.
	 */
	protected GameObject view;
	/*	

	//Unique identifier for the buidling.
	virtual public string uid {
		get { return userAsset.id.ToString(); }
		protected set { userAsset.id =  value; }
	}
	*/
	
	
	/**
	 * Unique identifier for the buidling.
	 */ 
	virtual public long userAssetId {
		get { return userAsset.id; }
		set { userAsset.id = value; }
	}
	
	protected Asset _asset;
	
	/**
	 * The data defining the type of this building.
	 */ 
	virtual public Asset asset {
		get { return _asset; }
		protected set {
			_asset = value;
			userAsset.asset = value;
			userAsset.assetId = value.id;
		}
	}
	
	/**

	/**
	 * Shape of the building in terms of the grid positions it fills. 
	 */
	virtual public List<GridPosition> Shape {
		get {
			return asset.shape;
		}
	}
	
	
	/**
	 * Current position of the building
	 */
	virtual public GridPosition Position {
		get {
			return userAsset.position;
		}
		set {
			userAsset.position = value;
			MovePosition = value;
		}
	}
	
	/**
	 * Current position of the building
	 */
	virtual public GridHeight Height {
		get {
			return userAsset.height;
		}
		set {
			userAsset.height = value;
			MoveHeight = value;
		}
	}
	
	/**
	 * Position the building may be moved to.
	 */
	virtual public GridPosition MovePosition {
		get; set;
	}
	
	/**
	 * Position the building may be moved to.
	 */
	virtual public GridHeight MoveHeight {
		get; set;
	}

	protected abstract void CreateEmptyUserAsset();

	virtual public void Init(Asset asset, GridPosition pos){
		//userAsset = new IUserAsset();
		CreateEmptyUserAsset();
		userAsset.level = 1;
		Position = pos;
		InitView (asset);
	}

	/**
	 * Initialise the building with the given type and position.
	 */ 
	virtual public void Init(Asset asset, GridPosition pos, GridHeight height){
		//userAsset = new IUserAsset();
		CreateEmptyUserAsset();
		userAsset.level = 1;
		Position = pos;
		Height = height;
		InitView (asset);
	}
	
	protected virtual void InitView(Asset asset) {
		//uid = System.Guid.NewGuid ().ToString ();
		userAssetId = DataHandler.wrapper.nextUserAssetId;
		this.asset = asset;
		
		view = gameObject;
		view.SendMessage ("UI_Init", this);
		view.SendMessage("SetPosition", userAsset.position);
		view.SendMessage("SetHeight", userAsset.height);
		//not for character
	//	view.SendMessage("SetEditModeColor");
	}
	
	/**
	 * Initialise the building with the given data
	 */ 
	virtual public void Init(Asset asset, IUserAsset data){
		StartCoroutine(DoInit (asset, data));
	}
	
	/**
	 * Used on obstacles to start the clearing activity.
	 */ 
	virtual public void StartClear() {
		//StartCoroutine (GenericTransition (StateTransitionType.CLEAR, System.DateTime.Now, ""));	
	}
	
	/**
	 * Create a building from userAsset. Uses a coroutine to ensure view can be synced with userAsset.
	 */ 
	protected virtual IEnumerator DoInit(Asset asset, IUserAsset userAsset) {
		this.userAsset = userAsset;
		this.asset = asset;
		this.Position = userAsset.position;
		this.Height = userAsset.height;
		
		// Update view
		view = gameObject;
		view.SendMessage ("UI_Init", this);
        view.SendMessage ("SetPosition", this.userAsset.position);
        view.SendMessage("SetHeight", userAsset.height);
		PostInit();
        // Wait one frame to ensure everything is initialised
        yield return true;
    }
    
	protected virtual void PostInit(){}

    /**
	 * Sets the view position back to the buildings position
	 */ 
    virtual public void ResetPosition() {
        view.SendMessage ("SetPosition", Position);
        view.SendMessage("SetHeight", Height);
    }
    
    virtual public void ResetView(){
        view.SendMessage("Reset");
    }
    
    virtual public void SetView(){
        view.SendMessage("Set");
    }
}

