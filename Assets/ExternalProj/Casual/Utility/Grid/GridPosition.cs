/*
 * GridPosition
 * 
 * Represent a position on a 2D grid.
 * 
 * Project: Awesome Knight
 * Author: John Avery (2012)
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct GridPosition {
	[System.Xml.Serialization.XmlAttribute]
	public int x;
	[System.Xml.Serialization.XmlAttribute]
	public int y;
	
	/*
	 * The default 1x1 building shape
	 */ 
	public static GridPosition[] DefaultShape = new GridPosition[]{new GridPosition(0,0)};
	//public static List<GridPosition> DefaultShape = new List<GridPosition>().Adnew GridPosition(0,0)};


	public override string ToString(){
		return ToString(x, y);
	}

	public static string ToString(int x, int y) {
		return x.ToString() + ServerConfig.colonDelimiters[0] + y.ToString();
	}

	public static GridPosition DeSerialize(string gridPos){
		string[] positions = gridPos.Split(ServerConfig.colonDelimiters);
		GridPosition position = new GridPosition(); 
		position.x = Convert.ToInt32(positions[0]);
		position.y = Convert.ToInt32(positions[1]);
		return position;
	}

	public static List<GridPosition> GetGridPositions(int sizex, int sizey) {
		GridPosition gridPosition;
		List<GridPosition> list = new List<GridPosition>();
		for (int i = 0; i < sizex; i++) {
			for (int j = 0; j < sizey; j++) {
				gridPosition = new GridPosition();
				gridPosition.x = i;
				gridPosition.y = j;
				list.Add(gridPosition);
			}
		}
		return list;
	}


	//TODO : MANJEET : NEED TO REDIFINE THIS .. AS NOW BUILDING IS PLACED AT RIGHT UPPER TILES RELATIVE TO ORIGIN
	//SO FOR 2X2 IT SHOULD BECOME 0,0;0,1;1,0;1,1
	/*
	 * The default 2x2 building shape
	 */ 
	public static GridPosition[] TwoByTwoShape = new GridPosition[]{new GridPosition(-1,-1), new GridPosition(-2,-2), new GridPosition(-2,-1), new GridPosition(-1,-2)};
	
	/*
	 * The default 2x2 building shape
	 */ 
	public static GridPosition[] TwoByTwoMoatShape = new GridPosition[]{new GridPosition(0,0), new GridPosition(-1,-1), new GridPosition(-1,0), new GridPosition(-1,0)};
	
	
	/*
	 * The default 3x3 building shape
	 */ 
	public static GridPosition[] ThreeByThreeShape = new GridPosition[]{new GridPosition( 0,0), new GridPosition( 0,-1), new GridPosition( 0,-2),
																		new GridPosition(-1,0), new GridPosition(-1,-1), new GridPosition(-1,-2),
																		new GridPosition(-2,0), new GridPosition(-2,-1), new GridPosition(-2,-2)};

	
	/*
	 * The default 4x4 building shape
	 */ 
	public static GridPosition[] FourByFourShape = new GridPosition[]{	new GridPosition( 0,0), new GridPosition( 0,-1), new GridPosition( 0,-2), new GridPosition( 0,-3),
																		new GridPosition(-1,0), new GridPosition(-1,-1), new GridPosition(-1,-2), new GridPosition(-1,-3),
																		new GridPosition(-2,0), new GridPosition(-2,-1), new GridPosition(-2,-2), new GridPosition(-2,-3),
																	  	new GridPosition(-3,0), new GridPosition(-3,-1), new GridPosition(-3,-2), new GridPosition(-3,-3)};
	
	/*
	 * A 4x2 shape with along the NE/SW axis
	 */ 
	public static GridPosition[] FourByTwoNEShape = new GridPosition[]{	new GridPosition( 0,-2), new GridPosition( 0,-1),
																		new GridPosition(-1,-2), new GridPosition(-1,-1),
																		new GridPosition(-2,-2), new GridPosition(-2,-1),
																	  	new GridPosition(-3,-2), new GridPosition(-3,-1)};

	/*
	 * A 4x2 shape with along the NW/SE axis
	 */ 
	public static GridPosition[] FourByTwoNWShape = new GridPosition[]{	new GridPosition(-1, 0), new GridPosition(-2, 0),
																		new GridPosition(-1,-1), new GridPosition(-2,-1),
																		new GridPosition(-1,-2), new GridPosition(-2,-2),
																	  	new GridPosition(-1,-3), new GridPosition(-2,-3)};
	
	public GridPosition(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public float Magnitude() {
		return Mathf.Sqrt(this.x*this.x + this.y* this.y);
	}

	public static float Magnitude(GridPosition point) {
		return Mathf.Sqrt (point.x*point.x + point.y*point.y);
	}

	public static float SqrMagnitude(GridPosition point) {
		return point.x*point.x + point.y*point.y;
	}

	public float SqrDistanceFrom(GridPosition point) {
		return  SqrMagnitude(this - point);
	}

	public static GridPosition operator +(GridPosition g1, GridPosition g2) 
   	{
    	return new GridPosition(g1.x + g2.x, g1.y + g2.y);
   	}
	
	public static GridPosition operator -(GridPosition g1, GridPosition g2) 
   	{
    	return new GridPosition(g1.x - g2.x, g1.y - g2.y);
   	}
	
	
	public static bool operator ==(GridPosition g1, GridPosition g2) 
   	{
   		if (g1.x != g2.x) return false; 
		if (g1.y != g2.y) return false;
		return true;
   	}
	
	public static bool operator !=(GridPosition g1, GridPosition g2) 
   	{
   		return !(g1 == g2);
   	}
	
 	public override bool Equals(object obj)  
   	{
		if (obj != null && obj is GridPosition) return this == (GridPosition)obj;
		return false;
   	}
	
	/*
	 * Simple hashcode override to avoid the warnirng.
	 */ 
	override public int GetHashCode(){
		return x << 16 + y;	
	}
	
}