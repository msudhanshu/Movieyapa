using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityExtensions;

public class GridView3D :  GridView { //Manager<GridView3D> {

	public float TileGapFactor = 0.1f;
	public Material NormalTileMaterial;
	public const string TILENAMEPREFIX = "TILE";
	public const string DELIMITER = "-";
	public GameObject gridTilePrefab;
	/**
	 * Layer used for gridtile.
	 */ 
	public const int GRID_TILE_LAYER = 13;

	void Awake()

	{
		Instance= this;
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(GenerateGridView());
	}
	
	/// <summary>
	/// Generates the grid sprites.
	/// </summary>
	/// <returns>The grid view.</returns>
	virtual protected IEnumerator GenerateGridView () {
		yield return true;
		
		int sizeX = BuildingModeGrid.GetInstance().gridSizeX;
		int sizeY = BuildingModeGrid.GetInstance().gridSizeY;
		sprites = new UISprite[sizeX,sizeY];
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				IGridObject gridObject = BuildingModeGrid3D.GetInstance().GetObjectAtPosition(new GridPosition(x, y));
				if (!(gridObject is UnusableGrid))
				{
					CreateGridTile(x, y);
				}
			}
		}

	}

	override public void ShowGrid(GridPosition gridPos, List<GridPosition> shape ) {
		FindTileAndSetVisible (GetTileName (gridPos.x, gridPos.y), true);
	}

	override public void HideGrid(GridPosition gridPos, List<GridPosition> shape ) {
		FindTileAndSetVisible (GetTileName (gridPos.x, gridPos.y), false);
	}

	override protected void FindTileAndSetVisible(string name, bool setVisible) {
		GameObject tileobject = GameObject.Find (name);
		if (tileobject != null) {
			tileobject.SetActive(true);
		}
	}
	
	override protected void CreateGridTile(int x, int y)
	{

		Vector3 position = BuildingModeGrid3D.GetInstance().GridPositionToArenaPosition(new GridPosition(x,y));
		GridPosition g = BuildingModeGrid3D.GetInstance ().ArenaPositionToGridPosition (position+new Vector3(0.2f,0.3f,0.4f));

		//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); 	//Destroy(cube.GetComponent(typeof(BoxCollider)));
		GameObject cube = (GameObject)Instantiate (gridTilePrefab);
		cube.transform.ResetTransformation ();
		string tileName = GetTileName (x, y);
		cube.name = tileName;

		GridTile gt = cube.GetComponent<GridTile> ();
		if (gt != null) {
						gt.TileName = tileName;
						gt.x = x;
						gt.y = y;
		}

		cube.transform.parent = transform;
		cube.transform.localPosition = position;// - new Vector3(BuildingModeGrid3D.GetInstance().gridWidth/2,0,BuildingModeGrid3D.GetInstance().gridHeight/2);
		cube.layer = GRID_TILE_LAYER;
		cube.transform.localScale = new Vector3(BuildingModeGrid3D.GetInstance().gridWidth*(1-TileGapFactor),0.3f,BuildingModeGrid3D.GetInstance().gridHeight*(1-TileGapFactor));

	}

	public void OnDrawGizmos () {
		Gizmos.color = Color.blue;
	//	Gizmos.DrawWireCube (ArenaTransform.position + new Vector3(gridSize * gridWidth, 0, gridSize * gridHeight)/2.0f, new Vector3(gridSize * gridWidth, 1, gridSize * gridHeight) );
	}

	private string GetTileName(int x, int y) {
		return TILENAMEPREFIX + DELIMITER + x + DELIMITER + y;
	}

	public void NormalMode()
	{
		int sizeX = BuildingModeGrid.GetInstance().gridSizeX;
		int sizeY = BuildingModeGrid.GetInstance().gridSizeY;
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				if (sprites[x,y] != null)
				{
					sprites[x,y].color = normal;
				}
			}
		}
	}

	public void BuildingMode()
	{
		int sizeX = BuildingModeGrid.GetInstance().gridSizeX;
		int sizeY = BuildingModeGrid.GetInstance().gridSizeY;
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				if (sprites[x,y] != null)
				{
					if ( BuildingModeGrid3D.GetInstance().CanObjectBePlacedAtPosition(GridPosition.DefaultShape, new GridPosition(x,y)))
					    sprites[x,y].color = buildingModeAvailable;
					else
					    sprites[x,y].color = buildingModeUnavailable;
				}
			}
		}
	}

	public void PathMode()
	{
		int sizeX = BuildingModeGrid.GetInstance().gridSizeX;
		int sizeY = BuildingModeGrid.GetInstance().gridSizeY;
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				if (sprites[x,y] != null)
				{
					if ( BuildingModeGrid3D.GetInstance().CanObjectBePlacedAtPosition(GridPosition.DefaultShape, new GridPosition(x,y)))
						sprites[x,y].color = pathModeAvailable;
					else
						sprites[x,y].color = pathModeUnavailable;
				}
			}
		}
	}

	public static GridView Instance
	{
		get; protected set;
	}
}
