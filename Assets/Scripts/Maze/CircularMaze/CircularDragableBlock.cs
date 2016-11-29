using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityExtensions;

public class CircularDragableBlock : CoreBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
	BaseBlock _gridObject;
	BaseBlock gridObject {
		get {
			if(_gridObject == null) {
				_gridObject = GetComponent<BaseBlock>();
			}
			return _gridObject;
		}
	}

	Vector3 Center {
		get {
			return MazeManager.GetInstance().maze3DGrid.gridOrigin;
		}
	}

	private Vector3 oldSnappedPos;
	private GameObject dummyXrayBlock = null;
	virtual public void OnBeginDrag(PointerEventData eventData){
		//transform.position = GameManager.GetInstance().camera.ScreenToWorldPoint(eventData.position
		oldSnappedPos = transform.position; //SnapToWorldGridPosition(pos);
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

	//radius
	private bool IsMovingInX(Vector3 oldPos , Vector3 newPos) {

		float slope = Vector3.Dot(oldPos-Center , newPos - oldPos);

		if(slope > 0.5f || slope < -0.5f )
 			return true;
		else
			return false;

		float dispX = Mathf.Abs(newPos.x - oldPos.x);
		float dispZ = Mathf.Abs(newPos.z - oldPos.z);
		if(dispX > DispXorZThreshold && dispZ > DispXorZThreshold && Mathf.Abs(dispX/dispZ) > SlopeXoverZThreshold)
				return true;
		return false;
	}

	//angle
	private bool IsMovingInZ(Vector3 oldPos , Vector3 newPos) {

		float slope = Vector3.Dot(oldPos-Center , newPos - oldPos);
		
		if(slope > 0.5f || slope < -0.5f )
			return false;
		else
			return true;

		float dispX = Mathf.Abs(newPos.x - oldPos.x);
		float dispZ = Mathf.Abs(newPos.z - oldPos.z);
		if(dispX > DispXorZThreshold && dispZ > DispXorZThreshold && Mathf.Abs(dispZ/dispX) > SlopeXoverZThreshold)
			return true;
		return false;
	}

	virtual public void OnDrag(PointerEventData eventData) {
		Vector3 pos = GetWorldPosition(eventData);
	//	GridPosition destGridPos = MazeManager.GetInstance().maze3DGrid.WorldPositionToGridPosition(currentSnappedPos);
		Vector3 snappedPos = SnapToWorldGridPosition(pos);
		//rebuild the mesh
	//	transform.localRotation = Quaternion.Euler(0.0, 45.0, 0.0);
		float angleInDegree = Vector3.Angle(oldSnappedPos,pos);
	//	transform.LookAt(pos);


		float angle =  CircularMazeGridManager.GetObtuseAngle(new Vector2(1,0) , new Vector2(pos.x,pos.z)); // returns  in degree

		//transform.localEulerAngles = new Vector3(0,angleInDegree ,0);
	//	transform.RotateAround(Vector3.up,10);

		Vector3 constrainedPos = pos;
		
		if(IsMovingInX(oldSnappedPos,constrainedPos)) {
			(MazeManager.GetInstance().mazeModelBuilder as CircularTextureFillMazeBuilder).UpdateMeshRadius(
				gameObject,gridObject.OriginalGridPosition,  gridObject.Position, Vector3.Magnitude(pos - oldSnappedPos) );

		} else if(IsMovingInZ(oldSnappedPos,pos)) {
			(MazeManager.GetInstance().mazeModelBuilder as CircularTextureFillMazeBuilder).UpdateMeshAngle(
				gameObject,gridObject.OriginalGridPosition,  gridObject.Position, angle );
		}

		oldSnappedPos = snappedPos;
		Debug.Log(" drag "+name+",pos"+eventData.worldPosition);
	}

	virtual public void OnEndDrag(PointerEventData eventData){

		Debug.Log("end drag "+name+",pos"+eventData.worldPosition);
	}

	private void SetToSnappedPosition(Vector3 snappedPos) {
		if(CanBePlaced(snappedPos)) {
			dummyXrayBlock.transform.position = snappedPos;
			//TODO : TEMP
			dummyXrayBlock.SetActive(true);
			MazeManager.GetInstance().maze3DGrid.MoveGridBlockToGridPosition(
				gridObject,MazeManager.GetInstance().maze3DGrid.WorldPositionToGridPosition(snappedPos));
		}else
			dummyXrayBlock.SetActive(false);
	}

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
		return true;
	}

}
