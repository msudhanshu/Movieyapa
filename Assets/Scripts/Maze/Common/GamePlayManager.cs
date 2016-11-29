using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePlayManager : MonoBehaviour
{
	public void BlockMoved() {
		BlockMoved(null,new GridPosition());
	}
	public void BlockMoved(BaseBlock b, GridPosition from) {
		int mazeX = MazeManager.GetInstance().maze3DGrid.gridSizeX;
		int mazeY = MazeManager.GetInstance().maze3DGrid.gridSizeY;
		bool flagAllBlockSet = true;
		for (int x = 0; x < mazeX; x++)
		{
			if(!flagAllBlockSet) break;
			for (int y = 0; y < mazeY; y++)
			{
				BaseBlock b1 = MazeManager.GetInstance().maze3DGrid.GetBaseBlockAtPosition(new GridPosition(x,y));
				if(b1!=null && b1.Position!=b1.OriginalGridPosition) {
					flagAllBlockSet = false;
					break;
				}
			}
		}

		if(flagAllBlockSet) {
			LevelCompleted();
		}
	}

	//map of sinkmazeblock and sinkblocktype select
	private Dictionary<MazeBlock,MazeBlockType> markedSinkedBlocks = new Dictionary<MazeBlock, MazeBlockType>();
	public void OnSinkBlockSelected(MazeBlock mb, MazeBlockType selecteType) {
		if(markedSinkedBlocks.ContainsKey(mb))
			markedSinkedBlocks[mb] = selecteType;
		else
			markedSinkedBlocks.Add(mb,selecteType);

		//temp
		return;

		StartCoroutine(ExecutePathCreation(mb,selecteType));
	}

	public void AllSinkSelected() {
		StartCoroutine(AllSinkSelectedCorotuine());
	}

	public IEnumerator AllSinkSelectedCorotuine() {
		foreach(KeyValuePair<MazeBlock,MazeBlockType> markedBlocks in markedSinkedBlocks) {
			yield return StartCoroutine( ExecutePathCreation(markedBlocks.Key,markedBlocks.Value) );
		}
		yield break;
	}

	public IEnumerator ExecutePathCreation(MazeBlock mb,MazeBlockType selectedBlockType) {
		if(mb.mazeBlockType != selectedBlockType ){//} || !MazeBlock.IsSinkBlock(selectedBlockType)) {
			mb.renderer.material.color = MazeBlockEffect.GetWrongSinkTargetClickEffect().color;
			yield break;
		} else {
//		Debug.Log("clicked MAZEOBJECT Sink :"+mb.name+","+mb.x+","+mb.y);
			List<MazePoint> path =  GetPathToDestination(mb, MazeBlock.GetSourceForSinkBlock(mb.mazeBlockType));
		    if(path !=null) {
				selectedBlockType = GetSelectedPathTypeBlock(path[0]);
				foreach (MazePoint p in path) {
					MazeBlock mbPath  = MazeManager.GetInstance().GetMazeBlock(p);
//					Debug.Log("Painted Red: "+ mbPath.x+","+mbPath.y);
					yield return StartCoroutine(mbPath.PathTraceEffect(selectedBlockType));
					//mbPath.renderer.material.color = Color.red;
					yield return new WaitForSeconds(0.1f);
				}
				LevelCompleted();
			}
		}
		yield break;
	}

	private MazeBlockType GetSelectedPathTypeBlock(MazePoint mazePoint) {
		return MazeManager.GetInstance().GetMazeBlock(mazePoint).mazeBlockType;
	}

	private List<MazePoint> GetPathToDestination(MazeBlock mb, MazeBlockType destinationType) {
		FindPath.FindPath findPath = new FindPath.FindPath();
		//LEVEL WHERE ANY BLOCK CAN BE CLICKED
		return FindPath.Path.GetPathPoints(findPath.getPath(mb.y,mb.x,new AnySinkDestination()));
	
		//LEVEL WHERE SINK IS ONLY CLICKED 
		//	return FindPath.Path.GetPathPoints(findPath.getPath(mb.y,mb.x,new BlockTypeDestination(destinationType)));
	}

	//public UILevelController uiLevelController;
	private void LevelCompleted() {
		//add resources through resouces manager
		//unload current level by shown shome finishing animation 
		//and level up animation popup .. depending upon failure and success
		//open uilevelcontroller popup.. by unlocking next level;
		//uiLevelController.Show(true);
		PopupManager.GetInstance().ShowPanel(PanelType.SHOP);
		MazeManager.GetInstance().UnLoadLevel(null);
	}

}

public interface IPathFinderParam {
	int xMax{get;}
	int yMax{get;}
	bool IsConnectedBlock(int x, int y);
	bool IsDestination(int x, int y);
	bool IsInMazeBound(int x, int y);
}

public abstract class PathFinderParam : IPathFinderParam {
	//protected MazeMap mazeMap;
	protected MazeGridManager maze3DGrid;
	protected void Init() {
		//mazeMap =  MazeManager.GetInstance ().mazeMap;
		maze3DGrid = MazeManager.GetInstance ().maze3DGrid;
	}

	public int xMax{get{return maze3DGrid.gridSizeX;}}
	public int yMax{get{return maze3DGrid.gridSizeY;}}

	public virtual bool IsConnectedBlock(int x, int y) {
		if(!IsInMazeBound(x,y)) return false;
		MazeBlock block = maze3DGrid.GetObjectAtPosition(x,y) as MazeBlock ;
		if(block ==  null) return false;
		if(MazeBlock.IsAnyBlock(block.mazeBlockType)) return true;
		//if( MazeBlock.IsAnyBlock(mazeMap.mazeGrid[x, y]) ) return true;
		return false;
	}

	public abstract bool IsDestination(int x, int y);

	public virtual bool IsInMazeBound(int x, int y) {
		return ( (x>=0 && x<=xMax-1) && (y>=0 && y<=yMax-1));
	}
}

public class BlockTypeDestination : PathFinderParam {
	MazeBlockType destinationBlockType;

	public BlockTypeDestination(MazeBlockType destinationBlockType) {
		this.destinationBlockType = destinationBlockType;
		Init();
	}

	public override bool IsDestination(int x, int y) {
		if(!IsInMazeBound(x,y)) return false;
		MazeBlock block = maze3DGrid.GetObjectAtPosition(x,y) as MazeBlock ;
		if(block ==  null) return false;
		if( destinationBlockType == block.mazeBlockType) return true;

		//if(mazeMap.mazeGrid[x, y] ==  destinationBlockType) return true;
		return false;
	}
}

public class AnySinkDestination : PathFinderParam {
	
	public AnySinkDestination() {
		Init();
	}
	
	public override bool IsDestination(int x, int y) {
		if(!IsInMazeBound(x,y)) return false;
		MazeBlock block = maze3DGrid.GetObjectAtPosition(x,y) as MazeBlock ;
		if(block ==  null) return false;
		if(MazeBlock.IsSourceBlock(block.mazeBlockType)) return true;
		//if( MazeBlock.IsSourceBlock(mazeMap.mazeGrid[x, y])) return true;
		return false;
	}
}
