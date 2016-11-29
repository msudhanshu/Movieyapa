using System;
using System.Collections.Generic;
using UnityEngine;

namespace FindPath {


	public class DebugPath {
		public static bool DebugTrace = false;
		public static void Log(string s) {
			if(DebugTrace)
				Debug.Log(s);
		}
		public static void LogError(string s) {
			if(DebugTrace)
				Debug.LogError(s);
		}
		public static void LogWarning(string s) {
			if(DebugTrace)
				Debug.LogWarning(s);
		}
	}


public class FindPath
{
	public Point startPoint;
	public Point endPoint;
    
	public Path foundPath;
	//public MazeBlockType[,] mazeGrid;
	public int xMax;
	public int yMax;
	static int pathsCount =0;
    private int[,] traversedPoints;
    

	IPathFinderParam pathParam;

//	MazeBlockType destinationBlockType;

	/*
	public FindPath (MazeBlockType[,] mazeGrid)
	{
		this.mazeGrid = mazeGrid;
		this.yMax = mazeGrid.GetLength(0);
		this.xMax =	mazeGrid.GetLength(1);
	}
	
	public FindPath (Point startPoint,  MazeBlockType[,] mazeGrid )
	{
		this.startPoint = startPoint;
		this.mazeGrid = mazeGrid;
		this.yMax = mazeGrid.GetLength(0);
		this.xMax =	mazeGrid.GetLength(1);
		setDirectionOfPoint();
	}
    */

	private void setDirectionOfPoint(){
			if(this.startPoint.x <=0  ){
				this.startPoint.restrictedDirection = Edirection.eLeft;
			}
		   if(this.startPoint.x >= this.xMax -1 ){
				this.startPoint.restrictedDirection = Edirection.eRight;
			}
			if(this.startPoint.y <=0  ){
				this.startPoint.restrictedDirection = Edirection.eUp;
			}
			if(this.startPoint.x >= this.yMax -1 ){
				this.startPoint.restrictedDirection = Edirection.eDown;
			}
	}

	public void printPath(Path p){
		if(p!=null){
			string pathfinal="Path = ";
			for(int i =p.arrayPoints.Count -1 ; i>=0 ; i--){
				Point pc = p.arrayPoints[i];
				pathfinal+="("+pc.x+ ","+ pc.y+"),";
			}
			pathfinal += " PathEnd";
			DebugPath.Log(pathfinal);
			printPath(p.parentPath);
		}
	}

	private bool isPath(Point p){
		if(p==null){
			return false;
		}
		//if((p.x > 0  || p.x < this.xMax-1) && (p.y > 0 || p.y < this.yMax-1)){
		//if((p.x > 0  || p.x < this.xMax-1) && (p.y > 0 || p.y < this.yMax-1)){
			if( pathParam.IsConnectedBlock(p.x,p.y)){
				DebugPath.Log("found path : source1  "+p.x+","+p.y);
				return true;
			}
		//}
		return false;
	}

	private Point validatePoint(Point p){
		if(p==null){
			return null;
		}
	//	if((p.x ==0 || p.x == this.xMax-1 || p.y == 0 || p.y == yMax-1) && !startPoint.isSameLocation(p)){
			//if(mazeGrid[p.x, p.y] ==  destinationBlockType){
			if(pathParam.IsDestination(p.x, p.y)){
				return p;
			}
			else
				return null;
	//	}
		//else
		return null;
	}

	int count=0;
	public Path updatePath(Path w){
		count++;
		//	if(count>10) return null;
		Point p = w.arrayPoints[w.arrayPoints.Count-1];
		DebugPath.Log("update called : "+ p.x+ " " +p.y+"  Count"+w.countOfTravel);
//		if(p.x>60 || p.y>60){
//			return null;
//		}
//		if(w.countOfTravel >= (this.xMax-1)*(this.yMax-1)){
//				return null;
//		}
//		
		//NSMutableArray ar = NSMutableArray allocinit;
		List<Point> ar = new List<Point>();
		List<Point> arVar = new List<Point>();

		Point l  = new Point(p,Edirection.eLeft);
		Point r  = new Point(p,Edirection.eRight);
		Point up = new Point(p,Edirection.eUp);
		Point d  = new Point(p,Edirection.eDown);
		ar.Add(l);
		ar.Add(r);
		ar.Add(up);
		ar.Add(d);
		for(int i =0; i<4; i++){
			Point t = ar[i];
			if(!pathParam.IsInMazeBound(t.x,t.y)) continue;
//			DebugPath.Log("point array at :"+t.x+","+t.y);
		
			if(p.restrictedDirection != t.movedDirection){
					//Point pt = traversedPoints[t.x,t.y];
					if(traversedPoints[t.x,t.y] == 1){
						 continue;
                    }
                    if(validatePoint(t) !=null ){
					w.arrayPoints.Add(t);
					traversedPoints[t.x,t.y] = 1;
					w.isFound = true;
					DebugPath.Log(@"found "+ w.arrayPoints);
					foundPath = w;
					printPath(w);
					return foundPath;
				}
				if(isPath(t)){
					DebugPath.Log(@"main ppint (%d, %d)"+p.x+","+p.y);
					arVar.Add(t);
					traversedPoints[t.x,t.y] = 1;
				}
			}
		}
		
		if(arVar.Count==0){
			w.isInvalid = true;
			//DebugPath.Log(@"invalid %@",w.arrayPoints);
			//w = nil;
			return w;
		}
		else if(arVar.Count==1){
			
				Point t = arVar[0];
				w.arrayPoints.Add(t);
				w.countOfTravel++;
				return updatePath(w);
			
		}
		else{
			for(int i =0; i<arVar.Count; i++){
				Point t = arVar[i];
				Path wt = new Path(t);
				w.arrayChildPath.Add(wt);
				w.countOfTravel++;
				wt.parentPath = w;
				//DebugPath.Log(@"-------------%d\n" + pathsCount++);
				Path temp = updatePath(wt);
				if(temp!=null && temp.isFound){
					return temp;
				}
				else{
					//return null;
				}
			}
		}
		return null;
	}

		public Path getPath(int y, int x, IPathFinderParam pathParam){
			this.xMax = pathParam.xMax;
			this.yMax = pathParam.yMax;
			this.pathParam = pathParam;
			Point p = new Point(y,x,Edirection.eNoDir);
			traversedPoints = new int[xMax,yMax];
			traversedPoints[p.x,p.y] = 1;
		//	this.destinationBlockType = destinationBlockType;
			this.startPoint = p;
			Path path = new Path(p);
			Path final  = updatePath(path);
			return final;
		}

}

}