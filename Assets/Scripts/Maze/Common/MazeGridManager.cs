using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The grid used in building mode for the 3D view.
 */ 
public class MazeGridManager : BuildingModeGrid3D
{
	public Dictionary<string, BaseBlock> mazeBlockOccupied = new Dictionary<string,BaseBlock> ();

	//public BaseBlock[,] blockGrid;

	public Vector3 gridOrigin{
		get {
			return GameManager.GetInstance().gameView.transform.position;
		}
	}
	/**
	 * Get the instance of the grid class or create if one has not yet been created.
	 * 
	 * @returns An instance of the grid class.
	 */ 
	public static MazeGridManager Get3DInstance(){
		return instance as MazeGridManager;
	}

//	override public void Init() {
//		base.Init ();
//	}
	override public void Init(int gridSizeX, int gridSizeY) {
		base.Init(gridSizeX,gridSizeY);
//		blockGrid = new BaseBlock[gridSizeX,gridSizeY];
	}
	public void DrawLine(Vector3 start, Vector3 end) {
		Gizmos.DrawLine(start,end);
	}

	virtual public void MoveGridBlockToGridPosition(BaseBlock gridObject, GridPosition position , bool force=false, bool changeTransform=true) {
		IGridObject destObject = GetObjectAtPosition(position);
		if(destObject == null  || force) {
			if(destObject!= null) RemoveObject(destObject);
			if(changeTransform)
				gridObject.gameObject.transform.position = GridPositionToWorldPosition(position);
			RemoveObject(gridObject);
			AddObjectAtPosition(gridObject,position);
		} else {
			Debug.LogError("gridObject "+gridObject.ToString() + "can not be moved to occupied place "+position.ToString());
		}
	}

	public BaseBlock GetBaseBlockAtPosition(GridPosition position) {
		IGridObject result;
		result = GetObjectAtPosition(position);
		if(result is BaseBlock)
			return result as BaseBlock;
		return null;
	}


	virtual public void TransformBlockToGridPosition(BaseBlock gridObject, GridPosition position , bool force=false) {
		IGridObject destObject = GetObjectAtPosition(position);
		if(destObject == null  || force) {
			if(destObject!= null) RemoveObject(destObject);
			gridObject.gameObject.transform.position = GridPositionToWorldPosition(position);
			RemoveObject(gridObject);
			AddObjectAtPosition(gridObject,position);
		} else {
			Debug.LogError("gridObject "+gridObject.ToString() + "can not be moved to occupied place "+position.ToString());
		}
	}

	//TODO : TEMP : HACK
	//move it to differet file : 
	private System.Random rnd = new System.Random();
	public IEnumerator SlideShuffle() {
		int mazeX = MazeManager.GetInstance().maze3DGrid.gridSizeX;
		int mazeY = MazeManager.GetInstance().maze3DGrid.gridSizeY;
		GridPosition freeGridPosition = FindFreeGridPosition();
		GridPosition oldFreeGridPos = freeGridPosition;
		int ShuffleIteration = mazeX*mazeY*2;
		
		List<GridPosition> movableTiles = new List<GridPosition>(4);
		for(int i=0;i<ShuffleIteration;i++) {
			movableTiles.Clear();
			if(freeGridPosition.x > 0) movableTiles.Add(freeGridPosition-new GridPosition(1,0));
			if(freeGridPosition.x < mazeX-1) movableTiles.Add(freeGridPosition + new GridPosition(1,0));
			if(freeGridPosition.y > 0) movableTiles.Add(freeGridPosition-new GridPosition(0,1));
			if(freeGridPosition.y < mazeY-1) movableTiles.Add(freeGridPosition+new GridPosition(0,1));
		
			movableTiles.RemoveAll(x=> x.x==oldFreeGridPos.x && x.y==oldFreeGridPos.y);

			int tileToMove = rnd.Next(movableTiles.Count);
			GridPosition tempGridPosition = movableTiles[tileToMove];
			yield return StartCoroutine(SlideBlock(movableTiles[tileToMove], freeGridPosition));
			oldFreeGridPos = freeGridPosition;
			freeGridPosition = tempGridPosition;
		}

		yield break;
	}


	IEnumerator SlideBlock(GridPosition start , GridPosition to, float ANIM_TIME = 0.1f) {
		BaseBlock b = MazeManager.GetInstance().maze3DGrid.GetBaseBlockAtPosition(start);
		
		MazeManager.GetInstance().maze3DGrid.MoveGridBlockToGridPosition(b, to,false);
		
		Vector3 finalPos =  GridPositionToWorldPosition(to);
		Vector3 startPos = b.gameObject.transform.position;
		
		float elapsedTime = 0;
		while (elapsedTime < ANIM_TIME)
		{
			b.gameObject.transform.position = Vector3.Lerp(startPos, finalPos , (elapsedTime / ANIM_TIME));
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
	}

	private GridPosition FindFreeGridPosition() {
		return new GridPosition();
	}
	
	private void LogDetail(BaseBlock b1 , BaseBlock b2) {
		if(b1!=null && b2!=null)  {
			Debug.Log("BaseBlock 1:"+b1.gameObject.name+","+b1.Position.ToString());
			Debug.Log("BaseBlock 2:"+b2.gameObject.name+","+b2.Position.ToString());
			
		}
	}
	
	
	public void RandomShuffle() {
		int mazeX = MazeManager.GetInstance().maze3DGrid.gridSizeX;
		int mazeY = MazeManager.GetInstance().maze3DGrid.gridSizeY;
		for (int x = 0; x < mazeX; x++)
		{
			for (int y = 0; y < mazeY; y++)
			{
				//MazeManager.GetInstance().maze3DGrid.blockGrid
				//BaseBlock b1 =  MazeManager.GetInstance().maze3DGrid.blockGrid[x,y];
				int randx = rnd.Next(mazeX-1);
				int randy = rnd.Next(mazeY-1);
				//BaseBlock b2 = MazeManager.GetInstance().maze3DGrid.blockGrid[randx , randy ];
				
				BaseBlock b1 = MazeManager.GetInstance().maze3DGrid.GetBaseBlockAtPosition(new GridPosition(x,y));
				BaseBlock b2 = MazeManager.GetInstance().maze3DGrid.GetBaseBlockAtPosition(new GridPosition(randx,randy));
				
				Debug.Log("Before suffling");
				LogDetail(b1,b2);
				if(b1==null || b2==null || b1==b2) {
					
					Debug.Log("b1 or b2 is null: b1="+x+y+"b2="+randx+randy);
					continue;
				}
				
				GridPosition tempGridPosition =  b1.Position;
				MazeManager.GetInstance().maze3DGrid.RemoveObject(b2);
				MazeManager.GetInstance().maze3DGrid.MoveGridBlockToGridPosition(b1, b2.Position,true);
				MazeManager.GetInstance().maze3DGrid.MoveGridBlockToGridPosition(b2, tempGridPosition);
				Debug.Log("After suffling");
				LogDetail(MazeManager.GetInstance().maze3DGrid.GetBaseBlockAtPosition(new GridPosition(x,y)),
				          MazeManager.GetInstance().maze3DGrid.GetBaseBlockAtPosition(new GridPosition(randx,randy)) );
				//Swap(b1,b2);
				//	MazeManager.GetInstance().maze3DGrid.blockGrid[x,y] = b2;
				//	MazeManager.GetInstance().maze3DGrid.blockGrid[rnd.Next(mazeMap.mazeX-1) , rnd.Next(mazeMap.mazeY-1)] = b1;
			}
		}
	}
	
	public void Swap(BaseBlock b1, BaseBlock b2) {
		
		if(b1==null || b2==null) return;
		
		//b1.gameObject.name = "Manjeet";
		//GameObject.Destroy(b1.gameObject);
		//GameObject.Destroy(b2.gameObject);
		
		Vector3 tempp = b1.gameObject.transform.position;
		b1.gameObject.transform.position = b2.gameObject.transform.position;
		b2.gameObject.transform.position = tempp;
		
		GridPosition tempGridPosition = b1.Position;
		MazeManager.GetInstance().maze3DGrid.MoveGridBlockToGridPosition(b1, b2.Position);
		MazeManager.GetInstance().maze3DGrid.MoveGridBlockToGridPosition(b2, tempGridPosition);
		
		
		
		//		string tempname = b1.gameObject.name;
		//		b1.gameObject.name = b2.gameObject.name;
		//		b2.gameObject.name = tempname;
		
	}
}

