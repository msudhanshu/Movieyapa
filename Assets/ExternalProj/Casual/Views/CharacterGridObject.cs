using UnityEngine;
using System.Collections;
using Pathfinding;
using UnityEngine.EventSystems;
/**
 * A Character view implementation
 */ 

public class CharacterGridObject : PlaceableGridObject
{
	private CharacterAIPath _aiPath;
	private IHelperReachedListener helperReachedListener;
	[HideInInspector]
	public CharacterAIPath characterAIPath {
		get {
			if (_aiPath == null) {
				_aiPath =  this.GetComponentInChildren<CharacterAIPath> ();
				if (_aiPath != null)
					_aiPath.OnTargetReachedCallback += OnReachedTo;
			}
			return _aiPath;
		}
	}

	[HideInInspector]
	public IActivityTarget lastTargetActor;

	//if it is upgrading or cooling down...etc .. or free to be assigned for some activity.
	private bool _isFree = true;
	public virtual bool isFree {
		get{
			return _isFree;
		}
		set {
			_isFree = value;
		}
	}

	public override void Awake() {
		base.Awake();
		//CharacterManager.GetInstance().addCharacterToList(this);
	}

	public override void OnPointerClick (PointerEventData eventData){
		Debug.Log("Clicked on Character");
	}

	/**
	 * reconstruct the boxcollider of character based on the grid size of character.
	 * box collider will be created arround the character, without careing about tiles allignment
	 */
	protected override void ReconstructBoxCollider() {
		BoxCollider boxCollider = GetComponent<BoxCollider> ();
		if (boxCollider == null) {
			boxCollider = gameObject.AddComponent<BoxCollider> ();
		}
		float gridWidth = BuildingModeGrid3D.GetInstance ().gridWidth;
		float gridHeight = BuildingModeGrid3D.GetInstance ().gridHeight;
		
		//todo : IT DEPEND ON THE SHAPE (COORDINATE BASED) MENTIONED IN THE XML: 
		//box collider will be created arround the character, without careing about tiles allignment 
		boxCollider.center = new Vector3(0, assetController.asset.sizeHeight/2.0f, 0 );// new Vector3(gridWidth*(assetController.Type.sizex-1),assetController.Type.sizeHeight,gridHeight*(assetController.Type.sizey-1)) / 2;
		boxCollider.size = new Vector3(gridWidth*assetController.asset.sizex,assetController.asset.sizeHeight,gridHeight*assetController.asset.sizey);
	}

	/*
	 * ALLIGN CHARACTER MESH AT  CENTER OF THE ACCUPIED GRID/TILE.
	 * //TODO: Assuming buidling mesh is having local coordinate at left-bottom corner.
	 * move the building by half of tile width because the local coordinate system is at center of the tiles.
	 */
	protected override Vector3 PositionMeshAtCenter() {
		return Vector3.zero;//new Vector3 (-BuildingModeGrid3D.GetInstance ().gridWidth / 2.0f, 0, -BuildingModeGrid3D.GetInstance ().gridHeight / 2.0f);
	}

	/**
	 * Store of auto generated rewards is full.
	 */ 
	public void UI_StoreFull() {
		
	}
	
	/**
	 * Activity acknowledged.
	 */ 
	public void UI_AcknowledgeActivity() {
		
	}

	/**
	 * Resets view if moving/placing
	 */
	public void ResetView(){

	}

	public void SetView(){

	}

	public void UI_UpdateDragState(DragState state){

	}

	public void TeleportTo(IActivityTarget actor,IHelperReachedListener _helperReachedListener=null) {
		//transform.position = actor.targetWorldPosition;
		/*transform.position = BuildingModeGrid3D.GetInstance().GridPositionToWorldPosition(
			BuildingModeGrid3D.GetInstance().ScanGridInSpiral(actor.targetGridPosition, assetController.Shape.ToArray() )
			);
		*/
		this.helperReachedListener = _helperReachedListener;

		Vector3 finalPos = BuildingModeGrid3D.Get3DInstance().SnapPositionToTerrainHeight(
			BuildingModeGrid3D.GetInstance().ScanGridInSpiral(actor.targetGridPosition, assetController.Shape.ToArray() )
			);
		//characterAIPath.SetToIdle();
		//characterAIPath.transform.position = finalPos;
		characterAIPath.startTeleporting(finalPos);
	}
	
	public void MoveTo(IActivityTarget actor,IHelperReachedListener _helperReachedListener=null) {
		MoveTo (actor.targetWorldPosition, _helperReachedListener);//gridPosition
	}

	public void MoveTo(GridPosition point,IHelperReachedListener _helperReachedListener=null) {
		MoveTo (BuildingModeGrid3D.GetInstance().GridPositionToWorldPosition(point) , _helperReachedListener);
	}

	public void MoveTo(Vector3 point,IHelperReachedListener _helperReachedListener=null) {
		this.helperReachedListener = _helperReachedListener;
		if(characterAIPath!=null)
			characterAIPath.startMoving(point);
	}

	//it takes the callback from AIPath(animating AI) once reached to target
	public void OnReachedTo() {
		characterAIPath.SetToIdle();
		if(helperReachedListener!=null) {
			helperReachedListener.OnHelperReached();
			helperReachedListener = null;
		}
		//if(lastTargetActor != null) 
		//	lastTargetActor.activityController.OnHelperReached();
	}

	public void SetToIdle() {
		characterAIPath.SetToIdle();
	}
	/*public void SetState(CharacterActivityState state) {
		this.currentAnimState = state;
		switch (state) {
			case CharacterActivityState.IDLE: 
				characterAIPath.SetToIdle();
			break; 
		   case CharacterActivityState.REACHED:
				characterAIPath.SetToIdle();
			break;
		}
	}*/


	//it will take activiy as input(acitivity which is mapped with each state/transition of building
	public void StartActivity(Activity activity) {
		characterAIPath.SetToActivityAnim (activity);
	}

	//FIXME : MANJEET-SUMANTH
	//CURRENT Implementation doesn't move CharacterGridObject when helpers moves.... So 
	public GridPosition OccupiedGridTile() {
		//TEMP
		var helper = GetComponentInChildren<CharacterController> ();
		if(helper!=null) 
			return BuildingModeGrid3D.GetInstance().WorldPositionToGridPosition( helper.transform.position) ;

		return assetController.Position;
	}

	//FIXME : MANJEET-SUMANTH
	//CURRENT Implementation doesn't move CharacterGridObject when helpers moves.... So 
	public bool IsGridPositionOccupied(GridPosition position) {
		//TEMP
		GridPosition occupiedGridPosition = OccupiedGridTile();
		if(occupiedGridPosition.x == position.x && occupiedGridPosition.y==position.y) return true;

		if (assetController == null)
						return false;
		return (assetController.Position.x==position.x && assetController.Position.y==position.y );
	}
}

