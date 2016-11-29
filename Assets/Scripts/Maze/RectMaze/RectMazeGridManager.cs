using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The grid used in building mode for the 3D view.
 */ 
/*
 * Old
o----------- > x
|
|
|
|
v
y
	override protected Vector3 CalculateGridPositionToArenaPosition(GridPosition position, List<GridPosition> shape) {
		//TODO : take shape into consideration: return center of the shape
		return new Vector3(position.x * gridWidth, 0, 
		                   (gridSizeY -1 - position.y) * gridHeight);
	}
	
	override protected GridPosition CalculateArenaPositionToGridPosition(Vector3 position) {
		return new GridPosition((int)((position.x + gridWidth/2.0f)/ gridWidth), gridSizeY -1 - (int)((position.z+gridHeight/2.0f) / gridHeight));
	}

	public void OnDrawGizmos () {
		if(initialised) {
		Gizmos.color = Color.red;
			//Gizmos.DrawWireCube (gridOrigin + new Vector3(gridSizeX * gridWidth, 0, gridSizeY * gridHeight)/2.0f,
		    //                 new Vector3(gridSizeX * gridWidth, 1, gridSizeY * gridHeight) );

			Gizmos.color = Color.blue;

			for (int x = 0; x <= gridSizeX; x++) {
				DrawLine(gridOrigin+ new Vector3(x*gridWidth - gridWidth/2.0f,0,-gridHeight/2.0f),gridOrigin+new Vector3(x*gridWidth-gridWidth/2.0f,0,gridSizeY*gridHeight-gridHeight/2.0f));
			}
			for (int y = 0; y <= gridSizeY; y++) {
				DrawLine(gridOrigin+ new Vector3(-gridWidth/2.0f,0,y*gridHeight-gridHeight/2.0f),gridOrigin+new Vector3(gridSizeX*gridWidth-gridWidth/2.0f,0,y*gridHeight-gridHeight/2.0f));
			}
		}
	}
*/

/* New
y
^
|
|
|
|
o----------- > x


	override protected Vector3 CalculateGridPositionToArenaPosition(GridPosition position, List<GridPosition> shape) {
		//TODO : take shape into consideration: return center of the shape
		return new Vector3(position.x * gridWidth, 0, 
		                   (gridSizeY -1 - position.y) * gridHeight);
	}
	
	override protected GridPosition CalculateArenaPositionToGridPosition(Vector3 position) {
		return new GridPosition((int)((position.x + gridWidth/2.0f)/ gridWidth), gridSizeY -1 - (int)((position.z+gridHeight/2.0f) / gridHeight));
	}
	
GridSizeX,GridSizeY
*/
public class RectMazeGridManager : MazeGridManager
{
	/**
	 * Get the instance of the grid class or create if one has not yet been created.
	 * 
	 * @returns An instance of the grid class.
	 */ 
	public static MazeGridManager Get3DInstance(){
		return instance as MazeGridManager;
	}

	override public void Init() {
		base.Init ();
	}

	override protected Vector3 CalculateGridPositionToArenaPosition(GridPosition position, List<GridPosition> shape) {
		//TODO : take shape into consideration: return center of the shape
		return new Vector3(position.x * gridWidth, 0, 
		                   (position.y) * gridHeight);
	}
	
	override protected GridPosition CalculateArenaPositionToGridPosition(Vector3 position) {
		return new GridPosition((int)((position.x + gridWidth/2.0f)/ gridWidth), (int)((position.z+gridHeight/2.0f) / gridHeight));
	}

	public void OnDrawGizmos () {
		if(initialised) {
		Gizmos.color = Color.red;
			//Gizmos.DrawWireCube (gridOrigin + new Vector3(gridSizeX * gridWidth, 0, gridSizeY * gridHeight)/2.0f,
		    //                 new Vector3(gridSizeX * gridWidth, 1, gridSizeY * gridHeight) );

			Gizmos.color = Color.blue;

			for (int x = 0; x <= gridSizeX; x++) {
				DrawLine(gridOrigin+ new Vector3(x*gridWidth - gridWidth/2.0f,0,-gridHeight/2.0f),gridOrigin+new Vector3(x*gridWidth-gridWidth/2.0f,0,gridSizeY*gridHeight-gridHeight/2.0f));
			}
			for (int y = 0; y <= gridSizeY; y++) {
				DrawLine(gridOrigin+ new Vector3(-gridWidth/2.0f,0,y*gridHeight-gridHeight/2.0f),gridOrigin+new Vector3(gridSizeX*gridWidth-gridWidth/2.0f,0,y*gridHeight-gridHeight/2.0f));
			}
		}
	}

	public void DrawLine(Vector3 start, Vector3 end) {
		Gizmos.DrawLine(start,end);
	}

}

