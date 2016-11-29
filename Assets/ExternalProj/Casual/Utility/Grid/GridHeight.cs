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

[System.Serializable]
public struct GridHeight {
	[System.Xml.Serialization.XmlAttribute]
	public float h;

	public GridHeight(float height) {
			this.h = height;
	}

	public override string ToString ()
	{
		return string.Format (""+h);
	}
	public static GridHeight DeSerialize(string h){
		float height = (float)Convert.ToDouble(h);
		return new GridHeight(height);
	}
	
}