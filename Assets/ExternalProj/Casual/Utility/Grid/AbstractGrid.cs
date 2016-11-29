using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class AbstractGrid : MonoBehaviour, IGrid
{
	/**
	 * Dimensions of the grid.
	 */ 
	public int gridSizeX;
	public int gridSizeY;
	/**
	 * Mapping between IGridObjects and position(s) on the grid.
	 */ 
	protected Dictionary <IGridObject, List<GridPosition>> gridObjects;
		
	/**
	 * The actual grid.
	 */ 
	protected IGridObject[,] grid;
	
	/**
	 * Return the object at the given position or null if no object
	 * at the given position.
	 */ 

	virtual public IGridObject GetObjectAtPosition(int x, int y ){
		return GetObjectAtPosition(new GridPosition(x,y));
	}

	virtual public IGridObject GetObjectAtPosition(GridPosition position){
		if (position.x < gridSizeX && position.y < gridSizeY && position.x  >= 0 &&position.y  >= 0) {
			return grid[position.x, position.y];
		}
		return null;
	}
	
	virtual public void AddObjectAtPosition(IGridObject gridObject, GridPosition position) {
		List<GridPosition> gridPosisitons = new List<GridPosition>();
		GridPosition newPosition;
		gridObject.Position = position;
		foreach (GridPosition g in gridObject.Shape) {
			newPosition = position + g;
			gridPosisitons.Add (newPosition);
			if(gridSizeX>newPosition.x && gridSizeY>newPosition.y)
				grid[newPosition.x, newPosition.y] = gridObject;
		}
		gridObjects.Add(gridObject, gridPosisitons);
	}
	
	virtual public void RemoveObject(IGridObject gridObject) {
		if(gridObject!=null && gridObjects.ContainsKey(gridObject)) {
			foreach (GridPosition g in gridObjects[gridObject]) {
				grid[g.x, g.y] = null;
			}
			gridObjects.Remove(gridObject);
		} else {
			Debug.LogWarning("Gridobject "+gridObject.ToString() +" can't be removed as it is not in record");
		}
	}

	virtual public void MoveGridBlockToGridPosition(IGridObject gridObject, GridPosition position) {
		RemoveObject(gridObject);
		AddObjectAtPosition(gridObject,position);
	}

	virtual public void RemoveObjectAtPosition(GridPosition position){
		IGridObject g = GetObjectAtPosition(position);
		if (g != null) RemoveObject(g);
	}
	
	virtual public bool CanObjectBePlacedAtPosition(IGridObject gridObject, GridPosition position) {
		GridPosition newPosition;
		foreach (GridPosition g in gridObject.Shape) {
			newPosition = position + g;
			if (newPosition.x < 0 || newPosition.x >= grid.GetLength(0)) return false;
			if (newPosition.y < 0 || newPosition.y >= grid.GetLength(1)) return false;
			if (grid[newPosition.x, newPosition.y] != null && grid[newPosition.x, newPosition.y] != gridObject)
				return false;
		}
		return true;
	}

	virtual public bool CanObjectBePlacedAtPosition(GridPosition[] shape, GridPosition position) {
		GridPosition newPosition;
		foreach (GridPosition g in shape) {
			newPosition = position + g;
			if (newPosition.x < 0 || newPosition.x >= grid.GetLength(0)) return false;
			if (newPosition.y < 0 || newPosition.y >= grid.GetLength(1)) return false;
			if (grid[newPosition.x, newPosition.y] != null) return false;
		}
		return true;
	}

	abstract public Vector3 GridPositionToArenaPosition(GridPosition position);
	abstract public Vector3 GridPositionToArenaPosition(GridPosition position, List<GridPosition> shape);
	//converts to Unity World
	abstract public Vector3 GridPositionToWorldPosition (GridPosition position);
	abstract public Vector3 GridPositionToWorldPosition (GridPosition position, List<GridPosition> shape);


	abstract public GridPosition ArenaPositionToGridPosition(Vector3 position);
	abstract public GridPosition WorldPositionToGridPosition(Vector3 position);

	abstract protected GridPosition CalculateArenaPositionToGridPosition(Vector3 position);
	abstract protected Vector3 CalculateGridPositionToArenaPosition (GridPosition position, List<GridPosition> shape);



	virtual public Vector3 SnapWorldPositionToGrid(Vector3 position){
		return GridPositionToArenaPosition(ArenaPositionToGridPosition(position));
	}
}

