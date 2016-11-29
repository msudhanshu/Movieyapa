using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityExtensions;

public class RectDragableBlock : CoreBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
	bool dragOn = false;
	BaseBlock _gridObject;
	BaseBlock gridObject {
		get {
			if(_gridObject == null) {
				_gridObject = GetComponent<BaseBlock>();
			}
			return _gridObject;
		}
	}

	private Vector3 oldSnappedPos;
	private GameObject dummyXrayBlock = null;

	virtual public void OnBeginDrag(PointerEventData eventData){
		dragOn = true;
		//transform.position = GameManager.GetInstance().camera.ScreenToWorldPoint(eventData.position
		oldSnappedPos = transform.position;
	/*	if(dummyXrayBlock == null) {
			dummyXrayBlock = MazeManager.GetInstance().mazeModelBuilder.GetHintDummyBlock(); //GameObject.CreatePrimitive(PrimitiveType.Cube);
			dummyXrayBlock.transform.localScale = new Vector3(MazeManager.GetInstance().maze3DGrid.gridWidth,
			                                                  gridObject.Height.h,
			                                                  MazeManager.GetInstance().maze3DGrid.gridHeight);
		}
		dummyXrayBlock.SetActive(true);
		dummyXrayBlock.transform.position = transform.position;
	*/
		
		//dummyXrayBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
		dummyXrayBlock = MazeManager.GetInstance().mazeModelBuilder.GetHintDummyBlock(0,0); 
//		dummyXrayBlock.transform.localScale = new Vector3(MazeManager.GetInstance().maze3DGrid.gridWidth,
//		                                                  gridObject.Height.h,
//		                                                  MazeManager.GetInstance().maze3DGrid.gridHeight);
//
		Debug.Log("begin drag "+name+",pos"+eventData.worldPosition);
	}

	float SlopeXoverZThreshold = 1.01f;
	float DispXorZThreshold = 0.01f;

	private bool IsMovingInX(Vector3 oldPos , Vector3 newPos) {
		float dispX = Mathf.Abs(newPos.x - oldPos.x);
		float dispZ = Mathf.Abs(newPos.z - oldPos.z);
		if(dispX > DispXorZThreshold && dispZ > DispXorZThreshold && Mathf.Abs(dispX/dispZ) > SlopeXoverZThreshold)
				return true;
		return false;
	}

	private bool IsMovingInZ(Vector3 oldPos , Vector3 newPos) {
		float dispX = Mathf.Abs(newPos.x - oldPos.x);
		float dispZ = Mathf.Abs(newPos.z - oldPos.z);
		if(dispX > DispXorZThreshold && dispZ > DispXorZThreshold && Mathf.Abs(dispZ/dispX) > SlopeXoverZThreshold)
			return true;
		return false;
	}

	virtual public void OnDrag(PointerEventData eventData) {
		Vector3 pos = GetWorldPosition(eventData);
		Vector3 snappedPos = SnapToWorldGridPosition(pos);
		//SetToSnappedPosition(snappedPos);

		Vector3 constrainedPos = pos;

		if(IsMovingInX(oldSnappedPos,constrainedPos)) {
			if(CanBePlacedInNeighbour(oldSnappedPos, (int) Mathf.Sign(constrainedPos.x - oldSnappedPos.x) , 0 ) ) {
			constrainedPos.z = oldSnappedPos.z;
				transform.position = constrainedPos;
			}
		} else if(IsMovingInZ(oldSnappedPos,pos)) {
			if(CanBePlacedInNeighbour(oldSnappedPos, 0, (int)Mathf.Sign(constrainedPos.z - oldSnappedPos.z) ) ) {
			constrainedPos.x = oldSnappedPos.x;
			transform.position = constrainedPos;
			}
		}

		//can not be placed and not himself
		//&& oldSnappedPos.x == snappedPos.x && oldSnappedPos.z == snappedPos.z
		if(CanBePlaced(snappedPos) ) {
			/*transform.position = pos;// GameManager.GetInstance().camera.ScreenToWorldPoint(eventData.position;
			if(oldSnappedPos.x == snappedPos.x) 
				transform.SetPositionX(oldSnappedPos.x);
			else if(oldSnappedPos.z == snappedPos.z) 
				transform.SetPositionZ(oldSnappedPos.z);
				*/
			dummyXrayBlock.transform.position = snappedPos;
			//TODO : TEMP
			dummyXrayBlock.SetActive(true);
			MazeManager.GetInstance().maze3DGrid.MoveGridBlockToGridPosition(
				gridObject,MazeManager.GetInstance().maze3DGrid.WorldPositionToGridPosition(snappedPos));
			MazeManager.GetInstance().gamePlayer.BlockMoved();

			oldSnappedPos = snappedPos;
		}else
			dummyXrayBlock.SetActive(false);
    
//		if(CanBePlaced(snappedPos)) {
//			dummyXrayBlock.SetActive(true);
//
//		}else 
		//	dummyXrayBlock.SetActive(false);

//		Debug.Log(" drag "+name+",pos"+eventData.worldPosition);
	}

	virtual public void OnEndDrag(PointerEventData eventData){
		if(!dragOn) return;
		dragOn = false;
		//transform.position = GameManager.GetInstance().camera.ScreenToWorldPoint(eventData.position
		Vector3 snappedPos = SnapToWorldGridPosition(eventData);
		//SetToSnappedPosition(snappedPos);
		if(CanBePlaced(snappedPos)) {
			dummyXrayBlock.transform.position = snappedPos;
			//TODO : TEMP
			dummyXrayBlock.SetActive(true);
			MazeManager.GetInstance().maze3DGrid.MoveGridBlockToGridPosition(
			gridObject,MazeManager.GetInstance().maze3DGrid.WorldPositionToGridPosition(snappedPos));
			MazeManager.GetInstance().gamePlayer.BlockMoved();
		//	transform.position = snappedPos;
		}else
			transform.position = oldSnappedPos;
//		dummyXrayBlock.SetActive(false);
		Debug.Log("end drag "+name+",pos"+eventData.worldPosition);
	}

//	private void SetToSnappedPosition(Vector3 snappedPos) {
//		if(CanBePlaced(snappedPos)) {
//			dummyXrayBlock.transform.position = snappedPos;
//			//TODO : TEMP
//			dummyXrayBlock.SetActive(true);
//			MazeManager.GetInstance().maze3DGrid.MoveObjectToGridPosition(
//				gridObject,MazeManager.GetInstance().maze3DGrid.WorldPositionToGridPosition(snappedPos));
//		}else
//			dummyXrayBlock.SetActive(false);
//	}

	protected Vector3 GetWorldPosition(PointerEventData eventData) {
		//eventData.pointerCurrentRaycast.module.
		Vector3 worldPosOnTerrain = transform.position;
		Ray ray = GameManager.GetInstance().gameCamera.ScreenPointToRay(eventData.position);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 1 << GameManager.TERRAIN_LAYER)) {
			worldPosOnTerrain = hit.point;
			worldPosOnTerrain.y = transform.position.y;
		}
		return worldPosOnTerrain;
	}
	protected Vector3 SnapToWorldGridPosition(PointerEventData eventData) {
		return MazeManager.GetInstance().maze3DGrid.SnapWorldPositionToGrid(GetWorldPosition(eventData));
		//return Maz  . SnapWorldPositionToGrid(SnapToWorldPosition(eventData));
	}
	protected Vector3 SnapToWorldGridPosition(Vector3 pos) {
		return MazeManager.GetInstance().maze3DGrid.SnapWorldPositionToGrid(pos);
		//return Maz  . SnapWorldPositionToGrid(SnapToWorldPosition(eventData));
	}


	//CONSTRAINTS
	private bool CanBePlacedInNeighbour(Vector3 currentSnappedPos , int x, int z) {
		GridPosition destGridPos = MazeManager.GetInstance().maze3DGrid.WorldPositionToGridPosition(currentSnappedPos);
		destGridPos += new GridPosition(x,z);
		return CanBePlaced(destGridPos);
	}

	private bool CanBePlaced(Vector3 snappedPos) {
	//	return true;
		//check if it lies in same line in the grid
		///gridobject is the source
		/// get the destination
		GridPosition destGridPos = MazeManager.GetInstance().maze3DGrid.WorldPositionToGridPosition(snappedPos);
		return CanBePlaced(destGridPos);
	}

	private bool CanBePlaced(GridPosition destGridPos) {
		if(! MazeManager.GetInstance().maze3DGrid.CanObjectBePlacedAtPosition(gridObject,destGridPos) ) return false;

		if(gridObject.Position.x == destGridPos.x) {
			int delta = gridObject.Position.y > destGridPos.y ? 1:-1 ;
			for(int i = destGridPos.y ; i!= gridObject.Position.y; i=i+delta) {
				if( MazeManager.GetInstance().maze3DGrid.GetObjectAtPosition(gridObject.Position.x,i) != null)
					return false;
			}
		}
		if(gridObject.Position.y == destGridPos.y) {
			int delta = gridObject.Position.x > destGridPos.x ? 1:-1 ;
			for(int i = destGridPos.x ; i!= gridObject.Position.x; i=i+delta) {
				if( MazeManager.GetInstance().maze3DGrid.GetObjectAtPosition(i,gridObject.Position.y) != null)
					return false;
			}
		}
		if(gridObject.Position.x != destGridPos.x && gridObject.Position.y != destGridPos.y) return false;
		if(gridObject.Position.x == destGridPos.x && gridObject.Position.y == destGridPos.y) return false;
		return true;
	}

}
