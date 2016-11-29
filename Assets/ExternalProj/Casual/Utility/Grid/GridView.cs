using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridView : MonoBehaviour {

	public string gridSpriteName;
	public UIAtlas gridAtlas;
	public Color buildingModeAvailable;
	public Color buildingModeUnavailable;
	public Color pathModeAvailable;
	public Color pathModeUnavailable;
	public Color normal = new Color (0,0,0,0);

	protected UISprite[,] sprites;

	protected bool init;

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
				IGridObject gridObject = BuildingModeGrid.GetInstance().GetObjectAtPosition(new GridPosition(x, y));
				if (!(gridObject is UnusableGrid))
				{
					CreateGridTile(x, y);
				}
			}
		}

	}

	virtual protected void CreateGridTile(int x, int y)
	{
		GameObject go = new GameObject();
		go.transform.parent = transform;
		go.layer = gameObject.layer;
		UISprite sprite = go.AddComponent<UISprite>();
		sprites[x, y] = sprite;
		sprite.atlas = gridAtlas;
		sprite.spriteName = gridSpriteName;
		Vector3 position = BuildingModeGrid.GetInstance().GridPositionToArenaPosition(new GridPosition(x,y));
		position.z = 0;
		go.transform.localPosition = position;
		sprite.depth = 5;
		sprite.MakePixelPerfect();
		sprite.color = normal;
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
					if ( BuildingModeGrid.GetInstance().CanObjectBePlacedAtPosition(GridPosition.DefaultShape, new GridPosition(x,y)))
					    sprites[x,y].color = buildingModeAvailable;
					else
					    sprites[x,y].color = buildingModeUnavailable;
				}
			}
		}
	}

	virtual public void ShowGrid(GridPosition gridPos, List<GridPosition> shape ) {
		//FindTileAndSetVisible (TILENAMEPREFIX+ gridPos.x + gridPos.y, true);
	}
	
	virtual public void HideGrid(GridPosition gridPos, List<GridPosition> shape ) {
		//FindTileAndSetVisible (TILENAMEPREFIX + gridPos.x + gridPos.y, false);
	}
	
	virtual protected void FindTileAndSetVisible(string name, bool setVisible) {
		//GameObject tileobject = GameObject.Find (name);
		//if (tileobject != null) {
		//	tileobject.SetActive(true);
		//}
	}

	public void PathMode()
	{
		int sizeX = BuildingModeGrid.GetInstance().gridSizeX;
		int sizeY = BuildingModeGrid.GetInstance().gridSizeY;
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				if (sprites[x,y] != null)
				{
					if ( BuildingModeGrid.GetInstance().CanObjectBePlacedAtPosition(GridPosition.DefaultShape, new GridPosition(x,y)))
						sprites[x,y].color = pathModeAvailable;
					else
						sprites[x,y].color = pathModeUnavailable;
				}
			}
		}
	}

	public static GridView GetInstance() {
		return Instance;
	}

	public static GridView Instance
	{
		get; protected set;
	}
}
