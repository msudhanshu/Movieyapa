using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The grid used in building mode for the 3D view.
 */ 

/*
x = different radius = gridSizeX is ring/radius count
y = different angle  = gridSizeY is angle sengment count

gridWidth = ring/radius count
gridheight = (360/gridSizeY)

*/

public class CircularMazeGridManager : MazeGridManager
{
	public float extraDiscHoleRadius = 5;

	//We will have to leave min hole at center so that lowest block/disc can fit.
	public float discHoleRadius {
		get {
			return gridWidth/2.0f + extraDiscHoleRadius;
		}
	}

	public float radius {
		get {
			return extraDiscHoleRadius + gridSizeX*gridWidth;
		}
	}
	public float discInitialAngle = 10;

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
		gridHeight = 360.0f/(float)gridSizeY;
	}
	override public void Init(int gridSizeX, int gridSizeY) {
		base.Init(gridSizeX,gridSizeY);
		gridHeight = 360.0f/(float)gridSizeY;
	}

	override protected Vector3 CalculateGridPositionToArenaPosition(GridPosition position, List<GridPosition> shape) {
		//TODO : take shape into consideration: return center of the shape
		float r = discHoleRadius + position.x * gridWidth;
		float a = discInitialAngle + position.y * gridHeight;
		return new Vector3(r * Mathf.Cos(Mathf.Deg2Rad * a), 0, r * Mathf.Sin(Mathf.Deg2Rad * a));
	}
	
	override protected GridPosition CalculateArenaPositionToGridPosition(Vector3 position) {
		float radius = position.magnitude - discHoleRadius;
		float angle =  GetObtuseAngle(new Vector2(1,0) , new Vector2(position.x,position.z)); // returns  in degree
	//	float angle =  Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;
		angle-=discInitialAngle;
		return new GridPosition((int)(( radius + gridWidth/2.0f)/ gridWidth), 
		                        (int)((angle + gridHeight/2.0f) / gridHeight));
	}

	public void OnDrawGizmos () {
		if(initialised) {
		//gridOrigin = GameManager.GetInstance().gameView.transform.position;
		Gizmos.color = Color.red;
			//Gizmos.DrawWireCube (gridOrigin + new Vector3(gridSizeX * gridWidth, 0, gridSizeY * gridHeight)/2.0f,
		    //                 new Vector3(gridSizeX * gridWidth, 1, gridSizeY * gridHeight) );

			Gizmos.color = Color.blue;

			for (int x = 0; x <= gridSizeX; x++) {
			//	Gizmos.draw
				//DrawLine(gridOrigin+ new Vector3(x*gridWidth - gridWidth/2.0f,0,-gridHeight/2.0f),gridOrigin+new Vector3(x*gridWidth-gridWidth/2.0f,0,gridSizeY*gridHeight-gridHeight/2.0f));
				DrawCircle(gridOrigin, x*gridWidth + discHoleRadius - gridWidth/2.0f);
			}

			Gizmos.color = Color.green;
			float rayAngle ;
			for (int y = 0; y <= gridSizeY; y++) {
				rayAngle = discInitialAngle + y * gridHeight - gridHeight/2.0f;
				DrawLine(gridOrigin ,
				         gridOrigin+new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * rayAngle), 0,
				                       radius * Mathf.Sin(Mathf.Deg2Rad * rayAngle)));
			}
		}
	}

	private static float GetSignAngle(Vector2 v1, Vector2 v2)
	{
		var sign = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
		return Vector2.Angle(v1, v2) * sign;
	}

	public static float GetObtuseAngle(Vector2 v1, Vector2 v2)
	{
		var sign = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
		return sign>0?Vector2.Angle(v1, v2):360-Vector2.Angle(v1, v2);
	}

	public void DrawCircle(Vector3 center, float radius) {
		int segmentCount = 50;
		float segmentAngle ;
		Vector3 startPoint,endPoint;
		for(int i=0;i < segmentCount; i++) {
			segmentAngle =  i*360.0f/(float)segmentCount;
			startPoint = center + new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * segmentAngle), 0 ,
				                                      radius * Mathf.Sin(Mathf.Deg2Rad * segmentAngle));
			segmentAngle = (i+1)*360.0f/(float)segmentCount;
			endPoint = center + new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * segmentAngle), 0 ,
			                                    radius * Mathf.Sin(Mathf.Deg2Rad * segmentAngle));
			Gizmos.DrawLine(startPoint,endPoint);
		}
	}

}

