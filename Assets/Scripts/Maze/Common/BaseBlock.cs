using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BaseBlock : MonoBehaviour, IGridObject
{
	//TODO : HACK 
	//IT SHOULD BE LIST OF RENDER 
	private Renderer _renderer=null;
	new public Renderer renderer {
		get {
			if(_renderer == null){
				_renderer = gameObject.GetComponentInChildren<Renderer>();
				if(_renderer==null)
					_renderer = gameObject.GetComponent<Renderer>();
			}
			return _renderer;
		}
	}

	List<GridPosition> _Shape;
	public List<GridPosition> Shape { 
		get{
			if(_Shape==null) {
				_Shape = new List<GridPosition>();
				_Shape.Add(new GridPosition(0,0));
			}
			return _Shape;
		}
	}

	public GridPosition Position{
		get{
			return new GridPosition(x,y);
		} set{
			x = value.x;
			y = value.y;
		} 
	}

	private GridPosition? _OriginalGridPosition=null;
	public GridPosition OriginalGridPosition {
		get {
			if(!_OriginalGridPosition.HasValue) {
				int _x = Convert.ToInt32( name.Split(MazeManager.DELIMITER.ToCharArray())[1] );
				int _y = Convert.ToInt32( name.Split(MazeManager.DELIMITER.ToCharArray())[2] );
				_OriginalGridPosition = new GridPosition(_x,_y);
			}
			return _OriginalGridPosition??Position;
		}
	}

	GridHeight _Height=new GridHeight(7);
	public GridHeight Height { get{return _Height;} set{_Height = value;} }

	private string _name=null;
	public string name {
				get {
						if (_name == null) {
							_name = gameObject.name;
						}
						return _name;
				}
				set {
						_name = value;
				}
		}

	private int _x=-1;
	public int x {
		get {
			if (_x == -1) {
				_x = Convert.ToInt32( name.Split(MazeManager.DELIMITER.ToCharArray())[1] );
			}
			return _x;
		}
		set {
			_x = value;
		}
	}

	private int _y=-1;
	public int y {
		get {
			if (_y == -1) {
				_y = Convert.ToInt32( name.Split(MazeManager.DELIMITER.ToCharArray())[2]);
			}
			return _y;
		}
		set {
			_y = value;
		}
	}
}