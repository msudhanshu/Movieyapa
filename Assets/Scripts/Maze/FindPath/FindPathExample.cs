using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FindPath;

public class FindPathExample : Manager<FindPathExample>
{
	public GameObject tapEffect;

	override public void StartInit() {
	}
	
	override public void PopulateDependencies() {}
	
	
	void Update() {
		//OnBlockClick ();
	}
	
	private void OnBlockClick() {
		if (Input.GetMouseButton(0)) {
			Ray ray = MazeManager.GetInstance().mazeCamera.ScreenPointToRay (UICamera.lastTouchPosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000, 1 << MazeManager.MAZE_BLOCK_LAYER)) {
				Debug.DrawLine(MazeManager.GetInstance().mazeCamera.transform.position,hit.point,Color.blue,1);
				GameObject tile = hit.collider.gameObject;
				MazeBlock gt = tile.GetComponent<MazeBlock>();

				DebugPath.Log("clicked MAZEOBJECT:"+gt.name+","+gt.x+","+gt.y);
				DebugPath.Log(SEnum.GetStringValue(gt.mazeBlockType));

				//if(gt.mazeEndType != MazeEndNodeType.NULL) {
				if(MazeBlock.IsSinkBlock(gt.mazeBlockType)) {
					DebugPath.Log("clicked MAZEOBJECT Sink :"+gt.name+","+gt.x+","+gt.y);
					List<MazePoint> path =  GetPathToDestination(gt, MazeBlock.GetSourceForSinkBlock(gt.mazeBlockType));

					foreach (MazePoint p in path) {
						MazeBlock mb  = MazeManager.GetInstance().GetMazeBlock(p);
						DebugPath.Log("Painted Red: "+ mb.x+","+mb.y);
						mb.renderer.material.color = Color.red;
					}

				//	if(otherEnd != null) {
				//		gt.mazeEndType = otherEnd.mazeEndType;
				//	}
				}
				//}
				switch(gt.mazeBlockType) {
					case MazeBlockType.END:
						GameObject.Destroy(tile);
						break;
				}
				//temp:
				DebugPath.Log("MANJEET DELTETING THIS OBJECT");
				//GameObject.Destroy(tile);
			}
		}
	}

	private List<MazePoint> GetPathToDestination(MazeBlock mb, MazeBlockType destinationType) {
		MazeMap mazeMap = MazeManager.GetInstance ().mazeMap;
		FindPath.FindPath findPath = new FindPath.FindPath();
		return Path.GetPathPoints(findPath.getPath(mb.y,mb.x,null));
	}

	/*
	private MazeBlock GetOtherEnd(MazeBlock mb) {
		MazeMap mazeMap = MazeManager.GetInstance ().mazeMap;
		FindPath.FindPath findPath = new FindPath.FindPath(mazeMap.mazeGrid);
		findPath.getPath(mb.y,mb.x);
		return null;
	}
	*/	
}