using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityExtensions;
using UnityEngine.EventSystems;

public class PlaceableGridObject : MonoBehaviour, IPointerClickHandler, ICallOutClickListner {
	protected bool isTransitioning = false;
	/**
	 * The grid to snap to.
	 */ 
	protected AbstractGrid grid;

	/**
	 * Cached transform
	 */
	protected Transform target;

	/**
	 * World position of the grid object.
	 */ 
	protected Vector3 myPosition;

	private BoxCollider _boxCollider;

	protected BoxCollider boxCollider {
			get {
			if (_boxCollider == null) {
				_boxCollider = gameObject.GetComponent<BoxCollider>();
				if(_boxCollider == null)
				   _boxCollider = gameObject.AddComponent<BoxCollider> ();
			}
			return _boxCollider;
			}
		}

	private string lastViewPrefabName = "";
	/**
	 * Building this view references.
	 */ 
	[HideInInspector]
	public UserAssetController assetController;

	protected List<GameObject> components;
	protected GameObject activeComponent;
	protected bool isLevelUpgradable = false;
	protected int currentLevel = -1;
	protected GameObject objectRootView;
	protected ParticleSystem particles;

	private Color WHITE = new Color(1, 1, 1, 1);

	/**
	 * Internal initialisation.
	 */
	public virtual void Awake(){
		target = transform;	
		myPosition = target.position;
		//grid = GameObject.FindObjectOfType(typeof(AbstractGrid)) as AbstractGrid;
		grid = BuildingModeGrid.GetInstance();
	}

	/**
	 * Update objects position
	 */ 
	/** -- Method originally in UIDraggableGridObject. But Bulding3DView overrides it with new SetPosition method
	virtual public void SetPosition(GridPosition pos) {
		Vector3 position = grid.GridPositionToWorldPosition(pos);
		target.localPosition = position;
		myPosition = target.localPosition;
	}
	**/

	
	UI3DCallOut activityStatus;
	public virtual UI3DCallOut GetActivityStatus() {
		if (activityStatus == null) {
			int CALLOUT_HIGHT_OFFSET = 2;
			Vector3 callOutPosition = boxCollider.center + new Vector3( 0,assetController.asset.sizeHeight/2.0f + CALLOUT_HIGHT_OFFSET,0);
			activityStatus = UI3DCallOut.GetUI3DActivityStatus(transform,callOutPosition,this);
		}
		return activityStatus;
	}

	#region ICallOutClickListner implementation

	public virtual void OnCallOutClick()
	{

	}

	#endregion

	/**
	 * Initialise the building view.
	 */ 
	virtual public void UI_Init(UserAssetController controller) {
		this.assetController = controller;
		myPosition = transform.localPosition;
		InitializeView();
	}

	virtual public void InitializeView() {
		string prefabName = assetController.userAsset.GetViewPrefabName();

		if (prefabName == lastViewPrefabName)
			return; //No need to update view

		if (objectRootView != null) 
			GameObject.Destroy(objectRootView);

		ReconstructBoxCollider();

		GameObject userAssetView = Util.InstantiatePrefab(prefabName);
		if (userAssetView != null) {
			objectRootView = userAssetView;
			userAssetView.transform.parent = transform;
			userAssetView.transform.localPosition = PositionMeshAtCenter();
			components = userAssetView.GetComponentsInChildren<Renderer>().Select (o=>o.gameObject).OrderBy(g=>g.name).ToList();
			Renderer render = userAssetView.GetComponent<Renderer>();
			if(GetComponent<Renderer>() != null)
				components.Add(GetComponent<Renderer>().gameObject);
			lastViewPrefabName = prefabName;
		} else {
			Debug.LogWarning("Can't find prefab " + prefabName);
		}
	}

	/**
	 * Snap object to grid. 
	 */
	virtual protected GridPosition SnapToGrid() {
		//myPosition.y = target.localPosition.y;
		GridPosition pos = grid.ArenaPositionToGridPosition(myPosition);
		Vector3 position = grid.GridPositionToArenaPosition(pos);
		//position.y = target.localPosition.y;
		position.y = myPosition.y;
		myPosition = position;
		target.localPosition = position;
		return pos;
	}
	
	/**
	 * Snap object to grid. 
	 */
	virtual protected void SnapToGrid(out GridPosition gridPosition, out GridHeight gridHeight) {
		//myPosition.y = target.localPosition.y;
		gridPosition = grid.ArenaPositionToGridPosition(myPosition);
		Vector3 position = grid.GridPositionToArenaPosition(gridPosition);
		//position.y = target.localPosition.y;
		position.y = myPosition.y;
		gridHeight = new GridHeight (position.y);
		myPosition = position;
		target.localPosition = position;
		return;
	}

	/**
	 * Update objects position 
	 * [Method taken from BuildingView3D]
	 */ 
	virtual public void SetPosition(GridPosition pos) {
		Vector3 position = grid.GridPositionToArenaPosition(pos);
		//position.y = target.localPosition.y;
		target.localPosition = position;
		myPosition = target.localPosition;
	}

	/**
	 * Update objects position 
	 * [Method taken from BuildingView3D]
	 */ 
	virtual public void SetHeight(GridHeight height) {
		//position.y = target.localPosition.y;
		target.SetLocalPositionY(height.h);
		myPosition.y = height.h;
	}

	 virtual public void SetAutoHeightOnTerrain(GridPosition pos) {
		GridHeight height = BuildingModeGrid3D.GetInstance().GetTerrainHeightAtPosition(pos);
		target.SetLocalPositionY(height.h);
		myPosition.y = height.h;
	}

	/**
	 * reconstruct the boxcollider of building based on the grid size of building.
	 */
	virtual protected void ReconstructBoxCollider() {
		float gridWidth = BuildingModeGrid3D.GetInstance ().gridWidth;
		float gridHeight = BuildingModeGrid3D.GetInstance ().gridHeight;
		
		//todo : IT DEPEND ON THE SHAPE (COORDINATE BASED) MENTIONED IN THE XML: 
		//Currently assuming building's origin is left-bottom corner of the shape. (0,0;0,1;1,0;1,1 for 2x2 building)
		boxCollider.center = new Vector3(gridWidth*(assetController.asset.sizex-1), assetController.asset.sizeHeight,gridHeight*(assetController.asset.sizey-1)) / 2;
		boxCollider.size = new Vector3(gridWidth*assetController.asset.sizex,assetController.asset.sizeHeight,gridHeight*assetController.asset.sizey);
	}


	/**
	 * Can we drag this object.
	 */
	virtual protected bool CanDrag {
		get; set;
	}

	/*
	 * ALLIGN BUILDING MESH AT LEFT-BOTTOM CORNER OF THE ACCUPIED GRID/TILE.
	 * //TODO: Assuming buidling mesh is having local coordinate at left-bottom corner.
	 * move the building by half of tile width because the local coordinate system is at center of the tiles.
	 */
	//	virtual protected Vector3 PositionMeshAtCenter() {
//		return new Vector3 (-BuildingModeGrid3D.GetInstance ().gridWidth / 2.0f, 0, -BuildingModeGrid3D.GetInstance ().gridHeight / 2.0f);
//	}
	

	/*
	 * ALLIGN BUILDING MESH AT LEFT-BOTTOM CORNER OF THE ACCUPIED GRID/TILE.
	 * //TODO: Assuming buidling mesh is having local coordinate at center
	 * move the building by half of tile width because the local coordinate system is at center of the tiles.
	 */
	virtual protected Vector3 PositionMeshAtCenter() {
				return new Vector3 (boxCollider.center.x, 0, boxCollider.center.z);
		}

	void OnDrawGizmos() {
		if (boxCollider != null) {
			// Draw a yellow sphere at the transform's position
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube( transform.position + boxCollider.center , boxCollider.size);
		}
	}

	/**
	 * Set color after drag.
	 */ 
	protected void SetColor(GridPosition pos) {
		if (BuildingModeGrid.GetInstance().CanObjectBePlacedAtPosition(assetController, pos)) {
			SetColor (true);
		} else {
			SetColor (false);
		}
	}

	private void SetEffectToComponents(ModeEffect effect) {
		if(components!=null) {
			foreach(GameObject go in components){
				if(isLevelUpgradable){
					GameObject[] childComponents = go.GetComponentsInChildren<MeshRenderer>().Select (o=>o.gameObject).OrderBy(g=>g.name).ToArray();
					foreach(GameObject cgo in childComponents){
						SetModeEffect(cgo,effect); 
					}
				}else{
					SetModeEffect(go,effect); 
				}
			}
		}
		if(isLevelUpgradable && activeComponent != null){
			GameObject[] childComponents = activeComponent.GetComponentsInChildren<MeshRenderer>().Select (o=>o.gameObject).OrderBy(g=>g.name).ToArray();
			foreach(GameObject cgo in childComponents){
				SetModeEffect(cgo,effect); 
			}
		}
	}

	protected void SetNormal(){
		SetEffectToComponents(ModeEffect.NORMAL);	
	}
	
	public void SetEditModeColor() {
		SetColor(assetController.Position);
	}

	protected void SetColor(bool canPlace){
		if (canPlace) {
			SetEffectToComponents(ModeEffect.PLACABLE);
		} else {
			SetEffectToComponents(ModeEffect.NONPLACABLE);
		}
	}

	protected void SetModeEffect(List<GameObject> components, ModeEffect modeEffect) {
		foreach (GameObject go in components) {
			SetModeEffect(go,modeEffect);
		}
	}

	protected void SetModeEffect(GameObject go, ModeEffect modeEffect) {
		if (go.GetComponent<Renderer> () == null)
						return;
		switch (modeEffect) {
//		case ModeEffect.PLACING:	//Not used anywhere
//			//go.renderer.material.shader = Shader.Find ("Twisting");
//			this.ApplyShaderEffect(go, "Silhouette", new Color(1,1,1,1));
//			break;
		case ModeEffect.NORMAL:
			this.ApplyShaderEffect(go, "Mobile/Diffuse", new Color(1,1,1,1));
			break;
		case ModeEffect.UPGRADING://Cutaways

			this.ApplyShaderEffect(go, "Diffuse", new Color(0.0f,0.0f,1.0f,1));

			//go.renderer.material.shader = Shader.Find ("Cutaways");
			//go.GetComponent<Renderer>().material.SetFloat("_baseZpos",assetController.Height.h);

			break;
		case ModeEffect.PLACABLE:
			this.ApplyShaderEffect(go, "Diffuse", new Color(0.5f,1,0.5f,1));
			break;
		case ModeEffect.NONPLACABLE:
			this.ApplyShaderEffect(go, "Diffuse", new Color(1,0.5f,0.5f,1));
			break;
		case ModeEffect.NORMAL_COLOR:
			go.GetComponent<Renderer>().material.shader = Shader.Find ("Mobile/Diffuse");
			go.GetComponent<Renderer>().material.color = new Color(1,0.5f,0.5f,1); 
			break;

		}
	}

	private void ApplyShaderEffect(GameObject go, string shaderName, Color? color){
		foreach(Material material in go.GetComponent<Renderer>().materials){
			material.shader = Shader.Find(shaderName);
			if(color.HasValue){	//if not null
				material.color = color ?? new Color(1,1,1,1); 
			}
		}
	}


	/*
	 * Set color on active game object
	 **/
	protected void SetColor(GameObject go, Color color){
		if (isLevelUpgradable) {
			GameObject[] childComponents = go.GetComponentsInChildren<MeshRenderer>().Select (o=>o.gameObject).OrderBy(g=>g.name).ToArray();
			foreach(GameObject cgo in childComponents){
				SetModeEffect(cgo,ModeEffect.NORMAL);
				cgo.GetComponent<Renderer>().material.color = color; 
			}
		} else {
			SetModeEffect(go,ModeEffect.NORMAL);
			go.GetComponent<Renderer>().material.color = color; 
		}
	}

	virtual public void OnPointerClick (PointerEventData eventData){
//		if (GameManager.GetInstance().GetFpsCameraComponent().IS_FPSVIEW_ENABLED_NOW() || this.gameObject.layer == BuildingManager3D.SELECTION_LAYER) {
//			return;
//		}
	}
	
	public UserAssetController  GetBasePrimaryTile() {
		return this.assetController;
	}

	public float DistanceFrom(Building helperBasePrimaryTile) {
		try{
			GridPosition gridPosition = helperBasePrimaryTile.Position - this.GetBasePrimaryTile().Position;
			return gridPosition.Magnitude();
		}catch(NullReferenceException e){
			return -1;
		}
	}



	//TODO : MOVE IT TO BUILDING OR TRANSITION CONTROLLER???? HOW USER - COLLECTABLES IS UPDATED AT SERVER SIDE?

        
	protected Dictionary<IGameResource, int> DoDoobersWork(AssetState state, int level) {
		if(state == null)
			return null;
        
		Dictionary<IGameResource, int> allRewards = AssetState.GetAllStateRewards(state,level);
		if(allRewards == null)
			return null ;

        List<Doober> doobers = new List<Doober>();

		foreach(KeyValuePair<IGameResource, int> entry in allRewards) {
			IGameResource rewardItem = entry.Key;
			int rewardQuantity = entry.Value;
			if(rewardQuantity <= 0)
				continue ;
			doobers.Add(Doober.GetDoober(rewardItem,rewardQuantity));
		}
		this.PopOutDoobers(doobers) ;
		
		return allRewards;
	}

	public void PopOutDoobers(List<Doober> doobers) {

		foreach (Doober doober in doobers) {

			float dooberPositionX  = Config.DOOBER_DROP_MIN_SPACING * (
				- (float)doobers.Count/2.0f + (float)doobers.IndexOf(doober) + 0.5f
				);
			doober.SetPosition(transform.position +new Vector3( 0,assetController.asset.sizeHeight,0),
				transform.position +new Vector3( dooberPositionX,0,0));
		}
	}

}

public enum ModeEffect {
//	PLACING,		// Not used anywhere, use dragstate.moving instead
	NORMAL,
	PLACABLE,
	NONPLACABLE,
	UPGRADING,
	READY,
	BUILT,
	NORMAL_COLOR
};

