using System.Collections.Generic;

[System.Serializable]

/**
 * Data for the type of a building. Public variables are used for simplicity of serialization
 * but should not be modified directly.
 */ 
public class StaticObstacleData {
	//List<GridPosition> _shape = new List<GridPosition>();


	public virtual string id {get; set;}						// Unique id of the building.
//	public virtual string name {get; set;}						// Human readable name of the building.
//	public virtual string description {get; set;}				// A human readable description of the building.
	
	//Todo : shape should be just intxint
//	public virtual int sizex {get; set;}	
//	public virtual int sizey {get; set;}	
//	public virtual int sizeHeight {get; set;}	

//	public virtual List<GridPosition> shape {get {return _shape;} set{ if (value != null) _shape = value;}}		// Shape of the building in the isometric grid.
	
	public virtual ObstacleType obstacleType {get; set;}
	/**
	 * Current position of the building
	 */
	private GridPosition _position;
	public virtual GridPosition position { 
		get {
			return _position;
		}
		set { 
			_position = value;
	}}
	
	/**
	 * Current position of the building
	 */
	//public virtual GridHeight height {get; set;}

}